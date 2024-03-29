using BiomeExtractorsMod.Common.Collections;
using BiomeExtractorsMod.Content.TileEntities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Common.Systems
{
    public class BiomeCheckSystem : ModSystem
    {

        public enum PoolType
        {
            MINERALS, GEMS, DROPS, TERRAIN, VEGETATION, CRITTERS
        }

        public struct ScanData
        {
            public BiomeExtractorEnt Extractor;
            public Vector2 Origin;
            public SceneMetrics Result;

            public readonly float X => Origin.X;
            public readonly float Y => Origin.Y;

            public readonly int Tiles(ushort tileId) => Result.GetTileCount(tileId);
            public readonly int Liquids(short liquidId) => Result.GetLiquidCount(liquidId);
            public readonly bool MinTier(int tier) => Extractor.GetTier() >= tier;

            public readonly int Tiles(List<ushort> tileIds)
            {
                int count = 0;
                foreach (ushort tileId in tileIds) count += Tiles(tileId);
                return count;
            }
            public readonly bool ValidWalls(List<ushort> wallIds, bool blacklist = false)
            {
                Point origin = Extractor.Position.ToPoint();
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                    {
                        bool match = blacklist;
                        foreach (int id in wallIds)
                        {
                            if(Main.tile[origin.Y + i, origin.X+j].WallType == id)
                                if (!blacklist) { match = true; break; }
                                else { return false; }
                        }
                        if (!match) return false;
                    }
                return true;
            }
        }

        public class ItemEntry(short item, int count)
        {
            public short Id { get; private set; } = item;
            public int Count { get; private set; } = count;

            public override bool Equals(object obj)
            {
                if (obj == null) return false;
                if (obj is not ItemEntry) return false;
                return Id == ((ItemEntry)obj).Id && Count == ((ItemEntry)obj).Count;
            }

            public override int GetHashCode()
            {
                return Id.GetHashCode() * Count.GetHashCode();
            }

            public override string ToString()
            {
                return Main.item[Id].Name + " (" + Count + ")";
            }
        }

        private readonly Dictionary<string, WeightedList<ItemEntry>> _itemPools = [];
        private readonly Dictionary<string, List<Predicate<ScanData>>> _poolRequirements = [];
        private readonly PriorityList<string> _priorityList = [];

        public static readonly string forest = "forest";
        public static readonly string sky = "sky";
        public static readonly string flight = "flight";
        public static readonly string snow = "snow";
        public static readonly string desert = "desert";
        public static readonly string jungle = "jungle";
        public static readonly string shells = "shells";
        public static readonly string hallow = "hallow";
        public static readonly string hallowed_bars = "hallowed_bars";
        public static readonly string hallowed_forest = "hallowed_forest";
        public static readonly string hallowed_desert = "hallowed_desert";
        public static readonly string hallowed_snow = "hallowed_snow";
        public static readonly string mushroom = "mushroom";
        public static readonly string corruption = "corruption";
        public static readonly string corruption_hm = "corruption_hm";
        public static readonly string corrupt_snow = "corrupt_snow";
        public static readonly string corrupt_desert = "corrupt_desert";
        public static readonly string corrupt_forest = "corrupt_forest";
        public static readonly string corrupt_dark_shard = "corrupt_dark_shard";
        public static readonly string crimson = "crimson";
        public static readonly string crimson_snow = "crimson_snow";
        public static readonly string crimson_desert = "crimson_desert";
        public static readonly string crimson_forest = "crimson_forest";
        public static readonly string crimson_dark_shard = "crimson_dark_shard";
        public static readonly string graveyard = "graveyard";
        public static readonly string caverns = "caverns";
        public static readonly string underground = "underground";
        public static readonly string evil_ores = "evil_ores";
        public static readonly string hm_ores = "hm_ores";
        public static readonly string faeling = "faeling";
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
        public static readonly string ug_hallow = "ug_hallow";
        public static readonly string ug_hallowed_bars = "ug_hallowed_bars";
        public static readonly string ug_hallowed_caverns = "ug_hallowed_caverns";
        public static readonly string ug_hallowed_snow = "ug_hallowed_snow";
        public static readonly string ug_hallowed_desert = "ug_hallowed_desert";
        public static readonly string light_shard = "light_shard";
        public static readonly string ug_mushroom = "ug_mushroom";
        public static readonly string truffle_worm = "truffle_worm";
        public static readonly string ug_corruption = "ug_corruption";
        public static readonly string ug_corruption_hm = "ug_corruption_hm";
        public static readonly string ug_corrupt_caverns = "ug_corrupt_caverns";
        public static readonly string ug_corrupt_snow = "ug_corrupt_snow";
        public static readonly string ug_corrupt_desert = "ug_corrupt_desert";
        public static readonly string ug_corrupt_dark_shard = "ug_corrupt_dark_shard";
        public static readonly string ug_crimson = "ug_crimson";
        public static readonly string ug_crimson_hm = "ug_crimson_hm";
        public static readonly string ug_crimson_caverns = "ug_crimson_caverns";
        public static readonly string ug_crimson_snow = "ug_crimson_snow";
        public static readonly string ug_crimson_desert = "ug_crimson_desert";
        public static readonly string ug_crimson_dark_shard = "ug_crimson_dark_shard";
        public static readonly string dark_shard = "dark_shard";
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
        static readonly List<ushort> crimsonForestBlocks = [TileID.CrimsonGrass, TileID.CrimsonJungleGrass,                        TileID.CorruptThorns,   TileID.Crimstone];
        static readonly List<ushort> corruptForestBlocks = [TileID.CorruptGrass, TileID.CorruptJungleGrass, TileID.CorruptThorns,  TileID.CorruptPlants,   TileID.Ebonstone];
        static readonly List<ushort> hallowForestBlocks = [TileID.HallowedGrass,                            TileID.HallowedPlants, TileID.HallowedPlants2, TileID.Pearlstone];
        static readonly List<ushort> crimsonBlocks = [TileID.CrimsonGrass,  TileID.CrimsonJungleGrass, TileID.CorruptThorns,                         TileID.Crimstone,  TileID.Crimsand,  TileID.CrimsonHardenedSand, TileID.CrimsonSandstone, TileID.FleshIce];
        static readonly List<ushort> corruptBlocks = [TileID.CorruptGrass,  TileID.CorruptJungleGrass, TileID.CorruptThorns,  TileID.CorruptPlants,  TileID.Ebonstone,  TileID.Ebonsand,  TileID.CorruptHardenedSand, TileID.CorruptSandstone, TileID.CorruptIce];
        static readonly List<ushort> hallowBlocks =  [TileID.HallowedGrass,                            TileID.HallowedPlants, TileID.HallowedPlants2,TileID.Pearlstone, TileID.Pearlsand, TileID.HallowHardenedSand,  TileID.HallowSandstone,  TileID.HallowedIce];
        static readonly List<ushort> glowMushroomBlocks = [TileID.MushroomGrass, TileID.MushroomPlants, TileID.MushroomTrees, TileID.MushroomVines];
        static readonly List<ushort> jungleBlocks = [TileID.JungleGrass, TileID.JunglePlants, TileID.JunglePlants2, TileID.PlantDetritus, TileID.JungleVines, TileID.Hive, TileID.LihzahrdBrick];
        static readonly List<ushort> frostBlocks = [TileID.SnowBlock, TileID.SnowBrick, TileID.IceBlock, TileID.BreakableIce, TileID.FleshIce, TileID.CorruptIce, TileID.HallowedIce];
        static readonly List<ushort> desertBlocks = [TileID.Sand, TileID.Crimsand, TileID.Ebonsand, TileID.Pearlsand, TileID.HardenedSand, TileID.CrimsonHardenedSand, TileID.CorruptHardenedSand, TileID.HallowHardenedSand, TileID.Sandstone, TileID.CrimsonSandstone, TileID.CorruptSandstone, TileID.HallowSandstone];

        //TIERS
        static readonly Predicate<ScanData> tierDemonic = scan => scan.MinTier((int)BiomeExtractorEnt.EnumTiers.DEMONIC);
        static readonly Predicate<ScanData> tierInfernal = scan => scan.MinTier((int)BiomeExtractorEnt.EnumTiers.INFERNAL);
        static readonly Predicate<ScanData> tierSteampunk = scan => scan.MinTier((int)BiomeExtractorEnt.EnumTiers.STEAMPUNK);
        static readonly Predicate<ScanData> tierCyber = scan => scan.MinTier((int)BiomeExtractorEnt.EnumTiers.CYBER);
        static readonly Predicate<ScanData> tierLunar = scan => scan.MinTier((int)BiomeExtractorEnt.EnumTiers.LUNAR);
        static readonly Predicate<ScanData> tierEthereal = scan => scan.MinTier((int)BiomeExtractorEnt.EnumTiers.ETHEREAL);

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
        static readonly Predicate<ScanData> undergroundLayer = scan => scan.Y > Main.worldSurface + 1;
        static readonly Predicate<ScanData> middleUnderground = scan => scan.Y > (Main.worldSurface + Main.rockLayer) / 2;
        static readonly Predicate<ScanData> cavernLayer = scan => scan.Y > Main.rockLayer + 1;
        static readonly Predicate<ScanData> underworldLayer = scan => scan.Y > Main.maxTilesY - 200;

        //SPECIFIC POSITIONS
        static readonly Predicate<ScanData> oceanArea = scan => scan.Y <= (Main.worldSurface + Main.rockLayer) / 2 && (scan.X < 339 || scan.Y > Main.maxTilesX - 339);

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
        static readonly Func<List<ushort>, Predicate<ScanData>> evil300 = tiles => (scan => scan.Tiles(tiles) - scan.Tiles(hallowBlocks) - scan.Tiles(TileID.Sunflower) * 80 >= 300);
        static readonly Func<List<ushort>, Predicate<ScanData>> hallow125 = tiles => (scan => scan.Tiles(tiles) - scan.Tiles(crimsonBlocks) - scan.Tiles(corruptBlocks) >= 125);
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
        static readonly Predicate<ScanData> tombstone5 = scan => scan.Tiles(TileID.Tombstones) > 5;




        //scans the area around the given ScanData.Origin
        //saves the result in the same Scandata's Result parameter, but also
        //returns it to allow for more inline processing
        //caution: if there already was a Result saved inside
        //this ScanData, it will be overwritten
        public static ScanData Scan(ScanData data)
        {
            SceneMetrics metric = new();
            SceneMetricsScanSettings settings = new()
            {
                BiomeScanCenterPositionInWorld = data.Origin,
                ScanOreFinderData = false
            };
            metric.ScanAndExportToMain(settings);
            data.Result = metric;
            return data;
        }

        public bool PoolExists(string poolName)
        {
            return _itemPools.ContainsKey(poolName);
        }
        public bool AddPool(string poolName, int priority)
        {
            if (!PoolExists(poolName))
            {
                _priorityList.Add(priority, poolName);
                _itemPools.Add(poolName, []); //Consider dividing pools up by content
                _poolRequirements.Add(poolName, []);
                return true;
            }
            return false;
        }
        public bool RemovePool(string poolName)
        {
            if (PoolExists(poolName))
            {
                foreach (List<string> l in _priorityList.Values)
                {
                    l.Remove(poolName);
                }
                _itemPools.Remove(poolName);
                _poolRequirements.Remove(poolName);
                return true;
            }
            return false;
        }

        public void AddPoolRequirements(string poolName, params Predicate<ScanData>[] conditions)
        {
            foreach(Predicate<ScanData> condition in conditions)
                _poolRequirements[poolName].Add(condition);
        }
        public void FlushPoolRequirements(string poolName)
        {
            _poolRequirements[poolName].Clear();

        }

        public void AddItemInPool(string poolName, short itemId) => AddItemInPool(poolName, new ItemEntry(itemId, 1), 1);
        public void AddItemInPool(string poolName, short itemId, int weight) => AddItemInPool(poolName, new ItemEntry(itemId, 1), weight);
        public void AddItemInPool(string poolName, short itemId, int count, int weight) => AddItemInPool(poolName, new ItemEntry(itemId, count), weight);
        public void AddItemInPool(string poolName, ItemEntry item, int weight)
        {
            _itemPools[poolName].Add(item, weight);
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

        public List<string> CheckValidBiomes(BiomeExtractorEnt extractor)
        {
            ScanData scan = new()
            {
                Extractor = extractor,
                Origin = extractor.Position.ToVector2() + Vector2.One
            };
            Scan(scan);

            int last_p = int.MaxValue;
            List<string> found = [];
            foreach(KeyValuePair<int, string> elem in _priorityList.EnumerateInOrder())
            {
                if (found.Count > 0 && last_p != elem.Key) break;
                last_p = elem.Key;

                bool check_passed = true;
                foreach (Predicate<ScanData> check in _poolRequirements[elem.Value])
                {
                    if (!check.Invoke(scan))
                    {
                        check_passed = false;
                        break;
                    }
                }
                if (check_passed && _itemPools[elem.Value].Count>0) found.Add(elem.Value);
            }
            return found;
        }

        public Item RollItem(List<string> pools)
        {
            if (pools.Count == 1)
            {
                ItemEntry entry = _itemPools[pools[0]].Roll();
                return new(entry.Id, entry.Count);
            }

            int totalWeight = 0;
            foreach (string pool in pools)
                totalWeight += _itemPools[pool].TotalWeight;
            int roll = Main.rand.Next(totalWeight);
            int current = 0;
            ItemEntry result = new(ItemID.None, 1);
            foreach (string pool in pools)
            {
                current += _itemPools[pool].TotalWeight;
                if (current > roll)
                {
                    int weight = roll - (current - _itemPools[pool].TotalWeight);
                    result = _itemPools[pool].FromWeight(weight);
                    break;
                }
            }

            Item item = new(result.Id, result.Count);

            return item;
        }

        public override void PostSetupContent()
        {
            InitializePools();
            SetRequirements();
            PopulatePools();
        }

        private void InitializePools()
        {
            AddPool(forest, 0);

            AddPool(snow,   50);
            AddPool(desert, 50);
            AddPool(jungle, 50);
            AddPool(shells, 50);
            AddPool(sky,    50);
            AddPool(flight, 50);

            AddPool(hallow,           100);
            AddPool(hallowed_bars,    100);
            AddPool(hallowed_forest,  100);      
            AddPool(hallowed_desert,  100);
            AddPool(hallowed_snow,    100);

            AddPool(mushroom, 200);
            
            AddPool(corruption,     300);
            AddPool(corruption_hm,  300);
            AddPool(crimson,        300);
            AddPool(corrupt_snow,   300);
            AddPool(crimson_snow,   300);
            AddPool(corrupt_desert, 300);
            AddPool(crimson_desert, 300);
            AddPool(dark_shard,     300);

            AddPool(graveyard, 500);

            AddPool(caverns,      1000);
            AddPool(underground,  1000);
            AddPool(evil_ores,    1000);
            AddPool(hm_ores,      1000);

            AddPool(faeling,      1000);
            AddPool(spider,       1000);
            AddPool(cobweb,       1000);
            AddPool(granite,      1000);
            AddPool(marble,       1000);

            AddPool(ug_snow,      1050);
            AddPool(ug_desert,    1050);
            AddPool(ug_desert_hm, 1050);
            AddPool(ug_jungle,    1050);
            AddPool(ug_shells,    1050);
            AddPool(life_fruit,   1050);
            AddPool(hive,         1050);
            AddPool(chlorophyte,  1050);

            AddPool(ug_hallow,               1100);
            AddPool(ug_hallowed_bars,        1100);
            AddPool(ug_hallowed_caverns,     1100);
            AddPool(ug_hallowed_snow,        1100);
            AddPool(ug_hallowed_desert,      1100);

            AddPool(ug_mushroom,  1200);
            AddPool(truffle_worm, 1200);

            AddPool(ug_corruption,         1300);
            AddPool(ug_crimson,            1300);
            AddPool(ug_corruption_hm,      1300);
            AddPool(ug_crimson_hm,         1300);
            AddPool(ug_corrupt_caverns,    1300);
            AddPool(ug_crimson_caverns,    1300);
            AddPool(ug_corrupt_snow,       1300);
            AddPool(ug_crimson_snow,       1300);
            AddPool(ug_corrupt_desert,     1300);
            AddPool(ug_crimson_desert,     1300);
            AddPool(ug_corrupt_dark_shard, 1300);
            AddPool(ug_crimson_dark_shard, 1300);
            
            AddPool(dungeon,   2000);
            AddPool(dungeon_p, 2000);
            AddPool(dungeon_g, 2000);
            AddPool(dungeon_b, 2000);
            AddPool(ectoplasm, 2000);
            AddPool(temple,    2000);

            AddPool(ocean,  2500);
            AddPool(pirate, 2500);
            
            AddPool(space,      4000);
            AddPool(spc_flight, 4000);
            AddPool(pillar,     4000);
            AddPool(luminite,   4000);
            AddPool(underworld, 4000);
            AddPool(uw_fire,    4000);
            AddPool(meteorite, 10000);
        }

        private void SetRequirements()
        {
            //Standard order: tier, progression, layer, blocks, liquids, more complicated stuff, walls 
            AddPoolRequirements(meteorite,  tierInfernal,                                 meteorite75);
            AddPoolRequirements(uw_fire,    tierSteampunk, hardmodeOnly, underworldLayer);
            AddPoolRequirements(underworld, tierInfernal,                underworldLayer);
            AddPoolRequirements(luminite,   tierLunar,     postML,       spaceLayer);
            AddPoolRequirements(pillar,     tierLunar,     postPillars,  spaceLayer);
            AddPoolRequirements(spc_flight, tierSteampunk, hardmodeOnly, spaceLayer);
            AddPoolRequirements(space,                                   spaceLayer);

            AddPoolRequirements(pirate, tierSteampunk, hardmodeOnly, water1k, oceanArea);
            AddPoolRequirements(ocean,                               water1k, oceanArea);

            AddPoolRequirements(temple,    tierCyber,   postGolem,                                  lihzahrd_bg);
            AddPoolRequirements(ectoplasm, tierCyber,   postPlantera, dungeon250, undergroundLayer, dungeon_bg);
            AddPoolRequirements(dungeon_p, tierDemonic, dungeon_p250,             undergroundLayer, dungeon_bg_p);
            AddPoolRequirements(dungeon_g, tierDemonic, dungeon_g250,             undergroundLayer, dungeon_bg_g);
            AddPoolRequirements(dungeon_b, tierDemonic, dungeon_b250,             undergroundLayer, dungeon_bg_b);
            AddPoolRequirements(dungeon,   tierDemonic, dungeon250,               undergroundLayer, dungeon_bg);

            AddPoolRequirements(ug_crimson_dark_shard, tierSteampunk, cavernLayer, evil300.Invoke(crimsonSandBlocks));
            AddPoolRequirements(ug_crimson_desert,     tierDemonic,   cavernLayer, evil300.Invoke(crimsonSandBlocks));
            AddPoolRequirements(ug_crimson_snow,       tierDemonic,   cavernLayer, evil300.Invoke(crimsonIceBlocks));
            AddPoolRequirements(ug_crimson_caverns,    tierDemonic,   cavernLayer, evil300.Invoke(crimsonForestBlocks));
            AddPoolRequirements(ug_crimson_hm,         tierSteampunk, cavernLayer, evil300.Invoke(crimsonBlocks));
            AddPoolRequirements(ug_crimson,            tierDemonic,   cavernLayer, evil300.Invoke(crimsonBlocks));

            AddPoolRequirements(ug_corrupt_dark_shard, tierSteampunk, cavernLayer, evil300.Invoke(corruptSandBlocks));
            AddPoolRequirements(ug_corrupt_desert,     tierDemonic,   cavernLayer, evil300.Invoke(corruptSandBlocks));
            AddPoolRequirements(ug_corrupt_snow,       tierDemonic,   cavernLayer, evil300.Invoke(corruptIceBlocks));
            AddPoolRequirements(ug_corrupt_caverns,    tierDemonic,   cavernLayer, evil300.Invoke(corruptForestBlocks));
            AddPoolRequirements(ug_corruption_hm,      tierSteampunk, cavernLayer, evil300.Invoke(corruptBlocks));
            AddPoolRequirements(ug_corruption,         tierDemonic,   cavernLayer, evil300.Invoke(corruptBlocks));

            AddPoolRequirements(truffle_worm, tierSteampunk, hardmodeOnly, cavernLayer, mush100);
            AddPoolRequirements(ug_mushroom,                               cavernLayer, mush100);

            AddPoolRequirements(ug_hallowed_bars,    tierSteampunk, postMechs, cavernLayer, evil300.Invoke(hallowBlocks));
            AddPoolRequirements(ug_hallowed_caverns, tierSteampunk,            cavernLayer, evil300.Invoke(hallowForestBlocks));
            AddPoolRequirements(ug_hallowed_desert,  tierSteampunk,            cavernLayer, evil300.Invoke(hallowSandBlocks));
            AddPoolRequirements(ug_hallowed_snow,    tierSteampunk,            cavernLayer, evil300.Invoke(hallowIceBlocks));
            AddPoolRequirements(ug_hallow,           tierSteampunk,            cavernLayer, evil300.Invoke(hallowBlocks));

            AddPoolRequirements(chlorophyte,  tierSteampunk, postMechs,    cavernLayer, jungle140);
            AddPoolRequirements(ug_shells,    tierSteampunk, hardmodeOnly, cavernLayer, jungle140);
            AddPoolRequirements(life_fruit,   tierSteampunk, postMech,     cavernLayer, jungle140);
            AddPoolRequirements(hive,                                      cavernLayer, hive100,    honey100, hive_bg);
            AddPoolRequirements(ug_jungle,                                 cavernLayer, jungle140);
            AddPoolRequirements(ug_desert_hm, tierSteampunk, hardmodeOnly, cavernLayer, desert1500);
            AddPoolRequirements(ug_desert,                                 cavernLayer, desert1500);
            AddPoolRequirements(ug_snow,                                   cavernLayer, frost1500);

            AddPoolRequirements(marble,  cavernLayer, marble150,    marble_bg);
            AddPoolRequirements(granite, cavernLayer, granite150,   granite_bg);
            AddPoolRequirements(cobweb,  cavernLayer, hardmodeOnly, spider_bg);
            AddPoolRequirements(spider,  cavernLayer,               spider_bg);

            AddPoolRequirements(caverns,                                 cavernLayer);
            AddPoolRequirements(underground,                             undergroundLayer);
            AddPoolRequirements(evil_ores,  tierDemonic,                 undergroundLayer);
            AddPoolRequirements(hm_ores,    tierSteampunk, hardmodeOnly, undergroundLayer);
            AddPoolRequirements(faeling,    tierDemonic,                 shimmer300);

            AddPoolRequirements(graveyard, surfaceLayer, tombstone5);

            AddPoolRequirements(crimson_dark_shard, tierSteampunk, hardmodeOnly, evil300.Invoke(crimsonSandBlocks));
            AddPoolRequirements(corrupt_dark_shard, tierSteampunk, hardmodeOnly, evil300.Invoke(corruptSandBlocks));
            AddPoolRequirements(crimson_forest,     tierDemonic,   surfaceLayer, evil300.Invoke(crimsonForestBlocks));
            AddPoolRequirements(corrupt_forest,     tierDemonic,   surfaceLayer, evil300.Invoke(corruptForestBlocks));
            AddPoolRequirements(crimson_desert,     tierDemonic,   surfaceLayer, evil300.Invoke(crimsonSandBlocks));
            AddPoolRequirements(corrupt_desert,     tierDemonic,   surfaceLayer, evil300.Invoke(corruptSandBlocks));
            AddPoolRequirements(crimson_snow,       tierDemonic,   surfaceLayer, evil300.Invoke(crimsonIceBlocks));
            AddPoolRequirements(corrupt_snow,       tierDemonic,   surfaceLayer, evil300.Invoke(corruptIceBlocks));
            AddPoolRequirements(crimson,            tierDemonic,   surfaceLayer, evil300.Invoke(crimsonBlocks));
            AddPoolRequirements(corruption,         tierDemonic,   surfaceLayer, evil300.Invoke(corruptBlocks));
            AddPoolRequirements(corruption_hm,      tierSteampunk, surfaceLayer, evil300.Invoke(corruptBlocks));

            AddPoolRequirements(mushroom, mush100);

            AddPoolRequirements(hallowed_desert,  tierSteampunk, hardmodeOnly, hallow125.Invoke(hallowIceBlocks));
            AddPoolRequirements(hallowed_forest,  tierSteampunk, hardmodeOnly, hallow125.Invoke(hallowForestBlocks));
            AddPoolRequirements(hallowed_desert,  tierSteampunk, hardmodeOnly, hallow125.Invoke(hallowSandBlocks));
            AddPoolRequirements(hallowed_bars,    tierSteampunk, postMechs,    hallow125.Invoke(hallowBlocks));
            AddPoolRequirements(hallow,           tierSteampunk, hardmodeOnly, hallow125.Invoke(hallowBlocks));

            AddPoolRequirements(shells, tierSteampunk, hardmodeOnly, jungle140);
            AddPoolRequirements(jungle,                              jungle140);
            AddPoolRequirements(desert,                              desert1500);
            AddPoolRequirements(snow,                                frost1500);
            AddPoolRequirements(flight, tierSteampunk, hardmodeOnly, skyLayer);
            AddPoolRequirements(sky,                                 skyLayer);
        }

        private void PopulatePools()
        {
            AddItemInPool(forest, ItemID.DirtBlock);
            AddItemInPool(forest, ItemID.StoneBlock);
            AddItemInPool(forest, ItemID.ClayBlock);
            AddItemInPool(forest, ItemID.Gel);
            AddItemInPool(forest, ItemID.PinkGel);
            AddItemInPool(forest, ItemID.GrassSeeds);
            AddItemInPool(forest, ItemID.Wood);
            AddItemInPool(forest, ItemID.Acorn);
            AddItemInPool(forest, ItemID.Daybloom);
            AddItemInPool(forest, ItemID.Mushroom);
            AddItemInPool(forest, ItemID.YellowMarigold);
            AddItemInPool(forest, ItemID.BlueBerries);
            AddItemInPool(forest, ItemID.JuliaButterfly);
            AddItemInPool(forest, ItemID.MonarchButterfly);
            AddItemInPool(forest, ItemID.PurpleEmperorButterfly);
            AddItemInPool(forest, ItemID.RedAdmiralButterfly);
            AddItemInPool(forest, ItemID.SulphurButterfly);
            AddItemInPool(forest, ItemID.TreeNymphButterfly);
            AddItemInPool(forest, ItemID.UlyssesButterfly);
            AddItemInPool(forest, ItemID.ZebraSwallowtailButterfly);
            AddItemInPool(forest, ItemID.BlueDragonfly);
            AddItemInPool(forest, ItemID.GreenDragonfly);
            AddItemInPool(forest, ItemID.RedDragonfly);
            AddItemInPool(forest, ItemID.Grasshopper);
            AddItemInPool(forest, ItemID.Firefly);
            AddItemInPool(forest, ItemID.Worm);
            AddItemInPool(forest, ItemID.Stinkbug);

            AddItemInPool(sky,    ItemID.Cloud, 7);
            AddItemInPool(sky,    ItemID.RainCloud, 3);
            AddItemInPool(sky,    ItemID.Feather, 20);
            AddItemInPool(flight, ItemID.SoulofFlight, 5);

            AddItemInPool(snow, ItemID.SnowBlock);
            AddItemInPool(snow, ItemID.IceBlock);
            AddItemInPool(snow, ItemID.BorealWood);
            AddItemInPool(snow, ItemID.Shiverthorn);

            AddItemInPool(desert, ItemID.SandBlock);
            AddItemInPool(desert, ItemID.Cactus);
            AddItemInPool(desert, ItemID.Waterleaf);
            AddItemInPool(desert, ItemID.PinkPricklyPear);
            AddItemInPool(desert, ItemID.Scorpion);
            AddItemInPool(desert, ItemID.BlackScorpion);

            AddItemInPool(jungle, ItemID.MudBlock);
            AddItemInPool(jungle, ItemID.RichMahogany);
            AddItemInPool(jungle, ItemID.BambooBlock);
            AddItemInPool(jungle, ItemID.JungleGrassSeeds);
            AddItemInPool(jungle, ItemID.Moonglow);
            AddItemInPool(jungle, ItemID.SkyBlueFlower);
            AddItemInPool(jungle, ItemID.Frog);
            AddItemInPool(jungle, ItemID.Grubby);
            AddItemInPool(jungle, ItemID.Sluggy);
            AddItemInPool(jungle, ItemID.Buggy);
            AddItemInPool(shells, ItemID.TurtleShell);

            AddItemInPool(hallow, ItemID.PixieDust);
            AddItemInPool(hallow, ItemID.UnicornHorn);
            AddItemInPool(hallow, ItemID.RainbowBrick);
            AddItemInPool(hallow, ItemID.FairyCritterBlue);
            AddItemInPool(hallow, ItemID.FairyCritterPink);
            AddItemInPool(hallow, ItemID.FairyCritterGreen);
            AddItemInPool(hallowed_forest, ItemID.DirtBlock);
            AddItemInPool(hallowed_forest, ItemID.PearlstoneBlock);
            AddItemInPool(hallowed_forest, ItemID.Gel);
            AddItemInPool(hallowed_forest, ItemID.Pearlwood);
            AddItemInPool(hallowed_forest, ItemID.LightningBug);
            AddItemInPool(hallowed_desert, ItemID.PearlsandBlock);
            AddItemInPool(hallowed_desert, ItemID.LightShard);
            AddItemInPool(hallowed_desert, ItemID.Cactus);
            AddItemInPool(hallowed_desert, ItemID.Waterleaf);
            AddItemInPool(hallowed_desert, ItemID.PinkPricklyPear);
            AddItemInPool(hallowed_desert, ItemID.Scorpion);
            AddItemInPool(hallowed_desert, ItemID.BlackScorpion);
            AddItemInPool(hallowed_snow, ItemID.SnowBlock);
            AddItemInPool(hallowed_snow, ItemID.PinkIceBlock);
            AddItemInPool(hallowed_snow, ItemID.BorealWood);
            AddItemInPool(hallowed_snow, ItemID.Shiverthorn);
            AddItemInPool(hallowed_bars, ItemID.HallowedBar);

            AddItemInPool(mushroom, ItemID.MudBlock, 5);
            AddItemInPool(mushroom, ItemID.MushroomGrassSeeds, 1);
            AddItemInPool(mushroom, ItemID.GlowingMushroom, 30);

            AddItemInPool(crimson, ItemID.Vertebrae);
            AddItemInPool(crimson_forest, ItemID.DirtBlock);
            AddItemInPool(corrupt_forest, ItemID.CrimsonSeeds);
            AddItemInPool(crimson_forest, ItemID.CrimstoneBlock);
            AddItemInPool(crimson_forest, ItemID.Shadewood);
            AddItemInPool(crimson_forest, ItemID.ViciousMushroom);
            AddItemInPool(crimson_forest, ItemID.Deathweed);
            AddItemInPool(crimson_desert, ItemID.CrimsandBlock);
            AddItemInPool(crimson_desert, ItemID.Cactus);
            AddItemInPool(crimson_dark_shard, ItemID.DarkShard);
            AddItemInPool(crimson_snow, ItemID.SnowBlock);
            AddItemInPool(crimson_snow, ItemID.RedIceBlock);
            AddItemInPool(crimson_snow, ItemID.BorealWood);

            AddItemInPool(corruption, ItemID.RottenChunk);
            AddItemInPool(corruption, ItemID.WormTooth);
            AddItemInPool(corruption_hm, ItemID.CursedFlame);
            AddItemInPool(corrupt_forest, ItemID.DirtBlock);
            AddItemInPool(corrupt_forest, ItemID.CorruptSeeds);
            AddItemInPool(corrupt_forest, ItemID.EbonstoneBlock);
            AddItemInPool(corrupt_forest, ItemID.Ebonwood);
            AddItemInPool(corrupt_forest, ItemID.VileMushroom);
            AddItemInPool(corrupt_forest, ItemID.Deathweed);
            AddItemInPool(corrupt_desert, ItemID.EbonsandBlock);
            AddItemInPool(corrupt_desert, ItemID.Cactus);
            AddItemInPool(corrupt_dark_shard, ItemID.DarkShard);
            AddItemInPool(corrupt_snow, ItemID.SnowBlock);
            AddItemInPool(corrupt_snow, ItemID.PurpleIceBlock);
            AddItemInPool(corrupt_snow, ItemID.BorealWood);

            AddItemInPool(graveyard, ItemID.Lens,  1000);
            AddItemInPool(graveyard, ItemID.Mouse,  100);
            AddItemInPool(graveyard, ItemID.Maggot, 100);

            AddItemInPool(underground, ItemID.StoneBlock);
            AddItemInPool(underground, ItemID.DirtBlock);
            AddItemInPool(underground, ItemID.CopperOre);
            AddItemInPool(underground, ItemID.TinOre);
            AddItemInPool(underground, ItemID.IronOre);
            AddItemInPool(underground, ItemID.LeadOre);
            AddItemInPool(underground, ItemID.SilverOre);
            AddItemInPool(underground, ItemID.TungstenOre);
            AddItemInPool(underground, ItemID.GoldOre);
            AddItemInPool(underground, ItemID.PlatinumOre);
            AddItemInPool(underground, ItemID.Amethyst);
            AddItemInPool(underground, ItemID.Topaz);
            AddItemInPool(underground, ItemID.Sapphire);
            AddItemInPool(underground, ItemID.Emerald);
            AddItemInPool(underground, ItemID.Ruby);
            AddItemInPool(underground, ItemID.SiltBlock);
            AddItemInPool(underground, ItemID.Diamond);
            AddItemInPool(underground, ItemID.OrangeBloodroot);
            AddItemInPool(underground, ItemID.GreenMushroom);
            AddItemInPool(underground, ItemID.TealMushroom);
            AddItemInPool(caverns, ItemID.RedHusk);
            AddItemInPool(caverns, ItemID.LimeKelp);
            AddItemInPool(evil_ores, ItemID.CrimtaneOre);
            AddItemInPool(evil_ores, ItemID.DemoniteOre);
            AddItemInPool(hm_ores, ItemID.CobaltOre);
            AddItemInPool(hm_ores, ItemID.PalladiumOre);
            AddItemInPool(hm_ores, ItemID.OrichalcumOre);
            AddItemInPool(hm_ores, ItemID.MythrilOre);
            AddItemInPool(hm_ores, ItemID.TitaniumOre);
            AddItemInPool(hm_ores, ItemID.AdamantiteOre);

            AddItemInPool(faeling, ItemID.Shimmerfly);
            AddItemInPool(marble,  ItemID.MarbleBlock);
            AddItemInPool(granite, ItemID.GraniteBlock);
            AddItemInPool(granite, ItemID.Geode);
            AddItemInPool(cobweb,  ItemID.Cobweb);
            AddItemInPool(spider,  ItemID.SpiderFang);

            AddItemInPool(ug_snow, ItemID.SnowBlock);
            AddItemInPool(ug_snow, ItemID.IceBlock);
            AddItemInPool(ug_snow, ItemID.SlushBlock);
            AddItemInPool(ug_snow, ItemID.FlinxFur);
            AddItemInPool(ug_snow, ItemID.CyanHusk);

            AddItemInPool(ug_desert, ItemID.SandBlock);
            AddItemInPool(ug_desert, ItemID.HardenedSand);
            AddItemInPool(ug_desert, ItemID.Sandstone);
            AddItemInPool(ug_desert, ItemID.DesertFossil);
            AddItemInPool(ug_desert, ItemID.AntlionMandible);
            AddItemInPool(ug_desert, ItemID.Amber);
            AddItemInPool(ug_desert_hm, ItemID.FossilOre);

            AddItemInPool(ug_jungle,   ItemID.MudBlock);
            AddItemInPool(ug_jungle,   ItemID.Stinger);
            AddItemInPool(ug_jungle,   ItemID.Vine);
            AddItemInPool(ug_jungle,   ItemID.VioletHusk);
            AddItemInPool(ug_jungle,   ItemID.RichMahogany);
            AddItemInPool(ug_jungle,   ItemID.JungleGrassSeeds);
            AddItemInPool(ug_jungle,   ItemID.JungleSpores);
            AddItemInPool(ug_jungle,   ItemID.Moonglow);
            AddItemInPool(ug_jungle,   ItemID.SkyBlueFlower);
            AddItemInPool(ug_jungle,   ItemID.Grubby);
            AddItemInPool(ug_jungle,   ItemID.Sluggy);
            AddItemInPool(ug_jungle,   ItemID.Buggy);
            AddItemInPool(ug_shells,   ItemID.TurtleShell);
            AddItemInPool(life_fruit,  ItemID.LifeFruit);
            AddItemInPool(chlorophyte, ItemID.ChlorophyteOre);
            AddItemInPool(hive,        ItemID.Hive);
            AddItemInPool(hive,        ItemID.HoneyBlock);
            AddItemInPool(hive,        ItemID.BottledHoney);

            AddItemInPool(ug_hallow, ItemID.SoulofLight);
            AddItemInPool(ug_hallow, ItemID.CrystalShard);
            AddItemInPool(ug_hallowed_caverns, ItemID.DirtBlock);
            AddItemInPool(ug_hallowed_caverns, ItemID.PearlstoneBlock);
            AddItemInPool(ug_hallowed_desert,  ItemID.PearlsandBlock);
            AddItemInPool(ug_hallowed_desert,  ItemID.HallowHardenedSand);
            AddItemInPool(ug_hallowed_desert,  ItemID.HallowSandstone);
            AddItemInPool(ug_hallowed_desert,  ItemID.LightShard);
            AddItemInPool(ug_hallowed_snow,    ItemID.SnowBlock);
            AddItemInPool(ug_hallowed_snow,    ItemID.PinkIceBlock);
            AddItemInPool(ug_hallowed_bars,    ItemID.HallowedBar);

            AddItemInPool(mushroom, ItemID.MudBlock, 5);
            AddItemInPool(mushroom, ItemID.MushroomGrassSeeds, 1);
            AddItemInPool(mushroom, ItemID.GlowingMushroom, 30);

            AddItemInPool(ug_crimson, ItemID.Vertebrae);
            AddItemInPool(ug_crimson, ItemID.CrimtaneOre);
            AddItemInPool(ug_crimson_hm, ItemID.Ichor);
            AddItemInPool(ug_crimson_hm, ItemID.SoulofNight);
            AddItemInPool(ug_crimson_caverns, ItemID.DirtBlock);
            AddItemInPool(ug_crimson_caverns, ItemID.CrimstoneBlock);
            AddItemInPool(ug_crimson_desert, ItemID.CrimsandBlock);
            AddItemInPool(ug_crimson_desert, ItemID.CrimsonHardenedSand);
            AddItemInPool(ug_crimson_desert, ItemID.CrimsonSandstone);
            AddItemInPool(ug_crimson_dark_shard, ItemID.DarkShard);
            AddItemInPool(ug_crimson_snow, ItemID.SnowBlock);
            AddItemInPool(ug_crimson_snow, ItemID.RedIceBlock);

            AddItemInPool(ug_corruption, ItemID.RottenChunk);
            AddItemInPool(ug_corruption, ItemID.WormTooth);
            AddItemInPool(ug_corruption, ItemID.DemoniteOre);
            AddItemInPool(ug_corruption_hm, ItemID.CursedFlame);
            AddItemInPool(ug_corruption_hm, ItemID.SoulofNight);
            AddItemInPool(ug_corrupt_caverns, ItemID.DirtBlock);
            AddItemInPool(ug_corrupt_caverns, ItemID.EbonstoneBlock);
            AddItemInPool(ug_corrupt_desert, ItemID.EbonsandBlock);
            AddItemInPool(ug_corrupt_desert, ItemID.CorruptHardenedSand);
            AddItemInPool(ug_corrupt_desert, ItemID.CorruptSandstone);
            AddItemInPool(ug_corrupt_dark_shard, ItemID.DarkShard);
            AddItemInPool(ug_corrupt_snow, ItemID.SnowBlock);
            AddItemInPool(ug_corrupt_snow, ItemID.PurpleIceBlock);

            AddItemInPool(dungeon_p, ItemID.AncientPinkDungeonBrick);
            AddItemInPool(dungeon_g, ItemID.AncientGreenDungeonBrick);
            AddItemInPool(dungeon_b, ItemID.AncientBlueDungeonBrick);
            AddItemInPool(dungeon, ItemID.Spike);
            AddItemInPool(dungeon, ItemID.Bone);
            AddItemInPool(dungeon, ItemID.GoldenKey);
            AddItemInPool(ectoplasm, ItemID.Ectoplasm);

            AddItemInPool(temple, ItemID.LihzahrdBrick);
            AddItemInPool(temple, ItemID.WoodenSpike);
            AddItemInPool(temple, ItemID.LunarTabletFragment);
            AddItemInPool(temple, ItemID.LihzahrdPowerCell);

            AddItemInPool(space, ItemID.FallenStar, 5);
            AddItemInPool(space, ItemID.Feather, 4);
            AddItemInPool(spc_flight, ItemID.SoulofFlight, 1);
            AddItemInPool(pillar, ItemID.FragmentNebula, 10);
            AddItemInPool(pillar, ItemID.FragmentSolar, 10);
            AddItemInPool(pillar, ItemID.FragmentStardust, 10);
            AddItemInPool(pillar, ItemID.FragmentVortex, 10);
            AddItemInPool(luminite, ItemID.LunarOre, 20);

            AddItemInPool(underworld, ItemID.AshBlock);
            AddItemInPool(underworld, ItemID.Hellstone);
            AddItemInPool(underworld, ItemID.Obsidian);
            AddItemInPool(underworld, ItemID.Fireblossom);
            AddItemInPool(underworld, ItemID.AshWood);
            AddItemInPool(underworld, ItemID.AshGrassSeeds);
            AddItemInPool(underworld, ItemID.HellButterfly);
            AddItemInPool(underworld, ItemID.Lavafly);
            AddItemInPool(underworld, ItemID.MagmaSnail);
            AddItemInPool(uw_fire, ItemID.LivingFireBlock);

            AddItemInPool(meteorite, ItemID.Meteorite);
        }
    }
}