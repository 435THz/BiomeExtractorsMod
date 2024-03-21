using BiomeExtractorsMod.Common;
using BiomeExtractorsMod.Content.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace BiomeExtractorsMod.Content.TileEntities
{
    public abstract class BiomeExtractorEnt : ModTileEntity
    {
        public enum EnumTiers
        {
            BASIC = 0, DEMONIC = 1, INFERNAL = 2, STEAMPUNK = 3, CYBER = 4, LUNAR = 5, ETHEREAL = 6
        }

        private static readonly string tagTimer  = "timer";
        private static readonly string tagChestX = "chest_x";
        private static readonly string tagChestY = "chest_y";
        private static readonly int[] chestOffsetY    = [1, -1, 0, 2];
        private static readonly int[] chestOffsetX_fw = [-2, 3];
        private static readonly int[] chestOffsetX_bw = [3, -2];

        private int timer = 0;
        private Point chestPos;

        private int chestIndex;

        public int TimeElapsed
        {
            get => timer;
            protected set { timer = (value + ExtractionSpeed) % ExtractionSpeed; }
        }

        public int ExtractionSpeed { get => getSelfMaxTimer(); }
        public int ExtractionChance { get => getSelfChance(); }

        public abstract int GetTier();
        protected abstract int getSelfMaxTimer();
        protected abstract int getSelfChance();

        public override void SaveData(TagCompound tag)
        {
            tag.Add(tagTimer, TimeElapsed);
            tag.Add(tagChestX, Main.chest[chestIndex].x);
            tag.Add(tagChestY, Main.chest[chestIndex].y);
        }

        public override void LoadData(TagCompound tag)
        {
            TimeElapsed = tag.GetAsInt(tagTimer);
            chestPos = new Point(tag.GetAsInt(tagChestX), tag.GetAsInt(tagChestY));
            chestIndex = Chest.FindChest(chestPos.X, chestPos.Y);
        }

        public override void Update()
        {
            //Every time the timer wraps back to 0 the extraction routine is performed
            TimeElapsed++;
            if (TimeElapsed == 0)
            {
                //Discard the chest if it is not valid anymore
                if(!IsChestDataValid(chestPos, chestIndex)) {

                //If the chest data is not valid, a new one will be searched for
                    int index = getAdjacentChest();
                    if (index < 0)
                        return;
                    chestIndex = index;
                    chestPos = new Point(Main.chest[index].x, Main.chest[index].y);
                }
                
                Item generated = BiomeExtraction.GenerateItem(this);
                BiomeExtraction.AddToChest(generated, Main.chest[chestIndex]);
            }
        }

        private bool IsChestDataValid(Point pos, int index)
        {
            if (index < 0 || index >= 8000) return false; //if the index is invalid
            Chest chest = Main.chest[index];
            if (chest == null) return false; //if the chest is null
            if (chest.x != pos.X || chest.y != pos.Y) return false; //if the chest position doesn't match
            for (int Xi = 0; Xi < 2; Xi++) 
            {
                for (int Yi = 0; Yi < 4; Yi++)
                {
                    int X = Position.X + chestOffsetX_fw[Xi];
                    int Y = Position.Y + chestOffsetY[Yi];

                    if (chest.x == X && chest.y == Y && IsUnlocked(chest))
                    {
                        return true;
                    }
                }
            }
            return false; //if the position is invalid or the chest is locked
        }

        //Looks for a chest in the adjacent spaces to the left or right of this machine
        private int getAdjacentChest()
        {
            bool prioritizeLeft = Main.rand.NextBool();
            int[] chestOffsetX = prioritizeLeft ? chestOffsetX_fw : chestOffsetX_bw;

            int[] chests = [-1, -1, -1, -1, -1, -1, -1, -1];
            for (int i = 0; i < 8000; i++)
            {
                Chest chest = Main.chest[i];
                if (chest != null)
                {
                    for (int Yi = 0; Yi < 4; Yi ++)
                    {
                        for (int Xi = 0; Xi < 2; Xi ++)
                        {
                            int X = Position.X + chestOffsetX[Xi];
                            int Y = Position.Y + chestOffsetY[Yi];
                            int priority = Xi + Yi*2;

                            if (chest.x == X && chest.y == Y && IsUnlocked(chest))
                            {
                                chests[priority] = i;
                            }
                        }
                    }
                }
            }
            for(int i = 0; i<chests.Length; i++)
            {
                if (chests[i] >= 0) return chests[i];
            }
            return -1;
        }

        //Returns whether the queried chest is unlocked or not
        private bool IsUnlocked(Chest ch)
        {
            return !Chest.IsLocked(ch.x, ch.y);
        }




        // Spawning related code
        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == getTileType();
        }

        protected abstract int getTileType();

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                int width = 3;
                int height = 3;
                NetMessage.SendTileSquare(Main.myPlayer, i, j, width, height);

                NetMessage.SendData(MessageID.TileEntityPlacement, number: i, number2: j, number3: Type);
            }
            
            Point16 tileOrigin = BiomeExtractorTile.origin;
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