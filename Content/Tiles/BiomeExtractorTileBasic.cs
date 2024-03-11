
using BiomeExtractorsMod.Content.TileEntities;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Tiles
{
    class BiomeExtractorTileBasic : BiomeExtractorTile
    {
        protected override BiomeExtractorEnt getTileEntity()
        {
            return ModContent.GetInstance<BiomeExtractorEntBasic>();
        }
    }
}