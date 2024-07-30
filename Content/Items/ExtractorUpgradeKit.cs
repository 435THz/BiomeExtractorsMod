﻿using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using BiomeExtractorsMod.Content.Tiles;
using BiomeExtractorsMod.Content.TileEntities;

using static BiomeExtractorsMod.Common.Systems.BiomeExtractionSystem;
using Terraria.Localization;

namespace BiomeExtractorsMod.Content.Items
{
    public abstract class ExtractorUpgradeKit : ModItem
    {
        /// <summary>
        /// The tier that this kit is an upgrade to.
        /// </summary>
        protected abstract int Tier { get; }
        /// <summary>
        /// The id of the tile that will replace the current one.
        /// </summary>
        protected abstract int TileID { get; }
        /// <summary>
        /// The style of the tile that will replace the current one.
        /// </summary>
        protected virtual int TileStyle => 0;
        internal ExtractionTier UpgradedTier => Instance.GetTier(Tier, true);
        internal ExtractionTier LowerTier => Instance.GetClosestLowerTier(Tier);
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(LowerTier.Article, LowerTier.Name, UpgradedTier.Article, UpgradedTier.Name);


        public override void SetDefaults()
        {
            Item.consumable = true;
            Item.width = 14;
            Item.height = 14;
            Item.maxStack = 9999;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 15;
            Item.useTime = 15;
            //sublcasses should set rarity and cost
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                Vector2 m = Main.MouseWorld;
                Point16 tileMouse = m.ToTileCoordinates16();
                if (!TileUtils.TryGetTileEntityAs(tileMouse.X, tileMouse.Y, out BiomeExtractorEnt extractor))
                    return false;
                if (LowerTier.Tier != extractor.Tier)
                    return false;

                Point16 topLeft = extractor.Position;

                UpgradeEntity(topLeft.X, topLeft.Y, TileID, TileStyle);
                return true;
            }
            return false;
        }

        internal static void UpgradeEntity(int i, int j, int tileId, int tileStyle)
        {
            Point16 origin = new Point16(i, j) + BiomeExtractorTile.origin;
            if (!TileUtils.TryGetTileEntityAs(i, j, out BiomeExtractorEnt extractor))
                return;

            extractor.Kill(i, j);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                byte msgId = (byte)ClientMessageType.KILL_REQUEST;
                ModPacket statePacket = ModContent.GetInstance<BiomeExtractorsMod>().GetPacket(9);
                statePacket.Write(msgId);
                statePacket.Write(i);
                statePacket.Write(j);
                statePacket.Send();
            }

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    int X = i + x; int Y = j + y;
                    Main.tile[X, Y].ClearTile();
                }
            }
            WorldGen.PlaceObject(origin.X, origin.Y, tileId, true, tileStyle);
            BiomeExtractorEnt te = ((BiomeExtractorTile)ModContent.GetModTile(Main.tile[i, j].TileType)).GetTileEntity;
            te.Hook_AfterPlacement(origin.X, origin.Y, tileId, tileStyle, -1, 0);
        }
    }
}
