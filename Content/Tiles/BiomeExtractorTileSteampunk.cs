using BiomeExtractorsMod.Content.Items;
using BiomeExtractorsMod.Content.TileEntities;
using Terraria;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Tiles
{
    class BiomeExtractorTileSteampunk : BiomeExtractorTile
    {
        protected override int FrameCount => 8;
        protected override BiomeExtractorEnt TileEntity => ModContent.GetInstance<BiomeExtractorEntSteampunk>();

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.tileObsidianKill[Type] = true;
        }

        protected override int ItemType(Tile tile)
        {
            return tile.TileFrameX > 50 ? ModContent.ItemType<BiomeExtractorItemTitanium>() : ModContent.ItemType<BiomeExtractorItemAdamantite>();
        }
    }
}