using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Items
{
    abstract class BiomeExtractorItem : ModItem
    {
        protected abstract int getTileId();

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(getTileId());
        }
    }
}