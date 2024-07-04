using BiomeExtractorsMod.Content.Items;
using BiomeExtractorsMod.Content.TileEntities;
using Terraria;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Tiles
{
    class BiomeExtractorTileBasic : BiomeExtractorTile
    {
        protected override int FrameCount => 8;

        protected override int _tileStyles => 2;

        protected override BiomeExtractorEnt TileEntity => ModContent.GetInstance<BiomeExtractorEntBasic>();

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.tileLavaDeath[Type] = true;
        }

        protected override void CreateMapEntries()
        {
            AddMapEntry(new(140, 101, 80), MapEntryName);
            AddMapEntry(new(85, 114, 123), MapEntryName);
        }

        protected override int ItemType(Tile tile)
        {
            return tile.TileFrameX > 50 ? ModContent.ItemType<BiomeExtractorItemLead>() : ModContent.ItemType<BiomeExtractorItemIron>();
        }
    }
}