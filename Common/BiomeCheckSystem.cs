using BiomeExtractorsMod.Content.TileEntities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Common.Systems
{
    public class BiomeCheckSystem : ModSystem
    {
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

        public class ItemEntry(int item, int count)
        {
            public int Id { get; private set; } = item;
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
        public static readonly string hallowed_snow = "hallowed_snow";
        public static readonly string hallowed_bars_snow = "hallowed_bars_snow";
        public static readonly string hallowed_desert = "hallowed_desert";
        public static readonly string hallowed_bars_desert = "hallowed_bars_desert";
        public static readonly string mushroom = "mushroom";
        public static readonly string corruption = "corruption";
        public static readonly string corrupt_snow = "corrupt_snow";
        public static readonly string corrupt_desert = "corrupt_desert";
        public static readonly string crimson = "crimson";
        public static readonly string crimson_snow = "crimson_snow";
        public static readonly string crimson_desert = "crimson_desert";
        public static readonly string graveyard = "graveyard";
        public static readonly string basic_ores = "basic_ores";
        public static readonly string evil_ores = "evil_ores";
        public static readonly string hm_ores = "hm_ores";
        public static readonly string faeling = "faeling";
        public static readonly string granite = "granite";
        public static readonly string marble = "marble";
        public static readonly string cobweb = "cobweb";
        public static readonly string spider = "spider";
        public static readonly string ug_snow = "ug_snow";
        public static readonly string ug_desert = "ug_desert";
        public static readonly string ug_jungle = "ug_jungle";
        public static readonly string hive = "hive";
        public static readonly string ug_jungle_hm = "ug_jungle_hm";
        public static readonly string chlorophyte = "chlorophyte";
        public static readonly string ug_hallow = "ug_hallow";
        public static readonly string ug_hallowed_bars = "ug_hallowed_bars";
        public static readonly string ug_hallowed_snow = "ug_hallowed_snow";
        public static readonly string ug_hallowed_bars_snow = "ug_hallowed_bars_snow";
        public static readonly string ug_hallowed_desert = "ug_hallowed_desert";
        public static readonly string ug_hallowed_bars_desert = "ug_hallowed_bars_desert";
        public static readonly string light_shard = "light_shard";
        public static readonly string ug_mushroom = "ug_mushroom";
        public static readonly string truffle_worm = "truffle_worm";
        public static readonly string ug_corruption = "ug_corruption";
        public static readonly string ug_corrupt_snow = "ug_corrupt_snow";
        public static readonly string ug_corrupt_desert = "ug_corrupt_desert";
        public static readonly string ug_crimson = "ug_crimson";
        public static readonly string ug_crimson_snow = "ug_crimson_snow";
        public static readonly string ug_crimson_desert = "ug_crimson_desert";
        public static readonly string dark_shard = "dark_shard";
        public static readonly string dungeon = "dungeon";
        public static readonly string temple = "temple";
        public static readonly string ectoplasm = "ectoplasm";
        public static readonly string ocean = "ocean";
        public static readonly string pirate = "pirate";
        public static readonly string space = "space";
        public static readonly string spc_flight = "s_flight";
        public static readonly string pillar = "pillar";
        public static readonly string luminite = "luminite";
        public static readonly string underworld = "underworld";
        public static readonly string uw_fire= "uw_fire";
        public static readonly string meteorite = "meteorite";

        //WALL AND BLOCK LISTS
        static readonly List<ushort> dungeonWalls = [WallID.BlueDungeonUnsafe,  WallID.BlueDungeonSlabUnsafe,  WallID.BlueDungeonTileUnsafe, WallID.GreenDungeonUnsafe, WallID.GreenDungeonSlabUnsafe, WallID.GreenDungeonTileUnsafe, WallID.PinkDungeonUnsafe,  WallID.PinkDungeonSlabUnsafe,  WallID.PinkDungeonTileUnsafe];
        static readonly List<ushort> dungeonBricks = [TileID.BlueDungeonBrick, TileID.GreenDungeonBrick, TileID.PinkDungeonBrick];
        static readonly List<ushort> crimsonSandBlocks = [TileID.Crimsand, TileID.CrimsonHardenedSand, TileID.CrimsonSandstone];
        static readonly List<ushort> corruptSandBlocks = [TileID.Ebonsand, TileID.CorruptHardenedSand, TileID.CorruptSandstone];
        static readonly List<ushort> hallowSandBlocks = [TileID.Pearlsand, TileID.HallowHardenedSand, TileID.HallowSandstone];
        static readonly List<ushort> crimsonIceBlocks = [TileID.FleshIce];
        static readonly List<ushort> corruptIceBlocks = [TileID.CorruptIce];
        static readonly List<ushort> hallowIceBlocks = [TileID.HallowedIce];
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
        static readonly Predicate<ScanData> postMechs = scan => Condition.DownedMechBossAll.IsMet();
        static readonly Predicate<ScanData> postPlantera = scan => Condition.DownedPlantera.IsMet();
        static readonly Predicate<ScanData> postGolem = scan => Condition.DownedGolem.IsMet();
        static readonly Predicate<ScanData> postPillars = scan => Condition.DownedNebulaPillar.IsMet() && Condition.DownedSolarPillar.IsMet() && Condition.DownedStardustPillar.IsMet() && Condition.DownedVortexPillar.IsMet();
        static readonly Predicate<ScanData> postML = scan => Condition.DownedMoonLord.IsMet();

        //WORLDLAYER
        static readonly Predicate<ScanData> spaceLayer = scan => scan.Y < Main.worldSurface * 0.3;
        static readonly Predicate<ScanData> skyLayer = scan => scan.Y < Main.worldSurface * 0.35;
        static readonly Predicate<ScanData> undergroundLayer = scan => scan.Y > Main.worldSurface + 1;
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
        public ScanData Scan(ScanData data)
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
                _itemPools.Add(poolName, []);
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
        public void AddItemInPool(string poolName, int itemId) => AddItemInPool(poolName, new ItemEntry(itemId, 1), 1);
        public void AddItemInPool(string poolName, int itemId, int weight) => AddItemInPool(poolName, new ItemEntry(itemId, 1), weight);
        public void AddItemInPool(string poolName, int itemId, int count, int weight) => AddItemInPool(poolName, new ItemEntry(itemId, count), weight);
        public void AddItemInPool(string poolName, ItemEntry item, int weight)
        {
            _itemPools[poolName].Add(item, weight);
        }
        public void RemoveItemFromPool(string poolName, int itemId) => RemoveItemFromPool(poolName, new ItemEntry(itemId, 1));
        public void RemoveItemFromPool(string poolName, int itemId, int count) => RemoveItemFromPool(poolName, new ItemEntry(itemId, count));
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

        public Item RollItem(List<string> pools) //TODO MERGE POOLS INSTEAD
        {
            int roll = Main.rand.Next(pools.Count);
            string pool_name = pools[roll];

            ItemEntry result = _itemPools[pool_name].Roll();

            Item item = new();
            item.SetDefaults(result.Id);
            item.stack = result.Count;

            return item;
        }

        public override void PostSetupContent()
        {
            InitializePools();
            GenerateRequirements();
            PopulatePools();
        }

        private void InitializePools()
        {
            AddPool(forest, 0);
            AddPool(sky,    1);
            AddPool(flight, 1);

            AddPool(snow,   50);
            AddPool(desert, 50);
            AddPool(jungle, 50);
            AddPool(shells, 50);

            AddPool(hallow,               100);
            AddPool(hallowed_bars,        100);
            AddPool(hallowed_snow,        150);
            AddPool(hallowed_bars_snow,   150);
            AddPool(hallowed_desert,      150);
            AddPool(hallowed_bars_desert, 150);        

            AddPool(mushroom, 200);

            AddPool(corruption,     300);
            AddPool(crimson,        300);

            AddPool(corrupt_snow,   350);
            AddPool(crimson_snow,   350);
            AddPool(corrupt_desert, 350);
            AddPool(crimson_desert, 350);

            AddPool(dark_shard, 350);

            AddPool(graveyard, 500);

            AddPool(basic_ores, 1000);
            AddPool(evil_ores,  1000);
            AddPool(hm_ores,    1000);

            AddPool(spider,  1000);
            AddPool(cobweb,  1000);
            AddPool(granite, 1000);
            AddPool(marble,  1000);

            AddPool(ug_snow,      1050);
            AddPool(ug_desert,    1050);
            AddPool(ug_jungle,    1050);
            AddPool(ug_jungle_hm, 1050);
            AddPool(hive,         1050);
            AddPool(chlorophyte,  1050);

            AddPool(ug_hallow,               1100);
            AddPool(ug_hallowed_bars,        1100);
            AddPool(ug_hallowed_snow,        1150);
            AddPool(ug_hallowed_bars_snow,   1150);
            AddPool(ug_hallowed_desert,      1150);
            AddPool(ug_hallowed_bars_desert, 1150);

            AddPool(ug_mushroom,  1200);
            AddPool(truffle_worm, 1200);

            AddPool(ug_corruption,     1300);
            AddPool(ug_crimson,        1300);

            AddPool(ug_corrupt_snow,   1350);
            AddPool(ug_crimson_snow,   1350);
            AddPool(ug_corrupt_desert, 1350);
            AddPool(ug_crimson_desert, 1350);

            AddPool(dungeon,   7500);
            AddPool(ectoplasm, 7500);
            AddPool(temple,    7500);

            AddPool(ocean,  9000);
            AddPool(pirate, 9000);
            
            AddPool(space,      9999);
            AddPool(spc_flight, 9999);
            AddPool(pillar,     9999);
            AddPool(luminite,   9999);
            AddPool(underworld, 9999);
            AddPool(uw_fire,    9999);
            AddPool(meteorite, 10000);
        }

        private void GenerateRequirements()
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

            AddPoolRequirements(temple,    tierCyber, postGolem,                                  lihzahrd_bg);
            AddPoolRequirements(ectoplasm, tierCyber, postPlantera, dungeon250, undergroundLayer, dungeon_bg);

            AddPoolRequirements(dungeon, tierDemonic, dungeon250, undergroundLayer, dungeon_bg);

            AddPoolRequirements(ug_crimson_desert, tierSteampunk, undergroundLayer, evil300.Invoke(crimsonSandBlocks));
            AddPoolRequirements(ug_crimson_snow,   tierSteampunk, undergroundLayer, evil300.Invoke(crimsonIceBlocks));
            AddPoolRequirements(ug_crimson,        tierSteampunk, undergroundLayer, evil300.Invoke(crimsonBlocks));

            AddPoolRequirements(ug_corrupt_desert, tierSteampunk, undergroundLayer, evil300.Invoke(corruptSandBlocks));
            AddPoolRequirements(ug_corrupt_snow,   tierSteampunk, undergroundLayer, evil300.Invoke(corruptIceBlocks));
            AddPoolRequirements(ug_corruption,     tierSteampunk, undergroundLayer, evil300.Invoke(corruptBlocks));

            AddPoolRequirements(truffle_worm, tierSteampunk, hardmodeOnly, undergroundLayer, mush100);
            AddPoolRequirements(ug_mushroom,                               undergroundLayer, mush100);

            AddPoolRequirements(ug_hallowed_bars_desert, tierSteampunk, postMechs, undergroundLayer, evil300.Invoke(hallowSandBlocks));
            AddPoolRequirements(ug_hallowed_bars_snow,   tierSteampunk, postMechs, undergroundLayer, evil300.Invoke(hallowIceBlocks));
            AddPoolRequirements(ug_hallowed_bars,        tierSteampunk, postMechs, undergroundLayer, evil300.Invoke(hallowBlocks));

            AddPoolRequirements(ug_hallowed_desert, tierSteampunk, undergroundLayer, evil300.Invoke(hallowSandBlocks));
            AddPoolRequirements(ug_hallowed_snow,   tierSteampunk, undergroundLayer, evil300.Invoke(hallowIceBlocks));
            AddPoolRequirements(ug_hallow,          tierSteampunk, undergroundLayer, evil300.Invoke(hallowBlocks));

            AddPoolRequirements(chlorophyte,  tierSteampunk, postMechs,    cavernLayer,      jungle140);
            AddPoolRequirements(ug_jungle_hm, tierSteampunk, hardmodeOnly, undergroundLayer, jungle140);
            AddPoolRequirements(hive,                                      undergroundLayer, hive100,    honey100, hive_bg);
            AddPoolRequirements(ug_jungle,                                 undergroundLayer, jungle140);
            AddPoolRequirements(ug_desert,                                 undergroundLayer, desert1500);
            AddPoolRequirements(ug_snow,                                   undergroundLayer, frost1500);

            AddPoolRequirements(marble,  undergroundLayer, marble150,  marble_bg);
            AddPoolRequirements(granite, undergroundLayer, granite150, granite_bg);
            AddPoolRequirements(spider,  undergroundLayer,             spider_bg);

            AddPoolRequirements(basic_ores,                              undergroundLayer);
            AddPoolRequirements(evil_ores,  tierDemonic,                 undergroundLayer);
            AddPoolRequirements(hm_ores,    tierSteampunk, hardmodeOnly, undergroundLayer);
            AddPoolRequirements(faeling,    tierDemonic,                 undergroundLayer, shimmer300);

            AddPoolRequirements(graveyard, tombstone5);

            AddPoolRequirements(corrupt_desert, tierDemonic, evil300.Invoke(corruptSandBlocks));
            AddPoolRequirements(corrupt_snow,   tierDemonic, evil300.Invoke(corruptIceBlocks));
            AddPoolRequirements(corruption,     tierDemonic, evil300.Invoke(corruptBlocks));

            AddPoolRequirements(crimson_desert, tierDemonic, evil300.Invoke(crimsonSandBlocks));
            AddPoolRequirements(crimson_snow,   tierDemonic, evil300.Invoke(crimsonIceBlocks));
            AddPoolRequirements(crimson,        tierDemonic, evil300.Invoke(crimsonBlocks));

            AddPoolRequirements(mushroom, mush100);

            AddPoolRequirements(hallowed_bars_desert, tierSteampunk, postMechs, undergroundLayer, hallow125.Invoke(hallowSandBlocks));
            AddPoolRequirements(hallowed_bars_snow,   tierSteampunk, postMechs, undergroundLayer, hallow125.Invoke(hallowIceBlocks));
            AddPoolRequirements(hallowed_bars,        tierSteampunk, postMechs, undergroundLayer, hallow125.Invoke(hallowBlocks));

            AddPoolRequirements(hallowed_desert, tierSteampunk, hardmodeOnly, undergroundLayer, hallow125.Invoke(hallowSandBlocks));
            AddPoolRequirements(hallowed_snow,   tierSteampunk, hardmodeOnly, undergroundLayer, hallow125.Invoke(hallowIceBlocks));
            AddPoolRequirements(hallow,          tierSteampunk, hardmodeOnly, undergroundLayer, hallow125.Invoke(hallowBlocks));

            AddPoolRequirements(shells, tierSteampunk, hardmodeOnly, jungle140);
            AddPoolRequirements(jungle,                              jungle140);
            AddPoolRequirements(desert,                              desert1500);
            AddPoolRequirements(snow,                                frost1500);

            AddPoolRequirements(flight, tierSteampunk, hardmodeOnly, skyLayer);
            AddPoolRequirements(sky,                                 skyLayer);
        }

        private void PopulatePools()
        {
            throw new NotImplementedException();
        }
    }
}