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
    internal class ThermoresistantExtractorTile : BiomeExtractorTile
    {
        protected override int FrameCount => 1; //TODO

        protected override BiomeExtractorEnt TileEntity => ModContent.GetInstance<ThermoresistantExtractorEnt>();

        protected override void SetupTileData()
        {
            DustType = DustID.Meteorite;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.LavaDeath = false;
        }

        protected override void CreateMapEntries()
        {
            AddMapEntry(new(125, 58, 58), MapEntryName);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            bool found = TileUtils.TryGetTileEntityAs(i, j, out BiomeExtractorEnt entity);
            if (!found || !entity.Active)
            {
                r = 0.025f;
                g = 0.015f;
                b = 0.005f;
                return;
            }
            r = 0.25f;
            g = 0.15f;
            b = 0.05f;
        }

        protected override int ItemType(Tile tile)
        {
            return ModContent.ItemType<ThermoresistantExtractorItem>();
        }
    }
}
