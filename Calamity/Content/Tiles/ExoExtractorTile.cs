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
    internal class ExoExtractorTile : BiomeExtractorTile
    {
        protected override int FrameCount => 8;
        protected override string GlowAsset => "Calamity/Content/Tiles/ExoExtractorTile_Glow";

        protected override BiomeExtractorEnt TileEntity => ModContent.GetInstance<ExoExtractorEnt>();

        protected override void SetupTileData()
        {
            DustType = DustID.Silver;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.LavaDeath = false;
        }

        protected override void CreateMapEntries()
        {
            AddMapEntry(new(193, 204, 209), MapEntryName);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            bool found = TileUtils.TryGetTileEntityAs(i, j, out BiomeExtractorEnt entity);
            if (!found || !entity.Active)
            {
                r = 0.3f;
                g = 0.3f;
                b = 0.3f;
                return;
            }
            r = 1.0f;
            g = 1.0f;
            b = 1.0f;
        }

        protected override int ItemType(Tile tile)
        {
            return ModContent.ItemType<ExoExtractorItem>();
        }
    }
}
