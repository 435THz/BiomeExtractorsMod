using BiomeExtractorsMod.Common.Collections;
using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Content.TileEntities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace BiomeExtractorsMod.Common.Systems
{
    /// <summary>
    /// A helper class used by <see cref="BiomeExtractionSystem"/>to scan an Extractor's surroundings.
    /// After a scan, the amount of tiles and liquids found can be checked by calling its getter methods.
    /// </summary>
    /// <param name="tier">The tier of the extractor requesting a scan</param>
    /// <param name="origin">The origin of the scan in tile coordinates</param>
    public class ScanData(BiomeExtractionSystem.ExtractionTier tier, Point16 origin)
    {

        private readonly BiomeExtractionSystem.ExtractionTier _tier = tier;
        private Point _origin = origin.ToPoint() + new Point(1, 1);
        private Point _size = new(Main.buffScanAreaWidth, Main.buffScanAreaHeight);
        private readonly Dictionary<int, int> _tileCounts = [];
        private readonly Dictionary<int, int> _liquidCounts = [];

        /// <summary>
        /// The X position of this scan's origin in tile coordinates.
        /// </summary>
        public float X => _origin.X;
        /// <summary>
        /// The Y position of this scan's origin in tile coordinates.
        /// </summary>
        public float Y => _origin.Y;

        /// <summary>
        /// Returns the amount of tiles in the scan area that correspond to any of the requested tile ids.
        /// </summary>
        /// <param name="tileIds">A list of tile ids look for</param>
        /// <returns>The total amount of the requested tiles.</returns>
        public int Tiles(List<ushort> tileIds) => Tiles(tileIds.ToArray());
        /// <summary>
        /// Returns the amount of tiles in the scan area that correspond to any of the requested tile ids.
        /// </summary>
        /// <param name="tileIds">An array of tile ids look for</param>
        /// <returns>The total amount of the requested tiles.</returns>
        public int Tiles(params ushort[] tileIds)
        {
            int count = 0;
            foreach (ushort tileId in tileIds) count += _tileCounts.GetValueOrDefault(tileId);
            return count;
        }

        /// <summary>
        /// Returns the amount of liquid tiles in the scan area that correspond to the requested liquid id.
        /// </summary>
        /// <param name="tileId">The id of the liquid to look for</param>
        /// <returns>The amount of the requested liquid tile</returns>
        public int Liquids(int liquidId) => _liquidCounts.GetValueOrDefault(liquidId);

        /// <summary>
        /// Returns whether or not the scan's tier is higher or equal than the provided one.
        /// </summary>
        /// <param name="tier">The tier to check for</param>
        /// <returns><see langword="true"/> if tier is lower or equal to the ScanData's tier, <see langword="false"/> otherwise</returns>
        public bool MinTier(int tier) => _tier != null && _tier.Tier >= tier;

        /// <summary>
        /// Checks if the area has the requested wall as a background.
        /// More specifically, it only checks for the 3x3 area centered on the scan's origin, and only returns
        /// <see langword="true"/> if they all match. For an Extractor, this 3x3 area is exactly the space
        /// occupied by the Extractor tiles.
        /// </summary>
        /// <param name="wallId">The id of the wall to check for</param>
        /// <param name="blacklist">If this is <see langword="true"/>, this method will check if the area does NOT contain the requested wall id</param>
        /// <returns><see langword="true"/> if all walls in the 3x3 area correspond to the wall id, <see langword="false"/> otherwise</returns>
        public bool ValidWalls(ushort wallId, bool blacklist = false) => ValidWalls(new List<ushort>() { wallId }, blacklist);
        /// <summary>
        /// Checks if the area has the requested walls as a background.
        /// More specifically, it only checks for the 3x3 area centered on the scan's origin, and only returns
        /// <see langword="true"/> if they all match at least one of the provided walls. For an Extractor, this
        /// 3x3 area is exactly the space occupied by the Extractor tiles.
        /// </summary>
        /// <param name="wallId">An array of wall ids to check for</param>
        /// <param name="blacklist">If this is <see langword="true"/>, this method will check if the area does NOT contain any of the requested wall ids</param>
        /// <returns><see langword="true"/> if all walls in the 3x3 area correspond to at least one of the requested wall ids each, <see langword="false"/> otherwise</returns>
        public bool ValidWalls(ushort[] wallIds, bool blacklist = false) => ValidWalls(wallIds.ToList(), blacklist);
        /// <summary>
        /// Checks if the area has the requested walls as a background.
        /// More specifically, it only checks for the 3x3 area centered on the scan's origin, and only returns
        /// <see langword="true"/> if they all match at least one of the provided walls. For an Extractor, this
        /// 3x3 area is exactly the space occupied by the Extractor tiles.
        /// </summary>
        /// <param name="wallId">An array of wall ids to check for</param>
        /// <param name="blacklist">If this is <see langword="true"/>, this method will check if the area does NOT contain any of the requested wall ids</param>
        /// <returns><see langword="true"/> if all walls in the 3x3 area correspond to at least one of the requested wall ids each, <see langword="false"/> otherwise</returns>
        public bool ValidWalls(List<ushort> wallIds, bool blacklist = false)
        {
            Point origin = _origin - new Point(1,1);
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

        internal void Scan()
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
        #region Data Structures
        //        public enum PoolType
        //        {
        //            MINERALS, GEMS, DROPS, TERRAIN, VEGETATION, CRITTERS
        //        }

        /// <summary>
        /// An identifier object that details an item pool's name, tier and behavior.
        /// </summary>
        /// <param name="name"> The PoolEntry's identification string.</param>
        /// <param name="tier"> The minimum Extractor tier required to access this pool.</param>
        /// <param name="blocking"> If true, biome scans will not scan priority values lower than the one this pool has if this pool is found.</param>
        /// <param name="localizationKey">If set, this pool will use this string to query its localized name.<br/>
        /// If <see langword="null"/>, this pool will not be displayed in the Extractor UI at all.</param>
        public class PoolEntry(string name, int tier, bool blocking = true, string localizationKey = null)
        {
            /// <summary>The pool's identification string.</summary>
            public string Name { get; private set; } = name;
            /// <summary>The minimum Tier required to access this pool.</summary>
            public int Tier { get; private set; } = tier;
            /// <summary>The localization key associated to this pool.</summary>
            public bool Blocking { get; private set; } = blocking;
            /// <summary>The localization key associated to this pool.</summary>
            public string LocalizationKey { get; private set; } = localizationKey;
            /// <summary>Returns whether or not this pool has a localized name.</summary>
            /// <returns><see langword="true"/> if <c>this.LocalizationKey != null</c>, <see langword="false"/> otherwise.</returns>
            public bool IsLocalized() => LocalizationKey != null;
            /// <summary>
            /// An identifier object that details an item pool's name, tier and behavior.
            /// </summary>
            /// <param name="name"> The PoolEntry's identification string.</param>
            /// <param name="tier"> The minimum Extractor tier required to access this pool.</param>
            /// <param name="localizationKey">If set, this pool will use this string to query its localized name.<br/>
            /// If <see langword="null"/>, this pool will not be displayed in the Extractor UI at all.</param>
            public PoolEntry(string name, int tier, string localizationKey) : this(name, tier, true, localizationKey) { }
        }

        /// <summary>
        /// An identifier object that details an item entry inside a pool, complete with minimum and maximum drop values.
        /// </summary>
        /// <param name="item"> The id of the item represented by this entry.</param>
        /// <param name="min"> Minimum amount of copies generated if this entry is chosen.</param>
        /// <param name="max"> Maximum amount of copies generated if this entry is chosen.</param>
        public class ItemEntry(short item, int min, int max)
        {
            ///<summary>The id of the item represented by this entry</summary>
            public short Id { get; private set; } = item;
            ///<summary>Minimum amount of copies generated if this entry is chosen.</summary>
            public int Min { get; private set; } = min;
            ///<summary> Maximum amount of copies generated if this entry is chosen.</summary>
            public int Max { get; private set; } = max;
            ///<summary>Returns a random number between <c>this.Min</c> and <c>this.Max</c></summary>
            public int Roll { get => Main.rand.Next(Min, Max + 1); }

            /// <summary>
            /// An identifier object that details an item entry inside a pool, complete with minimum and maximum drop values.
            /// </summary>
            /// <param name="item"> The id of the item represented by this entry.</param>
            /// <param name="count"> The amount of copies generated if this entry is chosen.</param>
            public ItemEntry(short item, int count) : this(item, count, count) { }

            public override bool Equals(object obj)
            {
                if (obj == null) return false;
                if (obj is not ItemEntry) return false;
                return Id == ((ItemEntry)obj).Id && Min == ((ItemEntry)obj).Min && Max == ((ItemEntry)obj).Max;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Id, Min, Max);
            }

            public override string ToString()
            {
                return Id + ": " + Main.item[Id].Name + " (" + Min + "-" + (Max) + ")";
            }
        }

        /// <summary>
        /// An identifier object that details an extraction tier's tier number, localization key and efficiency stats. 
        /// </summary>
        /// <param name="tier">The tier number of this tier</param>
        /// <param name="locKey">The localization key of this tier</param>
        /// <param name="rate">A <see cref="Func{TResult}"/> that returns the extraction rate of this Extraction Tier, in frames.</param>
        /// <param name="chance">A <see cref="Func{TResult}"/> that returns the extraction chance of this Extraction Tier, in percentage format.<br/>
        /// Example: 35 would be 35%</param>
        public class ExtractionTier(int tier, string locKey, Func<int> rate, Func<int> chance)
        {
            private readonly Func<int> _rate = rate;
            private readonly Func<int> _chance = chance;

            /// <summary>
            /// An empty ExtractionTier object.
            /// </summary>
            public readonly static ExtractionTier NULL = new(int.MinValue, "", null, null);
            /// <summary>
            /// The tier number of this tier.
            /// </summary>
            public readonly int Tier = tier;
            /// <summary>
            /// The loclization key associated to this Exraction Tier.
            /// </summary>
            public readonly string LocKey = locKey;
            /// <summary>
            /// Returns the extraction rate of this Extraction Tier, in frames.
            /// </summary>
            public int Rate => _rate is null ? 0 : _rate.Invoke();
            /// <summary>
            /// Returns the extraction chance of this Extraction Tier, in percentage format.<br/>
            /// Example: 35 would be 35%</param>
            /// </summary>
            public int Chance => _chance is null ? 0 : _chance.Invoke();
            /// <summary>
            /// Returns the localized name of this Extractor. This call is used by the UI to set up its header
            /// and by the Biome Scanner to show the tier names.
            /// </summary>
            public string Name => Language.GetTextValue(LocKey);
            public override bool Equals(object obj)
            {
                if (obj is not ExtractionTier) return false;
                ExtractionTier tier = (ExtractionTier)obj;
                return tier.Tier == Tier && tier.LocKey == LocKey;
            }
            public override int GetHashCode() => HashCode.Combine(Tier, LocKey);
            /// <summary>
            /// Creates a new copy of this ExtracionTier and shifts it tier number.
            /// </summary>
            /// <param name="newTier">The new tier to give to the copy of this tier</param>
            /// <returns>A copy of this tier but with <c>newTier</c> as its tier number</returns>
            public ExtractionTier CloneAndMove(int newTier) => new(newTier, LocKey, _rate, _chance);
        }
        #endregion

        #region IDs
        public static readonly string forest = "forest";
        public static readonly string sky = "sky";
        public static readonly string flight = "flight";
        public static readonly string snow = "snow";
        public static readonly string desert = "desert";
        public static readonly string jungle = "jungle";
        public static readonly string shells = "shells";
        public static readonly string hallowed_bars_forest = "hallowed_bars_forest";
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
        public static readonly string uw_fire = "uw_fire";
        public static readonly string meteorite = "meteorite";
        #endregion

        #region Checks
        //WALL AND BLOCK LISTS
        static readonly List<ushort> dungeonWalls =      [WallID.BlueDungeonUnsafe,  WallID.BlueDungeonSlabUnsafe,  WallID.BlueDungeonTileUnsafe, WallID.GreenDungeonUnsafe, WallID.GreenDungeonSlabUnsafe, WallID.GreenDungeonTileUnsafe, WallID.PinkDungeonUnsafe, WallID.PinkDungeonSlabUnsafe, WallID.PinkDungeonTileUnsafe];
        static readonly List<ushort> dungeonWallsPink =  [WallID.PinkDungeonUnsafe,  WallID.PinkDungeonSlabUnsafe,  WallID.PinkDungeonTileUnsafe];
        static readonly List<ushort> dungeonWallsGreen = [WallID.GreenDungeonUnsafe, WallID.GreenDungeonSlabUnsafe, WallID.GreenDungeonTileUnsafe];
        static readonly List<ushort> dungeonWallsBlue =  [WallID.BlueDungeonUnsafe,  WallID.BlueDungeonSlabUnsafe,  WallID.BlueDungeonTileUnsafe];
        static readonly List<ushort> dungeonBricks = [TileID.BlueDungeonBrick, TileID.GreenDungeonBrick, TileID.PinkDungeonBrick];
        static readonly List<ushort> crimsonSandBlocks = [TileID.Crimsand,  TileID.CrimsonHardenedSand, TileID.CrimsonSandstone];
        static readonly List<ushort> corruptSandBlocks = [TileID.Ebonsand,  TileID.CorruptHardenedSand, TileID.CorruptSandstone];
        static readonly List<ushort> hallowSandBlocks =  [TileID.Pearlsand, TileID.HallowHardenedSand,  TileID.HallowSandstone];
        static readonly List<ushort> crimsonIceBlocks = [TileID.FleshIce];
        static readonly List<ushort> corruptIceBlocks = [TileID.CorruptIce];
        static readonly List<ushort> hallowIceBlocks =  [TileID.HallowedIce];
        static readonly List<ushort> crimsonForestBlocks = [TileID.CrimsonGrass,  TileID.CrimsonJungleGrass, TileID.CrimsonThorns,                          TileID.Crimstone];
        static readonly List<ushort> corruptForestBlocks = [TileID.CorruptGrass,  TileID.CorruptJungleGrass, TileID.CorruptThorns,  TileID.CorruptPlants,   TileID.Ebonstone];
        static readonly List<ushort> hallowForestBlocks =  [TileID.HallowedGrass, TileID.GolfGrassHallowed,  TileID.HallowedPlants, TileID.HallowedPlants2, TileID.Pearlstone];
        static readonly List<ushort> crimsonBlocks = [TileID.CrimsonGrass,  TileID.CrimsonJungleGrass, TileID.CrimsonThorns,                         TileID.Crimstone,  TileID.Crimsand,  TileID.CrimsonHardenedSand, TileID.CrimsonSandstone, TileID.FleshIce];
        static readonly List<ushort> corruptBlocks = [TileID.CorruptGrass,  TileID.CorruptJungleGrass, TileID.CorruptThorns,   TileID.CorruptPlants, TileID.Ebonstone,  TileID.Ebonsand,  TileID.CorruptHardenedSand, TileID.CorruptSandstone, TileID.CorruptIce];
        static readonly List<ushort> hallowBlocks =  [TileID.HallowedGrass, TileID.HallowedPlants,     TileID.HallowedPlants2,                       TileID.Pearlstone, TileID.Pearlsand, TileID.HallowHardenedSand,  TileID.HallowSandstone,  TileID.HallowedIce];
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
        static readonly Predicate<ScanData> lihzahrd_bg = scan => scan.ValidWalls(WallID.LihzahrdBrickUnsafe);
        static readonly Predicate<ScanData> hive_bg = scan => scan.ValidWalls(WallID.HiveUnsafe);
        static readonly Predicate<ScanData> granite_bg = scan => scan.ValidWalls(WallID.GraniteUnsafe);
        static readonly Predicate<ScanData> marble_bg = scan => scan.ValidWalls(WallID.MarbleUnsafe);
        static readonly Predicate<ScanData> spider_bg = scan => scan.ValidWalls(WallID.SpiderUnsafe);
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
        #endregion

        #region API
        private readonly PriorityList<ExtractionTier> _tiers = [];
        private readonly Dictionary<string, PoolEntry> _poolNames = [];
        private readonly Dictionary<string, WeightedList<ItemEntry>> _itemPools = [];
        private readonly Dictionary<string, List<Predicate<ScanData>>> _poolRequirements = [];
        private readonly PriorityList<string> _priorityList = [];


        /// <summary>
        /// Checks if an ExtractionTier is registered to the provided tier number.
        /// </summary>
        /// <param name="tier">A tier number</param>
        /// <returns><see langword="true"/> if the ExtractionTier is registered, <see langword="false"/> otherwise.</returns>
        public bool TierExists(int tier) => _tiers.ContainsKey(tier);

        /// <summary>
        /// Checks if the provided ExtractionTier is in the list of registered tiers
        /// </summary>
        /// <param name="tier">An ExtractionTier</param>
        /// <returns><see langword="true"/> if the ExtractionTier is registered, <see langword="false"/> otherwise.</returns>
        public bool TierExists(ExtractionTier tier) => TierExists(tier.Tier);
        /// <summary>
        /// Returns a registered ExractionTier given its tier number.
        /// </summary>
        /// <param name="tier">A tier number</param>
        /// <param name="higher">If this is true, on a failure, this method will call <see cref="GetClosestHigherTier(int)"/>.</param>
        /// <param name="lower">If this is true, on a failure, this method will call <see cref="GetClosestLowerTier(int)"/>.<br/> This will be checked after <paramref name="higher"/> if that is <see langword="true"/> as well</param>
        /// <returns>The ExtractionTier registered to the provided tier number if one such tier is registered, <see langword="null"/> otherwise.</returns>
        public ExtractionTier GetTier(int tier, bool higher = false, bool lower = false)
        {
            if (_tiers.TryGetValue(tier, out List<ExtractionTier> l))
                return l[0];
            ExtractionTier ret = null;
            if (higher) ret = GetClosestHigherTier(tier);
            if (ret != null) return ret;
            if (lower) ret = GetClosestLowerTier(tier);
            return ret;
        }
        /// <summary>
        /// Returns the lowest registered tier whose tier number is striictly higher than the requested one.
        /// </summary>
        /// <param name="tier">A tier number</param>
        /// <returns>The closest ExtractionTier whose tier number is strictly higher than the requested one if one exists, <see langword="null"/> otherwise.</returns>

        public ExtractionTier GetClosestHigherTier(int tier)
        {
            ExtractionTier ret = null;
            foreach (int key in _tiers.Keys)
            {
                if (key > tier) ret = _tiers[key][0];
                else break;
            }
            return ret;
        }
        /// <summary>
        /// Returns the highest registered tier whose tier number is striictly lower than the requested one.
        /// </summary>
        /// <param name="tier">A tier number</param>
        /// <returns>The closest ExtractionTier whose tier number is strictly lower than the requested one if one exists, <see langword="null"/> otherwise.</returns>
        public ExtractionTier GetClosestLowerTier(int tier)
        {
            List<int> keys = new(_tiers.Keys);
            if (_tiers.Count < 1) return null;
            if (keys[^1] > tier) return null;
            for (int i = keys.Count - 2; i >= 0; i--)
            {
                int key = keys[i];
                if (key > tier) return _tiers[keys[i+1]][0];
            }
            return _tiers[keys[0]][0];
        }

        /// <summary>
        /// Creates a new ExtractionTier and registers it.
        /// If an ExtractionTier with that tier number already exists, this method does nothing.
        /// </summary>
        /// <param name="tier">The tier number of this tier</param>
        /// <param name="locKey">The localization key of this tier</param>
        /// <param name="rate">A <see cref="Func{TResult}"/> that returns the extraction rate of this Extraction Tier, in frames.</param>
        /// <param name="chance">>A <see cref="Func{TResult}"/> that returns the extraction chance of this Extraction Tier, in percentage format.<br/>
        /// Example: 35 would be 35%</param>
        /// <returns><see langword="true"/> if the tier number was not occupied yet, <see langword="false"/> otherwise</returns>
        public bool AddTier(int tier, string locKey, Func<int> rate, Func<int> chance) => AddTier(new(tier, locKey, rate, chance));
        /// <summary>
        /// Registers an ExtractionTier.<br></br>
        /// If an ExtractionTier with that tier number already exists, this method does nothing.
        /// </summary>
        /// <param name="newTier">The tier to be registered</param>
        /// <returns><see langword="true"/> if the tier number was not occupied yet, <see langword="false"/> otherwise</returns>
        public bool AddTier(ExtractionTier newTier)
        {
            if (TierExists(newTier)) return false;
            if (_tiers.TryGetValue(newTier.Tier, out List<ExtractionTier> l) && l.Count>0) return false;
            _tiers.Add(newTier.Tier, newTier);
            return true;
        }

        /// <summary>
        /// Gets rid of an alredy registered tier.
        /// </summary>
        /// <param name="tier">The tier number of the tier to get rid of.</param>
        /// <returns><see langword="true"/> if there was an entry to delete, <see langword="false"/> otherwise</returns>
        public bool RemoveTier(int tier) => _tiers.TryGetValue(tier, out List<ExtractionTier> l) && l.Count>0 && RemoveTier(l[0]);
        /// <summary>
        /// Gets rid of an alredy registered tier.
        /// </summary>
        /// <param name="tier">The ExtractionTier to get rid of.</param>
        /// <returns><see langword="true"/> if there was an entry to delete, <see langword="false"/> otherwise</returns>
        public bool RemoveTier(ExtractionTier tier) => RemoveTier(tier.Tier);
        /// <summary>
        /// Changes the tier number of a registered tier.
        /// Does nothing if the tier was not registered.
        /// </summary>
        /// <param name="oldTier">The tier number of the registered ExtractionTier to be edited</param>
        /// <param name="newTier">The new tier that the ExtractionTier will have</param>
        /// <returns><see langword="true"/> if the tier was successfully moved, <see langword="false"/> otherwise.</returns>
        public bool MoveTier(int oldTier, int newTier)
        {
            ExtractionTier tier = GetTier(oldTier);
            return tier is not null && MoveTier(ref tier, newTier);
        }
        /// <summary>
        /// Changes the tier number of a registered tier.
        /// Does nothing if the tier was not registered.<br/>
        /// </summary>
        /// <param name="oldTier">The ExtractionTier to be edited</param>
        /// <param name="newTier">The new tier that the ExtractionTier will have</param>
        /// <returns><see langword="true"/> if the tier was successfully moved, <see langword="false"/> otherwise.</returns>
        public bool MoveTier(ref ExtractionTier oldTier, int newTier) 
        {
            if(!RemoveTier(oldTier.Tier)) return false;
            oldTier = oldTier.CloneAndMove(newTier);
            AddTier(oldTier);
            return true;
        }

        /// <summary>
        /// Checks if the provided PoolEntry is in the list of registered pools
        /// </summary>
        /// <param name="pool">A PoolEntry</param>
        /// <returns><see langword="true"/> if the PoolEntry is registered, <see langword="false"/> otherwise.</returns>
        public bool PoolExists(PoolEntry pool) => PoolExists(pool.Name);
        /// <summary>
        /// Checks if a PoolEntry with that name exists in the list of registered pools.
        /// </summary>
        /// <param name="name">A string corresponding to a pool id</param>
        /// <returns><see langword="true"/> if the string corresponds to a PoolEntry, <see langword="false"/> otherwise.</returns>
        public bool PoolExists(string name) => _poolNames.ContainsKey(name);

        /// <summary>
        /// Returns a registered PoolEntry given its name.
        /// </summary>
        /// <param name="name">A string corresponding to a pool id</param>
        /// <returns>The PoolEntry with the provided name if one such entry is registered, <see langword="null"/> otherwise.</returns>
        public PoolEntry GetPoolEntry(string name) => _poolNames.TryGetValue(name, out PoolEntry pool) ? pool : null;
        /// <summary>
        /// Returns a registered PoolEntry given its name.
        /// </summary>
        /// <param name="name">A string corresponding to a pool id</param>
        /// <param name="pool">If a PoolEntry with the provided name is registered, this value will<br/>
        /// hold that entry when the method returns. It will be <see langword="null"/> otherwise.</param>
        /// <returns><see langword="true"/> if a PoolEntry with the provided name is registered, <see langword="false"/> otherwise.</returns>
        public bool GetPoolEntry(string name, out PoolEntry pool)
        {
            bool ret = _poolNames.TryGetValue(name, out pool);
            if (!ret) pool = null;
            return ret;
        }

        /// <summary>
        /// Creates a new PoolEntry and:
        /// <list type="bullet">
        /// <item><description>Registers it</description></item>
        /// <item><description>Adds it to the main Pool Priority queue</description></item>
        /// <item><description>Creates a new list of ItemEntries assigned to it.</description></item>
        /// <item><description>Creates a new list of requirements necessary to consider this pool valid.</description></item>
        /// </list>
        /// This entry will have a minimum tier equal to <see cref="BiomeExtractorEnt.EnumTiers.BASIC"/>, and will be registered as blocking.<br/>
        /// If a pool entry with that name already exists, this method does nothing.
        /// </summary>
        /// <param name="name">The name of the new PoolEntry</param>
        /// <param name="priority">The Priority value of this pool entry. Higher priorities are evaluated first.<br/>
        /// Upon finding a valid PoolEntry, the priority system will finish checking other pools with the same priority, ignoring everything else.</param>
        /// <param name="localizationKey">The localization key that is used by this pool. </param>
        /// <returns><see langword="true"/> if the PoolEntry didn't exist already, <see langword="false"/> otherwise</returns>
        public bool AddPool(string name, int priority, string localizationKey) => AddPool(new PoolEntry(name, (int)BiomeExtractorEnt.EnumTiers.BASIC, localizationKey), priority);
        /// <summary>
        /// Creates a new PoolEntry and:
        /// <list type="bullet">
        /// <item><description>Registers it</description></item>
        /// <item><description>Adds it to the main Pool Priority queue</description></item>
        /// <item><description>Creates a new list of ItemEntries assigned to it.</description></item>
        /// <item><description>Creates a new list of requirements necessary to consider this pool valid.</description></item>
        /// </list>
        /// This entry will have a minimum tier equal to <see cref="BiomeExtractorEnt.EnumTiers.BASIC"/><br/>
        /// If a pool entry with that name already exists, this method does nothing.
        /// </summary>
        /// <param name="name">The name of the new PoolEntry</param>
        /// <param name="priority">The Priority value of this pool entry. Higher priorities are evaluated first.<br/>
        /// Upon finding a valid PoolEntry, the priority system will finish checking other pools with the same <br/>priority, ignoring everything else.</param>
        /// <param name="nonBlocking">If this is true, the priority system will keep checking for pools with lower <br/>priority even if this pool is valid.</param>
        /// <param name="localizationKey">The localization key that is used by this pool. </param>
        /// <returns><see langword="true"/> if the PoolEntry didn't exist already, <see langword="false"/> otherwise</returns>
        public bool AddPool(string name, int priority, bool nonBlocking, string localizationKey = null) => AddPool(new PoolEntry(name, (int)BiomeExtractorEnt.EnumTiers.BASIC, !nonBlocking, localizationKey), priority);
        /// <summary>
        /// Creates a new PoolEntry and:
        /// <list type="bullet">
        /// <item><description>Registers it</description></item>
        /// <item><description>Adds it to the main Pool Priority queue</description></item>
        /// <item><description>Creates a new list of ItemEntries assigned to it.</description></item>
        /// <item><description>Creates a new list of requirements necessary to consider this pool valid.</description></item>
        /// </list>
        /// This entry will be registered as blocking.<br/>
        /// If a pool entry with that name already exists, this method does nothing.
        /// </summary>
        /// <param name="name">The name of the new PoolEntry</param>
        /// <param name="priority">The Priority value of this pool entry. Higher priorities are evaluated first.<br/>
        /// Upon finding a valid PoolEntry, the priority system will finish checking other pools with the same <br/>priority, ignoring everything else.</param>
        /// <param name="tier">The minimum Extractor tier required to access this pool.</param>
        /// <param name="localizationKey">The localization key that is used by this pool. </param>
        /// <returns><see langword="true"/> if the PoolEntry didn't exist already, <see langword="false"/> otherwise</returns>
        public bool AddPool(string name, int priority, int tier, string localizationKey) => AddPool(new PoolEntry(name, tier, localizationKey), priority);
        /// <summary>
        /// Creates a new PoolEntry and:
        /// <list type="bullet">
        /// <item><description>Registers it</description></item>
        /// <item><description>Adds it to the main Pool Priority queue</description></item>
        /// <item><description>Creates a new list of ItemEntries assigned to it.</description></item>
        /// <item><description>Creates a new list of requirements necessary to consider this pool valid.</description></item>
        /// </list>
        /// If a pool entry with that name already exists, this method does nothing.
        /// </summary>
        /// <param name="name">The name of the new PoolEntry</param>
        /// <param name="priority">The Priority value of this pool entry. Higher priorities are evaluated first.<br/>
        /// Upon finding a valid PoolEntry, the priority system will finish checking other pools with the same <br/>priority, ignoring everything else.</param>
        /// <param name="tier">The minimum Extractor tier required to access this pool.</param>
        /// <param name="nonBlocking">If this is true, the priority system will keep checking for pools with lower <br/>priority even if this pool is valid.</param>
        /// <param name="localizationKey">The localization key that is used by this pool. </param>
        /// <returns><see langword="true"/> if the PoolEntry didn't exist already, <see langword="false"/> otherwise</returns>
        public bool AddPool(string name, int priority, int tier = (int)BiomeExtractorEnt.EnumTiers.BASIC, bool nonBlocking = false, string localizationKey = null) => AddPool(new PoolEntry(name, tier, !nonBlocking, localizationKey), priority);
        /// <summary>
        /// Registers a PoolEntry and adds it to the main priority queue, also creating a new list<br/>
        /// of requirements and ItemEntries for it.<br/>
        /// If a pool entry with that name already exists, this method does nothing.
        /// </summary>
        /// <param name="pool">The PoolEntry to be registered</param>
        /// <param name="priority">The Priority value of this pool entry. Higher priorities are evaluated first.<br/>
        /// Upon finding a valid PoolEntry, the priority system will finish checking other pools with the same <br/>priority, ignoring everything else.</param>
        /// <returns><see langword="true"/> if the PoolEntry didn't exist already, <see langword="false"/> otherwise</returns>
        public bool AddPool(PoolEntry pool, int priority)
        {
            if (PoolExists(pool.Name)) return false;
            _poolNames.Add(pool.Name, pool);
            _priorityList.Add(priority, pool.Name);
            _itemPools.Add(pool.Name, []);
            _poolRequirements.Add(pool.Name, []);
            return true;
        }
        /// <summary>
        /// Takes a PoolEntry and looks for an already registered entry with the same id. If found, this new<br/>
        /// pool will be swapped with the old one without interfering with its requirements and ItemEntries.<br/>
        /// Nothing will happen if there is no pool to swap this one with.
        /// </summary>
        /// <param name="newPool">The PoolEntry to be registered.</param>
        /// <returns><see langword="true"/> if there was an entry to swap with, <see langword="false"/> otherwise</returns>
        public bool ChangePool(PoolEntry newPool)
        {
            bool found = GetPoolEntry(newPool.Name, out PoolEntry pool);
            if (!found) return false;
            _poolNames[pool.Name] = newPool;
            return true;
        }
        /// <summary>
        /// Gets rid of an alredy registered pool, along with its requirements and ItemEntries.
        /// </summary>
        /// <param name="poolName">The name of the PoolEntry to get rid of.</param>
        /// <returns><see langword="true"/> if there was an entry to delete, <see langword="false"/> otherwise</returns>
        public bool RemovePool(string poolName)
        {
            if(!PoolExists(poolName)) return false;
            _poolNames.Remove(poolName);
            foreach (List<string> l in _priorityList.Values)
            {
                l.Remove(poolName);
            }
            _itemPools.Remove(poolName);
            _poolRequirements.Remove(poolName);
            return true;
        }

        /// <summary>
        /// Adds one or more predicates as requirements of a pool. They must all return true
        /// for the pool to be considered valid<br/> during a scan. If no predicates are registered,
        /// the pool is considered valid by default.
        /// </summary>
        /// <param name="poolName">The name of the PoolEntry to add the predicates to.</param>
        /// <param name="conditions">One or more predicates to add to this pool's list of conditions</param>
        /// <returns><see langword="true"/> if the conditions have succesfully been added to a pool, <see langword="false"/> otherwise</returns>
        public bool AddPoolRequirements(string poolName, params Predicate<ScanData>[] conditions)
        {
            PoolEntry pool = GetPoolEntry(poolName);
            if (pool == null) return false;
            foreach (Predicate<ScanData> condition in conditions) _poolRequirements[pool.Name].Add(condition);
            return true;
        }
        /// <summary>
        /// Removes all requirememnts registered to a pool.
        /// </summary>
        /// <param name="poolName">The name of the PoolEntry to remove all the predicates of.</param>
        /// <returns><see langword="true"/> if the method found a pool to remove the conditions of, <see langword="false"/> otherwise</returns>
        public bool FlushPoolRequirements(string poolName)
        {
            PoolEntry pool = GetPoolEntry(poolName);
            if (pool == null) return false;
            _poolRequirements[pool.Name].Clear();
            return true;
        }

        /// <summary>
        /// Adds a new ItemEntry to a pool.<br/>The item will be registered with a count equal to 1.
        /// </summary>
        /// <param name="poolName">The name of the pool to add the item to.</param>
        /// <param name="itemId">The id of the item to add</param>
        /// <param name="weight">The weight of probability associated to this ItemEntry.<br/>The higher the weight, the more common the item is.</param>
        /// <returns><see langword="true"/> if the method found a pool to add the item to, <see langword="false"/> otherwise</returns>
        public bool AddItemInPool(string poolName, short itemId, int weight) => AddItemInPool(poolName, new ItemEntry(itemId, 1), weight);
        /// <summary>
        /// Adds a new ItemEntry to a pool.
        /// </summary>
        /// <param name="poolName">The name of the pool to add the item to.</param>
        /// <param name="itemId">The id of the item to add</param>
        /// <param name="count">The amount of the given item this entry will contain.<br/>
        /// Use <see cref="AddItemInPool(string, ItemEntry, int)"/> if you need to make it a range.</param>
        /// <param name="weight">The weight of probability associated to this ItemEntry.<br/>The higher the weight, the more common the item is.</param>
        /// <returns><see langword="true"/> if the method found a pool to add the item to, <see langword="false"/> otherwise</returns>
        public bool AddItemInPool(string poolName, short itemId, int count = 1, int weight = 1) => AddItemInPool(poolName, new ItemEntry(itemId, count), weight);
        /// <summary>
        /// Adds a new ItemEntry to a pool.
        /// </summary>
        /// <param name="poolName">The name of the pool to add the item to.</param>
        /// <param name="item">The ItemEntry to add</param>
        /// <param name="weight">The weight of probability associated to this ItemEntry.<br/>The higher the weight, the more common the item is.</param>
        /// <returns><see langword="true"/> if the method found a pool to add the item to, <see langword="false"/> otherwise</returns>
        public bool AddItemInPool(string poolName, ItemEntry item, int weight)
        {
            PoolEntry pool = GetPoolEntry(poolName);
            if (pool == null) return false;
            _itemPools[poolName].Add(item, weight);
            return true;
        }

        /// <summary>
        /// Removes an ItemEntry from a pool.
        /// </summary>
        /// <param name="poolName">The name of the pool to remove the item from.</param>
        /// <param name="itemId">The id of the item to remove</param>
        /// <param name="count">The amount of the given item the target entry contains.<br/>
        /// <returns><see langword="true"/> if the pool exists and it contained the target entry, <see langword="false"/> otherwise</returns>
        public bool RemoveItemFromPool(string poolName, short itemId, int count) => RemoveItemFromPool(poolName, new ItemEntry(itemId, count));
        /// <summary>
        /// Removes an ItemEntry from a pool.
        /// </summary>
        /// <param name="poolName">The name of the pool to remove the item from.</param>
        /// <param name="item">The ItemEntry to remove</param>
        /// <returns><see langword="true"/> if the pool exists and it contained the target entry, <see langword="false"/> otherwise</returns>
        public bool RemoveItemFromPool(string poolName, ItemEntry item)
        {
            PoolEntry pool = GetPoolEntry(poolName);
            if (pool == null) return false;
            return _itemPools[poolName].Remove(item);
        }

        /// <summary>
        /// Removes all ItemEntries in a pool.
        /// </summary>
        /// <param name="poolName">The name of the pool to remove the item from.</param>
        /// <returns><see langword="true"/> if the pool exists, <see langword="false"/> otherwise</returns>
        public bool FlushPoolItems(string poolName)
        {
            PoolEntry pool = GetPoolEntry(poolName);
            if (pool == null) return false;
            _itemPools[poolName].Clear();
            return false;
        }

        internal List<PoolEntry> CheckValidBiomes(ExtractionTier tier, Point16 position)
        {
            ScanData scan = new(tier, position);
            scan.Scan();

            int last_p = int.MaxValue;
            bool stop = false;
            List<PoolEntry> found = [];
            foreach (KeyValuePair<int, string> elem in _priorityList.EnumerateInOrder())
            {
                if (stop && last_p != elem.Key) break;
                last_p = elem.Key;

                PoolEntry pool = GetPoolEntry(elem.Value);
                bool check_passed = true;

                foreach (Predicate<ScanData> check in _poolRequirements[pool.Name])
                {
                    if (!check.Invoke(scan))
                    {
                        check_passed = false;
                        break;
                    }
                }

                if (check_passed && _itemPools[elem.Value].Count > 0)
                {
                    if (scan.MinTier(pool.Tier))
                        found.Add(_poolNames[elem.Value]);
                    if (pool.Blocking) stop = true;
                }
            }
            return found;
        }

        internal Item RollItem(List<PoolEntry> pools)
        {
            if (pools.Count == 1)
            {
                ItemEntry entry = _itemPools[pools[0].Name].Roll();
                return new(entry.Id, entry.Roll);
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

            Item item = new(result.Id, result.Roll);

            return item;
        }

        internal WeightedList<ItemEntry> JoinPools(List<PoolEntry> pools)
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

        /// <summary>Makes tModLoader iterate through all registered pools and check for all registered
        /// localization keys.<br/> Any undefined localization key will be added to its respective mod's localization file.</summary>
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
        #endregion

        #region Database Setup
        private static string LocalizeAs(string suffix) => $"{BiomeExtractorsMod.LocPoolNames}.{suffix}";

        public override void PostSetupContent()
        {
            InitializeTiers();
            InitializePools();
            SetRequirements();
            PopulatePools();
            GenerateLocalizationKeys();
        }

        private void InitializeTiers()
        {
            ConfigCommon cfg = ModContent.GetInstance<ConfigCommon>();
            AddTier((int)BiomeExtractorEnt.EnumTiers.BASIC,     $"{BiomeExtractorsMod.LocExtractorPrefix}Iron.DisplayName",       delegate { return cfg.Tier1ExtractorRate; }, delegate { return cfg.Tier1ExtractorChance; });
            AddTier((int)BiomeExtractorEnt.EnumTiers.DEMONIC,   $"{BiomeExtractorsMod.LocExtractorPrefix}Corruption.DisplayName", delegate { return cfg.Tier2ExtractorRate; }, delegate { return cfg.Tier2ExtractorChance; });
            AddTier((int)BiomeExtractorEnt.EnumTiers.INFERNAL,  $"{BiomeExtractorsMod.LocExtractorPrefix}Infernal.DisplayName",   delegate { return cfg.Tier3ExtractorRate; }, delegate { return cfg.Tier3ExtractorChance; });
            AddTier((int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, $"{BiomeExtractorsMod.LocExtractorPrefix}Adamantite.DisplayName", delegate { return cfg.Tier4ExtractorRate; }, delegate { return cfg.Tier4ExtractorChance; });
            AddTier((int)BiomeExtractorEnt.EnumTiers.CYBER,     $"{BiomeExtractorsMod.LocExtractorPrefix}Cyber.DisplayName",      delegate { return cfg.Tier5ExtractorRate; }, delegate { return cfg.Tier5ExtractorChance; });
            AddTier((int)BiomeExtractorEnt.EnumTiers.LUNAR,     $"{BiomeExtractorsMod.LocExtractorPrefix}Lunar.DisplayName",      delegate { return cfg.Tier6ExtractorRate; }, delegate { return cfg.Tier6ExtractorChance; });
            AddTier((int)BiomeExtractorEnt.EnumTiers.ETHEREAL,  $"{BiomeExtractorsMod.LocExtractorPrefix}Ethereal.DisplayName",   delegate { return cfg.Tier7ExtractorRate; }, delegate { return cfg.Tier7ExtractorChance; });
        }

        private void InitializePools()
        {
            AddPool(forest, 0, LocalizeAs(forest));

            AddPool(caverns,     10, LocalizeAs(caverns));
            AddPool(underground, 10, LocalizeAs(underground));
            AddPool(evil_ores,   10, (int)BiomeExtractorEnt.EnumTiers.DEMONIC);
            AddPool(hm_ores,     10, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK);

            AddPool(snow,   50, LocalizeAs(snow));
            AddPool(desert, 50, LocalizeAs(desert));
            AddPool(jungle, 50, LocalizeAs(jungle));
            AddPool(shells, 50, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK);
            AddPool(sky,    50, LocalizeAs(sky));
            AddPool(flight, 50, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK);

            AddPool(hallowed_bars_forest, 100, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK);
            AddPool(hallowed_bars_desert, 100, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK);
            AddPool(hallowed_bars_snow,   100, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK);
            AddPool(hallowed_forest,      100, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, LocalizeAs(hallowed_forest));      
            AddPool(hallowed_desert,      100, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, LocalizeAs(hallowed_desert));
            AddPool(hallowed_snow,        100, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, LocalizeAs(hallowed_snow));

            AddPool(mushroom, 200);

            AddPool(crimson_dark_shard, 300, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK);
            AddPool(corrupt_forest_hm,  300, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK);
            AddPool(corrupt_desert_hm,  300, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK);
            AddPool(corrupt_snow_hm,    300, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK);
            AddPool(corrupt_forest,     300, (int)BiomeExtractorEnt.EnumTiers.DEMONIC, LocalizeAs(corrupt_forest));
            AddPool(crimson_forest,     300, (int)BiomeExtractorEnt.EnumTiers.DEMONIC, LocalizeAs(crimson_forest));
            AddPool(corrupt_snow,       300, (int)BiomeExtractorEnt.EnumTiers.DEMONIC, LocalizeAs(corrupt_snow));
            AddPool(crimson_snow,       300, (int)BiomeExtractorEnt.EnumTiers.DEMONIC, LocalizeAs(crimson_snow));
            AddPool(corrupt_desert,     300, (int)BiomeExtractorEnt.EnumTiers.DEMONIC, LocalizeAs(corrupt_desert));
            AddPool(crimson_desert,     300, (int)BiomeExtractorEnt.EnumTiers.DEMONIC, LocalizeAs(crimson_desert));

            AddPool(graveyard, 500, LocalizeAs(graveyard));

            AddPool(ug_snow,      1050, LocalizeAs(ug_snow));
            AddPool(ug_desert,    1050, LocalizeAs(ug_desert));
            AddPool(ug_desert_hm, 1050, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK);
            AddPool(ug_jungle,    1050, LocalizeAs(ug_jungle));
            AddPool(ug_shells,    1050, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK);
            AddPool(life_fruit,   1050, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK);
            AddPool(hive,         1050, LocalizeAs(hive));
            AddPool(chlorophyte,  1050, (int)BiomeExtractorEnt.EnumTiers.CYBER);

            AddPool(ug_hallowed_bars_caverns, 1100, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK);
            AddPool(ug_hallowed_bars_desert,  1100, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK);
            AddPool(ug_hallowed_bars_snow,    1100, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK);
            AddPool(ug_hallowed_caverns,      1100, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, LocalizeAs(ug_hallowed_caverns));
            AddPool(ug_hallowed_snow,         1100, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, LocalizeAs(ug_hallowed_snow));
            AddPool(ug_hallowed_desert,       1100, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK, LocalizeAs(ug_hallowed_desert));

            AddPool(ug_mushroom,  1200, LocalizeAs(ug_mushroom));
            AddPool(truffle_worm, 1200, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK);

            AddPool(ug_corrupt_caverns_hm, 1300, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK);
            AddPool(ug_crimson_caverns_hm, 1300, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK);
            AddPool(ug_corrupt_desert_hm,  1300, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK);
            AddPool(ug_crimson_desert_hm,  1300, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK);
            AddPool(ug_corrupt_snow_hm,    1300, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK);
            AddPool(ug_crimson_snow_hm,    1300, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK);
            AddPool(ug_corrupt_caverns,    1300, (int)BiomeExtractorEnt.EnumTiers.DEMONIC, LocalizeAs(ug_corrupt_caverns));
            AddPool(ug_crimson_caverns,    1300, (int)BiomeExtractorEnt.EnumTiers.DEMONIC, LocalizeAs(ug_crimson_caverns));
            AddPool(ug_corrupt_snow,       1300, (int)BiomeExtractorEnt.EnumTiers.DEMONIC, LocalizeAs(ug_corrupt_snow));
            AddPool(ug_crimson_snow,       1300, (int)BiomeExtractorEnt.EnumTiers.DEMONIC, LocalizeAs(ug_crimson_snow));
            AddPool(ug_corrupt_desert,     1300, (int)BiomeExtractorEnt.EnumTiers.DEMONIC, LocalizeAs(ug_corrupt_desert));
            AddPool(ug_crimson_desert,     1300, (int)BiomeExtractorEnt.EnumTiers.DEMONIC, LocalizeAs(ug_crimson_desert));
            
            AddPool(dungeon,   2000, (int)BiomeExtractorEnt.EnumTiers.DEMONIC, LocalizeAs(dungeon));
            AddPool(dungeon_p, 2000, (int)BiomeExtractorEnt.EnumTiers.DEMONIC);
            AddPool(dungeon_g, 2000, (int)BiomeExtractorEnt.EnumTiers.DEMONIC);
            AddPool(dungeon_b, 2000, (int)BiomeExtractorEnt.EnumTiers.DEMONIC);
            AddPool(ectoplasm, 2000, (int)BiomeExtractorEnt.EnumTiers.CYBER);
            AddPool(temple,    2000, (int)BiomeExtractorEnt.EnumTiers.CYBER,   LocalizeAs(temple));

            AddPool(ocean,  2500, LocalizeAs(ocean));
            AddPool(pirate, 2500, (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK);

            AddPool(shimmer, 3000, (int)BiomeExtractorEnt.EnumTiers.DEMONIC,  true, LocalizeAs(shimmer));
            AddPool(spider,  3000, (int)BiomeExtractorEnt.EnumTiers.INFERNAL, true);
            AddPool(cobweb,  3000, true, LocalizeAs(cobweb));
            AddPool(granite, 3000, true, LocalizeAs(granite));
            AddPool(marble,  3000, true, LocalizeAs(marble));

            AddPool(space,      4000,  LocalizeAs(space));
            AddPool(spc_flight, 4000,  (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK);
            AddPool(pillar,     4000,  (int)BiomeExtractorEnt.EnumTiers.LUNAR);
            AddPool(luminite,   4000,  (int)BiomeExtractorEnt.EnumTiers.ETHEREAL);
            AddPool(underworld, 4000,  (int)BiomeExtractorEnt.EnumTiers.INFERNAL, LocalizeAs(underworld));
            AddPool(uw_fire,    4000,  (int)BiomeExtractorEnt.EnumTiers.STEAMPUNK);
            AddPool(meteorite,  10000, (int)BiomeExtractorEnt.EnumTiers.INFERNAL, LocalizeAs(meteorite));
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

            AddItemInPool(snow, ItemID.SnowBlock,                       12);
            AddItemInPool(snow, ItemID.IceBlock,                        6);
            AddItemInPool(snow, new ItemEntry(ItemID.BorealWood, 1, 3), 40);
            AddItemInPool(snow, ItemID.Shiverthorn,                     7);

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
            AddItemInPool(shells, ItemID.TurtleShell,                       5);

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
            AddItemInPool(hallowed_snow, ItemID.SnowBlock,                       27);
            AddItemInPool(hallowed_snow, ItemID.PinkIceBlock,                    14);
            AddItemInPool(hallowed_snow, ItemID.PixieDust,                       15);
            AddItemInPool(hallowed_snow, ItemID.UnicornHorn,                     5);
            AddItemInPool(hallowed_snow, ItemID.RainbowBrick,                    10);
            AddItemInPool(hallowed_snow, new ItemEntry(ItemID.BorealWood, 1, 3), 50);
            AddItemInPool(hallowed_snow, ItemID.Shiverthorn,                     25);
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
            AddItemInPool(crimson_snow, ItemID.SnowBlock,                       27);
            AddItemInPool(crimson_snow, ItemID.RedIceBlock,                     14);
            AddItemInPool(crimson_snow, ItemID.Vertebrae,                       25);
            AddItemInPool(crimson_snow, new ItemEntry(ItemID.BorealWood, 1, 3), 50);

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
            AddItemInPool(corrupt_snow, ItemID.SnowBlock,                       27);
            AddItemInPool(corrupt_snow, ItemID.PurpleIceBlock,                  14);
            AddItemInPool(corrupt_snow, ItemID.RottenChunk,                     15);
            AddItemInPool(corrupt_snow, ItemID.WormTooth,                       10);
            AddItemInPool(corrupt_snow, new ItemEntry(ItemID.BorealWood, 1, 3), 50);
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
            AddItemInPool(ug_shells,   ItemID.TurtleShell, 4);
            AddItemInPool(life_fruit,  ItemID.LifeFruit, 1);
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

            AddItemInPool(dungeon_p, ItemID.PinkBrick,  12);
            AddItemInPool(dungeon_g, ItemID.GreenBrick, 12);
            AddItemInPool(dungeon_b, ItemID.BlueBrick,  12);
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
        #endregion
    }
}