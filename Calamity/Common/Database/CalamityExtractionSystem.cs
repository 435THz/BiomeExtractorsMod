using BiomeExtractorsMod.Common.Database;
using BiomeExtractorsMod.Common.Hooks;
using BiomeExtractorsMod.Content.Tiles;
using BiomeExtractorsMod.Calamity.Content.TileEntities;
using CalamityMod;
using CalamityMod.Items.Placeables.Astral;
using CalamityMod.Items.Placeables.SunkenSea;
using CalamityMod.Items.Placeables.Abyss;
using CalamityMod.Items.Placeables.FurnitureAcidwood;
using CalamityMod.Items.Placeables.Crags;
using CalamityMod.Items.Placeables.FurnitureMonolith;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Critters;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Items.SummonItems;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static BiomeExtractorsMod.Common.Database.BiomeExtractionSystem;
using NavystoneTile = CalamityMod.Tiles.SunkenSea.Navystone;
using EutrophicSandTile = CalamityMod.Tiles.SunkenSea.EutrophicSand;
using SeaPrismTile = CalamityMod.Tiles.SunkenSea.SeaPrism;
using SulphurousSandTile = CalamityMod.Tiles.Abyss.SulphurousSand;
using SulphurousSandstoneTile = CalamityMod.Tiles.Abyss.SulphurousSandstone;
using HardenedSulphurousSandstoneTile = CalamityMod.Tiles.Abyss.HardenedSulphurousSandstone;
using SulphurousShaleTile = CalamityMod.Tiles.Abyss.SulphurousShale;
using AbyssGravelTile = CalamityMod.Tiles.Abyss.AbyssGravel;
using PyreMantleTile = CalamityMod.Tiles.Abyss.PyreMantle;
using PyreMantleMoltenTile = CalamityMod.Tiles.Abyss.PyreMantleMolten;
using VoidstoneTile = CalamityMod.Tiles.Abyss.Voidstone;
using BrimstoneSlagTile = CalamityMod.Tiles.Crags.BrimstoneSlag;
using InfernalSueviteTile = CalamityMod.Tiles.Ores.InfernalSuevite;
using AstralDirtTile = CalamityMod.Tiles.Astral.AstralDirt;
using AstralStoneTile = CalamityMod.Tiles.Astral.AstralStone;
using AstralClayTile = CalamityMod.Tiles.Astral.AstralClay;
using AstralOreTile = CalamityMod.Tiles.Ores.AstralOre;
using NovaeSlagTile = CalamityMod.Tiles.Astral.NovaeSlag;
using AstralGrassTile = CalamityMod.Tiles.Astral.AstralGrass;
using AstralIceTile = CalamityMod.Tiles.AstralSnow.AstralIce;
using AstralSandTile = CalamityMod.Tiles.AstralDesert.AstralSand;
using AstralSandstoneTile = CalamityMod.Tiles.AstralDesert.AstralSandstone;
using HardenedAstralSandTile = CalamityMod.Tiles.AstralDesert.HardenedAstralSand;
using Fraction = BiomeExtractorsMod.Common.Fraction;

namespace BiomeExtractorsMod.Calamity.Common.Database
{
    [ExtendsFromMod("CalamityMod")]
    [JITWhenModsEnabled("CalamityMod")]
    public class CalamityExtractionSystem : ExtractionSystemExtension
    {
        public static CalamityExtractionSystem Instance => ModContent.GetInstance<CalamityExtractionSystem>();

        #region IDs
        public static readonly string snow_hm = "snow_hm";
        public static readonly string ug_snow_hm = "ug_snow_hm";
        public static readonly string ug_snow_cryo = "ug_snow_cryo";
        public static readonly string snow_hm_remix = "snow_hm_remix";
        public static readonly string ug_snow_hm_remix = "ug_snow_hm_remix";
        public static readonly string ug_snow_cryo_remix = "ug_snow_cryo_remix";
        public static readonly string jungle_glm = "jungle_glm";
        public static readonly string jungle_ml = "jungle_ml";
        public static readonly string ug_jungle_glm = "ug_jungle_glm";
        public static readonly string ug_jungle_prv = "ug_jungle_prv";
        public static readonly string dungeon_ml = "dungeon_ml";
        public static readonly string space_evil2 = "space_evil2";
        public static readonly string space_hm = "space_hm";
        public static readonly string space_ml = "space_ml";
        public static readonly string space_exo = "space_exo";
        public static readonly string caverns_pla = "caverns_pla";
        public static readonly string caverns_yhr = "caverns_yhr";
        public static readonly string caverns_pla_remix = "caverns_pla_remix";
        public static readonly string caverns_yhr_remix = "caverns_yhr_remix";
        public static readonly string hallowed_forest_ml = "hallowed_forest_ml";
        public static readonly string hallowed_desert_ml = "hallowed_desert_ml";
        public static readonly string hallowed_snow_ml = "hallowed_snow_ml";
        public static readonly string hallowed_forest_ml_remix = "hallowed_forest_ml_remix";
        public static readonly string hallowed_desert_ml_remix = "hallowed_desert_ml_remix";
        public static readonly string hallowed_snow_ml_remix = "hallowed_snow_ml_remix";
        public static readonly string ug_hallowed_ore = "ug_hallowed_ore";
        public static readonly string ug_hallowed_ore_desert = "ug_hallowed_ore_desert";
        public static readonly string ug_hallowed_ore_snow = "ug_hallowed_ore_snow";
        public static readonly string ug_hallowed_ore_remix = "ug_hallowed_ore_remix";
        public static readonly string ug_hallowed_ore_desert_remix = "ug_hallowed_ore_desert_remix";
        public static readonly string ug_hallowed_ore_snow_remix = "ug_hallowed_ore_snow_remix";
        public static readonly string underworld_ml = "underworld_ml";
        public static readonly string graveyard_infernal = "graveyard_infernal";
        public static readonly string graveyard_cal_clone = "graveyard_cal_clone";
        public static readonly string graveyard_dog = "graveyard_dog";
        public static readonly string graveyard_dog_cold = "graveyard_dog_cold";
        public static readonly string graveyard_dog_evil = "graveyard_dog_evil";

        public static readonly string sunken_sea = "sunken_sea";
        public static readonly string sunken_sea_hm = "sunken_sea_hm";
        public static readonly string sulphur_sea = "sulphur_sea";
        public static readonly string sulphur_sea_hm = "sulphur_sea_hm";
        public static readonly string sulphur_sea_as = "sulphur_sea_as";
        public static readonly string sulphur_sea_ml = "sulphur_sea_ml";
        public static readonly string sulphur_depths = "sulphur_depths";
        public static readonly string sulphur_depths_lev = "sulphur_depths_lev";
        public static readonly string acid_rain = "acid_rain";
        public static readonly string acid_rain_as = "acid_rain_as";
        public static readonly string murky_waters = "murky_waters";
        public static readonly string murky_waters_lev = "murky_waters_leb";
        public static readonly string murky_waters_glm = "murky_waters_glm";
        public static readonly string thermal_vents = "thermal_vents";
        public static readonly string thermal_vents_pla = "thermal_vents_pla";
        public static readonly string thermal_vents_lev = "thermal_vents_lev";
        public static readonly string thermal_vents_glm = "thermal_vents_glm";
        public static readonly string the_void = "the_void";
        public static readonly string the_void_pla = "the_void_pla";
        public static readonly string the_void_lev = "the_void_lev";
        public static readonly string the_void_pgh = "the_void_pgh";
        public static readonly string brimstone_crag = "brimstone_crag";
        public static readonly string brimstone_crag_hm = "brimstone_crag_hm";
        public static readonly string brimstone_crag_mch = "brimstone_crag_mch";
        public static readonly string brimstone_crag_prv = "brimstone_crag_prv";
        public static readonly string brimstone_crag_swc = "brimstone_crag_swc";
        public static readonly string astral_forest = "astral_forest";
        public static readonly string astral_ore_forest = "astral_ore_forest";
        public static readonly string astral_snow = "astral_snow";
        public static readonly string astral_ore_snow = "astral_ore_snow";
        public static readonly string astral_desert = "astral_desert";
        public static readonly string astral_ore_desert = "astral_ore_desert";
        public static readonly string ug_astral_forest = "ug_astral_forest";
        public static readonly string ug_astral_ore_forest = "ug_astral_ore_forest";
        public static readonly string ug_astral_snow = "ug_astral_snow";
        public static readonly string ug_astral_ore_snow = "ug_astral_ore_snow";
        public static readonly string ug_astral_desert = "ug_astral_desert";
        public static readonly string ug_astral_ore_desert = "ug_astral_ore_desert";

        //subelements
        public static readonly string post_acid_rain_key = "post_acid_rain";
        public static readonly string post_evil2_key = "post_evil2";
        public static readonly string post_cryogen_key = "post_cryogen";
        public static readonly string post_aq_scourge_key = "post_aq_scourge";
        public static readonly string post_acid_rain2_key = "post_acid_rain2";
        public static readonly string post_plantera_key = "post_plantera";
        public static readonly string post_golem_key = "post_golem";
        public static readonly string post_leviathan_key = "post_leviathan";
        public static readonly string post_moon_lord_key = BiomeExtractionSystem.post_moon_lord;
        public static readonly string post_providence_key = "post_providence";
        public static readonly string post_deus_key = "post_deus";
        public static readonly string post_polterghast_key = "post_polterghast";
        public static readonly string forest_post_cal_clone_key = "forest_post_cal_clone";
        public static readonly string forest_post_dog_key = "forest_post_dog";
        public static readonly string cold_post_dog_key = "cold_post_dog";
        public static readonly string evil_post_dog_key = "evil_post_dog";
        public static readonly string post_yharon_key = "post_yharon";
        public static readonly string post_exo_key = "post_exo";
        public static readonly string post_swc_key = "post_swc";
        #endregion

