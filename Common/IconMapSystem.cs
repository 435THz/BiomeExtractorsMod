using BiomeExtractorsMod.Content.Items;
using BiomeExtractorsMod.Content.TileEntities;
using BiomeExtractorsMod.Content.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.UI;

namespace BiomeExtractorsMod.Common
{

    internal class IconMapLayer : ModSystem
    {
        public override void Load()
        {
            On_TeleportPylonsMapLayer.Draw += DrawExtractorIcons;
        }

        private static void DrawExtractorIcons(On_TeleportPylonsMapLayer.orig_Draw orig, TeleportPylonsMapLayer self, ref MapOverlayDrawContext context, ref string text)
        {
            bool hasScanner = false;
            Item[] inventory = Main.LocalPlayer.inventory;
            for (int i = 0; i < inventory.Length; i++)
                if (inventory[i] != null && inventory[i].type == ModContent.GetInstance<BiomeScanner>().Type)
                    hasScanner = true;

            if (!hasScanner) return;
            IEnumerable<BiomeExtractorEnt> extractors = Terraria.DataStructures.TileEntity.ByPosition
                .Where(pair => pair.Value is BiomeExtractorEnt)
                .Select(pair => pair.Value as BiomeExtractorEnt);
            List<BiomeExtractorEnt> toDraw = extractors.ToList();

            foreach (BiomeExtractorEnt extractor in toDraw)
            {
                string hoverText = Language.GetTextValue(extractor.LocalName);
                Asset<Texture2D> asset = extractor.MapIcon;
                Tile tile = Main.tile[extractor.Position];
                byte row = 0;
                byte column = (byte)extractor.TileStyle;
                byte columns = (byte)((BiomeExtractorTile)ModContent.GetModTile(tile.TileType)).TileStyles(tile); //why the hell is getting to the tile type object so goddamn roundabout wtf
                Color drawColor = Color.White;
                if (!extractor.Active)
                {
                    hoverText = $"{hoverText} ({Language.GetTextValue(BiomeExtractorsMod.LocIconInactive)})";
                    drawColor = new(0.25f, 0.25f, 0.25f, 0.5f);
                }
                else if (!extractor.HasOutput)
                {
                    hoverText = $"{hoverText} ({Language.GetTextValue(BiomeExtractorsMod.LocIconNoOutput)})";
                    row = 1;
                }
                MapOverlayDrawContext.DrawResult result = context.Draw(asset.Value, extractor.Position.ToVector2() + Vector2.One * 1.5f, drawColor, new(columns, 2, column, row), 1f, 1f, Alignment.Center);
                if (result.IsMouseOver)
                {
                    text = hoverText;
                }
            }
            orig(self, ref context, ref text);
        }
    }
}
