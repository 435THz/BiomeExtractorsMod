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

        public readonly string forest = "forest";
        public readonly string snow = "snow";
        public readonly string desert = "desert";
        public readonly string jungle = "jungle";
        public readonly string shells = "shells";
        public readonly string hallow = "hallow";
        public readonly string hallowed_bars = "hallowed_bars";
        public readonly string hallowed_snow = "hallowed_snow";
        public readonly string hallowed_bars_snow = "hallowed_bars_snow";
        public readonly string hallowed_desert = "hallowed_desert";
        public readonly string hallowed_bars_desert = "hallowed_bars_desert";
        public readonly string mushroom = "mushroom";
        public readonly string corruption = "corruption";
        public readonly string corrupt_snow = "corrupt_snow";
        public readonly string corrupt_desert = "corrupt_desert";
        public readonly string crimson = "crimson";
        public readonly string crimson_snow = "crimson_snow";
        public readonly string crimson_desert = "crimson_desert";
        public readonly string graveyard = "graveyard";
        public readonly string cavern = "cavern";
        public readonly string granite = "granite";
        public readonly string marble = "marble";
        public readonly string ug_snow = "ug_snow";
        public readonly string ug_desert = "ug_desert";
        public readonly string ug_jungle = "ug_jungle";
        public readonly string hive = "hive";
        public readonly string ug_shells = "ug_shells";
        public readonly string life_fruits = "life_fruits";
        public readonly string chlorophyte = "chlorophyte";
        public readonly string ug_hallow = "ug_hallow";
        public readonly string ug_hallowed_bars = "ug_hallowed_bars";
        public readonly string ug_hallowed_snow = "ug_hallowed_snow";
        public readonly string ug_hallowed_bars_snow = "ug_hallowed_bars_snow";
        public readonly string ug_hallowed_desert = "ug_hallowed_desert";
        public readonly string ug_hallowed_bars_desert = "ug_hallowed_bars_desert";
        public readonly string ug_mushroom = "ug_mushroom";
        public readonly string truffle_worm = "truffle_worm";
        public readonly string ug_corruption = "ug_corruption";
        public readonly string ug_corrupt_snow = "ug_corrupt_snow";
        public readonly string ug_corrupt_desert = "ug_corrupt_desert";
        public readonly string ug_crimson = "ug_crimson";
        public readonly string ug_crimson_snow = "ug_crimson_snow";
        public readonly string ug_crimson_desert = "ug_crimson_desert";
        public readonly string dungeon = "dungeon";
        public readonly string temple = "temple";
        public readonly string ectoplasm = "ectoplasm";
        public readonly string ocean = "ocean";
        public readonly string pirate = "pirate";
        public readonly string sky = "sky";
        public readonly string flight = "flight";
        public readonly string pillar = "pillar";
        public readonly string luminite = "luminite";
        public readonly string underworld = "underworld";
        public readonly string uw_fire= "uw_fire";
        public readonly string meteorite = "meteorite";

        readonly List<ushort> dungeonWalls = [WallID.BlueDungeonUnsafe,  WallID.BlueDungeonSlabUnsafe,  WallID.BlueDungeonTileUnsafe,
                                              WallID.GreenDungeonUnsafe, WallID.GreenDungeonSlabUnsafe, WallID.GreenDungeonTileUnsafe,
                                              WallID.PinkDungeonUnsafe,  WallID.PinkDungeonSlabUnsafe,  WallID.PinkDungeonTileUnsafe];
        readonly List<ushort> dungeonBricks = [TileID.BlueDungeonBrick, TileID.GreenDungeonBrick, TileID.PinkDungeonBrick];
        readonly List<ushort> crimsonSandBlocks = [TileID.Crimsand, TileID.CrimsonHardenedSand, TileID.CrimsonSandstone];
        readonly List<ushort> corruptSandBlocks = [TileID.Ebonsand, TileID.CorruptHardenedSand, TileID.CorruptSandstone];
        readonly List<ushort> hallowSandBlocks = [TileID.Pearlsand, TileID.HallowHardenedSand, TileID.HallowSandstone];
        readonly List<ushort> crimsonIceBlocks = [TileID.FleshIce];
        readonly List<ushort> corruptIceBlocks = [TileID.CorruptIce];
        readonly List<ushort> hallowIceBlocks = [TileID.HallowedIce];
        readonly List<ushort> crimsonBlocks = [TileID.CrimsonGrass, TileID.CrimsonJungleGrass, TileID.CorruptThorns, TileID.CrimsonPlants, TileID.CrimsonVines, TileID.Crimstone, TileID.Crimsand, TileID.CrimsonHardenedSand, TileID.CrimsonSandstone, TileID.FleshIce];
        readonly List<ushort> corruptBlocks = [TileID.CorruptGrass, TileID.CorruptJungleGrass, TileID.CorruptThorns, TileID.CorruptPlants, TileID.CorruptVines, TileID.Ebonstone, TileID.Ebonsand, TileID.CorruptHardenedSand, TileID.CorruptSandstone, TileID.CorruptIce];
        readonly List<ushort> hallowBlocks = [TileID.HallowedGrass, TileID.HallowedPlants, TileID.HallowedPlants2, TileID.HallowedVines,TileID.Pearlstone, TileID.Pearlsand, TileID.HallowHardenedSand, TileID.HallowSandstone, TileID.HallowedIce];
        readonly List<ushort> glowMushroomBlocks = [TileID.MushroomGrass, TileID.MushroomPlants, TileID.MushroomVines];
        
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

            AddPool(snow,   50);
            AddPool(desert, 50);
            AddPool(jungle, 50);
            AddPool(shells, 50);

            AddPool(hallow,        100);
            AddPool(hallowed_bars, 100);
            AddPool(hallowed_snow,        150);
            AddPool(hallowed_bars_snow,   150);
            AddPool(hallowed_desert,      150);
            AddPool(hallowed_bars_desert, 150);

            AddPool(mushroom, 200);

            AddPool(corruption, 300);
            AddPool(corrupt_snow,   350);
            AddPool(corrupt_desert, 350);

            AddPool(crimson, 300);
            AddPool(crimson_snow,   350);
            AddPool(crimson_desert, 350);

            AddPool(graveyard, 500);

            AddPool(cavern,  1000);
            AddPool(granite, 1000);
            AddPool(marble,  1000);

            AddPool(ug_snow,     1050);
            AddPool(ug_desert,   1050);
            AddPool(ug_jungle,   1050);
            AddPool(ug_shells,   1050);
            AddPool(life_fruits, 1050);
            AddPool(chlorophyte, 1050);

            AddPool(ug_hallow,        1100);
            AddPool(ug_hallowed_bars, 1100);
            AddPool(ug_hallowed_snow,        1150);
            AddPool(ug_hallowed_bars_snow,   1150);
            AddPool(ug_hallowed_desert,      1150);
            AddPool(ug_hallowed_bars_desert, 1150);

            AddPool(ug_mushroom, 1200);
            AddPool(truffle_worm, 1200);

            AddPool(ug_corruption, 1300);
            AddPool(ug_corrupt_snow,   1350);
            AddPool(ug_corrupt_desert, 1350);

            AddPool(ug_crimson, 1300);
            AddPool(ug_crimson_snow,   1350);
            AddPool(ug_crimson_desert, 1350);

            AddPool(dungeon,   7500);
            AddPool(ectoplasm, 7500);
            AddPool(temple,    7500);

            AddPool(ocean,  9000);
            AddPool(pirate, 9000);

            AddPool(sky,        9999);
            AddPool(flight,     9999);
            AddPool(pillar,     9999);
            AddPool(luminite,   9999);
            AddPool(underworld, 9999);
            AddPool(uw_fire,    9999);
            AddPool(meteorite, 10000);
        }

        private void GenerateRequirements()
        {
            //TIERS
            Predicate<ScanData> tierDemonic =   scan => scan.MinTier((int)BiomeExtractorEnt.EnumTiers.DEMONIC);
            Predicate<ScanData> tierInfernal =  scan => scan.MinTier((int)BiomeExtractorEnt.EnumTiers.INFERNAL);
            Predicate<ScanData> tierSteampunk = scan => scan.MinTier((int)BiomeExtractorEnt.EnumTiers.STEAMPUNK);
            Predicate<ScanData> tierCyber =     scan => scan.MinTier((int)BiomeExtractorEnt.EnumTiers.CYBER);
            Predicate<ScanData> tierLunar =     scan => scan.MinTier((int)BiomeExtractorEnt.EnumTiers.LUNAR);
            Predicate<ScanData> tierEthereal =  scan => scan.MinTier((int)BiomeExtractorEnt.EnumTiers.ETHEREAL);

            //PROGRESSION
            Predicate<ScanData> hardmodeOnly = scan => Main.hardMode;
            Predicate<ScanData> postMechs =    scan => Condition.DownedMechBossAll.IsMet();
            Predicate<ScanData> postPlantera = scan => Condition.DownedPlantera.IsMet();
            Predicate<ScanData> postGolem =    scan => Condition.DownedGolem.IsMet();
            Predicate<ScanData> postPillars =  scan => Condition.DownedNebulaPillar.IsMet() && Condition.DownedSolarPillar.IsMet() && Condition.DownedStardustPillar.IsMet() && Condition.DownedVortexPillar.IsMet();
            Predicate<ScanData> postML =       scan => Condition.DownedMoonLord.IsMet();

            //WORLDLAYER
            Predicate<ScanData> spaceLayer =       scan => scan.Y < Main.worldSurface * 0.3;
            Predicate<ScanData> skyLayer =         scan => scan.Y < Main.worldSurface * 0.35;
            Predicate<ScanData> undergroundLayer = scan => scan.Y > Main.worldSurface + 1;
            Predicate<ScanData> cavernLayer =      scan => scan.Y > Main.rockLayer + 1;
            Predicate<ScanData> underworldLayer =  scan => scan.Y > Main.maxTilesY - 200;

            //SPECIFIC POSITIONS
            Predicate<ScanData> oceanArea = scan => scan.Y <= (Main.worldSurface + Main.rockLayer) / 2 && (scan.X < 339 || scan.Y > Main.maxTilesX - 339);

            //WALLS
            Predicate<ScanData> lihzahrd_bg = scan => scan.ValidWalls([WallID.LihzahrdBrickUnsafe]);
            Predicate<ScanData> dungeon_bg  = scan => scan.ValidWalls(dungeonWalls);

            //LIQUIDS
            Predicate<ScanData> water1k = scan => scan.Liquids(LiquidID.Water) >= 1000;

            //BLOCKS
            Func<List<ushort>, Predicate<ScanData>> evil300 =   tiles => (scan => scan.Tiles(tiles) - scan.Tiles(hallowBlocks) -  scan.Tiles(TileID.Sunflower) * 80 >= 300);
            Func<List<ushort>, Predicate<ScanData>> hallow125 = tiles => (scan => scan.Tiles(tiles) - scan.Tiles(crimsonBlocks) - scan.Tiles(corruptBlocks)         >= 125);
            Predicate<ScanData> meteorite75 = scan => scan.Tiles(TileID.Meteorite) >= 75;
            Predicate<ScanData> dungeon250  = scan => scan.Tiles(dungeonBricks) >= 250;
            Predicate<ScanData> mush100     = scan => scan.Tiles(glowMushroomBlocks) >= 100;
            


            AddPoolRequirements(meteorite, tierInfernal, meteorite75);
            AddPoolRequirements(uw_fire, tierSteampunk, hardmodeOnly, underworldLayer);
            AddPoolRequirements(underworld, tierInfernal, underworldLayer);
            AddPoolRequirements(luminite, tierLunar, postML, spaceLayer);
            AddPoolRequirements(pillar, tierLunar, postPillars, spaceLayer);
            AddPoolRequirements(flight, tierSteampunk, hardmodeOnly, skyLayer);
            AddPoolRequirements(sky, skyLayer);

            AddPoolRequirements(pirate, tierSteampunk, hardmodeOnly, oceanArea, water1k);
            AddPoolRequirements(ocean, oceanArea, water1k);

            AddPoolRequirements(temple, tierCyber, postGolem, lihzahrd_bg);
            AddPoolRequirements(ectoplasm, tierCyber, postPlantera, dungeon_bg, dungeon250, undergroundLayer);

            AddPoolRequirements(dungeon, tierDemonic, dungeon_bg, dungeon250, undergroundLayer);

            AddPoolRequirements(ug_crimson_desert, tierSteampunk, cavernLayer, evil300.Invoke(crimsonSandBlocks));
            AddPoolRequirements(ug_crimson_snow,   tierSteampunk, cavernLayer, evil300.Invoke(crimsonIceBlocks));
            AddPoolRequirements(ug_crimson,        tierSteampunk, cavernLayer, evil300.Invoke(crimsonBlocks));

            AddPoolRequirements(ug_corrupt_desert, tierSteampunk, cavernLayer, evil300.Invoke(corruptSandBlocks));
            AddPoolRequirements(ug_corrupt_snow,   tierSteampunk, cavernLayer, evil300.Invoke(corruptIceBlocks));
            AddPoolRequirements(ug_corruption,     tierSteampunk, cavernLayer, evil300.Invoke(corruptBlocks));

            AddPoolRequirements(truffle_worm, tierSteampunk, hardmodeOnly, cavernLayer, mush100);
            AddPoolRequirements(ug_mushroom,  cavernLayer, mush100);

            AddPoolRequirements(ug_hallowed_bars_desert, tierSteampunk, postMechs, cavernLayer, evil300.Invoke(hallowSandBlocks));
            AddPoolRequirements(ug_hallowed_bars_snow,   tierSteampunk, postMechs, cavernLayer, evil300.Invoke(hallowIceBlocks));
            AddPoolRequirements(ug_hallowed_bars,        tierSteampunk, postMechs, cavernLayer, evil300.Invoke(hallowBlocks));

            AddPoolRequirements(ug_hallowed_desert, tierSteampunk, cavernLayer, evil300.Invoke(hallowSandBlocks));
            AddPoolRequirements(ug_hallowed_snow,   tierSteampunk, cavernLayer, evil300.Invoke(hallowIceBlocks));
            AddPoolRequirements(ug_hallow,          tierSteampunk, cavernLayer, evil300.Invoke(hallowBlocks));




        }

        private void PopulatePools()
        {
            throw new NotImplementedException();
        }

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            int JungleBiomeBlocks = tileCounts[TileID.JungleGrass] + tileCounts[TileID.JunglePlants] + tileCounts[TileID.JunglePlants2] + tileCounts[TileID.JungleThorns] + tileCounts[TileID.Hive];
            int SnowBiomeBlocks = tileCounts[TileID.SnowBlock] + tileCounts[TileID.IceBlock] + tileCounts[TileID.BreakableIce];
            int DesertBiomeBlocks = tileCounts[TileID.Sand] + tileCounts[TileID.HardenedSand] + tileCounts[TileID.Sandstone];



            int HiveBiomeBlocks = tileCounts[TileID.Hive];
            int GraniteBiomeBlocks = tileCounts[TileID.Granite];
            int MarbleBiomeBlocks = tileCounts[TileID.Marble];
            //int GraveyardBiomeBlocks = tileCounts[TileID.Tombstones];
        }
    }
}