        #region Checks
        //BLOCK LISTS
        public static readonly ushort[] forestBlocks = [TileID.Grass];
        public static readonly ushort[] sunken_sea_blocks = [(ushort)ModContent.TileType<NavystoneTile>(), (ushort)ModContent.TileType<EutrophicSandTile>(), (ushort)ModContent.TileType<SeaPrismTile>()];
        public static readonly ushort[] sulphur_sea_blocks = [(ushort)ModContent.TileType<SulphurousSandTile>(), (ushort)ModContent.TileType<SulphurousSandstoneTile>(), (ushort)ModContent.TileType<HardenedSulphurousSandstoneTile>()];
        public static readonly ushort[] sulphur_depths_blocks = [(ushort)ModContent.TileType<SulphurousShaleTile>()];
        public static readonly ushort[] abyss_gravel = [(ushort)ModContent.TileType<AbyssGravelTile>()];
        public static readonly ushort[] thermal_blocks = [(ushort)ModContent.TileType<PyreMantleTile>(), (ushort)ModContent.TileType<PyreMantleMoltenTile>()];
        public static readonly ushort[] voidstone = [(ushort)ModContent.TileType<VoidstoneTile>()];
        public static readonly ushort[] brimstone_blocks = [(ushort)ModContent.TileType<BrimstoneSlagTile>(), (ushort)ModContent.TileType<InfernalSueviteTile>()];
        public static readonly ushort[] astral_forest_blocks = [(ushort)ModContent.TileType<AstralDirtTile>(), (ushort)ModContent.TileType<AstralStoneTile>(), (ushort)ModContent.TileType<AstralOreTile>(), (ushort)ModContent.TileType<AstralClayTile>(), (ushort)ModContent.TileType<NovaeSlagTile>(), (ushort)ModContent.TileType<AstralGrassTile>()];
        public static readonly ushort[] astral_snow_blocks = [(ushort)ModContent.TileType<AstralIceTile>()];
        public static readonly ushort[] astral_desert_blocks = [(ushort)ModContent.TileType<AstralSandTile>(), (ushort)ModContent.TileType<AstralSandstoneTile>(), (ushort)ModContent.TileType<HardenedAstralSandTile>()];

        //COMPLEX
        public static readonly Func<ushort[], ushort[], Predicate<ScanData>> more_than_tiles = (tiles1, tiles2) => scan => scan.Tiles(tiles1) > scan.Tiles(tiles2);
        public static readonly Predicate<ScanData> in_sulphur_sea = scan => abyss_area.Invoke(scan) || sulphur_sea300.Invoke(scan);

        //POSITION
        public static readonly Predicate<ScanData> abyss_area = scan => BiomeChecker.IsInAbyssArea(scan);

        //PROGRESSION
        public static readonly Predicate<ScanData> acid_rain_finished = scan => CalamityConditions.DownedAcidRainT1.IsMet() || acid_rain2_finished.Invoke(scan);
        public static readonly Predicate<ScanData> post_scourge = scan => CalamityConditions.DownedDesertScourge.IsMet();
        public static readonly Predicate<ScanData> post_evil2 = scan => CalamityConditions.DownedHiveMindOrPerforator.IsMet();
        public static readonly Predicate<ScanData> hardmodeOnly = scan => Main.hardMode;
        public static readonly Predicate<ScanData> post_cryogen = scan => CalamityConditions.DownedCryogen.IsMet();
        public static readonly Predicate<ScanData> post_scourge2 = scan => CalamityConditions.DownedAquaticScourge.IsMet();
        public static readonly Predicate<ScanData> acid_rain2_finished = scan => CalamityConditions.DownedAcidRainT2.IsMet() || acid_rain3_finished.Invoke(scan);
        public static readonly Predicate<ScanData> post_cal_clone = scan => CalamityConditions.DownedCalamitasClone.IsMet();
        public static readonly Predicate<ScanData> post_plantera = scan => Condition.DownedPlantera.IsMet();
        public static readonly Predicate<ScanData> post_leviathan = scan => CalamityConditions.DownedLeviathan.IsMet();
        public static readonly Predicate<ScanData> post_golem = scan => Condition.DownedGolem.IsMet();
        public static readonly Predicate<ScanData> post_deus = scan => CalamityConditions.DownedAstrumDeus.IsMet();
        public static readonly Predicate<ScanData> post_moon_lord = scan => Condition.DownedMoonLord.IsMet();
        public static readonly Predicate<ScanData> acid_rain3_finished = scan => CalamityConditions.DownedOldDuke.IsMet();
        public static readonly Predicate<ScanData> post_providence = scan => CalamityConditions.DownedProvidence.IsMet();
        public static readonly Predicate<ScanData> post_polterghast = scan => CalamityConditions.DownedPolterghast.IsMet();
        public static readonly Predicate<ScanData> post_dog = scan => CalamityConditions.DownedDevourerOfGods.IsMet();
        public static readonly Predicate<ScanData> post_yharon = scan => CalamityConditions.DownedYharon.IsMet();
        public static readonly Predicate<ScanData> post_exo_mechs = scan => CalamityConditions.DownedExoMechs.IsMet();
        public static readonly Predicate<ScanData> post_calamitas = scan => CalamityConditions.DownedSupremeCalamitas.IsMet();

        //TIERS
        static readonly Predicate<ScanData> demonic = scan => scan.MinTier(ExtractionTiers.DEMONIC);
        static readonly Predicate<ScanData> infernal = scan => scan.MinTier(ExtractionTiers.INFERNAL);
        static readonly Predicate<ScanData> steampunk = scan => scan.MinTier(ExtractionTiers.STEAMPUNK);
        static readonly Predicate<ScanData> cyber = scan => scan.MinTier(ExtractionTiers.CYBER);
        static readonly Predicate<ScanData> lunar = scan => scan.MinTier(ExtractionTiers.LUNAR);
        static readonly Predicate<ScanData> ethereal = scan => scan.MinTier(ExtractionTiers.ETHEREAL);
        static readonly Predicate<ScanData> spectral = scan => scan.MinTier(ExtractionTiers.SPECTRAL);
        static readonly Predicate<ScanData> auric = scan => scan.MinTier(ExtractionTiers.AURIC);
        static readonly Predicate<ScanData> exo = scan => scan.MinTier(ExtractionTiers.EXO);

        //BLOCKS
        static readonly Predicate<ScanData> grass100 = scan => scan.Tiles(forestBlocks) >= 100;
        static readonly Predicate<ScanData> anyEvil300 = scan => evil300.Invoke(corruptBlocks).Invoke(scan) || evil300.Invoke(crimsonBlocks).Invoke(scan);

        static readonly Predicate<ScanData> sunken_sea150 = scan => scan.Tiles(sunken_sea_blocks) >= 150;
        static readonly Predicate<ScanData> sulphur_sea300 = scan => scan.Tiles(sulphur_sea_blocks) >= 300;
        static readonly Predicate<ScanData> abyss_gravel300 = scan => scan.Tiles(abyss_gravel) >= 300;
        static readonly Predicate<ScanData> thermal_blocks300 = scan => scan.Tiles(thermal_blocks) >= 300;
        static readonly Predicate<ScanData> voidstone300 = scan => scan.Tiles(voidstone) >= 300;
        static readonly Predicate<ScanData> brimstone100 = scan => scan.Tiles(brimstone_blocks) >= 100;
        static readonly Predicate<ScanData> astral_forest950 = scan => scan.Tiles(astral_forest_blocks) >= 950;
        static readonly Predicate<ScanData> astral_snow951 = scan => scan.Tiles(astral_snow_blocks) >= 951;
        static readonly Predicate<ScanData> astral_desert951 = scan => scan.Tiles(astral_desert_blocks) >= 951;

        //MACHINE(, TURN BACK NOW)
        static readonly Predicate<ScanData> sulphuric_extractor = scan => pressurized_extractor.Invoke(scan) || TileUtils.TryGetTileEntityAs((int)scan.X, (int)scan.Y, out SulphuricExtractorEnt _);
        static readonly Predicate<ScanData> pressurized_extractor = scan => thermal_extractor.Invoke(scan) || TileUtils.TryGetTileEntityAs((int)scan.X, (int)scan.Y, out PressurizedExtractorEnt _);
        static readonly Predicate<ScanData> thermal_extractor = scan => abyssal_extractor.Invoke(scan) || TileUtils.TryGetTileEntityAs((int)scan.X, (int)scan.Y, out ThermoresistantExtractorEnt _);
        static readonly Predicate<ScanData> abyssal_extractor = scan => TileUtils.TryGetTileEntityAs((int)scan.X, (int)scan.Y, out AbyssalExtractorEnt _) || scan.IsScanner;
        #endregion

