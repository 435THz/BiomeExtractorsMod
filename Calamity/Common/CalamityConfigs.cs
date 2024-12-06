using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace BiomeExtractorsMod.Calamity.Common
{
    [ExtendsFromMod("CalamityMod")]
    [JITWhenModsEnabled("CalamityMod")]
    internal class CalamityConfigs : ModConfig
    {
        public static CalamityConfigs Instance => ModContent.GetInstance<CalamityConfigs>();
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.SulphuricExtractor.Header")]
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.SulphuricExtractor.Rate.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.SulphuricExtractor.Rate.Tooltip")]
        [Range(1, 86400)]
        [DefaultValue(250)] //345.6 attempts/day
        public int SulphuricExtractorRate;
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.SulphuricExtractor.Chance.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.SulphuricExtractor.Chance.Tooltip")]
        [DefaultValue(75)]
        public int SulphuricExtractorChance; //259.2 extractions/day
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.SulphuricExtractor.Amount.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.SulphuricExtractor.Amount.Tooltip")]
        [Range(1, 99)]
        [DefaultValue(1)]
        public int SulphuricExtractorAmount; //259.2 items/day
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.SulphuricExtractor.DryEfficiency.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.SulphuricExtractor.DryEfficiency.Tooltip")]
        [DefaultValue(50)]
        public int SulphuricExtractorDryEfficiency; //129.6 extractions/day when dry

        [Header("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.PressurizedExtractor.Header")]
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.PressurizedExtractor.Rate.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.PressurizedExtractor.Rate.Tooltip")]
        [Range(1, 86400)]
        [DefaultValue(200)] //432 attempts/day
        public int PressurizedExtractorRate;
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.PressurizedExtractor.Chance.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.PressurizedExtractor.Chance.Tooltip")]
        [DefaultValue(85)]
        public int PressurizedExtractorChance; //367.2 extractions/day
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.PressurizedExtractor.Amount.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.PressurizedExtractor.Amount.Tooltip")]
        [Range(1, 99)]
        [DefaultValue(1)]
        public int PressurizedExtractorAmount; //367.2 items/day

        [Header("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.ThermoresistantExtractor.Header")]
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.ThermoresistantExtractor.Rate.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.ThermoresistantExtractor.Rate.Tooltip")]
        [Range(1, 86400)]
        [DefaultValue(150)] //576 attempts/day
        public int ThermoresistantExtractorRate;
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.ThermoresistantExtractor.Chance.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.ThermoresistantExtractor.Chance.Tooltip")]
        [DefaultValue(94)]
        public int ThermoresistantExtractorChance; //541.44 extractions/day
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.ThermoresistantExtractor.Amount.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.ThermoresistantExtractor.Amount.Tooltip")]
        [Range(1, 99)]
        [DefaultValue(1)]
        public int ThermoresistantExtractorAmount; //541.44 items/day

        [Header("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.AbyssalExtractor.Header")]
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.AbyssalExtractor.Rate.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.AbyssalExtractor.Rate.Tooltip")]
        [Range(1, 86400)]
        [DefaultValue(100)] //864 attempts/day
        public int AbyssalExtractorRate;
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.AbyssalExtractor.Chance.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.AbyssalExtractor.Chance.Tooltip")]
        [DefaultValue(100)]
        public int AbyssalExtractorChance; //864 extractions/day
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.AbyssalExtractor.Amount.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.AbyssalExtractor.Amount.Tooltip")]
        [Range(1, 99)]
        [DefaultValue(1)]
        public int AbyssalExtractorAmount; //864 items/day

        [Header("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.SpectralExtractor.Header")]
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.SpectralExtractor.Rate.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.SpectralExtractor.Rate.Tooltip")]
        [Range(1, 86400)]
        [DefaultValue(70)] //1234.3 attempts/day
        public int SpectralExtractorRate;
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.SpectralExtractor.Chance.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.SpectralExtractor.Chance.Tooltip")]
        [DefaultValue(100)]
        public int SpectralExtractorChance; //1234.3 extractions/day
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.SpectralExtractor.Amount.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.SpectralExtractor.Amount.Tooltip")]
        [Range(1, 99)]
        [DefaultValue(1)]
        public int SpectralExtractorAmount; //1234.2 items/day

        [Header("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.AuricExtractor.Header")]
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.AuricExtractor.Rate.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.AuricExtractor.Rate.Tooltip")]
        [Range(1, 86400)]
        [DefaultValue(65)] //1329.2 attempts/day
        public int AuricExtractorRate;
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.AuricExtractor.Chance.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.AuricExtractor.Chance.Tooltip")]
        [DefaultValue(100)]
        public int AuricExtractorChance; //1329.2 extractions/day
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.AuricExtractor.Amount.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.AuricExtractor.Amount.Tooltip")]
        [Range(1, 99)]
        [DefaultValue(1)]
        public int AuricExtractorAmount; //1329.2 items/day

        [Header("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.ExoExtractor.Header")]
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.ExoExtractor.Rate.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.ExoExtractor.Rate.Tooltip")]
        [Range(1, 86400)]
        [DefaultValue(60)] //1440 attempts/day
        public int ExoExtractorRate;
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.ExoExtractor.Chance.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.ExoExtractor.Chance.Tooltip")]
        [DefaultValue(100)]
        public int ExoExtractorChance; //1440 extractions/day
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.ExoExtractor.Amount.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.ExoExtractor.Amount.Tooltip")]
        [Range(1, 99)]
        [DefaultValue(1)]
        public int ExoExtractorAmount; //1440 items/day

        [OnDeserialized]
        internal void EnforceRanges(StreamingContext context)
        {
            SulphuricExtractorRate = Utils.Clamp(SulphuricExtractorRate, 1, 86400);
            SulphuricExtractorChance = Utils.Clamp(SulphuricExtractorChance, 0, 100);
            SulphuricExtractorAmount = Utils.Max(SulphuricExtractorAmount, 1);
            SulphuricExtractorDryEfficiency = Utils.Clamp(SulphuricExtractorDryEfficiency, 0, 100);

            PressurizedExtractorRate = Utils.Clamp(PressurizedExtractorRate, 1, 86400);
            PressurizedExtractorChance = Utils.Clamp(PressurizedExtractorChance, 0, 100);
            PressurizedExtractorAmount = Utils.Max(PressurizedExtractorAmount, 1);

            ThermoresistantExtractorRate = Utils.Clamp(ThermoresistantExtractorRate, 1, 86400);
            ThermoresistantExtractorChance = Utils.Clamp(ThermoresistantExtractorChance, 0, 100);
            ThermoresistantExtractorAmount = Utils.Max(ThermoresistantExtractorAmount, 1);

            AbyssalExtractorRate = Utils.Clamp(AbyssalExtractorRate, 1, 86400);
            AbyssalExtractorChance = Utils.Clamp(AbyssalExtractorChance, 0, 100);
            AbyssalExtractorAmount = Utils.Max(AbyssalExtractorAmount, 1);

            SpectralExtractorRate = Utils.Clamp(SpectralExtractorRate, 1, 86400);
            SpectralExtractorChance = Utils.Clamp(SpectralExtractorChance, 0, 100);
            SpectralExtractorAmount = Utils.Max(SpectralExtractorAmount, 1);

            AuricExtractorRate = Utils.Clamp(AuricExtractorRate, 1, 86400);
            AuricExtractorChance = Utils.Clamp(AuricExtractorChance, 0, 100);
            AuricExtractorAmount = Utils.Max(AuricExtractorAmount, 1);

            ExoExtractorRate = Utils.Clamp(ExoExtractorRate, 1, 86400);
            ExoExtractorChance = Utils.Clamp(ExoExtractorChance, 0, 100);
            ExoExtractorAmount = Utils.Max(ExoExtractorAmount, 1);
        }
    }
}
