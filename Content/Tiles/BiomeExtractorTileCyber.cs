using BiomeExtractorsMod.Content.Items;
using BiomeExtractorsMod.Content.TileEntities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace BiomeExtractorsMod.Content.Tiles
{
    public class BiomeExtractorTileCyber : BiomeExtractorTile
    {
        protected override int FrameCount => 8;
        protected override int IdleFrame => 4;

        protected override string GlowAsset => "Content/Tiles/BiomeExtractorTileCyber_Glow";

        protected override BiomeExtractorEnt TileEntity => ModContent.GetInstance<BiomeExtractorEntCyber>();

        protected override void SetupTileData()
        {
            DustType = DustID.Chlorophyte;

            Main.tileObsidianKill[Type] = true;
            TileObjectData.newTile.LavaDeath = false;
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