        #region Database Setup
        private static string LocalizeAs(string suffix) => BiomeExtractionSystem.LocalizeAs(suffix);
        private static string SubLocalizeAs(string suffix) => BiomeExtractionSystem.SubLocalizeAs(suffix);
        public override void LoadDatabase()
        {
            ExpandVanillaPools();
            SetupAstralForest();
            SetupAstralDesert();
            SetupAstralSnow();
            SetupSunkenSea();
            SetupSulphurousSea();
            SetupCrags();
            SetupAbyss();
        }

        private static void ExpandVanillaPools()
        {
            ExpandForest();
            ExpandUnderground();
            ExpandCaverns();
            ExpandDesert();
            ExpandSnow();
            ExpandJungle();
            ExpandCrimson();
            ExpandCorruption();
            ExpandHallow();
            SubstituteHallowOre();
            ExpandGraveyard();
            ExpandDungeon();
            ExpandSpace();
            ExpandUnderworld();
        }
        private static void ExpandForest()
        {
            BES.AddItemInPool(forest, (short)ModContent.ItemType<WulfrumMetalScrap>(), 40);
            BES.AddItemInPool(forest, (short)ModContent.ItemType<EnergyCore>(), 15);
            BES.AddItemInPool(caverns_remix, (short)ModContent.ItemType<WulfrumMetalScrap>(), 40);
            BES.AddItemInPool(caverns_remix, (short)ModContent.ItemType<EnergyCore>(), 15);
        }
        private static void ExpandUnderground()
        {
            BES.AddItemInPool(underground, (short)ModContent.ItemType<AncientBoneDust>(), 10);
        }
        private static void ExpandCaverns()
        {
            BES.AddPool(caverns_pla, 10, true);
            BES.AddPool(caverns_yhr, 10, true);
            BES.AddPool(caverns_pla_remix, 10, true);
            BES.AddPool(caverns_yhr_remix, 10, true);

            BES.AddPoolParent(caverns_pla, caverns, SubLocalizeAs(post_plantera_key));
            BES.AddPoolParent(caverns_yhr, caverns, SubLocalizeAs(post_yharon_key));
            BES.AddPoolParent(caverns_pla_remix, caverns_remix, SubLocalizeAs(post_plantera_key));
            BES.AddPoolParent(caverns_yhr_remix, caverns_remix, SubLocalizeAs(post_yharon_key));

            BES.AddPoolVisibilityRequirements(caverns_pla, cyber);
            BES.AddPoolVisibilityRequirements(caverns_yhr, auric);
            BES.AddPoolVisibilityRequirements(caverns_pla_remix, cyber);
            BES.AddPoolVisibilityRequirements(caverns_yhr_remix, auric);

            BES.AddPoolAccessRequirements(caverns_pla, post_plantera);
            BES.AddPoolAccessRequirements(caverns_yhr, post_yharon);
            BES.AddPoolAccessRequirements(caverns_pla_remix, post_plantera);
            BES.AddPoolAccessRequirements(caverns_yhr_remix, post_yharon);

            BES.AddPoolWorldChecks(caverns_pla, notremix);
            BES.AddPoolWorldChecks(caverns_yhr, notremix);
            BES.AddPoolWorldChecks(caverns_pla_remix, remix);
            BES.AddPoolWorldChecks(caverns_yhr_remix, remix);

            BES.AddPoolBiomeChecks(caverns_pla, purity100, cavernLayer);
            BES.AddPoolBiomeChecks(caverns_yhr, purity100, cavernLayer);
            BES.AddPoolBiomeChecks(caverns_pla_remix, purity100, cavernLayer);
            BES.AddPoolBiomeChecks(caverns_yhr_remix, purity100, cavernLayer);

            BES.AddItemInPool(caverns, (short)ModContent.ItemType<AncientBoneDust>(), 10);
            BES.AddItemInPool(caverns_pla, (short)ModContent.ItemType<PerennialOre>(), 12);
            BES.AddItemInPool(caverns_yhr, new ItemEntry((short)ModContent.ItemType<AuricOre>(), 5, 15), 15);
            BES.AddItemInPool(caverns_pla_remix, (short)ModContent.ItemType<PerennialOre>(), 25);
            BES.AddItemInPool(caverns_yhr_remix, new ItemEntry((short)ModContent.ItemType<AuricOre>(), 5, 15), 30);
        }
        private static void ExpandDesert()
        {
            BES.AddItemInPool(ug_desert, (short)ModContent.ItemType<StormlionMandible>(), 5);
        }
        private static void ExpandSnow()
        {
            BES.AddPool(snow_hm, 50);
            BES.AddPool(ug_snow_hm, 1050);
            BES.AddPool(ug_snow_cryo, 1050);
            BES.AddPool(snow_hm_remix, 50);
            BES.AddPool(ug_snow_hm_remix, 1050);
            BES.AddPool(ug_snow_cryo_remix, 1050);

            BES.AddPoolParent(snow_hm, snow, SubLocalizeAs(in_hardmode));
            BES.AddPoolParent(ug_snow_hm, ug_snow, SubLocalizeAs(in_hardmode));
            BES.AddPoolParent(ug_snow_cryo, ug_snow, SubLocalizeAs(post_cryogen_key));
            BES.AddPoolParent(snow_hm_remix, snow, SubLocalizeAs(in_hardmode));
            BES.AddPoolParent(ug_snow_hm_remix, ug_snow, SubLocalizeAs(in_hardmode));
            BES.AddPoolParent(ug_snow_cryo_remix, ug_snow, SubLocalizeAs(post_cryogen_key));

            BES.AddPoolVisibilityRequirements(snow_hm, steampunk);
            BES.AddPoolVisibilityRequirements(ug_snow_hm, steampunk);
            BES.AddPoolVisibilityRequirements(ug_snow_cryo, steampunk);
            BES.AddPoolVisibilityRequirements(snow_hm_remix, steampunk);
            BES.AddPoolVisibilityRequirements(ug_snow_hm_remix, steampunk);
            BES.AddPoolVisibilityRequirements(ug_snow_cryo_remix, steampunk);

            BES.AddPoolAccessRequirements(snow_hm, hardmodeOnly);
            BES.AddPoolAccessRequirements(ug_snow_hm, hardmodeOnly);
            BES.AddPoolAccessRequirements(ug_snow_cryo, post_cryogen);
            BES.AddPoolAccessRequirements(snow_hm_remix, hardmodeOnly);
            BES.AddPoolAccessRequirements(ug_snow_hm_remix, hardmodeOnly);
            BES.AddPoolAccessRequirements(ug_snow_cryo_remix, post_cryogen);

            BES.AddPoolWorldChecks(ug_snow_hm, notremix);
            BES.AddPoolWorldChecks(ug_snow_cryo, notremix);
            BES.AddPoolWorldChecks(snow_hm_remix, remix);
            BES.AddPoolWorldChecks(ug_snow_hm_remix, remix);
            BES.AddPoolWorldChecks(ug_snow_cryo_remix, remix);

            BES.AddPoolBiomeChecks(snow_hm, frost1500);
            BES.AddPoolBiomeChecks(ug_snow_hm, belowSurfaceLayer, frost1500);
            BES.AddPoolBiomeChecks(ug_snow_cryo, belowSurfaceLayer, frost1500);
            BES.AddPoolBiomeChecks(snow_hm_remix, cavernLayer, frost1500);
            BES.AddPoolBiomeChecks(ug_snow_hm_remix, belowSurfaceLayer, notCavernLayer, frost1500);
            BES.AddPoolBiomeChecks(ug_snow_cryo_remix, belowSurfaceLayer, notCavernLayer, frost1500);

            BES.AddItemInPool(snow, ItemID.Leather, 1);
            BES.AddItemInPool(snow_hm, (short)ModContent.ItemType<EssenceofEleum>(), 1);
            BES.AliasItemPool(snow_hm_remix, snow_hm);

            BES.AddItemInPool(ug_snow_hm, (short)ModContent.ItemType<EssenceofEleum>(), 4);
            BES.AddItemInPool(ug_snow_cryo, (short)ModContent.ItemType<CryonicOre>(), 4);
            BES.AliasItemPool(ug_snow_hm_remix, ug_snow_hm);
            BES.AliasItemPool(ug_snow_cryo_remix, ug_snow_cryo);
        }
        private static void ExpandJungle()
        {
            BES.AddPool(jungle_glm, 50);
            BES.AddPool(jungle_ml, 50);
            BES.AddPool(ug_jungle_glm, 1050);
            BES.AddPool(ug_jungle_prv, 1050);

            BES.AddPoolParent(jungle_glm, jungle, SubLocalizeAs(post_golem_key));
            BES.AddPoolParent(jungle_ml, jungle, SubLocalizeAs(post_moon_lord_key));
            BES.AddPoolParent(ug_jungle_glm, ug_jungle_glm, SubLocalizeAs(post_golem_key));
            BES.AddPoolParent(ug_jungle_prv, ug_jungle_prv, SubLocalizeAs(post_providence_key));

            BES.AddPoolVisibilityRequirements(jungle_glm, cyber);
            BES.AddPoolVisibilityRequirements(jungle_ml, ethereal);
            BES.AddPoolVisibilityRequirements(ug_jungle_glm, cyber);
            BES.AddPoolVisibilityRequirements(ug_jungle_prv, ethereal);

            BES.AddPoolAccessRequirements(jungle_glm, post_golem);
            BES.AddPoolAccessRequirements(jungle_ml, postML);
            BES.AddPoolAccessRequirements(ug_jungle_glm, post_golem);
            BES.AddPoolAccessRequirements(ug_jungle_prv, post_providence);

            BES.AddPoolBiomeChecks(jungle_glm, jungle140);
            BES.AddPoolBiomeChecks(jungle_ml, jungle140);
            BES.AddPoolBiomeChecks(ug_jungle_glm, belowSurfaceLayer, jungle140);
            BES.AddPoolBiomeChecks(ug_jungle_prv, belowSurfaceLayer, jungle140);

            BES.AddItemInPool(jungle_glm, (short)ModContent.ItemType<PlagueCellCanister>(), 10);
            BES.AddItemInPool(jungle_ml, (short)ModContent.ItemType<EffulgentFeather>(), 10);

            BES.AddItemInPool(ug_jungle_glm, (short)ModContent.ItemType<PlagueCellCanister>(), 10);
            BES.AddItemInPool(ug_jungle_prv, (short)ModContent.ItemType<UelibloomOre>(), 20);
        }
        private static void ExpandCrimson()
        {
            BES.AddItemInPool(crimson_forest, (short)ModContent.ItemType<BlightedGel>(), 15);
            BES.AddItemInPool(crimson_desert, (short)ModContent.ItemType<BlightedGel>(), 7);
            BES.AddItemInPool(crimson_snow, (short)ModContent.ItemType<BlightedGel>(), 7);
        }
        private static void ExpandCorruption()
        {
            BES.AddItemInPool(corrupt_forest, (short)ModContent.ItemType<BlightedGel>(), 15);
            BES.AddItemInPool(corrupt_desert, (short)ModContent.ItemType<BlightedGel>(), 7);
            BES.AddItemInPool(corrupt_snow, (short)ModContent.ItemType<BlightedGel>(), 7);
        }
        private static void ExpandHallow()
        {
            BES.AddPool(hallowed_forest_ml, 100);
            BES.AddPool(hallowed_desert_ml, 100);
            BES.AddPool(hallowed_snow_ml, 100);
            BES.AddPool(hallowed_forest_ml_remix, 100);
            BES.AddPool(hallowed_desert_ml_remix, 100);
            BES.AddPool(hallowed_snow_ml_remix, 100);

            BES.AddPoolParent(hallowed_forest_ml, hallowed_forest, SubLocalizeAs(post_moon_lord_key));
            BES.AddPoolParent(hallowed_desert_ml, hallowed_desert, SubLocalizeAs(post_moon_lord_key));
            BES.AddPoolParent(hallowed_snow_ml, hallowed_snow, SubLocalizeAs(post_moon_lord_key));
            BES.AddPoolParent(hallowed_forest_ml_remix, hallowed_forest_remix, SubLocalizeAs(post_moon_lord_key));
            BES.AddPoolParent(hallowed_desert_ml_remix, hallowed_desert_remix, SubLocalizeAs(post_moon_lord_key));
            BES.AddPoolParent(hallowed_snow_ml_remix, hallowed_snow_remix, SubLocalizeAs(post_moon_lord_key));

            BES.AddPoolVisibilityRequirements(hallowed_forest_ml, ethereal);
            BES.AddPoolVisibilityRequirements(hallowed_desert_ml, ethereal);
            BES.AddPoolVisibilityRequirements(hallowed_snow_ml, ethereal);
            BES.AddPoolVisibilityRequirements(hallowed_forest_ml_remix, ethereal);
            BES.AddPoolVisibilityRequirements(hallowed_desert_ml_remix, ethereal);
            BES.AddPoolVisibilityRequirements(hallowed_snow_ml_remix, ethereal);

            BES.AddPoolAccessRequirements(hallowed_forest_ml, post_moon_lord);
            BES.AddPoolAccessRequirements(hallowed_desert_ml, post_moon_lord);
            BES.AddPoolAccessRequirements(hallowed_snow_ml, post_moon_lord);
            BES.AddPoolAccessRequirements(hallowed_forest_ml_remix, post_moon_lord);
            BES.AddPoolAccessRequirements(hallowed_desert_ml_remix, post_moon_lord);
            BES.AddPoolAccessRequirements(hallowed_snow_ml_remix, post_moon_lord);

            BES.AddPoolWorldChecks(hallowed_forest_ml_remix, remix);
            BES.AddPoolWorldChecks(hallowed_desert_ml_remix, remix);
            BES.AddPoolWorldChecks(hallowed_snow_ml_remix, remix);

            BES.AddPoolBiomeChecks(hallowed_snow_ml, hallow125.Invoke(hallowIceBlocks));
            BES.AddPoolBiomeChecks(hallowed_forest_ml, hallow125.Invoke(hallowForestBlocks));
            BES.AddPoolBiomeChecks(hallowed_desert_ml, hallow125.Invoke(hallowSandBlocks));
            BES.AddPoolBiomeChecks(hallowed_snow_ml_remix, cavernLayer, hallow125.Invoke(hallowIceBlocks));
            BES.AddPoolBiomeChecks(hallowed_forest_ml_remix, cavernLayer, hallow125.Invoke(hallowForestBlocks));
            BES.AddPoolBiomeChecks(hallowed_desert_ml_remix, cavernLayer, hallow125.Invoke(hallowSandBlocks));

            BES.AddItemInPool(hallowed_forest_ml, (short)ModContent.ItemType<UnholyEssence>(), 5);
            BES.AddItemInPool(hallowed_desert_ml, (short)ModContent.ItemType<UnholyEssence>(), 5);
            BES.AddItemInPool(hallowed_snow_ml, (short)ModContent.ItemType<UnholyEssence>(), 5);
            BES.AliasItemPool(hallowed_forest_ml_remix, hallowed_forest_ml);
            BES.AliasItemPool(hallowed_desert_ml_remix, hallowed_desert_ml);
            BES.AliasItemPool(hallowed_snow_ml_remix, hallowed_snow_ml);
        }

