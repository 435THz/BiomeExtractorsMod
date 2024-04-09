using BiomeExtractorsMod.Content.TileEntities;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Tiles
{
    class BiomeExtractorTileLunar : BiomeExtractorTile
    {
        protected override int FrameCount => 1; //TODO set FrameCount
        protected override BiomeExtractorEnt GetTileEntity()
        {
            return ModContent.GetInstance<BiomeExtractorEntLunar>();
        }
    }
}