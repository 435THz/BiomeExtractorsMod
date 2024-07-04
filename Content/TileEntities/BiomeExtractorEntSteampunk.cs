using BiomeExtractorsMod.Common.Systems;
using BiomeExtractorsMod.Content.Tiles;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.TileEntities
{
    public class BiomeExtractorEntSteampunk : BiomeExtractorEnt
    {
        protected internal override BiomeExtractionSystem.ExtractionTier ExtractionTier => BES.GetTier((int)EnumTiers.STEAMPUNK, true);
        protected internal override int TileType => ModContent.TileType<BiomeExtractorTileSteampunk>();
    }
}