        private static void SubstituteHallowOre()
        {
            BES.RemovePool(hallowed_bars_forest_remix);
            BES.RemovePool(hallowed_bars_desert_remix);
            BES.RemovePool(hallowed_bars_snow_remix);
            BES.RemovePool(hallowed_bars_forest);
            BES.RemovePool(hallowed_bars_desert);
            BES.RemovePool(hallowed_bars_snow);
            BES.RemovePool(ug_hallowed_bars_remix);
            BES.RemovePool(ug_hallowed_bars_desert_remix);
            BES.RemovePool(ug_hallowed_bars_snow_remix);
            BES.RemovePool(ug_hallowed_bars);
            BES.RemovePool(ug_hallowed_bars_desert);
            BES.RemovePool(ug_hallowed_bars_snow);

            BES.AddPool(ug_hallowed_ore, 1100);
            BES.AddPool(ug_hallowed_ore_desert, 1100);
            BES.AddPool(ug_hallowed_ore_snow, 1100);
            BES.AddPool(ug_hallowed_ore_remix, 1100);
            BES.AddPool(ug_hallowed_ore_desert_remix, 1100);
            BES.AddPool(ug_hallowed_ore_snow_remix, 1100);

            BES.AddPoolParent(ug_hallowed_ore, ug_hallowed, SubLocalizeAs(post_mechs));
            BES.AddPoolParent(ug_hallowed_ore_desert, ug_hallowed_desert, SubLocalizeAs(post_mechs));
            BES.AddPoolParent(ug_hallowed_ore_snow, ug_hallowed_snow, SubLocalizeAs(post_mechs));
            BES.AddPoolParent(ug_hallowed_ore_remix, ug_hallowed_caverns_remix, SubLocalizeAs(post_mechs));
            BES.AddPoolParent(ug_hallowed_ore_desert_remix, ug_hallowed_desert_remix, SubLocalizeAs(post_mechs));
            BES.AddPoolParent(ug_hallowed_ore_snow_remix, ug_hallowed_snow_remix, SubLocalizeAs(post_mechs));

            BES.AddPoolVisibilityRequirements(ug_hallowed_ore, steampunk);
            BES.AddPoolVisibilityRequirements(ug_hallowed_ore_desert, steampunk);
            BES.AddPoolVisibilityRequirements(ug_hallowed_ore_snow, steampunk);
            BES.AddPoolVisibilityRequirements(ug_hallowed_ore_remix, steampunk);
            BES.AddPoolVisibilityRequirements(ug_hallowed_ore_desert_remix, steampunk);
            BES.AddPoolVisibilityRequirements(ug_hallowed_ore_snow_remix, steampunk);

            BES.AddPoolAccessRequirements(ug_hallowed_ore, postMechs);
            BES.AddPoolAccessRequirements(ug_hallowed_ore_desert, postMechs);
            BES.AddPoolAccessRequirements(ug_hallowed_ore_snow, postMechs);
            BES.AddPoolAccessRequirements(ug_hallowed_ore_remix, postMechs);
            BES.AddPoolAccessRequirements(ug_hallowed_ore_desert_remix, postMechs);
            BES.AddPoolAccessRequirements(ug_hallowed_ore_snow_remix, postMechs);

            BES.AddPoolWorldChecks(ug_hallowed_ore, notremix);
            BES.AddPoolWorldChecks(ug_hallowed_ore_desert, notremix);
            BES.AddPoolWorldChecks(ug_hallowed_ore_snow, notremix);
            BES.AddPoolWorldChecks(ug_hallowed_ore_remix, remix);
            BES.AddPoolWorldChecks(ug_hallowed_ore_desert_remix, remix);
            BES.AddPoolWorldChecks(ug_hallowed_ore_snow_remix, remix);

            BES.AddPoolBiomeChecks(ug_hallowed_ore, cavernLayer, hallow125.Invoke(hallowForestBlocks));
            BES.AddPoolBiomeChecks(ug_hallowed_ore_desert, cavernLayer, hallow125.Invoke(hallowSandBlocks));
            BES.AddPoolBiomeChecks(ug_hallowed_ore_snow, cavernLayer, hallow125.Invoke(hallowIceBlocks));
            BES.AddPoolBiomeChecks(ug_hallowed_ore_remix, belowSurfaceLayer, notCavernLayer, hallow125.Invoke(hallowForestBlocks), remix);
            BES.AddPoolBiomeChecks(ug_hallowed_ore_desert_remix, belowSurfaceLayer, notCavernLayer, hallow125.Invoke(hallowSandBlocks), remix);
            BES.AddPoolBiomeChecks(ug_hallowed_ore_snow_remix, belowSurfaceLayer, notCavernLayer, hallow125.Invoke(hallowIceBlocks), remix);
            
            BES.AddItemInPool(ug_hallowed_ore, (short)ModContent.ItemType<HallowedOre>(), 12);
            BES.AddItemInPool(ug_hallowed_ore_desert, (short)ModContent.ItemType<HallowedOre>(), 12);
            BES.AddItemInPool(ug_hallowed_ore_snow, (short)ModContent.ItemType<HallowedOre>(), 12);
            BES.AliasItemPool(ug_hallowed_ore_remix, ug_hallowed_ore);
            BES.AliasItemPool(ug_hallowed_ore_desert_remix, ug_hallowed_ore_desert);
            BES.AliasItemPool(ug_hallowed_ore_snow_remix, ug_hallowed_ore_snow);
        }

