
using BiomeExtractorsMod.Content.Items;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.Localization;

namespace BiomeExtractorsMod.Common.Database
{
    public class Recipes : ModSystem
    {
        public static readonly string goldBarGroupName =            nameof(ItemID.GoldBar);
        public static readonly string basicExtractorGroupName =     $"{nameof(BiomeExtractorsMod)}:{nameof(BiomeExtractorItemIron)}";
        public static readonly string demonicExtractorGroupName =   $"{nameof(BiomeExtractorsMod)}:{nameof(BiomeExtractorItemCorruption)}";
        public static readonly string steampunkExtractorGroupName = $"{nameof(BiomeExtractorsMod)}:{nameof(BiomeExtractorItemAdamantite)}";

        public override void AddRecipeGroups()
        {
            RecipeGroup goldBar = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.GoldBar)}", ItemID.GoldBar, ItemID.PlatinumBar);
            RecipeGroup.RegisterGroup(goldBarGroupName, goldBar);
        }
    }
}