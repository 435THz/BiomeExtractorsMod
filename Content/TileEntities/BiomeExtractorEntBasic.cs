using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Content.Tiles;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.TileEntities
{


    public class BiomeExtractorEntBasic : BiomeExtractorEnt
    {
        public override int GetTier()
        {
            return 1;
        }
        protected override int getSelfMaxTimer()
        {
            return ModContent.GetInstance<ExtractorConfig>().BasicExtractorSpeed;
        }

        protected override int getSelfChance()
        {
            return ModContent.GetInstance<ExtractorConfig>().BasicExtractorChance;
        }

        protected override int getTileType()
        {
            return ModContent.TileType<BiomeExtractorTileBasic>();
        }
    }
}