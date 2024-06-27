using BiomeExtractorsMod.Content.Items;
using BiomeExtractorsMod.Content.TileEntities;
using Terraria;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Tiles
{
    class BiomeExtractorTileLunar : BiomeExtractorTile
    {
        protected override int FrameCount => 8;

        protected override string glowAsset => "Content/Tiles/BiomeExtractorTileLunar_Glow";
        protected override BiomeExtractorEnt TileEntity => ModContent.GetInstance<BiomeExtractorEntLunar>();

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.tileLighted[Type] = true;
        }
        protected override void CreateMapEntries()
        {
            AddMapEntry(new(69, 167, 119), MapEntryName);
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            bool found = TileUtils.TryGetTileEntityAs(i, j, out BiomeExtractorEnt entity);
            if (!found || !entity.Active)
            {
                r = 0.100f;
                g = 0.100f;
                b = 0.075f;
                return;
            }
            r = 1.00f;
            g = 1.00f;
            b = 0.75f;
        }
        protected override int ItemType(Tile tile)
        {
            return ModContent.ItemType<BiomeExtractorItemLunar>();
        }
    }
}