using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    abstract class BiomeExtractorItem : ModItem
    {
        protected abstract int GetTileId();

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(GetTileId());
        }
    }
}