using BiomeExtractorsMod.Common;
using BiomeExtractorsMod.Common.Collections;
using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Common.UI;
using BiomeExtractorsMod.CrossMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using static BiomeExtractorsMod.Common.Systems.BiomeExtractionSystem;

namespace BiomeExtractorsMod.Content.TileEntities
{
    /// <summary>
    /// The core TileEntity class implemented by all BiomeExtractors. It contains most of the item generation and chest lookup logic.
    /// </summary>
    public abstract class BiomeExtractorEnt : ModTileEntity
    {
        private struct OutData(OutputType type, Point point)
        {
            public OutputType Type { get; private set; } = type;
            public Point Point { get; private set; } = point;
            public OutData() : this(OutputType.NONE, new Point()) { }
        }

        /// <summary>
        /// A list of all Tier values used by the mod. They have huge gaps in between each other for addons to fit in.
        /// </summary>
        public enum EnumTiers
        {
            BASIC = 1, DEMONIC = 1000, INFERNAL = 2000, STEAMPUNK = 3000, CYBER = 4000, LUNAR = 5000, ETHEREAL = 6000
        }

        internal enum OutputType
        { 
            NONE, CHEST, MS_ENVIRONMENTACCESS
        }

        private static readonly string tagXTimer = "extraction_timer";
        private static readonly string tagBTimer = "biome_scan_timer";
        private static readonly string tagState = "isActive";
        private static readonly string tagTType = "targetType";
        private static readonly string tagOutputX = "chest_x";
        private static readonly string tagOutputY = "chest_y";
        private static readonly Point[] PosOffsets = [new(-1,2), new(3, 2), new(-1, 0), new(3, 0), new(0, -1), new(2, -1)];

        private int XTimer = 0;
        private int BTimer = 0;
        private Point outputPos;
        private OutputType _outType;
        internal OutputType OutType
        {
            get => _outType;
            set
            {
                _outType = value;
                SendUpdatePacket(ServerMessageType.EXTRACTOR_UPDATE);
            }
        }
        public bool HasOutput => OutType != OutputType.NONE;

        private int ChestIndex { get => Chest.FindChest(outputPos.X, outputPos.Y); }

        protected List<PoolEntry> PoolList { get; private set; } = [];

        protected int ExtractionTimer
        {
            get => XTimer;
            set { XTimer = (value + ExtractionRate) % ExtractionRate; }
        }
        protected int ScanningTimer
        {
            get => BTimer;
            set { BTimer = (value + BiomeScanRate) % BiomeScanRate; }
        }
        private bool _active = true;
        internal bool Active
        {
            get => _active;
            set
            {
                _active = value;
                SendUpdatePacket(ServerMessageType.EXTRACTOR_UPDATE);
            }
        }


        /// <summary>
        /// Returns the id of the BiomeExtractorTile this Entity is bound to.
        /// </summary>
        protected internal abstract int TileType { get; }
        internal int TileStyle => TileObjectData.GetTileStyle(Main.tile[Position]);

        internal Asset<Texture2D> MapIcon => Mod.Assets.Request<Texture2D>(MapIconAsset);

        /// <summary>
        /// Returns the tier of this Extractor.
        /// </summary>
        protected internal abstract ExtractionTier ExtractionTier { get; }

        protected internal int Tier => ExtractionTier.Tier;
        protected internal string LocalName => ExtractionTier.Name;
        protected internal int ExtractionRate => ExtractionTier.Rate;
        protected internal int ExtractionChance => ExtractionTier.Chance;
        internal string MapIconAsset => ExtractionTier.IconPath;

        private static int BiomeScanRate => ModContent.GetInstance<ConfigCommon>().BiomeScanRate;

        //loading
        private void UpdatePoolList() {
            PoolList = Instance.CheckValidBiomes(ExtractionTier, Position + new Point16(1, 1));
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add(tagXTimer, ExtractionTimer);
            tag.Add(tagBTimer, ScanningTimer);
            tag.Add(tagState,  Active);
            tag.Add(tagTType,  (int)OutType);
            tag.Add(tagOutputX, outputPos.X);
            tag.Add(tagOutputY, outputPos.Y);
        }

        public override void LoadData(TagCompound tag)
        {
            tag.TryGet(tagXTimer, out int xtimer);
            tag.TryGet(tagBTimer, out int btimer);
            tag.TryGet(tagState, out bool active);
            tag.TryGet(tagOutputX, out int x);
            tag.TryGet(tagOutputY, out int y);
            tag.TryGet(tagTType, out int type);

            ExtractionTimer = xtimer;
            ScanningTimer   = btimer;
            Active          = active;
            outputPos       = new Point(x, y);
            OutType      = (OutputType)type;
            UpdatePoolList(); //always run on loading
        }

