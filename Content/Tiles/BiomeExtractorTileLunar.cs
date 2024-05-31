using BiomeExtractorsMod.Content.TileEntities;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Tiles
{
    class BiomeExtractorTileLunar : BiomeExtractorTile
    {
        protected override int FrameCount => 8;
        protected override BiomeExtractorEnt GetTileEntity()
        {
            return ModContent.GetInstance<BiomeExtractorEntLunar>();
        }
    }
}