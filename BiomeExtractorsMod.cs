using BiomeExtractorsMod.Common.Configs;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace BiomeExtractorsMod
{
    public class BiomeExtractorsMod : Mod
	{
        static string GetModName(SupportedMods mod) => Enum.GetName(typeof(SupportedMods), mod);
        enum SupportedMods
        {
            MagicStorage
        }

        static readonly private Dictionary<SupportedMods, bool> supportedModsLoaded = [];
        static internal bool MS_loaded => supportedModsLoaded[SupportedMods.MagicStorage] && ModContent.GetInstance<ConfigCompat>().MS;

        private static readonly object LocBase = "Mods.BiomeExtractorsMod";

        private static string LocItemsCategory => $"{LocBase}.Items";
        public static string LocExtractorPrefix => $"{LocItemsCategory}.BiomeExtractorItem";
        private static string LocConfigCategory => $"{LocBase}.Config";
        public static string LocClientConfig => $"{LocConfigCategory}.ConfigClient";
        public static string LocCommonConfig => $"{LocConfigCategory}.ConfigCommon";
        public static string LocCompatConfig => $"{LocConfigCategory}.ConfigCompat";

        private static string LocTextCategory => $"{LocBase}.Text";
        public static string LocDiagnostics => $"{LocTextCategory}.Diagnostics";
        public static string LocPoolNames => $"{LocTextCategory}.PoolNames";
        public static object LocItemGroups => $"{LocTextCategory}.ItemGroups";

        public override void Load()
        {
            foreach(SupportedMods mod in Enum.GetValues(typeof(SupportedMods)))
            supportedModsLoaded[mod] = ModLoader.HasMod(GetModName(mod));
        }
    }
}