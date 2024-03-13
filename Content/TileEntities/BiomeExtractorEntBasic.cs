using BiomeExtractorsMod.Common.Configs;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.TileEntities
{


    public class BiomeExtractorEntBasic : BiomeExtractorEnt
    {
        protected override int GetTier()
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
    }
}