        public override void Update()
        {
            if (!Active) { return; }
            //Every time the timer wraps back to 0 the extraction routine is performed
            ExtractionTimer++; //never run immediately upon placement
            if (ExtractionTimer == 0)
            {
                //Discard the output if it is not valid
                if (!IsOutputDataValid()) {

                    //If the output data is not valid, a new one will be searched for
                    OutData output = GetNewOutput();
                    OutType = output.Type;
                    outputPos = output.Point;
                }

                if (Main.rand.Next(100) < ExtractionChance)
                {
                    Item generated = Instance.RollItem(PoolList);
                    if (AddToOutput(generated.Clone()) && ModContent.GetInstance<ConfigCommon>().ShowTransferAnimation)
                    {
                        Vector2 start = (Position.ToVector2() + new Vector2(1.5f, 1.5f)) * 16;
                        Vector2 end = (outputPos.ToVector2() + Vector2.One) * 16;
                        Chest.VisualizeChestTransfer(start, end, generated, 1);
                    }
                }
            }

            //Every time this timer wraps back to 0 the scanning routine is performed
            if (ScanningTimer == 0)
            {
                UpdatePoolList();
                UISystem ui = ModContent.GetInstance<UISystem>();
                if (ui?.Extractor == this) ui.Interface.OnActivate();
            }
            ScanningTimer++; //always run immediately upon placement
        }

        private bool AddToOutput(Item newItem)
        {
            if (BiomeExtractorsMod.MS_loaded && OutType == OutputType.MS_ENVIRONMENTACCESS) return MagicStorageHook.AddItemToStorage(newItem, outputPos);
            if (OutType == OutputType.CHEST) return AddToChest(newItem);
            return false;
        }

        private bool AddToChest(Item newItem)
        {
            Chest chest = Main.chest[ChestIndex];

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

        private bool IsOutputDataValid()
        {
            if (BiomeExtractorsMod.MS_loaded && OutType == OutputType.MS_ENVIRONMENTACCESS)
                return MagicStorageHook.IsOutputValid(outputPos);
            if (OutType == OutputType.CHEST)
                return IsChestValid();
            return false;
        }

        private bool IsChestValid()
        {
            int index = ChestIndex;
            if (index < 0) return false; //if the chest does not exist
            Chest chest = Main.chest[index];
            if (!IsUnlocked(chest)) return false; //if the chest is locked

            for (int i = 0; i < 6; i++)
            {
                Point pos = PosOffsets[i];
                int X = Position.X + pos.X;
                int Y = Position.Y + pos.Y;

                if (outputPos.X > X - 2 && outputPos.X <= X && outputPos.Y > Y - 2 && outputPos.Y <= Y)
                {
                    return true;
                }
            }
            return false; //if the position is invalid
        }

        private OutData GetNewOutput()
        {
            OutData output;
            if (BiomeExtractorsMod.MS_loaded)
            {
                output = GetAdjacentMSAccess();
                if (output.Type != OutputType.NONE) return output;
            }
            output = GetAdjacentChest();
            return output;
        }

        //Looks for a Storage Configuration Interface in the adjacent spaces to the left, right or top of this machine
        private OutData GetAdjacentMSAccess()
        {
            bool prioritizeLeft = Main.rand.NextBool();

            for (int i = 0; i < 6; i++)
            {
                int n = i;
                if (!prioritizeLeft) n += (i % 2 == 0 ? 1 : -1); //invert 2 by 2 to prioritize rightmost first

                Point pos = PosOffsets[n]+Position.ToPoint();
                if (BiomeExtractorsMod.MS_loaded && MagicStorageHook.IsTileEnvAccess(pos))
                {
                    Point accessPosition = MagicStorageHook.GetAccessPosition(pos);
                    return new(OutputType.MS_ENVIRONMENTACCESS, accessPosition);
                }
            }
            return new();
        }

        //Looks for a chest in the adjacent spaces to the left or right of this machine
        private OutData GetAdjacentChest()
        {
            bool prioritizeLeft = Main.rand.NextBool();

            Chest[] chests = new Chest[4];
            for (int i = 0; i < 8000; i++)
            {
                Chest chest = Main.chest[i];
                if (chest != null)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        int n = j;
                        if (!prioritizeLeft) n += (j % 2 == 0 ? 1 : -1); //invert 2 by 2 to prioritize rightmost first

                        Point pos = PosOffsets[n] + Position.ToPoint();

                        if (chest.x > pos.X - 2 && chest.x <= pos.X && chest.y > pos.Y - 2 && chest.y <= pos.Y && IsUnlocked(chest))
                        {
                            chests[j] = chest;
                        }
                    }
                }
            }
            for(int i = 0; i<chests.Length; i++)
            {
                if (chests[i] != default(Chest))
                {
                    Chest chest = chests[i];
                    return new(OutputType.CHEST, new(chest.x, chest.y));
                }
            }
            return new();
        }

