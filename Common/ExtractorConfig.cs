using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace BiomeExtractorsMod.Common.Configs
{
    public class ConfigClient : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("$Mods.BiomeExtractorsMod.Configs.ConfigClient.UIConfigHeader")]
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.ConfigClient.WindowPosTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.ConfigClient.WindowPosTooltip")]
        [DefaultValue(false)]
        public bool SaveWindowPos;

        [LabelKey("$Mods.BiomeExtractorsMod.Configs.ConfigClient.UIPrintPoolsTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.ConfigClient.UIPrintPoolsTooltip")]
        [DefaultValue(false)]
        public bool DiagnosticPrintPools;
    }

    public class ConfigCommon : ModConfig
    {
        public static ConfigCommon Instance => ModContent.GetInstance<ConfigCommon>();
        public override ConfigScope Mode => ConfigScope.ServerSide;
        public enum FollowRateValues {NO, REDUCED, YES}

        [Header("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.GeneralConfigHeader")]
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.FollowDayRateTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.FollowDayRateTooltip")]
        [DefaultValue(FollowRateValues.REDUCED)]
        [DrawTicks]
        public FollowRateValues FollowDayRate;


        [LabelKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.TransferAnimationTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.TransferAnimationTooltip")]
        [DefaultValue(true)]
        public bool ShowTransferAnimation;

        [LabelKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.ScanRateTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.ScanRateTooltip")]
        [Range(1, 86400)]
        [DefaultValue(3600)]
        public int BiomeScanRate;

        [Header("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier1Header")]
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier1RateTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier1RateTooltip")]
        [Range(1, 86400)]
        [DefaultValue(300)] //288 attempts/day
        public int Tier1ExtractorRate;
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier1ChanceTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier1ChanceTooltip")]
        [DefaultValue(60)]
        public int Tier1ExtractorChance; //172.8 extractions/day
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier1AmountTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier1AmountTooltip")]
        [Range(1, 99)]
        [DefaultValue(1)]
        public int Tier1ExtractorAmount; //172.8 items/day

        [Header("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier2Header")]
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier2RateTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier2RateTooltip")]
        [Range(1, 86400)]
        [DefaultValue(240)] //360 attempts/day
        public int Tier2ExtractorRate;
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier2ChanceTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier2ChanceTooltip")]
        [DefaultValue(75)] //270 extractions/day
        public int Tier2ExtractorChance;
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier2AmountTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier2AmountTooltip")]
        [Range(1, 99)]
        [DefaultValue(1)]
        public int Tier2ExtractorAmount; //270 items/day

        [Header("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier3Header")]
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier3RateTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier3RateTooltip")]
        [Range(1, 86400)]
        [DefaultValue(180)] //480 attempts/day
        public int Tier3ExtractorRate;
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier3ChanceTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier3ChanceTooltip")]
        [DefaultValue(85)] //408 extractions/day
        public int Tier3ExtractorChance;
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier3AmountTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier3AmountTooltip")]
        [Range(1, 99)]
        [DefaultValue(1)]
        public int Tier3ExtractorAmount; //408 items/day

        [Header("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier4Header")]
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier4RateTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier4RateTooltip")]
        [Range(1, 86400)]
        [DefaultValue(150)] //576 attempts/day
        public int Tier4ExtractorRate;
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier4ChanceTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier4ChanceTooltip")]
        [Range(1, 100)]
        [DefaultValue(90)] //518.4 extractions/day
        public int Tier4ExtractorChance;
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier4AmountTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier4AmountTooltip")]
        [Range(1, 99)]
        [DefaultValue(1)]
        public int Tier4ExtractorAmount; //518.4 items/day

        [Header("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier5Header")]
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier5RateTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier5RateTooltip")]
        [Range(1, 86400)]
        [DefaultValue(120)] //720 attempts/day
        public int Tier5ExtractorRate;
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier5ChanceTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier5ChanceTooltip")]
        [Range(1, 100)]
        [DefaultValue(94)] //676.8 extractions/day
        public int Tier5ExtractorChance;
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier5AmountTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier5AmountTooltip")]
        [Range(1, 99)]
        [DefaultValue(1)]
        public int Tier5ExtractorAmount; //676.8 items/day

        [Header("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier6Header")]
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier6RateTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier6RateTooltip")]
        [Range(1, 86400)]
        [DefaultValue(90)] //960 attempts/day
        public int Tier6ExtractorRate;
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier6ChanceTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier6ChanceTooltip")]
        [Range(1, 100)]
        [DefaultValue(97)] //931.2 extractions/day
        public int Tier6ExtractorChance;
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier6AmountTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier6AmountTooltip")]
        [Range(1, 99)]
        [DefaultValue(1)]
        public int Tier6ExtractorAmount; //931.2 items/day

        [Header("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier7Header")]
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier7RateTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier7RateTooltip")]
        [Range(1, 86400)]
        [DefaultValue(75)] //1152 attempts/day
        public int Tier7ExtractorRate;
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier7ChanceTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier7ChanceTooltip")]
        [Range(1, 100)]
        [DefaultValue(100)] //1152 extractions/day
        public int Tier7ExtractorChance;
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier7AmountTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.ConfigCommon.Tier7AmountTooltip")]
        [Range(1, 99)]
        [DefaultValue(1)]
        public int Tier7ExtractorAmount; //1152 items/day

        // A method annotated with OnDeserialized will run after deserialization. You can use it for enforcing things like ranges, since Range and Increment are UI suggestions.
        [OnDeserialized]
        internal void EnforceRanges(StreamingContext context)
        {
            // RangeAttribute is just a suggestion to the UI. If we want to enforce constraints, we need to validate the data here. Users can edit config files manually with values outside the RangeAttribute, so we fix here if necessary.
            // Both enforcing ranges and not enforcing ranges have uses in mods. Make sure you fix config values if values outside the range will mess up your mod.
            BiomeScanRate = Utils.Clamp(BiomeScanRate, 1, 86400);

            Tier1ExtractorRate =   Utils.Clamp(Tier1ExtractorRate,   1, 86400);
            Tier1ExtractorChance = Utils.Clamp(Tier1ExtractorChance, 0, 100);
            Tier1ExtractorAmount = Utils.Max(Tier1ExtractorAmount, 1);

            Tier2ExtractorRate =   Utils.Clamp(Tier2ExtractorRate,   1, 86400);
            Tier2ExtractorChance = Utils.Clamp(Tier2ExtractorChance, 0, 100);
            Tier2ExtractorAmount = Utils.Max(Tier2ExtractorAmount, 1);

            Tier3ExtractorRate =   Utils.Clamp(Tier3ExtractorRate,   1, 86400);
            Tier3ExtractorChance = Utils.Clamp(Tier3ExtractorChance, 0, 100);
            Tier3ExtractorAmount = Utils.Max(Tier3ExtractorAmount, 1);

            Tier4ExtractorRate =   Utils.Clamp(Tier4ExtractorRate,   1, 86400);
            Tier4ExtractorChance = Utils.Clamp(Tier4ExtractorChance, 0, 100);
            Tier4ExtractorAmount = Utils.Max(Tier4ExtractorAmount, 1);

            Tier5ExtractorRate =   Utils.Clamp(Tier5ExtractorRate,   1, 86400);
            Tier5ExtractorChance = Utils.Clamp(Tier5ExtractorChance, 0, 100);
            Tier5ExtractorAmount = Utils.Max(Tier5ExtractorAmount, 1);

            Tier6ExtractorRate =   Utils.Clamp(Tier6ExtractorRate,   1, 86400);
            Tier6ExtractorChance = Utils.Clamp(Tier6ExtractorChance, 0, 100);
            Tier6ExtractorAmount = Utils.Max(Tier6ExtractorAmount, 1);

            Tier7ExtractorRate =   Utils.Clamp(Tier7ExtractorRate,   1, 86400);
            Tier7ExtractorChance = Utils.Clamp(Tier7ExtractorChance, 0, 100);
            Tier7ExtractorAmount = Utils.Max(Tier7ExtractorAmount, 1);
        }
    }

    public class ConfigCompat : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("$Mods.BiomeExtractorsMod.Configs.ConfigCompat.MSHeader")]
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.ConfigCompat.MSTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.ConfigCompat.MSTooltip")]
        [DefaultValue(true)]
        public bool MS; //if false, act like Magic Storage wasn't installed

        [LabelKey("$Mods.BiomeExtractorsMod.Configs.ConfigCompat.MaxMSTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.ConfigCompat.MaxMSTooltip")]
        [Range(1, int.MaxValue)]
        [DefaultValue(9999)]
        public int MaxMS;

        [LabelKey("$Mods.BiomeExtractorsMod.Configs.ConfigCompat.MaxMSStacksTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.ConfigCompat.MaxMSStacksTooltip")]
        [Range(1, int.MaxValue)]
        [DefaultValue(5)]
        public int MaxMSStacks;

        [OnDeserialized]
        internal void EnforceRanges(StreamingContext context)
        {
            MaxMS =       Math.Max(1, MaxMS);
            MaxMSStacks = Math.Max(1, MaxMSStacks);
        }
    }
}