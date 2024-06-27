using BiomeExtractorsMod.Content.Items;
using BiomeExtractorsMod.Content.TileEntities;
using Terraria;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Tiles
{
    class BiomeExtractorTileSteampunk : BiomeExtractorTile
    {
        protected override int FrameCount => 8;
        protected override string glowAsset => "Content/Tiles/BiomeExtractorTileSteampunk_Glow";

        protected override BiomeExtractorEnt TileEntity => ModContent.GetInstance<BiomeExtractorEntSteampunk>();

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.tileObsidianKill[Type] = true;
        }

        protected override void CreateMapEntries()
        {
            AddMapEntry(new(221, 61, 74), MapEntryName);
            AddMapEntry(new(121, 139, 150), MapEntryName);
        }
        protected override int ItemType(Tile tile)
        {
            return tile.TileFrameX > 50 ? ModContent.ItemType<BiomeExtractorItemTitanium>() : ModContent.ItemType<BiomeExtractorItemAdamantite>();
        }
    }
}