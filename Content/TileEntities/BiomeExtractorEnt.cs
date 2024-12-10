using BiomeExtractorsMod.Common.Collections;
using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Common.Database;
using BiomeExtractorsMod.Common.Hooks;
using BiomeExtractorsMod.Common.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using static BiomeExtractorsMod.Common.Database.BiomeExtractionSystem;

namespace BiomeExtractorsMod.Content.TileEntities
{
    /// <summary>
    /// The core TileEntity class implemented by all BiomeExtractors. It contains most of the item generation and chest lookup logic.
    /// </summary>
    public abstract class BiomeExtractorEnt : ModTileEntity
    {
        /// <summary>
        /// A structure that contains map icon drawing data. Used to allow specific extractor types to override the tier icon.
        /// </summary>
        /// <param name="hoverTextKey">The name that will be displayed on the map when hovering this icon.</param>
        /// <param name="icon">The icon asset to be used.</param>
        /// <param name="file_columns"></param> The number of columnts the icon file is divided into.
        /// <param name="column">The specific column of the icon file that will be used for the icon.</param>
        public readonly struct ExtractorIconOverride(string hoverTextKey, Func<Asset<Texture2D>> icon, byte column, byte file_columns)
        {
            public static ExtractorIconOverride Invalid { get; private set; } = new ExtractorIconOverride();

            public ExtractorIconOverride() : this("", null, 0, 0) { }
            public readonly string HoverTextKey = hoverTextKey;
            private readonly Func<Asset<Texture2D>> _icon = icon;
            public Asset<Texture2D> Icon => _icon.Invoke();
            public readonly byte Column = column;
            public readonly byte FileColumns = file_columns;
        }
        internal struct OutData(OutputType type, Point point) : TagSerializable
        {
            public static readonly Func<TagCompound, OutData> DESERIALIZER = Load;

            public TagCompound SerializeData()
            {
                return new TagCompound
                {
                    ["Type"] = (byte)Type,
                    ["Point"] = Point.ToVector2()
                };
            }
            public static OutData Load(TagCompound tag)
            {
                return new OutData((OutputType)tag.GetByte("Type"), tag.Get<Vector2>("Point").ToPoint());
            }

            public OutputType Type { get; private set; } = type;
            public Point Point { get; private set; } = point;
            public OutData() : this(OutputType.NONE, new Point()) { }
        }

        internal enum OutputType
        { 
            NONE, CHEST, MS_ENVIRONMENTACCESS
        }

        private static readonly string tagXTimer  = "extraction_timer";
        private static readonly string tagBTimer  = "biome_scan_timer";
        private static readonly string tagState   = "isActive";
        private static readonly string tagOutputs = "outputList";
        private static readonly string tagFilter  = "filterList";
        private static readonly Point[] PosOffsets = [new(-1,2), new(3, 2), new(-1, 0), new(3, 0), new(0, -1), new(2, -1)];

        private int XTimer = 0;
        private int BTimer = 0;
        private List<OutData> outputs = new(6);
        public bool HasOutput => outputs.Count > 0;

        private int ChestIndex(Point chestPos) => Chest.FindChest(chestPos.X, chestPos.Y);

        protected List<PoolEntry> PoolList { get; private set; } = [];

        protected List<Item> Filter { get; private set; } = [];

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
        public bool Active
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

        internal Asset<Texture2D> MapIcon => MapIconAsset;

        /// <summary>
        /// If set, this extractor will have a custom icon that is different from the one normally used for its tier.
        /// </summary>
        protected internal virtual ExtractorIconOverride IconOverride => ExtractorIconOverride.Invalid;
        internal bool HasIconOverride => !IconOverride.Equals(ExtractorIconOverride.Invalid);
        /// <summary>
        /// Returns the tier of this Extractor.
        /// </summary>
        protected internal abstract ExtractionTier ExtractionTier { get; }

        protected internal int Tier => ExtractionTier.Tier;
        /// <summary>
        /// Returns the name that appears in the ui when opening right clicking this extractor.
        /// </summary>
        protected internal virtual string LocalName => ExtractionTier.Name;
        /// <summary>
        /// Returns the extraction rate of this extractor, in frames.
        /// </summary>
        protected internal virtual int ExtractionRate => ExtractionTier.Rate;
        /// <summary>
        /// Returns the extraction chance of this extractor.
        /// </summary>
        protected internal virtual int ExtractionChance => ExtractionTier.Chance;
        /// <summary>
        /// Returns the extraction amount of this extractor.
        /// </summary>
        protected internal virtual int ExtractionAmount => ExtractionTier.Amount;
        internal Asset<Texture2D> MapIconAsset => ExtractionTier.Icon;

