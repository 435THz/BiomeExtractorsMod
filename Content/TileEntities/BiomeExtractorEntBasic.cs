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
        protected override int GetSelfMaxTimer()
        {
            return ModContent.GetInstance<ExtractorConfig>().BasicExtractorSpeed;
        }

        protected override int GetSelfChance()
        {
            return ModContent.GetInstance<ExtractorConfig>().BasicExtractorChance;
        }

        protected override int GetTileType()
        {
            return ModContent.TileType<BiomeExtractorTileBasic>();
        }
    }
}