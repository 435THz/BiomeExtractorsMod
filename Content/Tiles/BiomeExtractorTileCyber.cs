using BiomeExtractorsMod.Content.Items;
using BiomeExtractorsMod.Content.TileEntities;
using Terraria;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.Tiles
{
    class BiomeExtractorTileCyber : BiomeExtractorTile
    {
        protected override int FrameCount => 8;
        protected override int IdleFrame => 4;

        protected override string GlowAsset => "Content/Tiles/BiomeExtractorTileCyber_Glow";

        protected override BiomeExtractorEnt TileEntity => ModContent.GetInstance<BiomeExtractorEntCyber>();

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.tileObsidianKill[Type] = true;
        }

        protected override void CreateMapEntries()
        {
            AddMapEntry(new(117, 197, 46), MapEntryName);
        }
        protected override int ItemType(Tile tile)
        {
            return ModContent.ItemType<BiomeExtractorItemCyber>();
        }
    }
}