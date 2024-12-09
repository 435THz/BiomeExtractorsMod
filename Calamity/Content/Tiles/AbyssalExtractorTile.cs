using BiomeExtractorsMod.Content.TileEntities;
using BiomeExtractorsMod.Content.Tiles;
using BiomeExtractorsMod.Calamity.Content.Items;
using BiomeExtractorsMod.Calamity.Content.TileEntities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace BiomeExtractorsMod.Calamity.Content.Tiles
{
    [ExtendsFromMod("CalamityMod")]
    [JITWhenModsEnabled("CalamityMod")]
    internal class AbyssalExtractorTile : BiomeExtractorTile
    {
        protected override int FrameCount => 8;
        protected override int FrameDuration => 6;
        protected override string GlowAsset => "Calamity/Content/Tiles/AbyssalExtractorTile_Glow";

        protected override BiomeExtractorEnt TileEntity => ModContent.GetInstance<AbyssalExtractorEnt>();

        protected override void SetupTileData()
        {
            DustType = DustID.Granite;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.LavaDeath = false;
        }

        protected override void CreateMapEntries()
        {
            AddMapEntry(new(49, 39, 124), MapEntryName);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            bool found = TileUtils.TryGetTileEntityAs(i, j, out BiomeExtractorEnt entity);
            if (!found || !entity.Active)
            {
                r = 0.020f;
                g = 0.018f;
                b = 0.025f;
                return;
            }
            r = 0.20f;
            g = 0.18f;
            b = 0.25f;
        }

        protected override int ItemType(Tile tile)
        {
            return ModContent.ItemType<AbyssalExtractorItem>();
        }
    }
}
