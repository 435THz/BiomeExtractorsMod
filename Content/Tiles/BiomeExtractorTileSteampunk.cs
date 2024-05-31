using BiomeExtractorsMod.Content.TileEntities;
using Terraria;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Tiles
{
    class BiomeExtractorTileSteampunk : BiomeExtractorTile
    {
        protected override int FrameCount => 8;
        protected override BiomeExtractorEnt GetTileEntity()
        {
            return ModContent.GetInstance<BiomeExtractorEntSteampunk>();
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.tileObsidianKill[Type] = true;
        }
    }
}