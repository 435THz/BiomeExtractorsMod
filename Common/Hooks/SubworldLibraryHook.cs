
using System;
using SubworldLibrary;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Common.Hooks
{
    public abstract class SubworldLibraryHook
    {
        [JITWhenModsEnabled("SubworldLibrary")]
        public static bool IsInMainWorld => !SubworldSystem.AnyActive();

        [JITWhenModsEnabled("SubworldLibrary")]
        public static bool IsInSubworld(string id) => String.IsNullOrEmpty(id) ? IsInMainWorld : SubworldSystem.IsActive(id);
    }
}