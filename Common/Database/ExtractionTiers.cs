using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiomeExtractorsMod.Common.Database
{
    /// <summary>
    /// A list of all Tier values used by the mod. They have huge gaps in between each other for addons to fit in.
    /// </summary>
    internal static class ExtractionTiers
    {
        public static readonly int BASIC = 1;
        public static readonly int DEMONIC = 1000;
        public static readonly int INFERNAL = 2000;
        public static readonly int STEAMPUNK = 3000;
        public static readonly int CYBER = 4000;
        public static readonly int LUNAR = 5000;
        public static readonly int ETHEREAL = 6000;
        public static readonly int SPECTRAL = 7000;
        public static readonly int AURIC = 8000;
        public static readonly int EXO = 9000;
    }
}
