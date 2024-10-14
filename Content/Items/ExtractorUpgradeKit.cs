using static BiomeExtractorsMod.Common.Systems.BiomeExtractionSystem;
using Terraria.Localization;
using System.Linq;

namespace BiomeExtractorsMod.Content.Items
{
    public abstract class ExtractorUpgradeKit : ExtractorModificationKit
    {
        protected override int[] TargetTiles => LowerTier.Items.Select(item => item.TileId).ToArray();
        protected override int ResultTile => TileID;
        /// <summary>
        /// The tier that this kit upgrades extractors to.
        /// </summary>
        protected abstract int Tier { get; }
        /// <summary>
        /// The id of the tile that will replace the current one.
        /// </summary>
        protected abstract int TileID { get; }
        /// <summary>
        /// The style of the tile that will replace the current one.
        /// </summary>
        internal ExtractionTier UpgradedTier => Instance.GetTier(Tier, true);
        internal ExtractionTier LowerTier => Instance.GetClosestLowerTier(Tier);
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(LowerTier.Article, LowerTier.Name, UpgradedTier.Article, UpgradedTier.Name);
    }
}
