using BiomeExtractorsMod.Common.UI;
using BiomeExtractorsMod.Content.Items;
using BiomeExtractorsMod.Content.TileEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace BiomeExtractorsMod.Content.Tiles
{
    /// <summary>
    /// The core Tile class implemented by all BiomeExtractors. It handles wire and player interactions.
    /// </summary>
    public abstract class BiomeExtractorTile : ModTile
	{
        internal static Point16 origin = new(1, 2); // Bottom-center
        /// <summary>
        /// Returns the number of animation frames this Extractor has.
        /// </summary>
        protected abstract int FrameCount { get; }
        /// <summary>
        /// Returns the animation index used by the tile when inactive.
        /// </summary>
        protected virtual int IdleFrame { get; } = 0;
        /// <summary>
        /// Returns the duration, in game frames, of this Extractor's animation frames.
        /// </summary>
        protected virtual int FrameDuration => 5; //12FPS

        /// <summary>
        /// Returns the height of this Extractor's sprite.<br/>
        /// Useful if a sprite has an overhang meant to patch up holes in uneven terrain sprites.
        /// </summary>
        protected virtual int FrameHeight => 54;

        /// <summary>
        /// Returns the path to this tile's glowmask asset. If "", no glowmask will be applied.
        /// The default value is ""
        /// </summary>
        protected virtual string GlowAsset => "";

        /// <summary>
        /// Returns the template instance of this Extractor's TileEntity type (not the clone/new instance it is bound to during gameplay)
        /// </summary>
        protected abstract BiomeExtractorEnt TileEntity { get; }
        internal BiomeExtractorEnt GetTileEntity => TileEntity;

        /// <summary>
        /// Returns the id of the BiomeExtractorItem this Tile is bound to.<br/>
        /// It may have different results if the tile has multiple styles.
        /// </summary>
        protected abstract int ItemType(Tile tile);

        /// <summary>
        /// Called inside <see cref="SetStaticDefaults"/>. Its only purpose should be to call AddMapEntry
        /// as many times as it is necessary.<br/>
        /// <see cref="GetMapOption(int, int)"/> is already set to use the MapEntry corresponding to the
        /// tile style, but can be changed freely if more complex behavior is desired or needed.
        /// </summary>
        protected abstract void CreateMapEntries();
        public override ushort GetMapOption(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            return (ushort)TileStyle(tile);
        }

        protected static LocalizedText MapEntryName { get; } = Language.GetText(BiomeExtractorsMod.LocMapTileName);

        /// <summary>
        /// Returns the amount of tile styles this Tile has.
        /// </summary>
        protected virtual int _tileStyles => 1;
        protected internal int TileStyles(Tile tile) => tile.TileType == Type ? _tileStyles : -1;

        protected int TileStyle(Tile tile)
        {
            return TileObjectData.GetTileStyle(tile);
        }

        public override void SetStaticDefaults()
        {
            Main.tileNoAttach[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.Width  = 3;
            TileObjectData.newTile.Height = 3;

            TileObjectData.newTile.Origin = origin;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(TileEntity.Hook_AfterPlacement, -1, 0, false);
			TileObjectData.newTile.UsesCustomCanPlace = true;

            TileObjectData.newTile.StyleHorizontal = true;

            SetupTileData();
            TileObjectData.addTile(Type);

            CreateMapEntries();

            TileID.Sets.PreventsSandfall[Type] = true;
            TileID.Sets.AvoidedByMeteorLanding[Type] = true;
        }

        /// <summary>
        /// Called just before TileObjetData.NewTile is added. Make changes to tile behavior here.<br/>
        /// See the base method to know what happens here normally and <see cref="SetStaticDefaults"/> to<br/>
        /// know what is done regardless of this method being overridden.
        /// </summary>
        protected virtual void SetupTileData()
        {
            //DustType should be preferred if overriding CreateDust is not necessary

            //Main.tileObsidianKill[Type] = true; before Lunar. Not enforced to let addons make their own rules.
            //TileObjectData.newTile.LavaDeath = true; before Infernal. Not enforced to let addons make their own rules.
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameYOffset = GetAnimationFrame(type, i, j) * FrameHeight;
        }
        protected int GetAnimationFrame(int type, int i, int j)
        {
            bool found = TileUtils.TryGetTileEntityAs(i, j, out BiomeExtractorEnt entity);
            if (!found || !entity.Active)
            {
                return IdleFrame;
            }
            int x = entity.Position.X;
            int y = entity.Position.Y;
            //ultra spicy position-dependent animation desyncs right here
            int frame = Main.tileFrame[type] + x + y;
            if (x % 2 == 0) frame++;
            if (y % 3 == 0) frame++;
            if (y % 4 == 0) frame += 2;
            if (x % y == 0 || y % x == 0) frame += 3;
            return frame % FrameCount;
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (++frameCounter >= FrameDuration)
            {
                frameCounter = 0;
                frame = ++frame % FrameCount;
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (GlowAsset == "") return;
            bool found = TileUtils.TryGetTileEntityAs(i, j, out BiomeExtractorEnt entity);
            if (!found || !entity.Active)
                return;
            Tile tile = Main.tile[i, j];
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawPos = zero + 16f * new Vector2(i, j) - Main.screenPosition;
            int y = tile.TileFrameY + GetAnimationFrame(Type, i, j) * FrameHeight;
            Rectangle frame = new(tile.TileFrameX, y, 16, 16);
            Color lightColor = Lighting.GetColor(i, j, Color.White);
            Color color = Color.Lerp(Color.White, lightColor, 0.5f);
            spriteBatch.Draw(Mod.Assets.Request<Texture2D>(GlowAsset).Value, drawPos, frame, color);
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[i, j];
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ItemType(tile);
            player.noThrow = 2;

            base.MouseOver(i, j);
        }

        public override bool RightClick(int i, int j)
        {
            if (Main.LocalPlayer.HeldItem.type == ModContent.GetInstance<BiomeScanner>().Type) return false;
            if (TileUtils.TryGetTileEntityAs(i, j, out BiomeExtractorEnt entity))
            {
                ModContent.GetInstance<UISystem>().OpenExtractorInterface(entity);
                return true;
            }
            Main.NewText(Language.GetTextValue($"{BiomeExtractorsMod.LocDiagnostics}.MachineStateBroken"));
            return false;
        }

        public override void HitWire(int i, int j)
        {
            if (TileUtils.TryGetTileEntityAs(i, j, out BiomeExtractorEnt entity))
            {
                Point16 top_left = entity.Position;
                entity.ToggleState();
                UISystem ui = ModContent.GetInstance<UISystem>();
                if (ui?.Extractor == entity) ui.Interface.OnActivate();
                for (short x_off = 0; x_off < 3; x_off++)
                {
                    for (short y_off = 0; y_off < 3; y_off++)
                    {
                        Wiring.SkipWire(top_left.X + x_off, top_left.Y + y_off);
                    }
                }

                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    NetMessage.SendTileSquare(-1, top_left.X, top_left.Y, 3, 3);
                }
            }
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
            TileEntity.Kill(i, j);
        }
	}
}