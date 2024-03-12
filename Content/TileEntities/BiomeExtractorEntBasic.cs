using BiomeExtractorsMod.Common.Configs;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.TileEntities
{
    public class BiomeExtractorEntBasic : BiomeExtractorEnt
    {
        public override int GetTier()
        {
            return 1;
        }

        public override int GetExtractionSpeed()
        {
            return ModContent.GetInstance<ExtractorConfig>().BasicExtractorSpeed;
        }

        public override float GetExtractionChance()
        {
            return ModContent.GetInstance<ExtractorConfig>().BasicExtractorChance;
        }
    }
}