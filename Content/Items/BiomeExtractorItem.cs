using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    abstract class BiomeExtractorItem : ModItem
    {
        protected abstract int TileId { get; }
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