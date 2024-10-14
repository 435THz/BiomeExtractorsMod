using BiomeExtractorsMod.Common.Systems;
using BiomeExtractorsMod.Content.Items;
using BiomeExtractorsMod.Content.TileEntities;
using BiomeExtractorsMod.Content.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using Terraria.UI;

namespace BiomeExtractorsMod.Common.Systems
{
    internal enum ExtractorState { ACTIVE, INACTIVE, NO_OUTPUT, BROKEN }
    internal class IconMapSystem : ModSystem
    {
        static List<Point16> Positions = [];
        static List<int> Tiers = [];
        static List<ExtractorState> States = [];

        public override void SaveWorldData(TagCompound tag)
        {
            tag["extractorPositions"] = Positions;
            tag["extractorTiers"] = Tiers;
            tag["extractorStates"] = States.Select(state => (byte)state).ToList();
        }
        public override void LoadWorldData(TagCompound tag)
        {
            Positions = tag.Get<List<Point16>>("extractorPositions");
            Tiers = tag.Get<List<int>>("extractorTiers");
            States = tag.Get<List<byte>>("extractorStates").Select(s => (ExtractorState)s).ToList();

            ValidateLists();
        }
        public override void ClearWorld() => FlushLists();

        private static void FlushLists()
        {
            Positions = [];
            Tiers = [];
            States = [];
        }

        internal static ExtractorState StateOf(BiomeExtractorEnt extractor)
        {
            return !extractor.Active ? ExtractorState.INACTIVE : !extractor.HasOutput ? ExtractorState.NO_OUTPUT : ExtractorState.ACTIVE;
        }
        internal static void AddExtractorData(BiomeExtractorEnt extractor)
            => AddExtractorData(extractor.Position, extractor.Tier, StateOf(extractor));

        internal static void AddExtractorData(Point16 position, int tier, ExtractorState state)
        {
            int index = Positions.IndexOf(position);
            if (index >= 0) SetState(index, tier, state);
            else SetState(position, tier, state);
        }
        private static void SetState(int index, int tier, ExtractorState state)
        {
            Tiers[index] = tier;
            States[index] = state;
        }
        private static void SetState(Point16 position, int tier, ExtractorState state)
        {
            Positions.Add(position);
            Tiers.Add(tier);
            States.Add(state);
        }

        internal static void UpdateExtractorData(BiomeExtractorEnt extractor)
            => UpdateExtractorData(extractor.Position, StateOf(extractor));
        internal static void UpdateExtractorData(Point16 position, ExtractorState state)
        {
            int index = Positions.IndexOf(position);
            if (index >= 0) SetState(index, Tiers[index], state);
        }

        internal static void RemoveExtractorData(BiomeExtractorEnt extractor)
            => RemoveExtractorData(extractor.Position);
        internal static void RemoveExtractorData(Point16 position)
        {
            int index = Positions.IndexOf(position);
            if (index >= 0)
            {
                Positions.RemoveAt(index);
                Tiers.RemoveAt(index);
                States.RemoveAt(index);
            }
        }

        private static void ValidateLists()
        {
            for (int i = Positions.Count-1; i >= 0; i--)
            {
                Point16 position = Positions[i];
                if (TileUtils.TryGetTileEntityAs(position.X, position.Y, out BiomeExtractorEnt ent))
                    UpdateExtractorData(ent);

                else if (ModContent.GetModTile(Main.tile[position.ToPoint()].TileType) is BiomeExtractorTile)
                    UpdateExtractorData(position, ExtractorState.BROKEN);

                else
                    RemoveExtractorData(position);
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            ValidateLists();
            writer.Write(Positions.Count);
            for (int i = 0; i < Positions.Count; i++)
            {
                writer.Write(Positions[i].X);
                writer.Write(Positions[i].Y);
                writer.Write(Tiers[i]);
                writer.Write((byte)States[i]);
            }
        }

        public override void NetReceive(BinaryReader reader)
        {
            FlushLists();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                Point16 position = new(reader.ReadInt16(), reader.ReadInt16());
                int tier = reader.ReadInt32();
                ExtractorState state = (ExtractorState)reader.ReadByte();
                AddExtractorData(position, tier, state);
            }
        }



