using BiomeExtractorsMod.Content.Items;
using BiomeExtractorsMod.Content.TileEntities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace BiomeExtractorsMod.Content.Tiles
{
    public class BiomeExtractorTileBasic : BiomeExtractorTile
    {
        protected override int FrameCount => 8;

        protected override int _tileStyles => 2;

        protected override BiomeExtractorEnt TileEntity => ModContent.GetInstance<BiomeExtractorEntBasic>();

        protected override void SetupTileData()
        {
            Main.tileObsidianKill[Type] = true;
            TileObjectData.newTile.LavaDeath = true;
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            type = TileObjectData.GetTileStyle(Main.tile[i, j]) == 0 ? DustID.Iron : DustID.Lead;
            return true;
        }

        protected override void CreateMapEntries()
        {
            AddMapEntry(new(140, 101, 80), MapEntryName);
            AddMapEntry(new(85, 114, 123), MapEntryName);
        }

        protected override int ItemType(Tile tile)
        {
            return tile.TileFrameX > 50 ? ModContent.ItemType<BiomeExtractorItemLead>() : ModContent.ItemType<BiomeExtractorItemIron>();
        }
    }
}