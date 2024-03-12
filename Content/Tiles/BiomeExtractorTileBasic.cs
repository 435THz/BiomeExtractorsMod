using BiomeExtractorsMod.Content.TileEntities;
using Terraria;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Tiles
{
    class BiomeExtractorTileBasic : BiomeExtractorTile
    {
        protected override BiomeExtractorEnt GetTileEntity()
        {
            return ModContent.GetInstance<BiomeExtractorEntBasic>();
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.tileLavaDeath[Type] = true;
        }
    }
}