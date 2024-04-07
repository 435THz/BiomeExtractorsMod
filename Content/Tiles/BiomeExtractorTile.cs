using BiomeExtractorsMod.Content.TileEntities;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace BiomeExtractorsMod.Content.Tiles
{

    public abstract class BiomeExtractorTile : ModTile
	{
        internal static Point16 origin = new(1, 2); // Bottom-center

        protected abstract BiomeExtractorEnt GetTileEntity();

		public override void SetStaticDefaults()
		{
			Main.tileLavaDeath[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileFrameImportant[Type] = true;
            // set Main.tileLavaDeath[Type] = true; for low tiers

            DustType = DustID.Iron;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.Width  = 3;
            TileObjectData.newTile.Height = 3;

            TileObjectData.newTile.Origin = origin;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16]; // Extend into grass tiles?
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetTileEntity().Hook_AfterPlacement, -1, 0, false);
			TileObjectData.newTile.UsesCustomCanPlace = true;

            TileObjectData.addTile(Type);

            TileID.Sets.PreventsSandfall[Type] = true;
            TileID.Sets.AvoidedByMeteorLanding[Type] = true;
        }

        public override bool RightClick(int i, int j)
        {
            bool found = TileUtils.TryGetTileEntityAs(i, j, out BiomeExtractorEnt entity);
            if (found)
            {
                entity.DisplayStatus();
                return true;
            }
            return false;
        }

        public override void HitWire(int i, int j)
        {
            bool found = TileUtils.TryGetTileEntityAs(i, j, out BiomeExtractorEnt entity);
            Point16 top_left = entity.Position;
            if (found)
            {
                entity.ToggleState();
                for (short x_off = 0; x_off < 3; x_off++)
                {
                    for (short y_off = 0; y_off < 3; y_off++)
                    {
                        //TODO change visuals
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
            GetTileEntity().Kill(i, j);
        }
	}
}