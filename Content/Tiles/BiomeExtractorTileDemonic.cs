using BiomeExtractorsMod.Content.TileEntities;
using Terraria;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Tiles
{
    class BiomeExtractorTileDemonic : BiomeExtractorTile
    {
        protected override int FrameCount => 8;

        protected override BiomeExtractorEnt GetTileEntity()
        {
            return ModContent.GetInstance<BiomeExtractorEntDemonic>();
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.tileLavaDeath[Type] = true;
        }
    }
}