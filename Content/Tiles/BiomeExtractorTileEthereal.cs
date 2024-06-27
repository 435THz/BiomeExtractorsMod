using BiomeExtractorsMod.Content.Items;
using BiomeExtractorsMod.Content.TileEntities;
using Terraria;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Tiles
{
    class BiomeExtractorTileEthereal : BiomeExtractorTile
    {
        protected override int FrameCount => 8;
        protected override BiomeExtractorEnt TileEntity => ModContent.GetInstance<BiomeExtractorEntEthereal>();

        protected override void CreateMapEntries()
        {
            AddMapEntry(new(213, 20, 201), MapEntryName);
        }
        protected override int ItemType(Tile tile)
        {
            return ModContent.ItemType<BiomeExtractorItemEthereal>();
        }
    }
}