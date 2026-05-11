using System;
using BiomeExtractorsMod.Common.Database;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using BiomeExtractorsMod.Common.UI;
using Terraria.ID;
using System.Collections.Generic;
using Terraria.Enums;

namespace BiomeExtractorsMod.Content.Items
{
    public class ExtractionAnalyzer : BiomeScanner
    {
        
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.maxStack = 1;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.SetShopValues(ItemRarityColor.Orange3, Item.buyPrice(gold: 15)); //sell at 3
        }
        
        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.altFunctionUse != 2)
                {
                    BiomeExtractionSystem.ExtractionTier tier = CurrentTier ??
                        throw new IndexOutOfRangeException("Could not find a valid registered tier");

                    Point16 pCenter = new((int)((player.position.X + player.width * 0.5) / 16.0),
                                           (int)((player.position.Y + player.height * 0.5) / 16.0));
                    ModContent.GetInstance<UISystem>().OpenAnalyzerScannerInterface(pCenter, tier);
                    return true;
                }
                return base.UseItem(player);
            }
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> lines)
        {
            BiomeExtractionSystem.ExtractionTier tier = CurrentTier;

            int index = lines.FindIndex(static line => line.Mod == "Terraria" && line.Name == "Tooltip6");
            if (index >= 0)
            {
                if (tier is not null)
                    lines[index].Text += $" {tier.Name}";
                else
                    lines[index].Text += $" None";
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<BiomeScanner>())
                .AddIngredient(ItemID.BlackLens, 2)
                .AddIngredient(ItemID.MeteoriteBar, 5)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
    
}