        private static void ExpandGraveyard()
        {
            BES.AddPool(graveyard_infernal, 500);
            BES.AddPool(graveyard_cal_clone, 500);
            BES.AddPool(graveyard_dog, 500);
            BES.AddPool(graveyard_dog_cold, 500);
            BES.AddPool(graveyard_dog_evil, 500);

            BES.AddPoolParent(graveyard_infernal, graveyard);
            BES.AddPoolParent(graveyard_cal_clone, graveyard, SubLocalizeAs(forest_post_cal_clone_key));
            BES.AddPoolParent(graveyard_dog, graveyard, SubLocalizeAs(forest_post_dog_key));
            BES.AddPoolParent(graveyard_dog_cold, graveyard, SubLocalizeAs(cold_post_dog_key));
            BES.AddPoolParent(graveyard_dog_evil, graveyard, SubLocalizeAs(evil_post_dog_key));

            BES.AddPoolVisibilityRequirements(graveyard_infernal, demonic);
            BES.AddPoolVisibilityRequirements(graveyard_cal_clone, steampunk);
            BES.AddPoolVisibilityRequirements(graveyard_dog, spectral);
            BES.AddPoolVisibilityRequirements(graveyard_dog_cold, spectral);
            BES.AddPoolVisibilityRequirements(graveyard_dog_evil, spectral);

            BES.AddPoolAccessRequirements(graveyard_cal_clone, post_cal_clone);
            BES.AddPoolAccessRequirements(graveyard_dog, post_dog);
            BES.AddPoolAccessRequirements(graveyard_dog_cold, post_dog);
            BES.AddPoolAccessRequirements(graveyard_dog_evil, post_dog);

            BES.AddPoolBiomeChecks(graveyard_infernal, surfaceLayer, tombstone5);
            BES.AddPoolBiomeChecks(graveyard_cal_clone, grass100, surfaceLayer, tombstone5);
            BES.AddPoolBiomeChecks(graveyard_dog, grass100, surfaceLayer, tombstone5);
            BES.AddPoolBiomeChecks(graveyard_dog_cold, frost1500, surfaceLayer, tombstone5);
            BES.AddPoolBiomeChecks(graveyard_dog_evil, anyEvil300, surfaceLayer, tombstone5);

            BES.AddItemInPool(graveyard_infernal, (short)ModContent.ItemType<BloodOrb>(), 5);
            BES.AddItemInPool(graveyard_cal_clone, (short)ModContent.ItemType<SolarVeil>(), 5);
            BES.AddItemInPool(graveyard_dog, (short)ModContent.ItemType<DarksunFragment>(), 50);
            BES.AddItemInPool(graveyard_dog_cold, (short)ModContent.ItemType<EndothermicEnergy>(), 50);
            BES.AddItemInPool(graveyard_dog_evil, (short)ModContent.ItemType<NightmareFuel>(), 50);
        }
        private static void ExpandDungeon()
        {
            BES.AddPool(dungeon_ml, 2000);

            BES.AddPoolParent(dungeon_ml, dungeon, SubLocalizeAs(post_moon_lord_key));

            BES.AddPoolVisibilityRequirements(dungeon_ml, ethereal);
            BES.AddPoolAccessRequirements(dungeon_ml, post_moon_lord);
            BES.AddPoolBiomeChecks(dungeon_ml, dungeon250, belowSurfaceLayer, dungeon_bg);

            BES.AddItemInPool(dungeon_ml, (short)ModContent.ItemType<Necroplasm>(), 5);
        }
        private static void ExpandSpace()
        {
            BES.AddPool(space_evil2, 4000);
            BES.AddPool(space_hm, 4000);
            BES.AddPool(space_ml, 4000);
            BES.AddPool(space_exo, 4000);

            BES.AddPoolParent(space_evil2, space, SubLocalizeAs(post_evil2_key));
            BES.AddPoolParent(space_hm, space, SubLocalizeAs(in_hardmode));
            BES.AddPoolParent(space_ml, space, SubLocalizeAs(post_moon_lord_key));
            BES.AddPoolParent(space_exo, space, SubLocalizeAs(post_exo_key));

            BES.AddPoolVisibilityRequirements(space_evil2, infernal);
            BES.AddPoolVisibilityRequirements(space_hm, steampunk);
            BES.AddPoolVisibilityRequirements(space_ml, ethereal);
            BES.AddPoolVisibilityRequirements(space_exo, exo);

            BES.AddPoolAccessRequirements(space_evil2, post_evil2);
            BES.AddPoolAccessRequirements(space_hm, hardmodeOnly);
            BES.AddPoolAccessRequirements(space_ml, post_moon_lord);
            BES.AddPoolAccessRequirements(space_exo, post_exo_mechs);

            BES.AddPoolBiomeChecks(space_evil2, spaceLayer);
            BES.AddPoolBiomeChecks(space_hm, spaceLayer);
            BES.AddPoolBiomeChecks(space_ml, spaceLayer);
            BES.AddPoolBiomeChecks(space_exo, spaceLayer);

            BES.AddItemInPool(space_evil2, (short)ModContent.ItemType<AerialiteOre>(), 5);
            BES.AddItemInPool(space_hm, (short)ModContent.ItemType<EssenceofSunlight>(), 2);
            BES.AddItemInPool(pillar, (short)ModContent.ItemType<MeldBlob>(), 8);
            BES.AddItemInPool(space_ml, new ItemEntry((short)ModContent.ItemType<ExodiumCluster>(), 1, 3), 5);
            BES.AddItemInPool(space_exo, (short)ModContent.ItemType<ExoPrism>(), new Fraction(1, 2));
        }
        private static void ExpandUnderworld()
        {
            BES.AddPool(underworld_ml, 4000);

            BES.AddPoolParent(underworld_ml, underworld, SubLocalizeAs(post_moon_lord_key));

            BES.AddPoolVisibilityRequirements(underworld_ml, ethereal);
            BES.AddPoolAccessRequirements(underworld_ml, post_moon_lord);
            BES.AddPoolBiomeChecks(underworld_ml, underworldLayer);

            BES.AddItemInPool(underworld_ml, (short)ModContent.ItemType<UnholyEssence>(), 5);
        }

