using BiomeExtractorsMod.Common.Systems;
using BiomeExtractorsMod.Content.Tiles;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.TileEntities
{
    public class BiomeExtractorEntEthereal : BiomeExtractorEnt
    {
        internal override Asset<Texture2D> MapIcon => Mod.Assets.Request<Texture2D>("Content/MapIcons/BiomeExtractorIconEthereal");
        protected internal override BiomeExtractionSystem.ExtractionTier ExtractionTier => BES.GetTier((int)EnumTiers.ETHEREAL, true);
        protected internal override int TileType => ModContent.TileType<BiomeExtractorTileEthereal>();
    }
}