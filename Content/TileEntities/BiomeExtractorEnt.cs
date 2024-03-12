using BiomeExtractorsMod.Content.Tiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Content.TileEntities
{
    public abstract class BiomeExtractorEnt : ModTileEntity
    {
        public abstract int GetTier();
        public abstract int GetExtractionSpeed();
        public abstract float GetExtractionChance();

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == ModContent.TileType<BiomeExtractorTile>();
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                // Sync the entire multitile's area. Modify "width" and "height" to the size of your multitile in tiles
                int width = 3;
                int height = 3;
                NetMessage.SendTileSquare(Main.myPlayer, i, j, width, height);

                NetMessage.SendData(MessageID.TileEntityPlacement, number: i, number2: j, number3: Type);
            }

            Point16 tileOrigin = new(1, 1);
            int placedEntity = Place(i - tileOrigin.X, j - tileOrigin.Y);
            return placedEntity;
        }

        public override void OnNetPlace()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
            }
        }
    }
}