        private static void SetupAstralForest()
        {
            BES.AddPool(astral_forest, 350, LocalizeAs(astral_forest));
            BES.AddPool(astral_ore_forest, 350);
            BES.AddPool(ug_astral_forest, 1050, LocalizeAs(ug_astral_forest));
            BES.AddPool(ug_astral_ore_forest, 1050);

            BES.AddPoolParent(astral_ore_forest, astral_forest, SubLocalizeAs(post_deus_key));
            BES.AddPoolParent(ug_astral_ore_forest, ug_astral_forest, SubLocalizeAs(post_deus_key));

            BES.AddPoolVisibilityRequirements(astral_forest, steampunk);
            BES.AddPoolVisibilityRequirements(astral_ore_forest, lunar);
            BES.AddPoolVisibilityRequirements(ug_astral_forest, steampunk);
            BES.AddPoolVisibilityRequirements(ug_astral_ore_forest, lunar);

            BES.AddPoolAccessRequirements(astral_forest, hardmodeOnly);
            BES.AddPoolAccessRequirements(astral_ore_forest, post_deus);
            BES.AddPoolAccessRequirements(ug_astral_forest, hardmodeOnly);
            BES.AddPoolAccessRequirements(ug_astral_ore_forest, post_deus);

            BES.AddPoolBiomeChecks(astral_forest, astral_forest950);
            BES.AddPoolBiomeChecks(astral_ore_forest, astral_forest950);
            BES.AddPoolBiomeChecks(ug_astral_forest, belowSurfaceLayer, astral_forest950);
            BES.AddPoolBiomeChecks(ug_astral_ore_forest, belowSurfaceLayer, astral_forest950);

            BES.AddItemInPool(astral_forest, ItemID.None, 72);
            BES.AddItemInPool(astral_forest, (short)ModContent.ItemType<AstralDirt>(), 26);
            BES.AddItemInPool(astral_forest, (short)ModContent.ItemType<AstralStone>(), 7);
            BES.AddItemInPool(astral_forest, (short)ModContent.ItemType<AstralClay>(), 8);
            BES.AddItemInPool(astral_forest, (short)ModContent.ItemType<AstralMonolith>(), 50);
            BES.AddItemInPool(astral_forest, (short)ModContent.ItemType<StarblightSoot>(), 36);
            BES.AddItemInPool(astral_forest, (short)ModContent.ItemType<TitanHeart>(), 4);
            BES.AddItemInPool(astral_forest, (short)ModContent.ItemType<AstralGrassSeeds>(), 7);
            BES.AddItemInPool(astral_forest, ItemID.EnchantedNightcrawler, 1);
            BES.AddItemInPool(astral_forest, (short)ModContent.ItemType<TwinklerItem>(), 4);
            BES.AddItemInPool(astral_ore_forest, new ItemEntry((short)ModContent.ItemType<AstralOre>(), 1, 3), 1);

            BES.AddItemInPool(ug_astral_forest, ItemID.None, 70);
            BES.AddItemInPool(ug_astral_forest, (short)ModContent.ItemType<AstralDirt>(), 16);
            BES.AddItemInPool(ug_astral_forest, (short)ModContent.ItemType<AstralStone>(), 8);
            BES.AddItemInPool(ug_astral_forest, (short)ModContent.ItemType<NovaeSlag>(), 4);
            BES.AddItemInPool(ug_astral_forest, (short)ModContent.ItemType<StarblightSoot>(), 27);
            BES.AddItemInPool(ug_astral_forest, (short)ModContent.ItemType<TitanHeart>(), 3);
            BES.AddItemInPool(ug_astral_ore_forest, new ItemEntry((short)ModContent.ItemType<AstralOre>(), 1, 3), 1);
        }
        private static void SetupAstralDesert()
        {
            BES.AddPool(astral_desert, 350, LocalizeAs(astral_desert));
            BES.AddPool(astral_ore_desert, 350);
            BES.AddPool(ug_astral_desert, 1050, LocalizeAs(ug_astral_desert));
            BES.AddPool(ug_astral_ore_desert, 1050);

            BES.AddPoolParent(astral_ore_desert, astral_desert, SubLocalizeAs(post_deus_key));
            BES.AddPoolParent(ug_astral_ore_desert, ug_astral_desert, SubLocalizeAs(post_deus_key));

            BES.AddPoolVisibilityRequirements(astral_desert, steampunk);
            BES.AddPoolVisibilityRequirements(astral_ore_desert, lunar);
            BES.AddPoolVisibilityRequirements(ug_astral_desert, steampunk);
            BES.AddPoolVisibilityRequirements(ug_astral_ore_desert, lunar);

            BES.AddPoolAccessRequirements(astral_desert, hardmodeOnly);
            BES.AddPoolAccessRequirements(astral_ore_desert, post_deus);
            BES.AddPoolAccessRequirements(ug_astral_desert, hardmodeOnly);
            BES.AddPoolAccessRequirements(ug_astral_ore_desert, post_deus);

            BES.AddPoolBiomeChecks(astral_desert, astral_desert951);
            BES.AddPoolBiomeChecks(astral_ore_desert, astral_desert951);
            BES.AddPoolBiomeChecks(ug_astral_desert, belowSurfaceLayer, astral_desert951);
            BES.AddPoolBiomeChecks(ug_astral_ore_desert, belowSurfaceLayer, astral_desert951);

            BES.AddItemInPool(astral_desert, ItemID.None, 31);
            BES.AddItemInPool(astral_desert, (short)ModContent.ItemType<AstralSand>(), 41);
            BES.AddItemInPool(astral_desert, (short)ModContent.ItemType<AstralMonolith>(), 25);
            BES.AddItemInPool(astral_desert, (short)ModContent.ItemType<StarblightSoot>(), 27);
            BES.AddItemInPool(astral_desert, (short)ModContent.ItemType<TitanHeart>(), 3);
            BES.AddItemInPool(astral_desert, ItemID.EnchantedNightcrawler, 2);
            BES.AddItemInPool(astral_desert, (short)ModContent.ItemType<TwinklerItem>(), 5);
            BES.AddItemInPool(astral_ore_desert, new ItemEntry((short)ModContent.ItemType<AstralOre>(), 1, 3), 1);

            BES.AddItemInPool(ug_astral_desert, (short)ModContent.ItemType<AstralSand>(), 6);
            BES.AddItemInPool(ug_astral_desert, (short)ModContent.ItemType<HardenedAstralSand>(), 6);
            BES.AddItemInPool(ug_astral_desert, (short)ModContent.ItemType<AstralSandstone>(), 6);
            BES.AddItemInPool(ug_astral_desert, (short)ModContent.ItemType<CelestialRemains>(), 6);
            BES.AddItemInPool(ug_astral_desert, (short)ModContent.ItemType<StarblightSoot>(), 27);
            BES.AddItemInPool(ug_astral_desert, (short)ModContent.ItemType<TitanHeart>(), 3);
            BES.AddItemInPool(ug_astral_ore_desert, new ItemEntry((short)ModContent.ItemType<AstralOre>(), 1, 3), 1);
        }
        private static void SetupAstralSnow()
        {
            BES.AddPool(astral_snow, 350, LocalizeAs(astral_snow));
            BES.AddPool(astral_ore_snow, 350);
            BES.AddPool(ug_astral_snow, 1050, LocalizeAs(ug_astral_snow));
            BES.AddPool(ug_astral_ore_snow, 1050);

            BES.AddPoolParent(astral_ore_snow, astral_snow, SubLocalizeAs(post_deus_key));
            BES.AddPoolParent(ug_astral_ore_snow, ug_astral_snow, SubLocalizeAs(post_deus_key));

            BES.AddPoolVisibilityRequirements(astral_snow, steampunk);
            BES.AddPoolVisibilityRequirements(astral_ore_snow, lunar);
            BES.AddPoolVisibilityRequirements(ug_astral_snow, steampunk);
            BES.AddPoolVisibilityRequirements(ug_astral_ore_snow, lunar);

            BES.AddPoolAccessRequirements(astral_snow, hardmodeOnly);
            BES.AddPoolAccessRequirements(astral_ore_snow, post_deus);
            BES.AddPoolAccessRequirements(ug_astral_snow, hardmodeOnly);
            BES.AddPoolAccessRequirements(ug_astral_ore_snow, post_deus);

            BES.AddPoolBiomeChecks(astral_snow, astral_snow951);
            BES.AddPoolBiomeChecks(astral_ore_snow, astral_snow951);
            BES.AddPoolBiomeChecks(ug_astral_snow, belowSurfaceLayer, astral_snow951);
            BES.AddPoolBiomeChecks(ug_astral_ore_snow, belowSurfaceLayer, astral_snow951);

            BES.AddItemInPool(astral_snow, ItemID.None, 50);
            BES.AddItemInPool(astral_snow, ItemID.SnowBlock, 27);
            BES.AddItemInPool(astral_snow, (short)ModContent.ItemType<AstralIce>(), 14);
            BES.AddItemInPool(astral_snow, (short)ModContent.ItemType<AstralMonolith>(), 17);
            BES.AddItemInPool(astral_snow, (short)ModContent.ItemType<StarblightSoot>(), 27);
            BES.AddItemInPool(astral_snow, (short)ModContent.ItemType<TitanHeart>(), 3);
            BES.AddItemInPool(astral_snow, ItemID.EnchantedNightcrawler, 2);
            BES.AddItemInPool(astral_snow, (short)ModContent.ItemType<TwinklerItem>(), 6);
            BES.AddItemInPool(astral_ore_snow, new ItemEntry((short)ModContent.ItemType<AstralOre>(), 1, 3), 1);

            BES.AddItemInPool(ug_astral_snow, ItemID.None, 69);
            BES.AddItemInPool(ug_astral_snow, ItemID.SnowBlock, 10);
            BES.AddItemInPool(ug_astral_snow, (short)ModContent.ItemType<AstralIce>(), 10);
            BES.AddItemInPool(ug_astral_snow, (short)ModContent.ItemType<NovaeSlag>(), 4);
            BES.AddItemInPool(ug_astral_snow, (short)ModContent.ItemType<StarblightSoot>(), 27);
            BES.AddItemInPool(ug_astral_snow, (short)ModContent.ItemType<TitanHeart>(), 3);
            BES.AddItemInPool(ug_astral_ore_snow, new ItemEntry((short)ModContent.ItemType<AstralOre>(), 1, 3), 1);
        }
        private static void SetupSunkenSea()
        {
            BES.AddPool(sunken_sea, 1050, LocalizeAs(sunken_sea));
            BES.AddPool(sunken_sea_hm, 1050);

            BES.AddPoolParent(sunken_sea_hm, sunken_sea, SubLocalizeAs(in_hardmode));

            BES.AddPoolVisibilityRequirements(sunken_sea_hm, infernal);
            BES.AddPoolAccessRequirements(sunken_sea_hm, hardmodeOnly);

            BES.AddPoolBiomeChecks(sunken_sea, sunken_sea150);
            BES.AddPoolBiomeChecks(sunken_sea_hm, sunken_sea150);

            BES.AddItemInPool(sunken_sea, ItemID.None, 20);
            BES.AddItemInPool(sunken_sea, (short)ModContent.ItemType<EutrophicSand>(), 11);
            BES.AddItemInPool(sunken_sea, (short)ModContent.ItemType<Navystone>(), 7);
            BES.AddItemInPool(sunken_sea, (short)ModContent.ItemType<SeaPrism>(), 13);
            BES.AddItemInPool(sunken_sea, (short)ModContent.ItemType<PrismShard>(), 13);
            BES.AddItemInPool(sunken_sea, ItemID.WhitePearl, 5);
            BES.AddItemInPool(sunken_sea, ItemID.BlackPearl, new Fraction(3,2));
            BES.AddItemInPool(sunken_sea, ItemID.PinkPearl,  new Fraction(1,2));
            BES.AddItemInPool(sunken_sea, (short)ModContent.ItemType<BabyGhostBellItem>(), 3);
            BES.AddItemInPool(sunken_sea, (short)ModContent.ItemType<SeaMinnowItem>(), 4);
            BES.AddItemInPool(sunken_sea_hm, (short)ModContent.ItemType<MolluskHusk>(), 5);
        }
        private static void SetupSulphurousSea()
        {
            BES.AddPool(sulphur_sea, 2600, LocalizeAs(sulphur_sea));
            BES.AddPool(acid_rain, 2600);
            BES.AddPool(sulphur_sea_hm, 2600);
            BES.AddPool(sulphur_sea_as, 2600);
            BES.AddPool(acid_rain_as, 2600);
            BES.AddPool(sulphur_sea_ml, 2600);

            BES.AddPoolParent(astral_ore_desert, sulphur_sea, SubLocalizeAs(post_acid_rain_key));
            BES.AddPoolParent(sulphur_sea_hm, sulphur_sea, SubLocalizeAs(in_hardmode));
            BES.AddPoolParent(sulphur_sea_as, sulphur_sea, SubLocalizeAs(post_aq_scourge_key));
            BES.AddPoolParent(acid_rain_as, sulphur_sea, SubLocalizeAs(post_acid_rain2_key));
            BES.AddPoolParent(sulphur_sea_ml, sulphur_sea, SubLocalizeAs(post_moon_lord_key));

            BES.AddPoolVisibilityRequirements(sulphur_sea, demonic);
            BES.AddPoolVisibilityRequirements(acid_rain, demonic);
            BES.AddPoolVisibilityRequirements(sulphur_sea_hm, infernal);
            BES.AddPoolVisibilityRequirements(sulphur_sea_as, steampunk);
            BES.AddPoolVisibilityRequirements(acid_rain_as, steampunk);
            BES.AddPoolVisibilityRequirements(sulphur_sea_ml, ethereal);

            BES.AddPoolAccessRequirements(acid_rain, acid_rain_finished);
            BES.AddPoolAccessRequirements(sulphur_sea_hm, hardmodeOnly);
            BES.AddPoolAccessRequirements(sulphur_sea_as, post_scourge2);
            BES.AddPoolAccessRequirements(acid_rain_as, acid_rain2_finished);
            BES.AddPoolAccessRequirements(sulphur_sea_ml, post_moon_lord);

            BES.AddPoolBiomeChecks(sulphur_sea, in_sulphur_sea);
            BES.AddPoolBiomeChecks(acid_rain, in_sulphur_sea);
            BES.AddPoolBiomeChecks(sulphur_sea_hm, in_sulphur_sea);
            BES.AddPoolBiomeChecks(sulphur_sea_as, in_sulphur_sea);
            BES.AddPoolBiomeChecks(acid_rain_as, in_sulphur_sea);
            BES.AddPoolBiomeChecks(sulphur_sea_ml, in_sulphur_sea);

            BES.AddItemInPool(sulphur_sea, ItemID.None, 90);
            BES.AddItemInPool(sulphur_sea, (short)ModContent.ItemType<SulphurousSand>(), 12);
            BES.AddItemInPool(sulphur_sea, (short)ModContent.ItemType<SulphurousSandstone>(), 6);
            BES.AddItemInPool(sulphur_sea, (short)ModContent.ItemType<HardenedSulphurousSandstone>(), 4);
            BES.AddItemInPool(sulphur_sea, ItemID.Coral, 5);
            BES.AddItemInPool(sulphur_sea, ItemID.Seashell, 4);
            BES.AddItemInPool(sulphur_sea, ItemID.Starfish, 3);
            BES.AddItemInPool(sulphur_sea, (short)ModContent.ItemType<Acidwood>(), 20);
            BES.AddItemInPool(acid_rain, (short)ModContent.ItemType<SulphuricScale>(), 8);
            BES.AddItemInPool(sulphur_sea_hm, ItemID.TurtleShell, 5);
            BES.AddItemInPool(sulphur_sea_as, (short)ModContent.ItemType<BabyFlakCrabItem>(), 6);
            BES.AddItemInPool(acid_rain_as, (short)ModContent.ItemType<CorrodedFossil>(), 6);
            BES.AddItemInPool(sulphur_sea_ml, (short)ModContent.ItemType<BloodwormItem>(), 1);
        }
        private static void SetupCrags()
        {
            BES.AddPool(brimstone_crag, 4100, LocalizeAs(brimstone_crag));
            BES.AddPool(brimstone_crag_hm, 4100);
            BES.AddPool(brimstone_crag_mch, 4100);
            BES.AddPool(brimstone_crag_prv, 4100);
            BES.AddPool(brimstone_crag_swc, 4100);

            BES.AddPoolParent(brimstone_crag_hm, brimstone_crag, SubLocalizeAs(in_hardmode));
            BES.AddPoolParent(brimstone_crag_mch, brimstone_crag); //only requires steampunk tier
            BES.AddPoolParent(brimstone_crag_prv, brimstone_crag, SubLocalizeAs(post_providence_key));
            BES.AddPoolParent(brimstone_crag_swc, brimstone_crag, SubLocalizeAs(post_swc_key));

            BES.AddPoolVisibilityRequirements(brimstone_crag, infernal);
            BES.AddPoolVisibilityRequirements(brimstone_crag_hm, infernal);
            BES.AddPoolVisibilityRequirements(brimstone_crag_mch, steampunk);
            BES.AddPoolVisibilityRequirements(brimstone_crag_prv, ethereal);
            BES.AddPoolVisibilityRequirements(brimstone_crag_swc, exo);

            BES.AddPoolAccessRequirements(brimstone_crag_hm, hardmodeOnly);
            BES.AddPoolAccessRequirements(brimstone_crag_prv, post_providence);
            BES.AddPoolAccessRequirements(brimstone_crag_swc, post_calamitas);

            BES.AddPoolBiomeChecks(brimstone_crag, underworldLayer, brimstone100);
            BES.AddPoolBiomeChecks(brimstone_crag_hm, underworldLayer, brimstone100);
            BES.AddPoolBiomeChecks(brimstone_crag_mch, underworldLayer, brimstone100);
            BES.AddPoolBiomeChecks(brimstone_crag_prv, underworldLayer, brimstone100);
            BES.AddPoolBiomeChecks(brimstone_crag_swc, underworldLayer, brimstone100);

            BES.AddItemInPool(brimstone_crag, ItemID.None, 45);
            BES.AddItemInPool(brimstone_crag, (short)ModContent.ItemType<BrimstoneSlag>(), 10);
            BES.AddItemInPool(brimstone_crag, (short)ModContent.ItemType<ScorchedRemains>(), 6);
            BES.AddItemInPool(brimstone_crag, ItemID.Lens, 3);
            BES.AddItemInPool(brimstone_crag, (short)ModContent.ItemType<SpineSapling>(), 5);
            BES.AddItemInPool(brimstone_crag_hm, (short)ModContent.ItemType<EssenceofHavoc>(), 6);
            BES.AddItemInPool(brimstone_crag_mch, (short)ModContent.ItemType<InfernalSuevite>(), 6);
            BES.AddItemInPool(brimstone_crag_prv, (short)ModContent.ItemType<Bloodstone>(), 5);
            BES.AddItemInPool(brimstone_crag_swc, (short)ModContent.ItemType<AshesofAnnihilation>(), new Fraction(1, 2));
        }
        private static void SetupAbyss()
        {
            SetupSulphurousDepths();
            SetupMurkyWaters();
            SetupThermalVents();
            SetupTheVoid();
        }
        private static void SetupSulphurousDepths()
        {
            BES.AddPool(sulphur_depths, 25000, LocalizeAs(sulphur_depths));
            BES.AddPool(sulphur_depths_lev, 25000);

            BES.AddPoolParent(sulphur_depths_lev, sulphur_depths, SubLocalizeAs(post_leviathan_key));

            BES.AddPoolVisibilityRequirements(sulphur_depths, demonic);
            BES.AddPoolVisibilityRequirements(sulphur_depths_lev, demonic);

            BES.AddPoolAccessRequirements(sulphur_depths, sulphuric_extractor);
            BES.AddPoolAccessRequirements(sulphur_depths_lev, sulphuric_extractor, post_leviathan);

            BES.AddPoolBiomeChecks(sulphur_depths, abyss_area);
            BES.AddPoolBiomeChecks(sulphur_depths_lev, abyss_area);

            BES.AddItemInPool(sulphur_depths, ItemID.None, 51);
            BES.AddItemInPool(sulphur_depths, (short)ModContent.ItemType<SulphurousShale>(), 18);
            BES.AddItemInPool(sulphur_depths, (short)ModContent.ItemType<BabyCannonballJellyfishItem>(), 7);
            BES.AddItemInPool(sulphur_depths_lev, (short)ModContent.ItemType<DepthCells>(), 1);
        }
        private static void SetupMurkyWaters()
        {
            BES.AddPool(murky_waters, 25001, LocalizeAs(murky_waters));
            BES.AddPool(murky_waters_lev, 25001);
            BES.AddPool(murky_waters_glm, 25001);

            BES.AddPoolParent(murky_waters_lev, murky_waters, SubLocalizeAs(post_leviathan_key));

            BES.AddPoolVisibilityRequirements(murky_waters, infernal);
            BES.AddPoolVisibilityRequirements(murky_waters_lev, infernal);
            BES.AddPoolVisibilityRequirements(murky_waters_glm, cyber);

            BES.AddPoolAccessRequirements(murky_waters, pressurized_extractor);
            BES.AddPoolAccessRequirements(murky_waters_lev, pressurized_extractor, post_leviathan);
            BES.AddPoolAccessRequirements(murky_waters_glm, thermal_extractor);

            BES.AddPoolBiomeChecks(murky_waters, abyss_area, abyss_gravel300, more_than_tiles.Invoke(abyss_gravel, sulphur_depths_blocks));
            BES.AddPoolBiomeChecks(murky_waters_lev, abyss_area, abyss_gravel300, more_than_tiles.Invoke(abyss_gravel, sulphur_depths_blocks));
            BES.AddPoolBiomeChecks(murky_waters_glm, abyss_area, abyss_gravel300, more_than_tiles.Invoke(abyss_gravel, sulphur_depths_blocks));

            BES.AddItemInPool(murky_waters, ItemID.None, 62);
            BES.AddItemInPool(murky_waters, (short)ModContent.ItemType<AbyssGravel>(), 26);
            BES.AddItemInPool(murky_waters, (short)ModContent.ItemType<PlantyMush>(), 10);
            BES.AddItemInPool(murky_waters, (short)ModContent.ItemType<Voidstone>(), 2);
            BES.AddItemInPool(murky_waters, ItemID.WhitePearl, 3);
            BES.AddItemInPool(murky_waters, ItemID.PinkPearl, 1);
            BES.AddItemInPool(murky_waters_lev, (short)ModContent.ItemType<Lumenyl>(), 20);
            BES.AddItemInPool(murky_waters_lev, (short)ModContent.ItemType<DepthCells>(), 28);
            BES.AddItemInPool(murky_waters_glm, (short)ModContent.ItemType<ScoriaOre>(), 12);
        }
        private static void SetupThermalVents()
        {
            BES.AddPool(thermal_vents, 25002, LocalizeAs(thermal_vents));
            BES.AddPool(thermal_vents_pla, 25002);
            BES.AddPool(thermal_vents_lev, 25002);
            BES.AddPool(thermal_vents_glm, 25002);

            BES.AddPoolParent(thermal_vents_pla, thermal_vents, SubLocalizeAs(post_plantera_key));
            BES.AddPoolParent(murky_waters_lev, thermal_vents, SubLocalizeAs(post_leviathan_key));
            BES.AddPoolParent(thermal_vents_glm, thermal_vents, SubLocalizeAs(post_golem_key));

            BES.AddPoolVisibilityRequirements(thermal_vents, cyber);
            BES.AddPoolVisibilityRequirements(thermal_vents_pla, cyber);
            BES.AddPoolVisibilityRequirements(thermal_vents_lev, cyber);
            BES.AddPoolVisibilityRequirements(thermal_vents_glm, cyber);

            BES.AddPoolAccessRequirements(thermal_vents, thermal_extractor);
            BES.AddPoolAccessRequirements(thermal_vents_pla, thermal_extractor, post_plantera);
            BES.AddPoolAccessRequirements(thermal_vents_lev, thermal_extractor, post_leviathan);
            BES.AddPoolAccessRequirements(thermal_vents_glm, thermal_extractor, post_golem);

            BES.AddPoolBiomeChecks(thermal_vents, abyss_area, thermal_blocks300, more_than_tiles.Invoke(thermal_blocks, abyss_gravel));
            BES.AddPoolBiomeChecks(thermal_vents_pla, abyss_area, thermal_blocks300, more_than_tiles.Invoke(thermal_blocks, abyss_gravel));
            BES.AddPoolBiomeChecks(thermal_vents_lev, abyss_area, thermal_blocks300, more_than_tiles.Invoke(thermal_blocks, abyss_gravel));
            BES.AddPoolBiomeChecks(thermal_vents_glm, abyss_area, thermal_blocks300, more_than_tiles.Invoke(thermal_blocks, abyss_gravel));

            BES.AddItemInPool(thermal_vents, ItemID.None, 26);
            BES.AddItemInPool(thermal_vents, (short)ModContent.ItemType<PyreMantle>(), 13);
            BES.AddItemInPool(thermal_vents, (short)ModContent.ItemType<PyreMantleMolten>(), 5);
            BES.AddItemInPool(thermal_vents, (short)ModContent.ItemType<ScoriaOre>(), 5);
            BES.AddItemInPool(thermal_vents, ItemID.BlackInk, 1);
            BES.AddItemInPool(thermal_vents_pla, ItemID.Ectoplasm, 2);
            BES.AddItemInPool(thermal_vents_lev, (short)ModContent.ItemType<Lumenyl>(), 13);
            BES.AddItemInPool(thermal_vents_lev, (short)ModContent.ItemType<DepthCells>(), 15);
            BES.AddItemInPool(thermal_vents_glm, (short)ModContent.ItemType<ScoriaOre>(), 5);
        }
        private static void SetupTheVoid()
        {
            BES.AddPool(the_void, 25003, LocalizeAs(the_void));
            BES.AddPool(the_void_pla, 25003);
            BES.AddPool(the_void_lev, 25003);
            BES.AddPool(the_void_pgh, 25003);

            BES.AddPoolParent(the_void_pla, the_void, SubLocalizeAs(post_plantera_key));
            BES.AddPoolParent(the_void_lev, the_void, SubLocalizeAs(post_leviathan_key));
            BES.AddPoolParent(the_void_pgh, the_void, SubLocalizeAs(post_polterghast_key));

            BES.AddPoolVisibilityRequirements(the_void, spectral);
            BES.AddPoolVisibilityRequirements(the_void_pla, spectral);
            BES.AddPoolVisibilityRequirements(the_void_lev, spectral);
            BES.AddPoolVisibilityRequirements(the_void_pgh, spectral);

            BES.AddPoolAccessRequirements(the_void, abyssal_extractor);
            BES.AddPoolAccessRequirements(the_void_pla, abyssal_extractor, post_plantera);
            BES.AddPoolAccessRequirements(the_void_lev, abyssal_extractor, post_leviathan);
            BES.AddPoolAccessRequirements(the_void_pgh, abyssal_extractor, post_polterghast);

            BES.AddPoolBiomeChecks(the_void, abyss_area, voidstone300, more_than_tiles.Invoke(voidstone, thermal_blocks));
            BES.AddPoolBiomeChecks(the_void_pla, abyss_area, voidstone300, more_than_tiles.Invoke(voidstone, thermal_blocks));
            BES.AddPoolBiomeChecks(the_void_lev, abyss_area, voidstone300, more_than_tiles.Invoke(voidstone, thermal_blocks));
            BES.AddPoolBiomeChecks(the_void_pgh, abyss_area, voidstone300, more_than_tiles.Invoke(voidstone, thermal_blocks));

            BES.AddItemInPool(the_void, ItemID.None, 57);
            BES.AddItemInPool(the_void, (short)ModContent.ItemType<Voidstone>(), 52);
            BES.AddItemInPool(the_void, ItemID.BlackInk, 3);
            BES.AddItemInPool(the_void_pla, ItemID.Ectoplasm, 10);
            BES.AddItemInPool(the_void_lev, (short)ModContent.ItemType<Lumenyl>(), 18);
            BES.AddItemInPool(the_void_lev, (short)ModContent.ItemType<DepthCells>(), 28);
            BES.AddItemInPool(the_void_pgh, (short)ModContent.ItemType<ReaperTooth>(), 3);
        }
        #endregion
    }
}
