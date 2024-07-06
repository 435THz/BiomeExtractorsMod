using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Content.TileEntities;
using BiomeExtractorsMod.Content.Tiles;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria.ID;
using Terraria;
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
        private static string LocMapCategory => $"{LocBase}.Map";
        public static string LocMapTileName => $"{LocMapCategory}.TileName";
        public static string LocIconInactive => $"{LocMapCategory}.IconInactive";
        public static string LocIconNoOutput => $"{LocMapCategory}.IconNoOutput";

        public override void Load()
        {
            foreach(SupportedMods mod in Enum.GetValues(typeof(SupportedMods)))
            supportedModsLoaded[mod] = ModLoader.HasMod(GetModName(mod));
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                short X = reader.ReadInt16();
                short Y = reader.ReadInt16();
                bool active = reader.ReadBoolean();
                if (TileUtils.TryGetTileEntityAs(X, Y, out BiomeExtractorEnt extractor))
                    extractor.Active = active;
            }
        }
    }
}