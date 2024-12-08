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
    internal class AuricExtractorTile : BiomeExtractorTile
    {
        protected override int FrameCount => 6;
        protected override int IdleFrame => 6;
        protected override string GlowAsset => "Calamity/Content/Tiles/AuricExtractorTile_Glow";

        protected override BiomeExtractorEnt TileEntity => ModContent.GetInstance<AuricExtractorEnt>();

        protected override void SetupTileData()
        {
            DustType = DustID.GemTopaz;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.LavaDeath = false;
        }

        protected override void CreateMapEntries()
        {
            AddMapEntry(new(231, 166, 79), MapEntryName);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            bool found = TileUtils.TryGetTileEntityAs(i, j, out BiomeExtractorEnt entity);
            if (!found || !entity.Active)
            {
                r = 0.082f;
                g = 0.094f;
                b = 0.100f;
                return;
            }
            r = 0.82f;
            g = 0.94f;
            b = 1.00f;
        }

        protected override int ItemType(Tile tile)
        {
            return ModContent.ItemType<AuricExtractorItem>();
        }
    }
}
