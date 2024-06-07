using BiomeExtractorsMod.Content.Items;
using BiomeExtractorsMod.Content.TileEntities;
using Terraria;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Tiles
{
    class BiomeExtractorTileBasic : BiomeExtractorTile
    {
        protected override int FrameCount => 8;

        protected override BiomeExtractorEnt GetTileEntity()
        {
            return ModContent.GetInstance<BiomeExtractorEntBasic>();
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.tileLavaDeath[Type] = true;
        }

        protected override int ItemType(Tile tile)
        {
            return tile.TileFrameX > 50 ? ModContent.ItemType<BiomeExtractorItemLead>() : ModContent.ItemType<BiomeExtractorItemIron>();
        }
    }
}