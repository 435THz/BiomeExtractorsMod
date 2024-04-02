using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Common.Systems;
using BiomeExtractorsMod.Content.Tiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
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

        private static readonly string tagXTimer = "extraction_timer";
        private static readonly string tagBTimer = "biome_scan_timer";
        private static readonly string tagState  = "isActive";
        private static readonly string tagChestX = "chest_x";
        private static readonly string tagChestY = "chest_y";
        private static readonly int[] chestOffsetY    = [1, -1, 0, 2];
        private static readonly int[] chestOffsetX_fw = [-2, 3];
        private static readonly int[] chestOffsetX_bw = [3, -2];


        private int XTimer = 0;
        private int BTimer = 0;
        private Point chestPos;

        private int chestIndex;
        protected List<string> PoolList { get; private set; } = [];

        public int ExtractionTimer
        {
            get => XTimer;
            protected set { XTimer = (value + ExtractionRate) % ExtractionRate; }
        }
        public int ScanningTimer
        {
            get => BTimer;
            protected set { BTimer = (value + BiomeScanRate) % BiomeScanRate; }
        }
        public bool Active { get; private set; } = true;

        // getter only
        protected abstract int TileType { get; }
        public abstract int Tier { get; }
        public abstract int ExtractionRate { get; }
        public abstract int ExtractionChance { get; }
        public static int BiomeScanRate { get => ModContent.GetInstance<ExtractorConfig>().BiomeScanRate; }

        public override void SaveData(TagCompound tag)
        {
            tag.Add(tagXTimer, ExtractionTimer);
            tag.Add(tagBTimer, ScanningTimer);
            tag.Add(tagState,  Active);
            tag.Add(tagChestX, Main.chest[chestIndex].x);
            tag.Add(tagChestY, Main.chest[chestIndex].y);
        }

        public override void LoadData(TagCompound tag)
        {
            ExtractionTimer = tag.GetAsInt(tagXTimer);
            ScanningTimer   = tag.GetAsInt(tagBTimer);
            Active          = tag.GetBool(tagState);
            chestPos        = new Point(tag.GetAsInt(tagChestX), tag.GetAsInt(tagChestY));
            chestIndex      = Chest.FindChest(chestPos.X, chestPos.Y);
            PoolList        = ModContent.GetInstance<BiomeExtractionSystem>().CheckValidBiomes(this); //always run on loading
        }

        public override void Update()
        {
            if (!Active) { return; }
            //Every time the timer wraps back to 0 the extraction routine is performed
            ExtractionTimer++; //never run immediately upon placement
            if (ExtractionTimer == 0)
            {
                //Discard the chest if it is not valid anymore
                if(!IsChestDataValid(chestPos, chestIndex)) {

                //If the chest data is not valid, a new one will be searched for
                    int index = GetAdjacentChest();
                    if (index < 0)
                        return;
                    chestIndex = index;
                    chestPos = new Point(Main.chest[index].x, Main.chest[index].y);
                }
                
                Item generated = ModContent.GetInstance<BiomeExtractionSystem>().RollItem(PoolList);
                AddToChest(generated);
            }

            //Every time this timer wraps back to 0 the scanning routine is performed
            if (ScanningTimer == 0)
            {
                PoolList = ModContent.GetInstance<BiomeExtractionSystem>().CheckValidBiomes(this);
            }
            ScanningTimer++; //always run immediately upon placement
        }


        private bool AddToChest(Item newItem)
        {
            Chest chest = Main.chest[chestIndex];

            int starting = newItem.stack;
            for (int inventoryIndex = 0; inventoryIndex < Chest.maxItems; inventoryIndex++)
            {
                Item item = chest.item[inventoryIndex];
                if (item.type == ItemID.None)
                {
                    item.SetDefaults(newItem.type);
                    newItem.stack -= item.stack;
                }
                if (item.type == newItem.type)
                {
                    int diff = item.maxStack - item.stack;
                    int transfer = Math.Min(diff, newItem.stack);
                    newItem.stack -= transfer;
                    item.stack += transfer;
                }
                if (newItem.stack < 1)
                {
                    return true;
                }
            }
            return starting != newItem.stack;
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
        private int GetAdjacentChest()
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
        private static bool IsUnlocked(Chest ch)
        {
            return !Chest.IsLocked(ch.x, ch.y);
        }

        //toggles the machine on and off
        internal void ToggleState() {
            Active = !Active;
            if(Active) PoolList = ModContent.GetInstance<BiomeExtractionSystem>().CheckValidBiomes(this); //must refresh when turned back on
        }

        // displays the machine's status in chat
        internal void DisplayStatus()
        {
            if (Active)
            {
                PoolList = ModContent.GetInstance<BiomeExtractionSystem>().CheckValidBiomes(this); //must refresh on right click
                ScanningTimer = 1; //we reset the timer as well
                if (PoolList.Count > 0)
                {
                    string s = "";
                    for (int i = 0; i < PoolList.Count; i++)
                    {
                        s += PoolList[i];
                        if (i < PoolList.Count - 1) s += ", ";
                    }
                    Main.NewText("The machine is extracting resources from the following biomes:\n" +
                        s);
                    if(ModContent.GetInstance<ExtractorClient>().DiagnosticPrint)
                        ModContent.GetInstance<BiomeExtractionSystem>().PrintDiagnostics(this, PoolList);
                }
                else
                    Main.NewText("The machine could not extract anything from this place.");
            }
            else
                Main.NewText("The machine is inactive.");
        }



        // Spawning related code
        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == TileType;
        }

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