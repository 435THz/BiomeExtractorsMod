using BiomeExtractorsMod.Content.Items;
using BiomeExtractorsMod.Content.TileEntities;
using Terraria;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Tiles
{
    class BiomeExtractorTileCyber : BiomeExtractorTile
    {
        protected override int FrameCount => 8;

        protected override BiomeExtractorEnt GetTileEntity()
        {
            return ModContent.GetInstance<BiomeExtractorEntCyber>();
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.tileObsidianKill[Type] = true;
        }

        protected override int ItemType(Tile tile)
        {
            return ModContent.ItemType<BiomeExtractorItemCyber>();
        }
    }
}