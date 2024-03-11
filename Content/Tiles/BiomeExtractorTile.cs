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
		protected abstract BiomeExtractorEnt getTileEntity();

		public override void SetStaticDefaults()
		{
			Main.tileLavaDeath[Type] = true;
            DustType = DustID.Iron;
            // set Main.tileLavaDeath[Type] = true; for low tiers
            
            TileObjectData.newTile.Width  = 3;
            TileObjectData.newTile.Height = 3;

            TileObjectData.newTile.Origin = new Point16(0, 2); // Bottom-left
            TileObjectData.newTile.CoordinateHeights = [16, 16, 18]; // Extend into grass tiles.
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(getTileEntity().Hook_AfterPlacement, -1, 0, false);
			TileObjectData.newTile.UsesCustomCanPlace = true;

            TileObjectData.addTile(Type);

            TileID.Sets.PreventsSandfall[Type] = true;
            TileID.Sets.AvoidedByMeteorLanding[Type] = true;
        }

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
            // ModTileEntity.Kill() handles checking if the tile entity exists and destroying it if it does exist in the world for you
            // The tile coordinate parameters already refer to the top-left corner of the multitile
            ModContent.GetInstance<BiomeExtractorEnt>().Kill(i, j);
        }
	}
}