        //Returns whether the queried chest is unlocked or not
        private static bool IsUnlocked(Chest ch)
        {
            return !Chest.IsLocked(ch.x, ch.y);
        }

        //toggles the machine on and off
        internal void ToggleState() {
            Active = !Active;
            if (Active) UpdatePoolList(); //must refresh when turned back on
        }

        // returns the machine's status message
        internal string GetStatus()
        {
            string baseText = $"{BiomeExtractorsMod.LocDiagnostics}.MachineState";
            if (Active)
            {
                UpdatePoolList(); //must refresh on right click
                ScanningTimer = 1; //we reset the timer as well
                if (PoolList.Count > 0)
                {
                    List<string> entries = [];
                    List<string> backup = [];
                    string s = "";
                    for (int i = 0; i < PoolList.Count; i++)
                    {
                        PoolEntry pool = PoolList[i];
                        backup.Add(pool.Name);
                        if (pool.IsLocalized())
                        {
                            string key = pool.LocalizationKey;
                            string entry = Language.GetTextValue(key);
                            if (entry != "" && !entries.Contains(entry)) entries.Add(entry);
                            else if (ModContent.GetInstance<ConfigClient>().DiagnosticPrintPools)
                                entries.Add(pool.Name);
                        }
                        else if (ModContent.GetInstance<ConfigClient>().DiagnosticPrintPools) {
                            entries.Add(pool.Name);
                        }
                    }

                    List<string> list = entries;
                    if (entries.Count == 0) list = backup;
                    for (int i = 0; i < list.Count; i++)
                    {
                        s += list[i];
                        if (i < list.Count - 1) s += ", ";
                    }

                    return Language.GetTextValue($"{baseText}Print") + " " + s;
                }
                else
                    return Language.GetTextValue($"{baseText}Fail");
            }
            else
                return Language.GetTextValue($"{baseText}Off");
        }

        internal WeightedList<ItemEntry> GetDropList() => Instance.JoinPools(PoolList);

        public override void OnKill() => SendUpdatePacket(ServerMessageType.EXTRACTOR_REMOVE);

        #region Netcode
        private readonly static List<int> msgSize = [9, 5, 4];
        internal void SendUpdatePacket(ServerMessageType msgType)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                byte msgId = (byte)msgType;
                ModPacket statePacket = ModContent.GetInstance<BiomeExtractorsMod>().GetPacket(msgSize[msgId]);
                statePacket.Write(msgId);
                statePacket.Write(Position.X);
                statePacket.Write(Position.Y);
                if (msgType != ServerMessageType.EXTRACTOR_REMOVE)
                {
                    if (msgType == ServerMessageType.EXTRACTOR_REGISTER) statePacket.Write(Tier);
                    statePacket.Write((byte)IconMapSystem.StateOf(this));
                }
                statePacket.Send();
                NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
            }
            else if (Main.netMode == NetmodeID.SinglePlayer)
            {
                switch (msgType)
                {
                    case ServerMessageType.EXTRACTOR_REGISTER: IconMapSystem.AddExtractorData(this);    break;
                    case ServerMessageType.EXTRACTOR_UPDATE:   IconMapSystem.UpdateExtractorData(this); break;
                    case ServerMessageType.EXTRACTOR_REMOVE:   IconMapSystem.RemoveExtractorData(this); break;
                }
                
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(outputPos.X);
            writer.Write(outputPos.Y);
            writer.Write((byte)OutType);
            writer.Write(Active);
        }

        public override void NetReceive(BinaryReader reader)
        {
            outputPos = new(reader.ReadInt32(), reader.ReadInt32());
            OutType = (OutputType)reader.ReadByte();
            Active = reader.ReadBoolean();
        }
        #endregion

        #region Placing
        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            bool valid = tile.HasTile && tile.TileType == TileType;
            if (!valid) SendUpdatePacket(ServerMessageType.EXTRACTOR_REMOVE);
            return valid;
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            Point16 tileOrigin = new(0, 0);
            int width = 3;
            int height = 3;
            var data = TileObjectData.GetTileData(TileType, 0);
            if (data != null)
            {
                tileOrigin = data.Origin;
                width = data.Width;
                height = data.Height;
            }

            int topLeftX = i - tileOrigin.X;
            int topLeftY = j - tileOrigin.Y;
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, topLeftX, topLeftY, width, height);
                NetMessage.SendData(MessageID.TileEntityPlacement, number: topLeftX, number2: topLeftY, number3: Type);
                return -1;
            }
            
            int placedEntity = Place(topLeftX, topLeftY);
            Position = new((short)topLeftX, (short)topLeftY);
            if (placedEntity != -1)
                SendUpdatePacket(ServerMessageType.EXTRACTOR_REGISTER);
            return placedEntity;
        }

        public override void OnNetPlace()
        {
            SendUpdatePacket(ServerMessageType.EXTRACTOR_REGISTER);
        }
        #endregion
    }
}