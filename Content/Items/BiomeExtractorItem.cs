using BiomeExtractorsMod.Common.Systems;
using BiomeExtractorsMod.Content.Tiles;
using System;
using Terraria;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    /// <summary>
    /// The core Item class implemented by all BiomeExtractors.
    /// </summary>
    public abstract class BiomeExtractorItem : ModItem
    {
        /// <summary>
        /// Returns the id of the BiomeExtractorTile this Item is bound to.
        /// </summary>
        protected abstract int TileId { get; }
        /// <summary>
        /// Returns the style index of the tile this Item creates.
        /// </summary>
        protected virtual int TileStyle => 0;

        /// <summary>
        /// Returns the ExtractorUpgradeKit required to craft this item.
        /// </summary>
        protected abstract ExtractorUpgradeKit UpgradeItemToCraftThis { get; }

        private BiomeExtractionSystem.ExtractionTier Tier => BiomeExtractionSystem.Instance.GetTier(((BiomeExtractorTile)ModContent.GetModTile(TileId)).GetTileEntity.Tier);

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
            Tier.Register(this);
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(TileId, TileStyle);
        }
    }
}