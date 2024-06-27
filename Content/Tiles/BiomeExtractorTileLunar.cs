using BiomeExtractorsMod.Content.Items;
using BiomeExtractorsMod.Content.TileEntities;
using Terraria;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Tiles
{
    class BiomeExtractorTileLunar : BiomeExtractorTile
    {
        protected override int FrameCount => 8;

        protected override BiomeExtractorEnt TileEntity => ModContent.GetInstance<BiomeExtractorEntLunar>();

        protected override void CreateMapEntries()
        {
            AddMapEntry(new(69, 167, 119), MapEntryName);
        }
        protected override int ItemType(Tile tile)
        {
            return ModContent.ItemType<BiomeExtractorItemLunar>();
        }
    }
}