using Terraria.ModLoader;

namespace BiomeExtractorsMod
{
    public class BiomeExtractorsMod : Mod
	{
        static internal bool MS_loaded;

        private static readonly object LocBase = "Mods.BiomeExtractorsMod";

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
            MS_loaded = ModLoader.HasMod("MagicStorage");
        }
    }
}