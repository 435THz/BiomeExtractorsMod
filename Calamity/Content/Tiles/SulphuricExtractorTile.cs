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
    internal class SulphuricExtractorTile : BiomeExtractorTile
    {
        protected override int FrameCount => 8;
        protected override string GlowAsset => "Calamity/Content/Tiles/SulphuricExtractorTile_Glow";

        protected override BiomeExtractorEnt TileEntity => ModContent.GetInstance<SulphuricExtractorEnt>();

        protected override void SetupTileData()
        {
            DustType = DustID.DemonTorch;

            Main.tileObsidianKill[Type] = true;
            TileObjectData.newTile.LavaDeath = true;
        }

        protected override void CreateMapEntries()
        {
            AddMapEntry(new(144, 114, 166), MapEntryName);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            bool found = TileUtils.TryGetTileEntityAs(i, j, out BiomeExtractorEnt entity);
            if (!found || !entity.Active)
            {
                r = 0;
                g = 0;
                b = 0;
                return;
            }
            r = 0.15f;
            g = 0.43f;
            b = 0.37f;
        }

        protected override int ItemType(Tile tile)
        {
            return ModContent.ItemType<SulphuricExtractorItem>();
        }
    }
}
