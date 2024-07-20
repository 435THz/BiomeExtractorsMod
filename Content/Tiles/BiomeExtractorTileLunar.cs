using BiomeExtractorsMod.Content.Items;
using BiomeExtractorsMod.Content.TileEntities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace BiomeExtractorsMod.Content.Tiles
{
    class BiomeExtractorTileLunar : BiomeExtractorTile
    {
        protected override int FrameCount => 8;

        protected override string GlowAsset => "Content/Tiles/BiomeExtractorTileLunar_Glow";
        protected override BiomeExtractorEnt TileEntity => ModContent.GetInstance<BiomeExtractorEntLunar>();

        protected override void SetupTileData()
        {
            TileObjectData.newTile.LavaDeath = false;
            Main.tileLighted[Type] = true;
        }
        public override bool CreateDust(int i, int j, ref int type)
        {

            bool found = TileUtils.TryGetTileEntityAs(i, j, out BiomeExtractorEnt entity);
            if (!found || !entity.Active)
            {
                type = DustID.LunarOre;
                return true;
            }
            else
            {
                int frameSector = GetAnimationFrame(Type, i, j) / 2;
                if (frameSector == 0) type = 72;
                else if (frameSector == 1) type = 229;
                else if (frameSector == 3) type = 187;
                else if (frameSector == 4) type = 259;
            }
            return true;
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