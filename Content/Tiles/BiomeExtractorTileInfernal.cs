using BiomeExtractorsMod.Content.TileEntities;
using Terraria;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Tiles
{
    class BiomeExtractorTileInfernal : BiomeExtractorTile
    {
        protected override int FrameCount => 1; //TODO set FrameCount
        protected override BiomeExtractorEnt GetTileEntity()
        {
            return ModContent.GetInstance<BiomeExtractorEntInfernal>();
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.tileObsidianKill[Type] = true;
        }
    }
}