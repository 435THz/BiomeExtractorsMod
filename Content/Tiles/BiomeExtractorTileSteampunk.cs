using BiomeExtractorsMod.Content.Items;
using BiomeExtractorsMod.Content.TileEntities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace BiomeExtractorsMod.Content.Tiles
{
    class BiomeExtractorTileSteampunk : BiomeExtractorTile
    {
        protected override int FrameCount => 8;
        protected override int IdleFrame => 4;
        protected override int _tileStyles => 2;

        protected override string GlowAsset => "Content/Tiles/BiomeExtractorTileSteampunk_Glow";

        protected override BiomeExtractorEnt TileEntity => ModContent.GetInstance<BiomeExtractorEntSteampunk>();

        protected override void SetupTileData()
        {
            Main.tileObsidianKill[Type] = true;
            TileObjectData.newTile.LavaDeath = false;
        }
        public override bool CreateDust(int i, int j, ref int type)
        {
            type = TileObjectData.GetTileStyle(Main.tile[i, j]) == 0 ? DustID.Adamantite : DustID.Titanium;
            return true;
        }

        protected override void CreateMapEntries()
        {
            AddMapEntry(new(221, 61, 74), MapEntryName);
            AddMapEntry(new(121, 139, 150), MapEntryName);
        }
        protected override int ItemType(Tile tile)
        {
            return tile.TileFrameX > 50 ? ModContent.ItemType<BiomeExtractorItemTitanium>() : ModContent.ItemType<BiomeExtractorItemAdamantite>();
        }
    }
}