        private static int BiomeScanRate => ModContent.GetInstance<ConfigCommon>().BiomeScanRate;

        //loading
        private protected void UpdatePoolList() {
            PoolList = Instance.CheckValidBiomes(ExtractionTier, Position + new Point16(1, 1));
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add(tagXTimer,  ExtractionTimer);
            tag.Add(tagBTimer,  ScanningTimer);
            tag.Add(tagState,   Active);
            tag.Add(tagOutputs, outputs);
            tag.Add(tagFilter,  Filter);
        }

        public override void LoadData(TagCompound tag)
        {
            tag.TryGet(tagXTimer, out int xtimer);
            tag.TryGet(tagBTimer, out int btimer);
            tag.TryGet(tagState,  out bool active);

            ExtractionTimer = xtimer;
            ScanningTimer   = btimer;
            Active          = active;
            outputs         = tag.GetList<OutData>(tagOutputs).ToList();
            Filter          = tag.GetList<Item>(tagFilter).ToList();
            UpdatePoolList(); //always run on loading
        }

        public override void Update()
        {
            if (!Active) { return; }
            //Every time the timer wraps back to 0 the extraction routine is performed
            int tick_increase = ConfigCommon.Instance.FollowDayRate == ConfigCommon.FollowRateValues.NO ? 1 : (int)Main.dayRate;
            int checks_this_frame = (ExtractionTimer + tick_increase) / ExtractionRate * ExtractionAmount;
            ExtractionTimer += tick_increase; //never run immediately upon placement
            if (ConfigCommon.Instance.FollowDayRate != ConfigCommon.FollowRateValues.YES)
                checks_this_frame = Math.Min(checks_this_frame, ExtractionAmount);
            int state_old = (int)IconMapSystem.StateOf(this);
            for (int check = 0; check < checks_this_frame; check++)
            {
                byte o = 0;
                for (byte i = 0; i < outputs.Count; i++)
                {
                    OutData output = outputs[i - o];
                    //Discard the output if it is not valid
                    if (!IsOutputDataValid(output))
                    {
                        outputs.RemoveAt(i - o);
                        o++;
                    }
                }
                if (outputs.Count == 0)
                {
                    //If no valid ouput exists, a new one will be searched for
                    OutData output = GetNewOutput();
                    if (output.Type != OutputType.NONE)
                        outputs.Add(output);
                }
                //if at least one valid output exists
                if (HasOutput)
                {
                    if (Main.rand.Next(100) < ExtractionChance)
                    {
                        // generate the new item
                        Item generated = Instance.RollItem(PoolList);
                        for (int i = 0; i < 6; i++)
                        {
                            //try to put it in an output 
                            OutData output = outputs[i];
                            if (AddToOutput(output, generated.Clone()))
                            {
                                //success. Let's stop
                                break;
                            }
                            else
                            {
                                //did we run out of outputs?
                                if (i + 1 >= outputs.Count)
                                {
                                    //look for a new one. If none are found, stop
                                    OutData newOutput = GetNewOutput();
                                    if (output.Type == OutputType.NONE)
                                        break;

                                    outputs.Add(newOutput);
                                }
                            }
                        }
                    }
                }
            }
            if (state_old != (int)IconMapSystem.StateOf(this))
                SendUpdatePacket(ServerMessageType.EXTRACTOR_UPDATE);

            //Every time this timer wraps back to 0 the scanning routine is performed
            if (ScanningTimer == 0)
            {
                UpdatePoolList();
                UISystem ui = ModContent.GetInstance<UISystem>();
                if (ui?.Extractor == this) ui.Interface.OnActivate();
            }
            ScanningTimer++; //always run immediately upon placement
        }

