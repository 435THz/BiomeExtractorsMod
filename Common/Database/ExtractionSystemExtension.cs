using Terraria.ModLoader;

namespace BiomeExtractorsMod.Common.Database
{
    internal abstract class ExtractionSystemExtension : ModSystem
    {
        protected static BiomeExtractionSystem BES => BiomeExtractionSystem.Instance;
        public abstract void LoadDatabase();
    }
}
