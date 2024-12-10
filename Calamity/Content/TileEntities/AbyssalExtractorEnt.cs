using BiomeExtractorsMod.Calamity.Common;
using BiomeExtractorsMod.Calamity.Content.Tiles;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;
using Terraria.ModLoader;
using static BiomeExtractorsMod.Common.Database.BiomeExtractionSystem;
using BiomeExtractorsMod.Common.Database;

namespace BiomeExtractorsMod.Calamity.Content.TileEntities
{
    [ExtendsFromMod("CalamityMod")]
    [JITWhenModsEnabled("CalamityMod")]
    public class AbyssalExtractorEnt : BiomeExtractorEntAbyss
    {
        private readonly ExtractorIconOverride _iconOverride = new($"{BiomeExtractorsMod.LocExtractorSuffix("Abyssal")}", delegate { return BiomeExtractorsMod.Instance.Assets.Request<Texture2D>("Calamity/Content/MapIcons/AbyssalExtractorIcon"); }, 0, 1);
        protected internal override ExtractorIconOverride IconOverride => _iconOverride;
        protected internal override string LocalName => Language.GetTextValue(BiomeExtractorsMod.LocExtractorSuffix("Abyssal"));
        protected internal override int ExtractionRate => CalamityConfigs.Instance.AbyssalExtractorRate;
        protected internal override int ExtractionChance => CalamityConfigs.Instance.AbyssalExtractorChance;
        protected internal override int ExtractionAmount => CalamityConfigs.Instance.AbyssalExtractorAmount;
        protected internal override int TileType => ModContent.TileType<AbyssalExtractorTile>();

        protected internal override ExtractionTier ExtractionTier => Instance.GetTier(ExtractionTiers.SPECTRAL, true);
    }
}
