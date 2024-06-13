using BiomeExtractorsMod.Common.Collections;
using BiomeExtractorsMod.Content.TileEntities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace BiomeExtractorsMod.Common.Systems
{
    public class ScanData(BiomeExtractorEnt extractor)
    {

        private readonly BiomeExtractorEnt _extractor = extractor;
        private Point _origin = extractor.Position.ToPoint() + new Point(1, 1);
        private Point _size = new(Main.buffScanAreaWidth, Main.buffScanAreaHeight);
        private readonly Dictionary<int, int> _tileCounts = [];
        private readonly Dictionary<int, int> _liquidCounts = [];

        public float X => _origin.X;
        public float Y => _origin.Y;

        public int Tiles(int tileId) => _tileCounts.GetValueOrDefault(tileId);
        public int Liquids(int liquidId) => _liquidCounts.GetValueOrDefault(liquidId);
        public bool MinTier(int tier) => _extractor != null && _extractor.Tier >= tier;

        public int Tiles(List<ushort> tileIds)
        {
            int count = 0;
            foreach (ushort tileId in tileIds) count += Tiles(tileId);
            return count;
        }
        public bool ValidWalls(List<ushort> wallIds, bool blacklist = false)
        {
            Point origin = _extractor.Position.ToPoint();
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    bool match = blacklist;
                    foreach (int id in wallIds)
                    {
                        if (Main.tile[origin.X + i, origin.Y + j].WallType == id)
                            if (!blacklist) { match = true; break; }
                            else { return false; }
                    }
                    if (!match) return false;
                }
            return true;
        }

        public void Scan()
        {
            _tileCounts.Clear();
            _liquidCounts.Clear();
            Rectangle tileRectangle = new(_origin.X - _size.X / 2, _origin.Y - _size.Y / 2, _size.X, _size.Y);
            tileRectangle = WorldUtils.ClampToWorld(tileRectangle);

            for (int x = tileRectangle.Left; x < tileRectangle.Right; x++)
            {
                for (int y = tileRectangle.Top; y < tileRectangle.Bottom; y++)
                {
                    if (!tileRectangle.Contains(x, y)) continue;

                    Tile tile = Main.tile[x, y];
                    if (tile == null) continue; //skip null
                    if (!tile.HasTile) //count liquids in empty tiles
                    {
                        if (tile.LiquidAmount > 0)
                        {
                            _liquidCounts.TryAdd(tile.LiquidType, 0);
                            _liquidCounts[tile.LiquidType]++;
                        }
                        continue;
                    }

                    //count non-empty tiles
                    if (!_tileCounts.ContainsKey(tile.TileType)) _tileCounts.Add(tile.TileType, 0);
                    _tileCounts[tile.TileType]++;
                }
            }
        }
    }

    public class BiomeExtractionSystem : ModSystem
    {
        public enum PoolType
        {
            MINERALS, GEMS, DROPS, TERRAIN, VEGETATION, CRITTERS
        }

        public class PoolEntry(string name, int tier, bool blocking, string localizationKey)
        {
            public string Name { get; private set; } = name;
            public int Tier { get; private set; } = tier;
            public bool Blocking { get; private set; } = blocking;
            public string LocalizationKey { get; private set; } = localizationKey;
            public bool IsLocalized() => LocalizationKey != null;

            public PoolEntry(string name, int tier) : this(name, tier, true, null) { }
            public PoolEntry(string name, int tier, string localizationKey) : this(name, tier, true, localizationKey) { }
            public PoolEntry(string name, int tier, bool blocking) : this(name, tier, blocking, null) { }
        }

        public class ItemEntry(short item, int min, int max)
        {
            public short Id { get; private set; } = item;
            public int Min { get; private set; } = min;
            public int Max { get; private set; } = max + 1;
            public int Count { get => Main.rand.Next(Min, Max); }

            public ItemEntry() : this(0, 1, 1) { }
            public ItemEntry(short item, int count) : this(item, count, count) { }

            public override bool Equals(object obj)
            {
                if (obj == null) return false;
                if (obj is not ItemEntry) return false;
                return Id == ((ItemEntry)obj).Id && Min == ((ItemEntry)obj).Min && Max == ((ItemEntry)obj).Max;
            }

            public override int GetHashCode()
            {
                return Id.GetHashCode() * Min.GetHashCode() * Max.GetHashCode();
            }

            public override string ToString()
            {
                return Id+": "+Main.item[Id].Name + " (" + Min +"-"+ (Max-1) + ")";
            }
        }

        public static readonly string forest = "forest";
        public static readonly string sky = "sky";
        public static readonly string flight = "flight";
        public static readonly string snow = "snow";
        public static readonly string desert = "desert";
        public static readonly string jungle = "jungle";
        public static readonly string shells = "shells";
        public static readonly string hallowed_bars_forest  = "hallowed_bars_forest";
        public static readonly string hallowed_bars_desert = "hallowed_bars_desert";
        public static readonly string hallowed_bars_snow = "hallowed_bars_snow";
        public static readonly string hallowed_forest = "hallowed_forest";
        public static readonly string hallowed_desert = "hallowed_desert";
        public static readonly string hallowed_snow = "hallowed_snow";
        public static readonly string mushroom = "mushroom";
        public static readonly string corrupt_forest_hm = "corrupt_forest_hm";
        public static readonly string corrupt_desert_hm = "corrupt_desert_hm";
        public static readonly string corrupt_snow_hm = "corrupt_snow_hm";
        public static readonly string corrupt_snow = "corrupt_snow";
        public static readonly string corrupt_desert = "corrupt_desert";
        public static readonly string corrupt_forest = "corrupt_forest";
        public static readonly string crimson_snow = "crimson_snow";
        public static readonly string crimson_desert = "crimson_desert";
        public static readonly string crimson_forest = "crimson_forest";
        public static readonly string crimson_dark_shard = "crimson_dark_shard";
        public static readonly string graveyard = "graveyard";
        public static readonly string caverns = "caverns";
        public static readonly string underground = "underground"; 
        public static readonly string evil_ores = "evil_ores";
        public static readonly string hm_ores = "hm_ores";
        public static readonly string shimmer = "shimmer";
        public static readonly string granite = "granite";
        public static readonly string marble = "marble";
        public static readonly string cobweb = "cobweb";
        public static readonly string spider = "spider";
        public static readonly string ug_snow = "ug_snow";
        public static readonly string ug_desert = "ug_desert";
        public static readonly string ug_desert_hm = "ug_desert_hm";
        public static readonly string ug_jungle = "ug_jungle";
        public static readonly string hive = "hive";
        public static readonly string ug_shells = "ug_shells";
        public static readonly string life_fruit = "life_fruit";
        public static readonly string chlorophyte = "chlorophyte";
        public static readonly string ug_hallowed_caverns = "ug_hallowed_caverns";
        public static readonly string ug_hallowed_snow = "ug_hallowed_snow";
        public static readonly string ug_hallowed_desert = "ug_hallowed_desert";
        public static readonly string ug_hallowed_bars_caverns = "ug_hallowed_bars_caverns";
        public static readonly string ug_hallowed_bars_snow = "ug_hallowed_bars_snow";
        public static readonly string ug_hallowed_bars_desert = "ug_hallowed_bars_desert";
        public static readonly string ug_mushroom = "ug_mushroom";
        public static readonly string truffle_worm = "truffle_worm";
        public static readonly string ug_corrupt_caverns = "ug_corrupt_caverns";
        public static readonly string ug_corrupt_snow = "ug_corrupt_snow";
        public static readonly string ug_corrupt_desert = "ug_corrupt_desert";
        public static readonly string ug_corrupt_caverns_hm = "ug_corrupt_caverns_hm";
        public static readonly string ug_corrupt_snow_hm = "ug_corrupt_snow_hm";
        public static readonly string ug_corrupt_desert_hm = "ug_corrupt_desert_hm";
        public static readonly string ug_crimson_caverns = "ug_crimson_caverns";
        public static readonly string ug_crimson_snow = "ug_crimson_snow";
        public static readonly string ug_crimson_desert = "ug_crimson_desert";
        public static readonly string ug_crimson_caverns_hm = "ug_crimson_caverns_hm";
        public static readonly string ug_crimson_snow_hm = "ug_crimson_snow_hm";
        public static readonly string ug_crimson_desert_hm = "ug_crimson_desert_hm";
        public static readonly string dungeon = "dungeon";
        public static readonly string dungeon_p = "dungeon_p";
        public static readonly string dungeon_g = "dungeon_g";
        public static readonly string dungeon_b = "dungeon_b";
        public static readonly string temple = "temple";
        public static readonly string ectoplasm = "ectoplasm";
        public static readonly string ocean = "ocean";
        public static readonly string pirate = "pirate";
        public static readonly string space = "space";
        public static readonly string spc_flight = "spc_flight";
        public static readonly string pillar = "pillar";
        public static readonly string luminite = "luminite";
        public static readonly string underworld = "underworld";
        public static readonly string uw_fire= "uw_fire";
        public static readonly string meteorite = "meteorite";

        //WALL AND BLOCK LISTS
        static readonly List<ushort> dungeonWalls = [WallID.BlueDungeonUnsafe, WallID.BlueDungeonSlabUnsafe, WallID.BlueDungeonTileUnsafe, WallID.GreenDungeonUnsafe, WallID.GreenDungeonSlabUnsafe, WallID.GreenDungeonTileUnsafe, WallID.PinkDungeonUnsafe, WallID.PinkDungeonSlabUnsafe, WallID.PinkDungeonTileUnsafe];
        static readonly List<ushort> dungeonWallsPink = [WallID.PinkDungeonUnsafe, WallID.PinkDungeonSlabUnsafe, WallID.PinkDungeonTileUnsafe];
        static readonly List<ushort> dungeonWallsGreen = [WallID.GreenDungeonUnsafe, WallID.GreenDungeonSlabUnsafe, WallID.GreenDungeonTileUnsafe];
        static readonly List<ushort> dungeonWallsBlue = [WallID.BlueDungeonUnsafe, WallID.BlueDungeonSlabUnsafe, WallID.BlueDungeonTileUnsafe];
        static readonly List<ushort> dungeonBricks = [TileID.BlueDungeonBrick, TileID.GreenDungeonBrick, TileID.PinkDungeonBrick];
        static readonly List<ushort> crimsonSandBlocks = [TileID.Crimsand, TileID.CrimsonHardenedSand, TileID.CrimsonSandstone];
        static readonly List<ushort> corruptSandBlocks = [TileID.Ebonsand, TileID.CorruptHardenedSand, TileID.CorruptSandstone];
        static readonly List<ushort> hallowSandBlocks = [TileID.Pearlsand, TileID.HallowHardenedSand, TileID.HallowSandstone];
        static readonly List<ushort> crimsonIceBlocks = [TileID.FleshIce];
        static readonly List<ushort> corruptIceBlocks = [TileID.CorruptIce];
        static readonly List<ushort> hallowIceBlocks = [TileID.HallowedIce];
        static readonly List<ushort> crimsonForestBlocks = [TileID.CrimsonGrass,  TileID.CrimsonJungleGrass,                        TileID.CorruptThorns,   TileID.Crimstone];
        static readonly List<ushort> corruptForestBlocks = [TileID.CorruptGrass,  TileID.CorruptJungleGrass, TileID.CorruptThorns,  TileID.CorruptPlants,   TileID.Ebonstone];
        static readonly List<ushort> hallowForestBlocks =  [TileID.HallowedGrass, TileID.GolfGrassHallowed,  TileID.HallowedPlants, TileID.HallowedPlants2, TileID.Pearlstone];
        static readonly List<ushort> crimsonBlocks = [TileID.CrimsonGrass,  TileID.CrimsonJungleGrass, TileID.CorruptThorns,                         TileID.Crimstone,  TileID.Crimsand,  TileID.CrimsonHardenedSand, TileID.CrimsonSandstone, TileID.FleshIce];
        static readonly List<ushort> corruptBlocks = [TileID.CorruptGrass,  TileID.CorruptJungleGrass, TileID.CorruptThorns,  TileID.CorruptPlants,  TileID.Ebonstone,  TileID.Ebonsand,  TileID.CorruptHardenedSand, TileID.CorruptSandstone, TileID.CorruptIce];
        static readonly List<ushort> hallowBlocks =  [TileID.HallowedGrass,                            TileID.HallowedPlants, TileID.HallowedPlants2,TileID.Pearlstone, TileID.Pearlsand, TileID.HallowHardenedSand,  TileID.HallowSandstone,  TileID.HallowedIce];
        static readonly List<ushort> glowMushroomBlocks = [TileID.MushroomGrass, TileID.MushroomPlants, TileID.MushroomTrees, TileID.MushroomVines];
        static readonly List<ushort> jungleBlocks = [TileID.JungleGrass, TileID.JunglePlants, TileID.JunglePlants2, TileID.PlantDetritus, TileID.JungleVines, TileID.Hive, TileID.LihzahrdBrick];
        static readonly List<ushort> frostBlocks = [TileID.SnowBlock, TileID.SnowBrick, TileID.IceBlock, TileID.BreakableIce, TileID.FleshIce, TileID.CorruptIce, TileID.HallowedIce];
        static readonly List<ushort> desertBlocks = [TileID.Sand, TileID.Crimsand, TileID.Ebonsand, TileID.Pearlsand, TileID.HardenedSand, TileID.CrimsonHardenedSand, TileID.CorruptHardenedSand, TileID.HallowHardenedSand, TileID.Sandstone, TileID.CrimsonSandstone, TileID.CorruptSandstone, TileID.HallowSandstone];
        
        //PROGRESSION
        static readonly Predicate<ScanData> hardmodeOnly = scan => Main.hardMode;
        static readonly Predicate<ScanData> postMech = scan => Condition.DownedMechBossAny.IsMet();
        static readonly Predicate<ScanData> postMechs = scan => Condition.DownedMechBossAll.IsMet();
        static readonly Predicate<ScanData> postPlantera = scan => Condition.DownedPlantera.IsMet();
        static readonly Predicate<ScanData> postGolem = scan => Condition.DownedGolem.IsMet();
        static readonly Predicate<ScanData> postPillars = scan => Condition.DownedNebulaPillar.IsMet() && Condition.DownedSolarPillar.IsMet() && Condition.DownedStardustPillar.IsMet() && Condition.DownedVortexPillar.IsMet();
        static readonly Predicate<ScanData> postML = scan => Condition.DownedMoonLord.IsMet();

        //WORLDLAYER
        static readonly Predicate<ScanData> spaceLayer = scan => scan.Y < Main.worldSurface * 0.3;
        static readonly Predicate<ScanData> skyLayer = scan => scan.Y < Main.worldSurface * 0.35;
        static readonly Predicate<ScanData> surfaceLayer = scan => scan.Y < Main.worldSurface + 1;
        static readonly Predicate<ScanData> belowSurfaceLayer = scan => scan.Y > Main.worldSurface + 1;
        static readonly Predicate<ScanData> middleUnderground = scan => scan.Y > (Main.worldSurface + Main.rockLayer) / 2;
        static readonly Predicate<ScanData> cavernLayer = scan => scan.Y > Main.rockLayer + 1;
        static readonly Predicate<ScanData> notCavernLayer = scan => scan.Y <= Main.rockLayer;
        static readonly Predicate<ScanData> underworldLayer = scan => scan.Y > Main.maxTilesY - 200;

        //SPECIFIC POSITIONS
        static readonly Predicate<ScanData> oceanArea = scan => scan.Y <= (Main.worldSurface + Main.rockLayer) / 2 && (scan.X < 339 || scan.X > Main.maxTilesX - 339);

        //WALLS
        static readonly Predicate<ScanData> lihzahrd_bg = scan => scan.ValidWalls([WallID.LihzahrdBrickUnsafe]);
        static readonly Predicate<ScanData> hive_bg = scan => scan.ValidWalls([WallID.HiveUnsafe]);
        static readonly Predicate<ScanData> granite_bg = scan => scan.ValidWalls([WallID.GraniteUnsafe]);
        static readonly Predicate<ScanData> marble_bg = scan => scan.ValidWalls([WallID.MarbleUnsafe]);
        static readonly Predicate<ScanData> spider_bg = scan => scan.ValidWalls([WallID.SpiderUnsafe]);
        static readonly Predicate<ScanData> dungeon_bg = scan => scan.ValidWalls(dungeonWalls);
        static readonly Predicate<ScanData> dungeon_bg_p = scan => scan.ValidWalls(dungeonWallsPink);
        static readonly Predicate<ScanData> dungeon_bg_g = scan => scan.ValidWalls(dungeonWallsGreen);
        static readonly Predicate<ScanData> dungeon_bg_b = scan => scan.ValidWalls(dungeonWallsBlue);

        //LIQUIDS
        static readonly Predicate<ScanData> water1k = scan => scan.Liquids(LiquidID.Water) >= 1000;
        static readonly Predicate<ScanData> honey100 = scan => scan.Liquids(LiquidID.Honey) >= 100;
        static readonly Predicate<ScanData> shimmer300 = scan => scan.Liquids(LiquidID.Shimmer) >= 300;

        //BLOCKS
        static readonly Func<List<ushort>, Predicate<ScanData>> evil300 = tiles => scan => scan.Tiles(tiles) - scan.Tiles(hallowBlocks) - scan.Tiles(TileID.Sunflower) * 10 >= 300;
        static readonly Func<List<ushort>, Predicate<ScanData>> hallow125 = tiles => scan => scan.Tiles(tiles) - scan.Tiles(crimsonBlocks) - scan.Tiles(corruptBlocks) >= 125;
        static readonly Predicate<ScanData> meteorite75 = scan => scan.Tiles(TileID.Meteorite) >= 75;
        static readonly Predicate<ScanData> hive100 = scan => scan.Tiles(TileID.Hive) > 100;
        static readonly Predicate<ScanData> marble150 = scan => scan.Tiles(TileID.Marble) > 150;
        static readonly Predicate<ScanData> granite150 = scan => scan.Tiles(TileID.Granite) > 150;
        static readonly Predicate<ScanData> dungeon_p250 = scan => scan.Tiles(TileID.PinkDungeonBrick) >= 250;
        static readonly Predicate<ScanData> dungeon_g250 = scan => scan.Tiles(TileID.GreenDungeonBrick) >= 250;
        static readonly Predicate<ScanData> dungeon_b250 = scan => scan.Tiles(TileID.BlueDungeonBrick) >= 250;
        static readonly Predicate<ScanData> dungeon250 = scan => scan.Tiles(dungeonBricks) >= 250;
        static readonly Predicate<ScanData> mush100 = scan => scan.Tiles(glowMushroomBlocks) >= 100;
        static readonly Predicate<ScanData> jungle140 = scan => scan.Tiles(jungleBlocks) > 140;
        static readonly Predicate<ScanData> desert1500 = scan => scan.Tiles(desertBlocks) > 1500;
        static readonly Predicate<ScanData> frost1500 = scan => scan.Tiles(frostBlocks) > 1500;
        static readonly Predicate<ScanData> tombstone5 = scan => scan.Tiles(TileID.Tombstones) > 20;

        private readonly Dictionary<string, PoolEntry> _poolNames = [];
        private readonly Dictionary<string, WeightedList<ItemEntry>> _itemPools = [];
        private readonly Dictionary<int,Dictionary<string, List<Predicate<ScanData>>>> _poolRequirements = [];
        private readonly PriorityList<string> _priorityList = [];

        private static string LocalizeAs(string suffix) => $"{BiomeExtractorsMod.LocPoolNames}.{suffix}";

        public bool PoolExists(PoolEntry pool) => PoolExists(pool.Name); 
        public bool PoolExists(string name) => _poolNames.ContainsKey(name);

        public PoolEntry GetPoolEntry(string name) => _poolNames[name];

        public bool AddPool(string name, int priority) => AddPool(new PoolEntry(name, (int)BiomeExtractorEnt.EnumTiers.BASIC), priority);
        public bool AddPool(string name, int priority, bool nonBlocking) => AddPool(new PoolEntry(name, (int)BiomeExtractorEnt.EnumTiers.BASIC, !nonBlocking), priority);
        public bool AddPool(string name, int priority, string localizationKey) => AddPool(new PoolEntry(name, (int)BiomeExtractorEnt.EnumTiers.BASIC, localizationKey), priority);
        public bool AddPool(string name, int priority, bool nonBlocking, string localizationKey) => AddPool(new PoolEntry(name, (int)BiomeExtractorEnt.EnumTiers.BASIC, !nonBlocking, localizationKey), priority);
        public bool AddPool(string name, int tier, int priority) => AddPool(new PoolEntry(name, tier), priority);
        public bool AddPool(string name, int tier, int priority, bool nonBlocking) => AddPool(new PoolEntry(name, tier, !nonBlocking), priority);
        public bool AddPool(string name, int tier, int priority, string localizationKey) => AddPool(new PoolEntry(name, tier, localizationKey), priority);
        public bool AddPool(string name, int tier, int priority, bool nonBlocking, string localizationKey) => AddPool(new PoolEntry(name, tier, !nonBlocking, localizationKey), priority);
        public bool AddPool(PoolEntry pool, int priority)
        {
            if (PoolExists(pool.Name)) return false;
            _poolNames.Add(pool.Name, pool);
            _priorityList.Add(priority, pool.Name);
            _itemPools.Add(pool.Name, []); //TODO Consider dividing pools up by content
            if (!_poolRequirements.ContainsKey(pool.Tier))
                _poolRequirements.Add(pool.Tier, []);
            return true;
        }
        public bool ChangePoolTier(string name, int newTier) => ChangePoolTier(GetPoolEntry(name), newTier);
        public bool ChangePoolTier(PoolEntry pool, int newTier)
        {
            if (pool == null) return false;
            PoolEntry newPool = new(pool.Name, newTier);
            _poolNames[pool.Name] = newPool;

            if (!_poolRequirements.ContainsKey(newTier))
                _poolRequirements.Add(newTier, []);
            _poolRequirements[newPool.Tier][newPool.Name] = _poolRequirements[pool.Tier][pool.Name];
            _poolRequirements[pool.Tier].Remove(pool.Name);
            return true;
        }
        public bool RemovePool(string name) => PoolExists(name) && RemovePoolWithoutChecking(GetPoolEntry(name));
        public bool RemovePool(PoolEntry pool)
        {
            if (PoolExists(pool)) { return RemovePoolWithoutChecking(pool); }
            return false;
        }
        bool RemovePoolWithoutChecking(PoolEntry pool)
        {
            _poolNames.Remove(pool.Name);
            foreach (List<string> l in _priorityList.Values)
            {
                l.Remove(pool.Name);
            }
            _itemPools.Remove(pool.Name);
            _poolRequirements[pool.Tier].Remove(pool.Name);
            return true;
        }

        public void AddPoolRequirements(string name, params Predicate<ScanData>[] conditions)
        {
            PoolEntry pool = GetPoolEntry(name);
            if (!_poolRequirements[pool.Tier].ContainsKey(pool.Name))
                _poolRequirements[pool.Tier].Add(pool.Name, []);
            foreach (Predicate<ScanData> condition in conditions)
                _poolRequirements[pool.Tier][pool.Name].Add(condition);
        }
        public void FlushPoolRequirements(string name) {
            PoolEntry pool = (GetPoolEntry(name));
            if (pool != null && _poolRequirements[pool.Tier][pool.Name] != null)
                _poolRequirements[pool.Tier][pool.Name].Clear();
        }

        public void AddItemInPool(string name, short itemId) => AddItemInPool(name, new ItemEntry(itemId, 1), 1);
        public void AddItemInPool(string name, short itemId, int weight) => AddItemInPool(name, new ItemEntry(itemId, 1), weight);
        public void AddItemInPool(string name, short itemId, int count, int weight) => AddItemInPool(name, new ItemEntry(itemId, count), weight);
        public void AddItemInPool(string name, ItemEntry item, int weight)
        {
            _itemPools[name].Add(item, weight);
        }
        public void RemoveItemFromPool(string poolName, short itemId) => RemoveItemFromPool(poolName, new ItemEntry(itemId, 1));
        public void RemoveItemFromPool(string poolName, short itemId, int count) => RemoveItemFromPool(poolName, new ItemEntry(itemId, count));
        public void RemoveItemFromPool(string poolName, ItemEntry item)
        {
            _itemPools[poolName].Remove(item);
        }
        public void FlushPoolItems(string poolName)
        {
            _itemPools[poolName].Clear();
        }

        public List<PoolEntry> CheckValidBiomes(BiomeExtractorEnt extractor)
        {
            ScanData scan = new(extractor);
            scan.Scan();

            int last_p = int.MaxValue;
            bool stop = false;
            List<PoolEntry> found = [];
            foreach(KeyValuePair<int, string> elem in _priorityList.EnumerateInOrder())
            {
                if (stop &&  last_p != elem.Key) break;
                last_p = elem.Key;

                PoolEntry pool = GetPoolEntry(elem.Value);
                bool check_passed = true;
                if (_poolRequirements[pool.Tier].ContainsKey(pool.Name))
                {
                    foreach (Predicate<ScanData> check in _poolRequirements[pool.Tier][pool.Name])
                    {
                        if (!check.Invoke(scan))
                        {
                            check_passed = false;
                            break;
                        }
                    }
                }
                if (check_passed && _itemPools[elem.Value].Count > 0)
                {
                    if(scan.MinTier(pool.Tier))
                        found.Add(_poolNames[elem.Value]);
                    if(pool.Blocking) stop = true;
                }
            }
            return found;
        }

        public Item RollItem(List<PoolEntry> pools)
        {
            if (pools.Count == 1)
            {
                ItemEntry entry = _itemPools[pools[0].Name].Roll();
                return new(entry.Id, entry.Count);
            }

            int totalWeight = 0;
            foreach (PoolEntry pool in pools)
                totalWeight += _itemPools[pool.Name].TotalWeight;
            int roll = Main.rand.Next(totalWeight);
            int current = 0;
            ItemEntry result = new(ItemID.None, 1);
            foreach (PoolEntry pool in pools)
            {
                current += _itemPools[pool.Name].TotalWeight;
                if (current > roll)
                {
                    int weight = roll - (current - _itemPools[pool.Name].TotalWeight);
                    result = _itemPools[pool.Name].FromWeight(weight);
                    break;
                }
            }

            Item item = new(result.Id, result.Count);

            return item;
        }

        public WeightedList<ItemEntry> JoinPools(List<PoolEntry> pools)
        {
            WeightedList<ItemEntry> joinedPool = [];
            if (pools is not null)
            {
                foreach (PoolEntry pool in pools)
                {
                    WeightedList<ItemEntry> items = _itemPools[pool.Name];
                    foreach (KeyValuePair<ItemEntry, int> entry in items)
                        joinedPool.Add(entry);
                }
            }
            return joinedPool;
        }

        public void GenerateLocalizationKeys()
        {
            foreach(PoolEntry pool in _poolNames.Values)
            {
                if(pool.IsLocalized())
                {
                    Language.GetOrRegister(pool.LocalizationKey, () => pool.Name);
                }
            }
        }

        public override void PostSetupContent()
        {
            InitializePools();
            SetRequirements();
            PopulatePools();
            GenerateLocalizationKeys();
        }

        private void InitializePools()
        {
            AddPool(forest, 0, LocalizeAs(forest));

            AddPool(caverns,                                             10, LocalizeAs(caverns));
            AddPool(underground,                                         10, LocalizeAs(underground));
            AddPool(evil_ores, (int)BiomeExtractorEnt.EnumTiers.DEMONIC, 10);
            AddPool(hm_ores, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, 10);

            AddPool(snow,                                               50, LocalizeAs(snow));
            AddPool(desert,                                             50, LocalizeAs(desert));
            AddPool(jungle,                                             50, LocalizeAs(jungle));
            AddPool(shells, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, 50);
            AddPool(sky,                                                50, LocalizeAs(sky));
            AddPool(flight, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, 50);

            AddPool(hallowed_bars_forest, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, 100);
            AddPool(hallowed_bars_desert, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, 100);
            AddPool(hallowed_bars_snow,   (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, 100);
            AddPool(hallowed_forest,      (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, 100, LocalizeAs(hallowed_forest));      
            AddPool(hallowed_desert,      (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, 100, LocalizeAs(hallowed_desert));
            AddPool(hallowed_snow,        (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, 100, LocalizeAs(hallowed_snow));

            AddPool(mushroom, 200);

            AddPool(crimson_dark_shard, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, 300);
            AddPool(corrupt_forest_hm,  (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, 300);
            AddPool(corrupt_desert_hm,  (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, 300);
            AddPool(corrupt_snow_hm,    (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, 300);
            AddPool(corrupt_forest,     (int)BiomeExtractorEnt.EnumTiers.DEMONIC,   300, LocalizeAs(corrupt_forest));
            AddPool(crimson_forest,     (int)BiomeExtractorEnt.EnumTiers.DEMONIC,   300, LocalizeAs(crimson_forest));
            AddPool(corrupt_snow,       (int)BiomeExtractorEnt.EnumTiers.DEMONIC,   300, LocalizeAs(corrupt_snow));
            AddPool(crimson_snow,       (int)BiomeExtractorEnt.EnumTiers.DEMONIC,   300, LocalizeAs(crimson_snow));
            AddPool(corrupt_desert,     (int)BiomeExtractorEnt.EnumTiers.DEMONIC,   300, LocalizeAs(corrupt_desert));
            AddPool(crimson_desert,     (int)BiomeExtractorEnt.EnumTiers.DEMONIC,   300, LocalizeAs(crimson_desert));

            AddPool(graveyard, 500, LocalizeAs(graveyard));

            AddPool(ug_snow,                                                  1050, LocalizeAs(ug_snow));
            AddPool(ug_desert,                                                1050, LocalizeAs(ug_desert));
            AddPool(ug_desert_hm, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, 1050);
            AddPool(ug_jungle,                                                1050, LocalizeAs(ug_jungle));
            AddPool(ug_shells,    (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, 1050);
            AddPool(life_fruit,   (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, 1050);
            AddPool(hive,                                                     1050, LocalizeAs(hive));
            AddPool(chlorophyte,  (int)BiomeExtractorEnt.EnumTiers.CYBER,     1050);

            AddPool(ug_hallowed_bars_caverns, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, 1100);
            AddPool(ug_hallowed_bars_desert,  (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, 1100);
            AddPool(ug_hallowed_bars_snow,    (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, 1100);
            AddPool(ug_hallowed_caverns,      (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, 1100, LocalizeAs(ug_hallowed_caverns));
            AddPool(ug_hallowed_snow,         (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, 1100, LocalizeAs(ug_hallowed_snow));
            AddPool(ug_hallowed_desert,       (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, 1100, LocalizeAs(ug_hallowed_desert));

            AddPool(ug_mushroom,                                              1200, LocalizeAs(ug_mushroom));
            AddPool(truffle_worm, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, 1200);

            AddPool(ug_corrupt_caverns_hm, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, 1300);
            AddPool(ug_crimson_caverns_hm, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, 1300);
            AddPool(ug_corrupt_desert_hm,  (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, 1300);
            AddPool(ug_crimson_desert_hm,  (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, 1300);
            AddPool(ug_corrupt_snow_hm,    (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, 1300);
            AddPool(ug_crimson_snow_hm,    (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, 1300);
            AddPool(ug_corrupt_caverns,    (int)BiomeExtractorEnt.EnumTiers.DEMONIC,   1300, LocalizeAs(ug_corrupt_caverns));
            AddPool(ug_crimson_caverns,    (int)BiomeExtractorEnt.EnumTiers.DEMONIC,   1300, LocalizeAs(ug_crimson_caverns));
            AddPool(ug_corrupt_snow,       (int)BiomeExtractorEnt.EnumTiers.DEMONIC,   1300, LocalizeAs(ug_corrupt_snow));
            AddPool(ug_crimson_snow,       (int)BiomeExtractorEnt.EnumTiers.DEMONIC,   1300, LocalizeAs(ug_crimson_snow));
            AddPool(ug_corrupt_desert,     (int)BiomeExtractorEnt.EnumTiers.DEMONIC,   1300, LocalizeAs(ug_corrupt_desert));
            AddPool(ug_crimson_desert,     (int)BiomeExtractorEnt.EnumTiers.DEMONIC,   1300, LocalizeAs(ug_crimson_desert));
            
            AddPool(dungeon,   (int)BiomeExtractorEnt.EnumTiers.DEMONIC, 2000, LocalizeAs(dungeon));
            AddPool(dungeon_p, (int)BiomeExtractorEnt.EnumTiers.DEMONIC, 2000);
            AddPool(dungeon_g, (int)BiomeExtractorEnt.EnumTiers.DEMONIC, 2000);
            AddPool(dungeon_b, (int)BiomeExtractorEnt.EnumTiers.DEMONIC, 2000);
            AddPool(ectoplasm, (int)BiomeExtractorEnt.EnumTiers.CYBER,   2000);
            AddPool(temple,    (int)BiomeExtractorEnt.EnumTiers.CYBER,   2000, LocalizeAs(temple));

            AddPool(ocean,                                              2500, LocalizeAs(ocean));
            AddPool(pirate, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, 2500);

            AddPool(shimmer, (int)BiomeExtractorEnt.EnumTiers.DEMONIC,  3000, true, LocalizeAs(shimmer));
            AddPool(spider,  (int)BiomeExtractorEnt.EnumTiers.INFERNAL, 3000, true);
            AddPool(cobweb,                                             3000, true, LocalizeAs(cobweb));
            AddPool(granite,                                            3000, true, LocalizeAs(granite));
            AddPool(marble,                                             3000, true, LocalizeAs(marble));

            AddPool(space,                                                  4000, LocalizeAs(space));
            AddPool(spc_flight, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, 4000);
            AddPool(pillar,     (int)BiomeExtractorEnt.EnumTiers.LUNAR,     4000);
            AddPool(luminite,   (int)BiomeExtractorEnt.EnumTiers.ETHEREAL,  4000);
            AddPool(underworld, (int)BiomeExtractorEnt.EnumTiers.INFERNAL,  4000, LocalizeAs(underworld));
            AddPool(uw_fire,    (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, 4000);
            AddPool(meteorite,  (int)BiomeExtractorEnt.EnumTiers.INFERNAL,  10000, LocalizeAs(meteorite));
        }

        private void SetRequirements()
        {
            //Standard order: progression, layer, blocks, liquids, more complicated stuff, walls 
            AddPoolRequirements(meteorite,                                 meteorite75);
            AddPoolRequirements(uw_fire,    hardmodeOnly, underworldLayer);
            AddPoolRequirements(underworld,               underworldLayer);
            AddPoolRequirements(luminite,   postML,       spaceLayer);
            AddPoolRequirements(pillar,     postPillars,  spaceLayer);
            AddPoolRequirements(spc_flight, hardmodeOnly, spaceLayer);
            AddPoolRequirements(space,                    spaceLayer);

            AddPoolRequirements(pirate, hardmodeOnly, water1k, oceanArea);
            AddPoolRequirements(ocean,                water1k, oceanArea);

            AddPoolRequirements(temple,    postGolem,                                     lihzahrd_bg);
            AddPoolRequirements(ectoplasm, postPlantera, dungeon250,   belowSurfaceLayer, dungeon_bg);
            AddPoolRequirements(dungeon_p,               dungeon_p250, belowSurfaceLayer, dungeon_bg_p);
            AddPoolRequirements(dungeon_g,               dungeon_g250, belowSurfaceLayer, dungeon_bg_g);
            AddPoolRequirements(dungeon_b,               dungeon_b250, belowSurfaceLayer, dungeon_bg_b);
            AddPoolRequirements(dungeon,                 dungeon250,   belowSurfaceLayer, dungeon_bg);

            AddPoolRequirements(ug_crimson_desert,     cavernLayer, evil300.Invoke(crimsonSandBlocks));
            AddPoolRequirements(ug_crimson_snow,       cavernLayer, evil300.Invoke(crimsonIceBlocks));
            AddPoolRequirements(ug_crimson_caverns,    cavernLayer, evil300.Invoke(crimsonForestBlocks));
            AddPoolRequirements(ug_crimson_desert_hm,  cavernLayer, evil300.Invoke(crimsonSandBlocks));
            AddPoolRequirements(ug_crimson_snow_hm,    cavernLayer, evil300.Invoke(crimsonIceBlocks));
            AddPoolRequirements(ug_crimson_caverns_hm, cavernLayer, evil300.Invoke(crimsonForestBlocks));

            AddPoolRequirements(ug_corrupt_desert,     cavernLayer, evil300.Invoke(corruptSandBlocks));
            AddPoolRequirements(ug_corrupt_snow,       cavernLayer, evil300.Invoke(corruptIceBlocks));
            AddPoolRequirements(ug_corrupt_caverns,    cavernLayer, evil300.Invoke(corruptForestBlocks));
            AddPoolRequirements(ug_corrupt_desert_hm,  cavernLayer, evil300.Invoke(corruptSandBlocks));
            AddPoolRequirements(ug_corrupt_snow_hm,    cavernLayer, evil300.Invoke(corruptIceBlocks));
            AddPoolRequirements(ug_corrupt_caverns_hm, cavernLayer, evil300.Invoke(corruptForestBlocks));

            AddPoolRequirements(truffle_worm, hardmodeOnly, cavernLayer, mush100);
            AddPoolRequirements(ug_mushroom,                cavernLayer, mush100);

            AddPoolRequirements(ug_hallowed_bars_caverns, postMechs, cavernLayer, hallow125.Invoke(hallowForestBlocks));
            AddPoolRequirements(ug_hallowed_bars_desert,  postMechs, cavernLayer, hallow125.Invoke(hallowSandBlocks));
            AddPoolRequirements(ug_hallowed_bars_snow,    postMechs, cavernLayer, hallow125.Invoke(hallowIceBlocks));
            AddPoolRequirements(ug_hallowed_caverns,                 cavernLayer, hallow125.Invoke(hallowForestBlocks));
            AddPoolRequirements(ug_hallowed_desert,                  cavernLayer, hallow125.Invoke(hallowSandBlocks));
            AddPoolRequirements(ug_hallowed_snow,                    cavernLayer, hallow125.Invoke(hallowIceBlocks));

            AddPoolRequirements(chlorophyte,  postMechs,    middleUnderground, jungle140);
            AddPoolRequirements(life_fruit,   postMech,     middleUnderground, jungle140);
            AddPoolRequirements(ug_shells,    hardmodeOnly, middleUnderground, jungle140);
            AddPoolRequirements(hive,                       middleUnderground, hive100,    honey100, hive_bg);
            AddPoolRequirements(ug_jungle,                  middleUnderground, jungle140);
            AddPoolRequirements(ug_desert_hm, hardmodeOnly, belowSurfaceLayer, desert1500);
            AddPoolRequirements(ug_desert,                  belowSurfaceLayer, desert1500);
            AddPoolRequirements(ug_snow,                    belowSurfaceLayer, frost1500);

            AddPoolRequirements(marble,  cavernLayer, marble150,    marble_bg);
            AddPoolRequirements(granite, cavernLayer, granite150,   granite_bg);
            AddPoolRequirements(cobweb,  cavernLayer,               spider_bg);
            AddPoolRequirements(spider,  cavernLayer, hardmodeOnly, spider_bg);

            AddPoolRequirements(caverns,                   cavernLayer);
            AddPoolRequirements(underground,               belowSurfaceLayer, notCavernLayer);
            AddPoolRequirements(evil_ores,                 belowSurfaceLayer);
            AddPoolRequirements(hm_ores,     hardmodeOnly, belowSurfaceLayer);
            AddPoolRequirements(shimmer,                   shimmer300);

            AddPoolRequirements(graveyard, surfaceLayer, tombstone5);

            AddPoolRequirements(crimson_dark_shard, hardmodeOnly, evil300.Invoke(crimsonSandBlocks));
            AddPoolRequirements(corrupt_forest_hm,  hardmodeOnly, evil300.Invoke(corruptForestBlocks));
            AddPoolRequirements(corrupt_desert_hm,  hardmodeOnly, evil300.Invoke(corruptSandBlocks));
            AddPoolRequirements(corrupt_snow_hm,    hardmodeOnly, evil300.Invoke(corruptIceBlocks));
            AddPoolRequirements(crimson_forest,                   evil300.Invoke(crimsonForestBlocks));
            AddPoolRequirements(corrupt_forest,                   evil300.Invoke(corruptForestBlocks));
            AddPoolRequirements(crimson_desert,                   evil300.Invoke(crimsonSandBlocks));
            AddPoolRequirements(corrupt_desert,                   evil300.Invoke(corruptSandBlocks));
            AddPoolRequirements(crimson_snow,                     evil300.Invoke(crimsonIceBlocks));
            AddPoolRequirements(corrupt_snow,                     evil300.Invoke(corruptIceBlocks));

            AddPoolRequirements(mushroom, mush100);

            AddPoolRequirements(hallowed_snow,        hardmodeOnly, hallow125.Invoke(hallowIceBlocks));
            AddPoolRequirements(hallowed_forest,      hardmodeOnly, hallow125.Invoke(hallowForestBlocks));
            AddPoolRequirements(hallowed_desert,      hardmodeOnly, hallow125.Invoke(hallowSandBlocks));
            AddPoolRequirements(hallowed_bars_snow,   postMechs,    hallow125.Invoke(hallowIceBlocks));
            AddPoolRequirements(hallowed_bars_forest, postMechs,    hallow125.Invoke(hallowForestBlocks));
            AddPoolRequirements(hallowed_bars_desert, postMechs,    hallow125.Invoke(hallowSandBlocks));

            AddPoolRequirements(shells, hardmodeOnly, jungle140);
            AddPoolRequirements(jungle,               jungle140);
            AddPoolRequirements(desert,               desert1500);
            AddPoolRequirements(snow,                 frost1500);
            AddPoolRequirements(flight, hardmodeOnly, skyLayer);
            AddPoolRequirements(sky,                  skyLayer);
        }

        private void PopulatePools()
        {
            //TERRAIN:18
            AddItemInPool(forest, ItemID.DirtBlock,                 70);
            AddItemInPool(forest, ItemID.StoneBlock,                18);
            AddItemInPool(forest, ItemID.ClayBlock,                 20);
            //MATERIALS:40
            AddItemInPool(forest, ItemID.Gel,                       60);
            AddItemInPool(forest, ItemID.PinkGel,                   10);
            AddItemInPool(forest, new ItemEntry(ItemID.Wood, 1, 3), 150);
            AddItemInPool(forest, ItemID.Acorn,                     20);
            //VEGETATION:11
            AddItemInPool(forest, ItemID.GrassSeeds,                16);
            AddItemInPool(forest, ItemID.Daybloom,                  25);
            AddItemInPool(forest, ItemID.Mushroom,                  25);
            //COLORS: 7
            AddItemInPool(forest, ItemID.YellowMarigold,            21);
            AddItemInPool(forest, ItemID.BlueBerries,               21);
            //BAITS: 7
            AddItemInPool(forest, ItemID.JuliaButterfly,            3);
            AddItemInPool(forest, ItemID.MonarchButterfly,          4);
            AddItemInPool(forest, ItemID.PurpleEmperorButterfly,    2);
            AddItemInPool(forest, ItemID.RedAdmiralButterfly,       2);
            AddItemInPool(forest, ItemID.SulphurButterfly,          4);
            AddItemInPool(forest, ItemID.TreeNymphButterfly,        1);
            AddItemInPool(forest, ItemID.UlyssesButterfly,          3);
            AddItemInPool(forest, ItemID.ZebraSwallowtailButterfly, 3);
            AddItemInPool(forest, ItemID.BlueDragonfly,             2);
            AddItemInPool(forest, ItemID.GreenDragonfly,            2);
            AddItemInPool(forest, ItemID.RedDragonfly,              2);
            AddItemInPool(forest, ItemID.Grasshopper,               4);
            AddItemInPool(forest, ItemID.Firefly,                   3);
            AddItemInPool(forest, ItemID.Worm,                      3);
            AddItemInPool(forest, ItemID.Stinkbug,                  4);

            AddItemInPool(sky,    ItemID.Cloud,        7);
            AddItemInPool(sky,    ItemID.RainCloud,    3);
            AddItemInPool(sky,    ItemID.Feather,      20);
            AddItemInPool(flight, ItemID.SoulofFlight, 5);

            //TERRAIN
            AddItemInPool(underground, ItemID.StoneBlock,      69);
            AddItemInPool(underground, ItemID.DirtBlock,       18);
            AddItemInPool(underground, ItemID.SiltBlock,       12);
            //MINERALS
            AddItemInPool(underground, ItemID.CopperOre,       14);
            AddItemInPool(underground, ItemID.TinOre,          14);
            AddItemInPool(underground, ItemID.IronOre,         12);
            AddItemInPool(underground, ItemID.LeadOre,         12);
            AddItemInPool(underground, ItemID.SilverOre,       11);
            AddItemInPool(underground, ItemID.TungstenOre,     11);
            AddItemInPool(underground, ItemID.GoldOre,         10);
            AddItemInPool(underground, ItemID.PlatinumOre,     10);
            //GEMS
            AddItemInPool(underground, ItemID.Amethyst,        12);
            AddItemInPool(underground, ItemID.Topaz,           11);
            AddItemInPool(underground, ItemID.Sapphire,        10);
            AddItemInPool(underground, ItemID.Emerald,         9);
            AddItemInPool(underground, ItemID.Ruby,            8);
            AddItemInPool(underground, ItemID.Diamond,         6);
            //COLORS
            AddItemInPool(underground, ItemID.OrangeBloodroot, 7);
            AddItemInPool(underground, ItemID.GreenMushroom,   7);
            AddItemInPool(underground, ItemID.TealMushroom,    7);
            AddItemInPool(caverns, ItemID.StoneBlock, 69);
            AddItemInPool(caverns, ItemID.DirtBlock, 18);
            AddItemInPool(caverns, ItemID.SiltBlock, 12);
            AddItemInPool(caverns, ItemID.CopperOre, 14);
            AddItemInPool(caverns, ItemID.TinOre, 14);
            AddItemInPool(caverns, ItemID.IronOre, 12);
            AddItemInPool(caverns, ItemID.LeadOre, 12);
            AddItemInPool(caverns, ItemID.SilverOre, 11);
            AddItemInPool(caverns, ItemID.TungstenOre, 11);
            AddItemInPool(caverns, ItemID.GoldOre, 10);
            AddItemInPool(caverns, ItemID.PlatinumOre, 10);
            AddItemInPool(caverns, ItemID.Amethyst, 12);
            AddItemInPool(caverns, ItemID.Topaz, 11);
            AddItemInPool(caverns, ItemID.Sapphire, 10);
            AddItemInPool(caverns, ItemID.Emerald, 9);
            AddItemInPool(caverns, ItemID.Ruby, 8);
            AddItemInPool(caverns, ItemID.Diamond, 6);
            AddItemInPool(caverns, ItemID.OrangeBloodroot, 7);
            AddItemInPool(caverns, ItemID.GreenMushroom, 7);
            AddItemInPool(caverns, ItemID.TealMushroom, 7);
            AddItemInPool(caverns, ItemID.RedHusk,  8);
            AddItemInPool(caverns, ItemID.LimeKelp, 8);
            AddItemInPool(evil_ores, ItemID.CrimtaneOre, 8);
            AddItemInPool(evil_ores, ItemID.DemoniteOre, 8);
            AddItemInPool(hm_ores, ItemID.CobaltOre,     20);
            AddItemInPool(hm_ores, ItemID.PalladiumOre,  20);
            AddItemInPool(hm_ores, ItemID.OrichalcumOre, 18);
            AddItemInPool(hm_ores, ItemID.MythrilOre,    18);
            AddItemInPool(hm_ores, ItemID.TitaniumOre,   16);
            AddItemInPool(hm_ores, ItemID.AdamantiteOre, 16);

            AddItemInPool(snow, ItemID.SnowBlock,   12);
            AddItemInPool(snow, ItemID.IceBlock,    6);
            AddItemInPool(snow, ItemID.BorealWood,  40);
            AddItemInPool(snow, ItemID.Shiverthorn, 7);

            AddItemInPool(desert, ItemID.SandBlock,       75);
            AddItemInPool(desert, ItemID.Cactus,          18);
            AddItemInPool(desert, ItemID.Waterleaf,       41);
            AddItemInPool(desert, ItemID.PinkPricklyPear, 26);
            AddItemInPool(desert, ItemID.Scorpion,        8);
            AddItemInPool(desert, ItemID.BlackScorpion,   6);
            AddItemInPool(desert, ItemID.YellowDragonfly, 4);
            AddItemInPool(desert, ItemID.BlackDragonfly,  4);
            AddItemInPool(desert, ItemID.OrangeDragonfly, 4);

            AddItemInPool(jungle, ItemID.MudBlock,                          10);
            AddItemInPool(jungle, ItemID.JungleGrassSeeds,                  5);
            AddItemInPool(jungle, new ItemEntry(ItemID.RichMahogany, 1, 3), 35);
            AddItemInPool(jungle, new ItemEntry(ItemID.BambooBlock,  1, 3), 35);
            AddItemInPool(jungle, ItemID.Moonglow,                          12);
            AddItemInPool(jungle, ItemID.SkyBlueFlower,                     12);
            AddItemInPool(jungle, ItemID.Frog,                              4);
            AddItemInPool(jungle, ItemID.Grubby,                            3);
            AddItemInPool(jungle, ItemID.Sluggy,                            3);
            AddItemInPool(jungle, ItemID.Buggy,                             2);
            AddItemInPool(shells, ItemID.TurtleShell,                       20);

            AddItemInPool(hallowed_forest, ItemID.DirtBlock,                      26);
            AddItemInPool(hallowed_forest, ItemID.PearlstoneBlock,                7);
            AddItemInPool(hallowed_forest, ItemID.MudBlock,                       8);
            AddItemInPool(hallowed_forest, ItemID.PixieDust,                      15);
            AddItemInPool(hallowed_forest, ItemID.UnicornHorn,                    5);
            AddItemInPool(hallowed_forest, ItemID.RainbowBrick,                   10);
            AddItemInPool(hallowed_forest, ItemID.Gel,                            11);
            AddItemInPool(hallowed_forest, new ItemEntry(ItemID.Pearlwood, 1, 3), 50);
            AddItemInPool(hallowed_forest, ItemID.HallowedSeeds,                  7);
            AddItemInPool(hallowed_forest, ItemID.FairyCritterBlue,               1);
            AddItemInPool(hallowed_forest, ItemID.FairyCritterPink,               1);
            AddItemInPool(hallowed_forest, ItemID.FairyCritterGreen,              1);
            AddItemInPool(hallowed_forest, ItemID.LightningBug,                   2);
            AddItemInPool(hallowed_desert, ItemID.PearlsandBlock,  41);
            AddItemInPool(hallowed_desert, ItemID.Cactus,          25);
            AddItemInPool(hallowed_desert, ItemID.PixieDust,       15);
            AddItemInPool(hallowed_desert, ItemID.UnicornHorn,     5);
            AddItemInPool(hallowed_desert, ItemID.RainbowBrick,    10);
            AddItemInPool(hallowed_desert, ItemID.LightShard,      15);
            AddItemInPool(hallowed_desert, ItemID.Waterleaf,       25);
            AddItemInPool(hallowed_desert, ItemID.PinkPricklyPear, 16);
            AddItemInPool(hallowed_desert, ItemID.Scorpion,        4);
            AddItemInPool(hallowed_desert, ItemID.BlackScorpion,   3);
            AddItemInPool(hallowed_snow, ItemID.SnowBlock,    27);
            AddItemInPool(hallowed_snow, ItemID.PinkIceBlock, 14);
            AddItemInPool(hallowed_snow, ItemID.PixieDust,    15);
            AddItemInPool(hallowed_snow, ItemID.UnicornHorn,  5);
            AddItemInPool(hallowed_snow, ItemID.RainbowBrick, 10);
            AddItemInPool(hallowed_snow, ItemID.BorealWood,   50);
            AddItemInPool(hallowed_snow, ItemID.Shiverthorn,  25);
            AddItemInPool(hallowed_bars_forest, ItemID.HallowedBar, 5);
            AddItemInPool(hallowed_bars_desert, ItemID.HallowedBar, 5);
            AddItemInPool(hallowed_bars_snow,   ItemID.HallowedBar, 5);

            AddItemInPool(mushroom, ItemID.MudBlock,           5);
            AddItemInPool(mushroom, ItemID.MushroomGrassSeeds, 1);
            AddItemInPool(mushroom, ItemID.GlowingMushroom,    30);

            AddItemInPool(crimson_forest, ItemID.DirtBlock,                      26);
            AddItemInPool(crimson_forest, ItemID.CrimstoneBlock,                 7);
            AddItemInPool(crimson_forest, ItemID.MudBlock,                       8);
            AddItemInPool(crimson_forest, ItemID.Vertebrae,                      25);
            AddItemInPool(crimson_forest, ItemID.CrimsonSeeds,                   7);
            AddItemInPool(crimson_forest, new ItemEntry(ItemID.Shadewood, 1, 3), 50);
            AddItemInPool(crimson_forest, ItemID.ViciousMushroom,                12);
            AddItemInPool(crimson_forest, ItemID.Deathweed,                      25);
            AddItemInPool(crimson_desert, ItemID.CrimsandBlock, 41);
            AddItemInPool(crimson_desert, ItemID.Cactus,        25);
            AddItemInPool(crimson_desert, ItemID.Vertebrae,     25);
            AddItemInPool(crimson_dark_shard, ItemID.DarkShard, 10);
            AddItemInPool(crimson_snow, ItemID.SnowBlock,   27);
            AddItemInPool(crimson_snow, ItemID.RedIceBlock, 14);
            AddItemInPool(crimson_snow, ItemID.Vertebrae,   25);
            AddItemInPool(crimson_snow, ItemID.BorealWood,  50);

            AddItemInPool(corrupt_forest, ItemID.DirtBlock,                     26);
            AddItemInPool(corrupt_forest, ItemID.EbonstoneBlock,                7);
            AddItemInPool(corrupt_forest, ItemID.MudBlock,                      8);
            AddItemInPool(corrupt_forest, ItemID.RottenChunk,                   15);
            AddItemInPool(corrupt_forest, ItemID.WormTooth,                     10);
            AddItemInPool(corrupt_forest, ItemID.CorruptSeeds,                  7);
            AddItemInPool(corrupt_forest, new ItemEntry(ItemID.Ebonwood, 1, 3), 50);
            AddItemInPool(corrupt_forest, ItemID.VileMushroom,                  12);
            AddItemInPool(corrupt_forest, ItemID.Deathweed,                     25);
            AddItemInPool(corrupt_desert, ItemID.EbonsandBlock, 41);
            AddItemInPool(corrupt_desert, ItemID.RottenChunk,   15);
            AddItemInPool(corrupt_desert, ItemID.WormTooth,     10);
            AddItemInPool(corrupt_desert, ItemID.Cactus,        25);
            AddItemInPool(corrupt_snow, ItemID.SnowBlock,      27);
            AddItemInPool(corrupt_snow, ItemID.PurpleIceBlock, 14);
            AddItemInPool(corrupt_snow, ItemID.RottenChunk,    15);
            AddItemInPool(corrupt_snow, ItemID.WormTooth,      10);
            AddItemInPool(corrupt_snow, ItemID.BorealWood,     50);
            AddItemInPool(corrupt_forest_hm, ItemID.CursedFlame, 18);
            AddItemInPool(corrupt_desert_hm, ItemID.CursedFlame, 18);
            AddItemInPool(corrupt_desert_hm, ItemID.DarkShard,   10);
            AddItemInPool(corrupt_snow_hm, ItemID.CursedFlame,   18);

            AddItemInPool(graveyard, ItemID.Lens,   40);
            AddItemInPool(graveyard, ItemID.Mouse,  4);
            AddItemInPool(graveyard, ItemID.Maggot, 3);

            AddItemInPool(shimmer, ItemID.Amethyst,   12);
            AddItemInPool(shimmer, ItemID.Topaz,      11);
            AddItemInPool(shimmer, ItemID.Sapphire,   10);
            AddItemInPool(shimmer, ItemID.Emerald,    9);
            AddItemInPool(shimmer, ItemID.Ruby,       8);
            AddItemInPool(shimmer, ItemID.Diamond,    6);
            AddItemInPool(shimmer, ItemID.Shimmerfly, 44);
            AddItemInPool(marble,  ItemID.Marble,     75);
            AddItemInPool(granite, ItemID.Granite,    75);
            AddItemInPool(granite, ItemID.Geode,      50);
            AddItemInPool(cobweb,  ItemID.Cobweb,     50);
            AddItemInPool(spider,  ItemID.SpiderFang, 25);

            AddItemInPool(ug_snow, ItemID.SnowBlock,  12);
            AddItemInPool(ug_snow, ItemID.IceBlock,   6);
            AddItemInPool(ug_snow, ItemID.SlushBlock, 8);
            AddItemInPool(ug_snow, ItemID.FlinxFur,   6);
            AddItemInPool(ug_snow, ItemID.CyanHusk,   8);

            AddItemInPool(ug_desert, ItemID.SandBlock,       12);
            AddItemInPool(ug_desert, ItemID.HardenedSand,    12);
            AddItemInPool(ug_desert, ItemID.Sandstone,       12);
            AddItemInPool(ug_desert, ItemID.DesertFossil,    12);
            AddItemInPool(ug_desert, ItemID.AntlionMandible, 30);
            AddItemInPool(ug_desert, ItemID.Amber,           8);
            AddItemInPool(ug_desert_hm, ItemID.FossilOre, 10);

            AddItemInPool(ug_jungle,   ItemID.MudBlock,                          16);
            AddItemInPool(ug_jungle,   ItemID.Stinger,                           6);
            AddItemInPool(ug_jungle,   ItemID.Vine,                              5);
            AddItemInPool(ug_jungle,   ItemID.VioletHusk,                        4);
            AddItemInPool(ug_jungle,   new ItemEntry(ItemID.RichMahogany, 1, 3), 30);
            AddItemInPool(ug_jungle,   ItemID.JungleGrassSeeds,                  4);
            AddItemInPool(ug_jungle,   ItemID.JungleSpores,                      5);
            AddItemInPool(ug_jungle,   ItemID.Moonglow,                          13);
            AddItemInPool(ug_jungle,   ItemID.SkyBlueFlower,                     4);
            AddItemInPool(ug_jungle,   ItemID.Grubby,                            3);
            AddItemInPool(ug_jungle,   ItemID.Sluggy,                            3);
            AddItemInPool(ug_jungle,   ItemID.Buggy,                             2);
            AddItemInPool(ug_shells,   ItemID.TurtleShell, 18);
            AddItemInPool(life_fruit,  ItemID.LifeFruit, 10);
            AddItemInPool(chlorophyte, ItemID.ChlorophyteOre, 30);
            AddItemInPool(hive,        ItemID.Hive,         50);
            AddItemInPool(hive,        ItemID.HoneyBlock,   100);
            AddItemInPool(hive,        ItemID.BottledHoney, 150);

            AddItemInPool(ug_hallowed_caverns, ItemID.DirtBlock,       20);
            AddItemInPool(ug_hallowed_caverns, ItemID.PearlstoneBlock, 6);
            AddItemInPool(ug_hallowed_caverns, ItemID.SoulofLight,     24);
            AddItemInPool(ug_hallowed_caverns, ItemID.CrystalShard,    40);
            AddItemInPool(ug_hallowed_desert,  ItemID.PearlsandBlock,     8);
            AddItemInPool(ug_hallowed_desert,  ItemID.HallowHardenedSand, 8);
            AddItemInPool(ug_hallowed_desert,  ItemID.HallowSandstone,    8);
            AddItemInPool(ug_hallowed_desert,  ItemID.SoulofLight,        18);
            AddItemInPool(ug_hallowed_desert,  ItemID.CrystalShard,       30);
            AddItemInPool(ug_hallowed_desert,  ItemID.LightShard,         18);
            AddItemInPool(ug_hallowed_snow,    ItemID.SnowBlock,    12);
            AddItemInPool(ug_hallowed_snow,    ItemID.PinkIceBlock, 12);
            AddItemInPool(ug_hallowed_snow,    ItemID.SoulofLight,  25);
            AddItemInPool(ug_hallowed_snow,    ItemID.CrystalShard, 41);
            AddItemInPool(ug_hallowed_bars_caverns, ItemID.HallowedBar, 5);
            AddItemInPool(ug_hallowed_bars_snow,    ItemID.HallowedBar, 5);
            AddItemInPool(ug_hallowed_bars_desert,  ItemID.HallowedBar, 5);

            AddItemInPool(ug_mushroom,  ItemID.MudBlock,           5);
            AddItemInPool(ug_mushroom,  ItemID.MushroomGrassSeeds, 1);
            AddItemInPool(ug_mushroom,  ItemID.GlowingMushroom,    30);
            AddItemInPool(truffle_worm, ItemID.TruffleWorm,        1);

            AddItemInPool(ug_crimson_caverns, ItemID.DirtBlock,      20);
            AddItemInPool(ug_crimson_caverns, ItemID.CrimstoneBlock, 6);
            AddItemInPool(ug_crimson_caverns, ItemID.Vertebrae,      36);
            AddItemInPool(ug_crimson_caverns, ItemID.CrimtaneOre,    8);
            AddItemInPool(ug_crimson_caverns_hm, ItemID.Ichor,       20);
            AddItemInPool(ug_crimson_caverns_hm, ItemID.SoulofNight, 20);
            AddItemInPool(ug_crimson_desert, ItemID.CrimsandBlock,       8);
            AddItemInPool(ug_crimson_desert, ItemID.CrimsonHardenedSand, 8);
            AddItemInPool(ug_crimson_desert, ItemID.CrimsonSandstone,    8);
            AddItemInPool(ug_crimson_desert, ItemID.Vertebrae,           36);
            AddItemInPool(ug_crimson_desert, ItemID.CrimtaneOre,         8);
            AddItemInPool(ug_crimson_desert_hm, ItemID.Ichor,       16);
            AddItemInPool(ug_crimson_desert_hm, ItemID.SoulofNight, 16);
            AddItemInPool(ug_crimson_desert_hm, ItemID.DarkShard,   16);
            AddItemInPool(ug_crimson_snow, ItemID.SnowBlock,   12);
            AddItemInPool(ug_crimson_snow, ItemID.RedIceBlock, 12);
            AddItemInPool(ug_crimson_snow, ItemID.Vertebrae,   36);
            AddItemInPool(ug_crimson_snow, ItemID.CrimtaneOre, 8);
            AddItemInPool(ug_crimson_snow_hm, ItemID.Ichor,       20);
            AddItemInPool(ug_crimson_snow_hm, ItemID.SoulofNight, 20);

            AddItemInPool(ug_corrupt_caverns, ItemID.DirtBlock,      20);
            AddItemInPool(ug_corrupt_caverns, ItemID.EbonstoneBlock, 6);
            AddItemInPool(ug_corrupt_caverns, ItemID.RottenChunk,    18);
            AddItemInPool(ug_corrupt_caverns, ItemID.WormTooth,      18);
            AddItemInPool(ug_corrupt_caverns, ItemID.DemoniteOre,    8);
            AddItemInPool(ug_corrupt_caverns_hm, ItemID.CursedFlame, 20);
            AddItemInPool(ug_corrupt_caverns_hm, ItemID.SoulofNight, 20);
            AddItemInPool(ug_corrupt_desert, ItemID.EbonsandBlock,       8);
            AddItemInPool(ug_corrupt_desert, ItemID.CorruptHardenedSand, 8);
            AddItemInPool(ug_corrupt_desert, ItemID.CorruptSandstone,    8);
            AddItemInPool(ug_corrupt_desert, ItemID.RottenChunk,         18);
            AddItemInPool(ug_corrupt_desert, ItemID.WormTooth,           18);
            AddItemInPool(ug_corrupt_desert, ItemID.DemoniteOre,         8);
            AddItemInPool(ug_corrupt_desert_hm, ItemID.CursedFlame, 16);
            AddItemInPool(ug_corrupt_desert_hm, ItemID.SoulofNight, 16);
            AddItemInPool(ug_corrupt_desert_hm, ItemID.DarkShard,   16);
            AddItemInPool(ug_corrupt_snow, ItemID.SnowBlock,      12);
            AddItemInPool(ug_corrupt_snow, ItemID.PurpleIceBlock, 12);
            AddItemInPool(ug_corrupt_snow, ItemID.RottenChunk,    18);
            AddItemInPool(ug_corrupt_snow, ItemID.WormTooth,      18);
            AddItemInPool(ug_corrupt_snow, ItemID.DemoniteOre,    8);
            AddItemInPool(ug_corrupt_snow_hm, ItemID.CursedFlame, 20);
            AddItemInPool(ug_corrupt_snow_hm, ItemID.SoulofNight, 20);

            AddItemInPool(dungeon_p, ItemID.AncientPinkDungeonBrick,  12);
            AddItemInPool(dungeon_g, ItemID.AncientGreenDungeonBrick, 12);
            AddItemInPool(dungeon_b, ItemID.AncientBlueDungeonBrick,  12);
            AddItemInPool(dungeon, ItemID.Spike,     6);
            AddItemInPool(dungeon, ItemID.Bone,      36);
            AddItemInPool(dungeon, ItemID.GoldenKey, 1);
            AddItemInPool(ectoplasm, ItemID.Ectoplasm, 20);

            AddItemInPool(temple, ItemID.LihzahrdBrick,       12);
            AddItemInPool(temple, ItemID.WoodenSpike,         6);
            AddItemInPool(temple, ItemID.LunarTabletFragment, 30);
            AddItemInPool(temple, ItemID.LihzahrdPowerCell,   10);

            AddItemInPool(ocean, ItemID.SandBlock,                     16);
            AddItemInPool(ocean, ItemID.ShellPileBlock,                6);
            AddItemInPool(ocean, ItemID.Coral,                         4);
            AddItemInPool(ocean, ItemID.Seashell,                      3);
            AddItemInPool(ocean, ItemID.Starfish,                      2);
            AddItemInPool(ocean, ItemID.TulipShell,                    2);
            AddItemInPool(ocean, ItemID.LightningWhelkShell,           2);
            AddItemInPool(ocean, ItemID.JunoniaShell,                  1);
            AddItemInPool(ocean, new ItemEntry(ItemID.PalmWood, 1, 3), 64);
            AddItemInPool(ocean, ItemID.LimeKelp,                      6);
            AddItemInPool(ocean, ItemID.BlackInk,                      6);
            AddItemInPool(ocean, ItemID.PurpleMucos,                   6);
            AddItemInPool(ocean, ItemID.SharkFin,                      16);
            AddItemInPool(pirate, ItemID.PirateMap, 4);

            AddItemInPool(space, ItemID.Cloud,      4);
            AddItemInPool(space, ItemID.RainCloud,  2);
            AddItemInPool(space, ItemID.FallenStar, 5);
            AddItemInPool(space, ItemID.Feather,    4);
            AddItemInPool(spc_flight, ItemID.SoulofFlight, 2);
            AddItemInPool(pillar, ItemID.FragmentNebula,   8);
            AddItemInPool(pillar, ItemID.FragmentSolar,    8);
            AddItemInPool(pillar, ItemID.FragmentStardust, 8);
            AddItemInPool(pillar, ItemID.FragmentVortex,   8);
            AddItemInPool(luminite, new ItemEntry(ItemID.LunarOre, 1,5), 10);

            AddItemInPool(underworld, ItemID.AshBlock,      20);
            AddItemInPool(underworld, ItemID.Hellstone,     15);
            AddItemInPool(underworld, ItemID.Obsidian,      5);
            AddItemInPool(underworld, ItemID.AshWood,       25);
            AddItemInPool(underworld, ItemID.Fireblossom,   8);
            AddItemInPool(underworld, ItemID.AshGrassSeeds, 3);
            AddItemInPool(underworld, ItemID.HellButterfly, 3);
            AddItemInPool(underworld, ItemID.Lavafly,       3);
            AddItemInPool(underworld, ItemID.MagmaSnail,    2);
            AddItemInPool(uw_fire, new ItemEntry(ItemID.LivingFireBlock, 4, 10), 5);

            AddItemInPool(meteorite, ItemID.Meteorite);
        }
    }
}