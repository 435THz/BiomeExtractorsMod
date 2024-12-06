using BiomeExtractorsMod.Content.TileEntities;
using BiomeExtractorsMod.Calamity.Common;
using BiomeExtractorsMod.Calamity.Content.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using static BiomeExtractorsMod.Common.Database.BiomeExtractionSystem;
using BiomeExtractorsMod.Common.Database;

namespace BiomeExtractorsMod.Calamity.Content.TileEntities
{
    [ExtendsFromMod("CalamityMod")]
    [JITWhenModsEnabled("CalamityMod")]
    internal class ThermoresistantExtractorEnt : BiomeExtractorEnt
    {
        private readonly ExtractorIconOverride _iconOverride = new($"{BiomeExtractorsMod.LocExtractorSuffix("Thermoresistant")}", delegate { return BiomeExtractorsMod.Instance.Assets.Request<Texture2D>("Calamity/Content/MapIcons/ThermoresistantExtractorIcon"); }, 0, 1);
        protected internal override ExtractorIconOverride IconOverride => _iconOverride;
        protected internal override string LocalName => Language.GetTextValue(BiomeExtractorsMod.LocExtractorSuffix("Thermoresistant"));
        protected internal override int ExtractionRate => CalamityConfigs.Instance.ThermoresistantExtractorRate;
        protected internal override int ExtractionChance => CalamityConfigs.Instance.ThermoresistantExtractorChance;
        protected internal override int ExtractionAmount => CalamityConfigs.Instance.ThermoresistantExtractorAmount;
        protected internal override int TileType => ModContent.TileType<ThermoresistantExtractorTile>();

        protected internal override ExtractionTier ExtractionTier => Instance.GetTier(ExtractionTiers.CYBER, true);

        public override void Update()
        {
            Point point = Position.ToPoint() + new Point(1, 1);
            if (!BiomeChecker.IsInAbyssArea(point) || !BiomeChecker.IsSubmerged(point))
                Active = false;

            base.Update();
        }
    }
}
