using BiomeExtractorsMod.Common.Systems;
using BiomeExtractorsMod.Content.Tiles;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.TileEntities
{
    public class BiomeExtractorEntInfernal : BiomeExtractorEnt
    {
        internal override Asset<Texture2D> MapIcon => Mod.Assets.Request<Texture2D>("Content/MapIcons/BiomeExtractorIconInfernal");
        protected internal override BiomeExtractionSystem.ExtractionTier ExtractionTier => BES.GetTier((int)EnumTiers.INFERNAL, true);
        protected internal override int TileType => ModContent.TileType<BiomeExtractorTileInfernal>();
    }
}