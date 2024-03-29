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
        internal static Point16 origin = new Point16(1, 2); // Bottom-center

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
            TileObjectData.newTile.CoordinateHeights = [16, 16, 18]; // Extend into grass tiles.
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetTileEntity().Hook_AfterPlacement, -1, 0, false);
			TileObjectData.newTile.UsesCustomCanPlace = true;

            TileObjectData.addTile(Type);

            TileID.Sets.PreventsSandfall[Type] = true;
            TileID.Sets.AvoidedByMeteorLanding[Type] = true;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
            GetTileEntity().Kill(i, j);
        }
	}
}