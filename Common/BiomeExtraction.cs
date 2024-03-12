using System.Collections.Generic;

namespace BiomeExtractorsMod
{
    public static class BiomeExtraction
    {
        private static List<int> ExtractorCooldownTable = [];

        internal static void PopulateExtractionSpeedTable()
        {
            //TODO populate
        }

        public static int GetExtractionSpeed(int v)
        {
            return ExtractorCooldownTable[v];
        }
    }
}