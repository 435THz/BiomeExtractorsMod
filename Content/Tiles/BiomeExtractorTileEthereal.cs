using BiomeExtractorsMod.Content.Items;
using BiomeExtractorsMod.Content.TileEntities;
using Terraria;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Tiles
{
    class BiomeExtractorTileEthereal : BiomeExtractorTile
    {
        protected override int FrameCount => 8;

        protected override BiomeExtractorEnt TileEntity => ModContent.GetInstance<BiomeExtractorEntEthereal>();

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.tileLighted[Type] = true;
        }
        protected override void CreateMapEntries()
        {
            AddMapEntry(new(213, 20, 201), MapEntryName);
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            bool found = TileUtils.TryGetTileEntityAs(i, j, out BiomeExtractorEnt entity);
            if (!found || !entity.Active)
            {
                r = 0.095f;
                g = 0.075f;
                b = 0.100f;
                return;
            }
            r = 0.95f;
            g = 0.75f;
            b = 1.00f;
        }
        protected override int ItemType(Tile tile)
        {
            return ModContent.ItemType<BiomeExtractorItemEthereal>();
        }
    }
}