using BiomeExtractorsMod.Content.TileEntities;
using BiomeExtractorsMod.Content.Tiles;
using BiomeExtractorsMod.Calamity.Content.Items;
using BiomeExtractorsMod.Calamity.Content.TileEntities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BiomeExtractorsMod.Calamity.Content.Tiles
{
    [ExtendsFromMod("CalamityMod")]
    [JITWhenModsEnabled("CalamityMod")]
    internal class SpectralExtractorTile : BiomeExtractorTile
    {
        protected override int FrameCount => 6;
        protected override int IdleFrame => 6;
        protected override int FrameDuration => 10;
        protected override string GlowAsset => "Calamity/Content/Tiles/SpectralExtractorTile_Glow";

        protected override BiomeExtractorEnt TileEntity => ModContent.GetInstance<SpectralExtractorEnt>();

        protected override void SetupTileData()
        {
            DustType = DustID.UltraBrightTorch;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.LavaDeath = false;
        }

        protected override void CreateMapEntries()
        {
            AddMapEntry(new(35, 200, 254), MapEntryName);
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawPos = zero + 16f * new Vector2(i, j) - Main.screenPosition;
            int y = tile.TileFrameY + GetAnimationFrame(Type, i, j) * FrameHeight;
            Rectangle frame = new(tile.TileFrameX, y, 16, 16);
            Color lightColor = Lighting.GetColor(i, j, Color.White);
            Color color = Color.Lerp(Color.White, lightColor, 0.5f);
            spriteBatch.Draw(Mod.Assets.Request<Texture2D>(GlowAsset).Value, drawPos, frame, color);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            bool found = TileUtils.TryGetTileEntityAs(i, j, out BiomeExtractorEnt entity);
            if (!found || !entity.Active)
            {
                r = 0.077f;
                g = 0.097f;
                b = 0.100f;
                return;
            }
            r = 0.77f;
            g = 0.97f;
            b = 1.00f;
        }

        protected override int ItemType(Tile tile)
        {
            return ModContent.ItemType<SpectralExtractorItem>();
        }
    }
}
