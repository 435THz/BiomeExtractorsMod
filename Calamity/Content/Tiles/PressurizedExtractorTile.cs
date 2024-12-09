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
    internal class PressurizedExtractorTile : BiomeExtractorTile
    {
        protected override int FrameCount => 10;

        protected override BiomeExtractorEnt TileEntity => ModContent.GetInstance<PressurizedExtractorEnt>();

        protected override void SetupTileData()
        {
            DustType = DustID.GrassBlades;

            Main.tileObsidianKill[Type] = true;
            TileObjectData.newTile.LavaDeath = true;
        }

        protected override void CreateMapEntries()
        {
            AddMapEntry(new(126, 40, 48), MapEntryName);
        }

        protected override int ItemType(Tile tile)
        {
            return ModContent.ItemType<PressurizedExtractorItem>();
        }
    }
}