        private bool AddToOutput(OutData dest, Item newItem)
        {
            if (newItem.type == ItemID.None || FilterContains(newItem)) return true;
            if (BiomeExtractorsMod.MS_loaded && dest.Type == OutputType.MS_ENVIRONMENTACCESS) return AddToStorage(newItem, dest.Point);
            if (dest.Type == OutputType.CHEST) return AddToChest(newItem, dest.Point);
            return false;
        }
        private bool AddToStorage(Item newItem, Point storagePos)
        {
            if (MagicStorageHook.AddItemToStorage(newItem, storagePos) && newItem.stack > 0)
            {
                AnimateItemMovement(storagePos, newItem);
            }
            return true;
        }

        private bool AddToChest(Item newItem, Point chestPos)
        {
            Item clone = newItem.Clone();
            Chest chest = Main.chest[ChestIndex(chestPos)];

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
                    newItem.stack = Math.Max(newItem.stack, 0);
                    break;
                }
            }
            
            clone.stack = starting - newItem.stack;
            if (clone.stack > 0)
            {
                AnimateItemMovement(chestPos, clone);
                return true;
            }
            return false;
        }

        private void AnimateItemMovement(Point target, Item item)
        {
            if (ModContent.GetInstance<ConfigCommon>().ShowTransferAnimation)
            {
                Vector2 start = (Position.ToVector2() + new Vector2(1.5f, 1.5f)) * 16;
                Vector2 end = (target.ToVector2() + Vector2.One) * 16;
                Chest.VisualizeChestTransfer(start, end, item, 1);
            }
        }

        private bool IsOutputDataValid(OutData output)
        {
            if (BiomeExtractorsMod.MS_loaded && output.Type == OutputType.MS_ENVIRONMENTACCESS)
                return MagicStorageHook.IsOutputValid(output.Point);
            if (output.Type == OutputType.CHEST)
                return IsChestValid(output.Point);
            return false;
        }

        private bool IsChestValid(Point chestPos)
        {
            int index = ChestIndex(chestPos);
            if (index < 0) return false; //if the chest does not exist
            Chest chest = Main.chest[index];
            if (!IsUnlocked(chest)) return false; //if the chest is locked

            for (int i = 0; i < 6; i++)
            {
                Point pos = PosOffsets[i];
                int X = Position.X + pos.X;
                int Y = Position.Y + pos.Y;

                if (chestPos.X > X - 2 && chestPos.X <= X && chestPos.Y > Y - 2 && chestPos.Y <= Y)
                {
                    return true;
                }
            }
            return false; //if the position is invalid
        }

        private OutData GetNewOutput()
        {
            if (BiomeExtractorsMod.MS_loaded)
            {
                OutData output = GetAdjacentMSAccess();
                if (output.Type != OutputType.NONE) return output;
            }
            return GetAdjacentChest();
        }

        //Looks for a Storage Configuration Interface in the adjacent spaces to the left, right or top of this machine
        private OutData GetAdjacentMSAccess()
        {
            bool prioritizeLeft = Position.X % 2 == 0;

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
            
            bool prioritizeLeft = Position.X%2==0;

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
                    bool newChest = true;
                    foreach (OutData output in outputs)
                    {
                        if (output.Point == new Point(chest.x, chest.y)) { newChest = false; }
                    }
                    if (newChest)
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
        public void ToggleState() {
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

        internal bool FilterContains(Item item)
        {
            for (int i = 0; i < Filter.Count; i++)
            {
                if (Filter[i].type == item.type) return true;
            }
            return false;
        }

        internal void RemoveFilter(Item item)
        {
            for (int i = 0; i <= Filter.Count; i++)
            {
                if (Filter[i].type == item.type)
                {
                    Filter.RemoveAt(i);
                    return;
                }
            }
        }

        internal void AddFilter(Item item)
        {
            foreach (Item item2 in Filter)
            {
                if (item2.type == item.type)
                {
                    return;
                }
            }
            Filter.Add(item);
        }

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
            writer.Write(Active);
            writer.Write((byte)outputs.Count);
            for (int i = 0; i < outputs.Count; i++)
            {
                writer.Write((byte)outputs[i].Type);
                writer.Write(outputs[i].Point.X);
                writer.Write(outputs[i].Point.Y);
            }
        }

        public override void NetReceive(BinaryReader reader)
        {
            Active = reader.ReadBoolean();
            byte len = reader.ReadByte();
            outputs = new List<OutData>(6);
            for (int i = 0; i < len; i++)
            {
                outputs.Add(new OutData((OutputType)reader.ReadByte(), new Point(reader.ReadInt32(), reader.ReadInt32())));
            }
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