        public override void Load()
        {
            On_TeleportPylonsMapLayer.Draw += DrawExtractorIcons;
        }

        private void DrawExtractorIcons(On_TeleportPylonsMapLayer.orig_Draw orig, TeleportPylonsMapLayer self, ref MapOverlayDrawContext context, ref string text)
        {
            bool hasScanner = false;
            Item[] inventory = Main.LocalPlayer.inventory;
            for (int i = 0; i < inventory.Length; i++)
                if (inventory[i] != null && inventory[i].type == ModContent.GetInstance<BiomeScanner>().Type)
                    hasScanner = true;

            if (hasScanner)
            {
                
                IEnumerable<BiomeExtractorEnt> extractors = TileEntity.ByPosition
                    .Where(pair => pair.Value is BiomeExtractorEnt)
                    .Select(pair => pair.Value as BiomeExtractorEnt);
                List<BiomeExtractorEnt> toDraw = extractors.ToList();

                for (int i = 0; i < Positions.Count; i++)
                {
                    Tile tile = Main.tile[Positions[i]];
                    ModTile tileObj = ModContent.GetModTile(tile.TileType);
                    if (tileObj is null || !tile.HasTile) continue;
                    TileUtils.TryGetTileEntityAs<BiomeExtractorEnt>(Positions[i].X, Positions[i].Y, out BiomeExtractorEnt extractor);
                    if (extractor is null) continue;

                    string hoverText;
                    Asset<Texture2D> asset;
                    byte row = 0;
                    byte column;
                    byte columns;
                    if (extractor.HasIconOverride)
                    {
                        hoverText = Language.GetTextValue(extractor.IconOverride.HoverTextKey);
                        asset = Mod.Assets.Request<Texture2D>(extractor.IconOverride.IconPath);
                        column = extractor.IconOverride.Column;
                        columns = extractor.IconOverride.FileColumns;
                    }
                    else
                    {
                        BiomeExtractionSystem.ExtractionTier tier = BiomeExtractionSystem.Instance.GetTier(Tiers[i]);
                        hoverText = Language.GetTextValue(tier.Name);
                        asset = Mod.Assets.Request<Texture2D>(tier.IconPath);
                        column = (byte)TileObjectData.GetTileStyle(Main.tile[Positions[i]]);
                        columns = (byte)((BiomeExtractorTile)tileObj).TileStyles(tile);
                    }

                    Color drawColor = Color.White;
                    switch (States[i])
                    {
                        case ExtractorState.INACTIVE:
                            hoverText = $"{hoverText} ({Language.GetTextValue(BiomeExtractorsMod.LocIconInactive)})";
                            drawColor = new(0.25f, 0.25f, 0.25f, 0.5f);
                            break;

                        case ExtractorState.NO_OUTPUT:
                            hoverText = $"{hoverText} ({Language.GetTextValue(BiomeExtractorsMod.LocIconNoOutput)})";
                            row = 1;
                            break;

                        case ExtractorState.BROKEN:
                            hoverText = $"{hoverText} ({Language.GetTextValue(BiomeExtractorsMod.LocIconError)})";
                            drawColor = new(0.1f, 0.1f, 0.1f, 1f);
                            break;
                    }
                
                    MapOverlayDrawContext.DrawResult result = context.Draw(asset.Value, Positions[i].ToVector2() + Vector2.One * 1.5f, drawColor, new(columns, 2, column, row), 1f, 1f, Alignment.Center);
                    if (result.IsMouseOver)
                    {
                        text = hoverText;
                    }
                }
            }
            orig(self, ref context, ref text);
        }
    }
}
