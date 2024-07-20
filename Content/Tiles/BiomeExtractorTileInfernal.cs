using BiomeExtractorsMod.Content.Items;
using BiomeExtractorsMod.Content.TileEntities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace BiomeExtractorsMod.Content.Tiles
{
    class BiomeExtractorTileInfernal : BiomeExtractorTile
    {
        protected override int FrameCount => 8;
        protected override BiomeExtractorEnt TileEntity => ModContent.GetInstance<BiomeExtractorEntInfernal>();

        protected override void SetupTileData()
        {
            DustType = DustID.Torch;

            Main.tileObsidianKill[Type] = true;
            TileObjectData.newTile.LavaDeath = false;
        }

        protected override void CreateMapEntries()
        {
            AddMapEntry(new(226, 116, 56), MapEntryName);
        }
        protected override int ItemType(Tile tile)
        {
            return ModContent.ItemType<BiomeExtractorItemInfernal>();
        }
    }
}