using Terraria.ModLoader;

namespace BiomeExtractorsMod.Common.Database
{
    internal class WeakReferenceLoader : ModSystem
    {
        internal static void LoadWeakReferences()
        {
            if(ModContent.TryFind("BiomeExtractorsMod", "CalamityExtractionSystem", out ExtractionSystemExtension calamity))
                calamity.LoadDatabase();
        }
    }
}
