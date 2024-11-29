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

        [Header("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.AbyssExtractors.Header")]
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.AbyssExtractors.Sulphuric.Rate.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.AbyssExtractors.Sulphuric.Rate.Tooltip")]
        [Range(1, 86400)]
        [DefaultValue(250)] //345.6 attempts/day
        public int SulphuricExtractorRate;
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.AbyssExtractors.Sulphuric.Chance.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.AbyssExtractors.Sulphuric.Chance.Tooltip")]
        [DefaultValue(75)]
        public int SulphuricExtractorChance; //259.2 extractions/day
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.AbyssExtractors.Sulphuric.DryEfficiency.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.AbyssExtractors.Sulphuric.DryEfficiency.Tooltip")]
        [DefaultValue(50)]
        public int SulphuricExtractorDryEfficiency; //129.6 extractions/day when dry

        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.AbyssExtractors.Pressurized.Rate.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.AbyssExtractors.Pressurized.Rate.Tooltip")]
        [Range(1, 86400)]
        [DefaultValue(200)] //432 attempts/day
        public int PressurizedExtractorRate;
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.AbyssExtractors.Pressurized.Chance.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.AbyssExtractors.Pressurized.Chance.Tooltip")]
        [DefaultValue(85)]
        public int PressurizedExtractorChance; //367.2 extractions/day

        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.AbyssExtractors.Thermoresistant.Rate.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.AbyssExtractors.Thermoresistant.Rate.Tooltip")]
        [Range(1, 86400)]
        [DefaultValue(150)] //576 attempts/day
        public int ThermoresistantExtractorRate;
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.AbyssExtractors.Thermoresistant.Chance.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.AbyssExtractors.Thermoresistant.Chance.Tooltip")]
        [DefaultValue(94)]
        public int ThermoresistantExtractorChance; //541.44 extractions/day

        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.AbyssExtractors.Abyssal.Rate.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.AbyssExtractors.Abyssal.Rate.Tooltip")]
        [Range(1, 86400)]
        [DefaultValue(100)] //864 attempts/day
        public int AbyssalExtractorRate;
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.AbyssExtractors.Abyssal.Chance.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.AbyssExtractors.Abyssal.Chance.Tooltip")]
        [DefaultValue(100)]
        public int AbyssalExtractorChance; //864 extractions/day

        [Header("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.PostMoonLordExtractors.Header")]
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.PostMoonLordExtractors.Spectral.Rate.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.PostMoonLordExtractors.Spectral.Rate.Tooltip")]
        [Range(1, 86400)]
        [DefaultValue(70)] //1234.3 attempts/day
        public int SpectralExtractorRate;
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.PostMoonLordExtractors.Spectral.Chance.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.PostMoonLordExtractors.Spectral.Chance.Tooltip")]
        [DefaultValue(100)]
        public int SpectralExtractorChance; //1234.3 extractions/day

        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.PostMoonLordExtractors.Auric.Rate.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.PostMoonLordExtractors.Auric.Rate.Tooltip")]
        [Range(1, 86400)]
        [DefaultValue(65)] //1329.2 attempts/day
        public int AuricExtractorRate;
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.PostMoonLordExtractors.Auric.Chance.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.PostMoonLordExtractors.Auric.Chance.Tooltip")]
        [DefaultValue(100)]
        public int AuricExtractorChance; //1329.2 extractions/day

        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.PostMoonLordExtractors.Exo.Rate.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.PostMoonLordExtractors.Exo.Rate.Tooltip")]
        [Range(1, 86400)]
        [DefaultValue(60)] //1440 attempts/day
        public int ExoExtractorRate;
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.PostMoonLordExtractors.Exo.Chance.Title")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.CalamityConfigs.PostMoonLordExtractors.Exo.Chance.Tooltip")]
        [DefaultValue(100)]
        public int ExoExtractorChance; //1440 extractions/day

        [OnDeserialized]
        internal void EnforceRanges(StreamingContext context)
        {
            SulphuricExtractorRate = Utils.Clamp(SulphuricExtractorRate, 1, 86400);
            SulphuricExtractorChance = Utils.Clamp(SulphuricExtractorChance, 0, 100);
            SulphuricExtractorDryEfficiency = Utils.Clamp(SulphuricExtractorDryEfficiency, 0, 100);

            PressurizedExtractorRate = Utils.Clamp(PressurizedExtractorRate, 1, 86400);
            PressurizedExtractorChance = Utils.Clamp(PressurizedExtractorChance, 0, 100);

            ThermoresistantExtractorRate = Utils.Clamp(ThermoresistantExtractorRate, 1, 86400);
            ThermoresistantExtractorChance = Utils.Clamp(ThermoresistantExtractorChance, 0, 100);

            AbyssalExtractorRate = Utils.Clamp(AbyssalExtractorRate, 1, 86400);
            AbyssalExtractorChance = Utils.Clamp(AbyssalExtractorChance, 0, 100);

            SpectralExtractorRate = Utils.Clamp(SpectralExtractorRate, 1, 86400);
            SpectralExtractorChance = Utils.Clamp(SpectralExtractorChance, 0, 100);

            AuricExtractorRate = Utils.Clamp(AuricExtractorRate, 1, 86400);
            AuricExtractorChance = Utils.Clamp(AuricExtractorChance, 0, 100);

            ExoExtractorRate = Utils.Clamp(ExoExtractorRate, 1, 86400);
            ExoExtractorChance = Utils.Clamp(ExoExtractorChance, 0, 100);
        }
    }
}
