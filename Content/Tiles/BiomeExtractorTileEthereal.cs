using BiomeExtractorsMod.Content.TileEntities;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Tiles
{
    class BiomeExtractorTileEthereal : BiomeExtractorTile
    {
        protected override int FrameCount => 8;
        protected override BiomeExtractorEnt GetTileEntity()
        {
            return ModContent.GetInstance<BiomeExtractorEntEthereal>();
        }
    }
}