using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ModLoader.Config;

namespace BiomeExtractorsMod.Common.Configs
{
    public class ExtractorConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        // The things in brackets are known as "Attributes".

        [Header("$Mods.BiomeExtractorsMod.Configs.Common.BasicTierHeader")]
        
        [LabelKey("$Mods.BiomeExtractorsMod.Configs.Common.BasicTierSpeedTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.Common.BasicTierSpeedTooltip")]
        [Range(1, 86400)]
        [DefaultValue(180)]
        public int BasicExtractorSpeed;

        [LabelKey("$Mods.BiomeExtractorsMod.Configs.Common.BasicTierChanceTitle")]
        [TooltipKey("$Mods.BiomeExtractorsMod.Configs.Common.BasicTierChanceTooltip")]
        [DefaultValue(60)]
        public int BasicExtractorChance;
        
        // A method annotated with OnDeserialized will run after deserialization. You can use it for enforcing things like ranges, since Range and Increment are UI suggestions.
        [OnDeserialized]
        internal void EnforceRanges(StreamingContext context)
        {
            // RangeAttribute is just a suggestion to the UI. If we want to enforce constraints, we need to validate the data here. Users can edit config files manually with values outside the RangeAttribute, so we fix here if necessary.
            // Both enforcing ranges and not enforcing ranges have uses in mods. Make sure you fix config values if values outside the range will mess up your mod.
            BasicExtractorSpeed = Math.Max(BasicExtractorSpeed, 1);
            BasicExtractorChance = Utils.Clamp(BasicExtractorChance, 0, 100);

        }
    }


}