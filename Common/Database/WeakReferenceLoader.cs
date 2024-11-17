using BiomeExtractorsMod.Calamity.Common.Database;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Common.Database
{
    internal class WeakReferenceLoader : ModSystem
    {
        internal static void LoadWeakReferences()
        {
            if(ModLoader.HasMod("CalamityMod"))
                CalamityExtractionSystem.Instance.LoadDatabase();
        }
    }
}
