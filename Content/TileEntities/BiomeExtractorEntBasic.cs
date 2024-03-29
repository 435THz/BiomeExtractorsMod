using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Content.Tiles;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Personalities;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.TileEntities
{
    public class BiomeExtractorEntBasic : BiomeExtractorEnt
    {
        public override int Tier { get => 1; }
        public override int ExtractionRate { get => ModContent.GetInstance<ExtractorConfig>().BasicExtractorRate; }
        public override int ExtractionChance { get => ModContent.GetInstance<ExtractorConfig>().BasicExtractorChance; }
        protected override int TileType => ModContent.TileType<BiomeExtractorTileBasic>();

        internal override void DisplayStatus()
        {
                string s = PoolList.ToString();
                Main.NewTextMultiline("This machine is extracting from the following biomes:\n" +
                    s); //TODO format list
        }
    }
}