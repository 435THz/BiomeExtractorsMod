using BiomeExtractorsMod.Calamity.Content.TileEntities;
using BiomeExtractorsMod.Content.Tiles;

namespace BiomeExtractorsMod.Calamity.Content.Tiles
{
    internal abstract class BiomeExtractorTileAbyss : BiomeExtractorTile
    {
        protected override int GetAnimationFrame(int type, int i, int j)
        {
            bool found = TileUtils.TryGetTileEntityAs(i, j, out BiomeExtractorEntAbyss entity);
            if (!found || entity.PressureLock)
            {
                return IdleFrame;
            }
            return base.GetAnimationFrame(type, i, j);
        }
    }
}
