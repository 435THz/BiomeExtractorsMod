using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    /// <summary>
    /// The core Item class implemented by all BiomeExtractors.
    /// </summary>
    abstract class BiomeExtractorItem : ModItem
    {
        /// <summary>
        /// Returns the id of the BiomeExtractorTile this Item is bound to.
        /// </summary>
        protected abstract int TileId { get; }
        /// <summary>
        /// Returns the style index of the tile this Item creates.
        /// </summary>
        protected virtual int TileStyle => 0;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(TileId, TileStyle);
        }
    }
}