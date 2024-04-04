using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ModLoader.Config;

namespace BiomeExtractorsMod.Common.Configs
{
    public class ExtractorClient : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [LabelKey("$Mods.BiomeExtractorsMod.Configs.Client.DiagPrintTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.Client.DiagPrintTooltip")]
        [DefaultValue(false)]
        public bool DiagnosticPrint;
    }

    public class ExtractorConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("$Mods.BiomeExtractorsMod.Configs.Common.GeneralConfigHeader")]
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.Common.ScanRateTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.Common.ScanRateTooltip")]
        [Range(1, 86400)]
        [DefaultValue(3600)]
        public int BiomeScanRate;

        [Header("$Mods.BiomeExtractorsMod.Configs.Common.Tier1Header")]
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.Common.Tier1RateTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.Common.Tier1RateTooltip")]
        [Range(1, 86400)]
        [DefaultValue(180)] //480 attempts/day
        public int Tier1ExtractorRate;
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.Common.Tier1ChanceTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.Common.Tier1ChanceTooltip")]
        [DefaultValue(60)]
        public int Tier1ExtractorChance; //480 extractions/day

        [Header("$Mods.BiomeExtractorsMod.Configs.Common.Tier2Header")]
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.Common.Tier2RateTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.Common.Tier2RateTooltip")]
        [Range(1, 86400)]
        [DefaultValue(120)] //720 attempts/day
        public int Tier2ExtractorRate;
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.Common.Tier2ChanceTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.Common.Tier2ChanceTooltip")]
        [DefaultValue(75)] //540 extractions/day
        public int Tier2ExtractorChance;

        [Header("$Mods.BiomeExtractorsMod.Configs.Common.Tier3Header")]
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.Common.Tier3RateTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.Common.Tier3RateTooltip")]
        [Range(1, 86400)]
        [DefaultValue(90)] //960 attempts/day
        public int Tier3ExtractorRate;
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.Common.Tier3ChanceTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.Common.Tier3ChanceTooltip")]
        [DefaultValue(85)] //816 extraction attempts/day
        public int Tier3ExtractorChance;

        [Header("$Mods.BiomeExtractorsMod.Configs.Common.Tier4Header")]
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.Common.Tier4RateTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.Common.Tier4RateTooltip")]
        [Range(1, 86400)]
        [DefaultValue(60)] //1440 attempts/day
        public int Tier4ExtractorRate;
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.Common.Tier4ChanceTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.Common.Tier4ChanceTooltip")]
        [Range(1, 100)]
        [DefaultValue(90)] //1296 extractions/day
        public int Tier4ExtractorChance;

        [Header("$Mods.BiomeExtractorsMod.Configs.Common.Tier5Header")]
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.Common.Tier5RateTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.Common.Tier5RateTooltip")]
        [Range(1, 86400)]
        [DefaultValue(45)] //1920 attempts/day
        public int Tier5ExtractorRate;
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.Common.Tier5ChanceTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.Common.Tier5ChanceTooltip")]
        [Range(1, 100)]
        [DefaultValue(94)] //1804.8 extractions/day
        public int Tier5ExtractorChance;

        [Header("$Mods.BiomeExtractorsMod.Configs.Common.Tier6Header")]
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.Common.Tier6RateTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.Common.Tier6RateTooltip")]
        [Range(1, 86400)]
        [DefaultValue(30)] //2880 attempts/day
        public int Tier6ExtractorRate;
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.Common.Tier6ChanceTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.Common.Tier6ChanceTooltip")]
        [Range(1, 100)]
        [DefaultValue(97)] //2793.6 extractions/day
        public int Tier6ExtractorChance;

        [Header("$Mods.BiomeExtractorsMod.Configs.Common.Tier7Header")]
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.Common.Tier7RateTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.Common.Tier7RateTooltip")]
        [Range(1, 86400)]
        [DefaultValue(24)] //3600 attempts/day
        public int Tier7ExtractorRate;
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.Common.Tier7ChanceTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.Common.Tier7ChanceTooltip")]
        [Range(1, 100)]
        [DefaultValue(100)] //3600 extractions/day
        public int Tier7ExtractorChance;

        // A method annotated with OnDeserialized will run after deserialization. You can use it for enforcing things like ranges, since Range and Increment are UI suggestions.
        [OnDeserialized]
        internal void EnforceRanges(StreamingContext context)
        {
            // RangeAttribute is just a suggestion to the UI. If we want to enforce constraints, we need to validate the data here. Users can edit config files manually with values outside the RangeAttribute, so we fix here if necessary.
            // Both enforcing ranges and not enforcing ranges have uses in mods. Make sure you fix config values if values outside the range will mess up your mod.
            BiomeScanRate = Utils.Clamp(BiomeScanRate, 1, 86400);

            Tier1ExtractorRate =   Utils.Clamp(Tier1ExtractorRate,   1, 86400);
            Tier1ExtractorChance = Utils.Clamp(Tier1ExtractorChance, 0, 100);

            Tier2ExtractorRate =   Utils.Clamp(Tier2ExtractorRate,   1, 86400);
            Tier2ExtractorChance = Utils.Clamp(Tier2ExtractorChance, 0, 100);

            Tier3ExtractorRate =   Utils.Clamp(Tier3ExtractorRate,   1, 86400);
            Tier3ExtractorChance = Utils.Clamp(Tier3ExtractorChance, 0, 100);

            Tier4ExtractorRate =   Utils.Clamp(Tier4ExtractorRate,   1, 86400);
            Tier4ExtractorChance = Utils.Clamp(Tier4ExtractorChance, 0, 100);

            Tier5ExtractorRate =   Utils.Clamp(Tier5ExtractorRate,   1, 86400);
            Tier5ExtractorChance = Utils.Clamp(Tier5ExtractorChance, 0, 100);

            Tier6ExtractorRate =   Utils.Clamp(Tier6ExtractorRate,   1, 86400);
            Tier6ExtractorChance = Utils.Clamp(Tier6ExtractorChance, 0, 100);

            Tier7ExtractorRate =   Utils.Clamp(Tier7ExtractorRate,   1, 86400);
            Tier7ExtractorChance = Utils.Clamp(Tier7ExtractorChance, 0, 100); 
        }
    }

    public class ExtractorCompat : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("$Mods.BiomeExtractorsMod.Configs.Compat.MSHeader")]
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.Compat.MaxMSTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.Compat.MaxMSTooltip")]
        [Range(1, int.MaxValue)]
        [DefaultValue(9999)]
        public int MaxMS;

        [OnDeserialized]
        internal void EnforceRanges(StreamingContext context)
        {
            MaxMS = Math.Max(-1, MaxMS); //-1: no limit, 0 = never push
        }
    }
}