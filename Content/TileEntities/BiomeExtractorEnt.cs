using BiomeExtractorsMod.Common;
using BiomeExtractorsMod.Content.Tiles;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace BiomeExtractorsMod.Content.TileEntities
{
    public abstract class BiomeExtractorEnt : ModTileEntity
    {
        private static string tagTimer = "timer";
        private static readonly int[] chestOffsetY = [1, -1, 0, 2];
        private static readonly int[] chestOffsetX = [-2, 3];
        public enum EnumTiers
        {
            BASIC = 0, DEMONIC = 1, INFERNAL = 2, STEAMPUNK = 3, CYBER = 4, LUNAR = 5, ETHEREAL = 6
        }


        private int timer = 0;

        public int TimeElapsed
        {
            get => timer;
            protected set { timer = (value + ExtractionSpeed) % ExtractionSpeed; }
        }

        public int ExtractionSpeed {
            get
            {
                return getSelfMaxTimer();
            }
        }
        public int ExtractionChance
        {
            get
            {
                return getSelfChance();
            }
        }

        protected abstract int GetTier();
        protected abstract int getSelfMaxTimer();
        protected abstract int getSelfChance();

        public override void SaveData(TagCompound tag)
        {
            tag.Add(tagTimer, TimeElapsed);
        }

        public override void LoadData(TagCompound tag)
        {
            int t;
            try { t = tag.GetAsInt(tagTimer); }
            catch(Exception) { t = 0; }
            TimeElapsed = t;
        }

        public override void Update()
        {

            TimeElapsed++;
            if (TimeElapsed == 0)
            {
                Chest chest = getAdjacentChest();
                if(chest == null)
                {
                    TimeElapsed--;
                    return;
                }

                Item generated = BiomeExtraction.generateItem(GetTier());
                bool success = BiomeExtraction.AddToChest(generated, chest);
                if(!success) TimeElapsed--;
            }
        }

        private Chest getAdjacentChest()
        {

            bool prioritizeLeft = Main.rand.NextBool();
            int start = prioritizeLeft ? 0 :  1;
            int shift = prioritizeLeft ? 1 : -1;

            for (int i = 0; i < 8000; i++)
            {
                Chest chest = Main.chest[i];
                if (chest != null)
                {
                    for(int Xi = start; i>=0 && i<2; i+=shift)
                    {
                        for (int Yi = 0; i < 4; i ++)
                        {
                            int X = Position.X + chestOffsetX[Xi];
                            int Y = Position.Y + chestOffsetY[Yi];

                            if (chest.x == X && chest.y == Y)
                            {
                                return chest;
                            }
                        }
                    }
                }
            }
            return null;
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