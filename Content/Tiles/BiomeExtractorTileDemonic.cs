using BiomeExtractorsMod.Content.Items;
using BiomeExtractorsMod.Content.TileEntities;
using Terraria;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Tiles
{
    class BiomeExtractorTileDemonic : BiomeExtractorTile
    {
        protected override int FrameCount => 8;

        protected override BiomeExtractorEnt TileEntity => ModContent.GetInstance<BiomeExtractorEntDemonic>();

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.tileLavaDeath[Type] = true;
        }

        protected override int ItemType(Tile tile)
        {
            return tile.TileFrameX > 50 ? ModContent.ItemType<BiomeExtractorItemCrimson>() : ModContent.ItemType<BiomeExtractorItemCorruption>();
        }
    }
}