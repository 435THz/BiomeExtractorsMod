using BiomeExtractorsMod.Content.Items;
using BiomeExtractorsMod.Content.TileEntities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace BiomeExtractorsMod.Content.Tiles
{
    class BiomeExtractorTileDemonic : BiomeExtractorTile
    {
        protected override int FrameCount => 8;

        protected override int _tileStyles => 2;

        protected override string GlowAsset => "Content/Tiles/BiomeExtractorTileDemonic_Glow";

        protected override BiomeExtractorEnt TileEntity => ModContent.GetInstance<BiomeExtractorEntDemonic>();

        protected override void SetupTileData()
        {
            Main.tileObsidianKill[Type] = true;
            TileObjectData.newTile.LavaDeath = true;
        }
        public override bool CreateDust(int i, int j, ref int type)
        {
            type = TileObjectData.GetTileStyle(Main.tile[i, j]) == 0 ? DustID.Corruption : DustID.Crimson;
            return true;
        }

        protected override void CreateMapEntries()
        {
            AddMapEntry(new(99, 74, 187), MapEntryName);
            AddMapEntry(new(183, 31, 49), MapEntryName);
        }
        protected override int ItemType(Tile tile)
        {
            return tile.TileFrameX > 50 ? ModContent.ItemType<BiomeExtractorItemCrimson>() : ModContent.ItemType<BiomeExtractorItemCorruption>();
        }
    }
}