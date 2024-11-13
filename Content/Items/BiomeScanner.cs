using BiomeExtractorsMod.Common.Database;
using BiomeExtractorsMod.Common.UI;
using BiomeExtractorsMod.Content.TileEntities;
using BiomeExtractorsMod.Content.Tiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace BiomeExtractorsMod.Content.Items
{
    public class BiomeScanner : ModItem
    {
        static readonly BiomeExtractionSystem BES = ModContent.GetInstance<BiomeExtractionSystem>();
        int highestTier = (int)BiomeExtractorEnt.EnumTiers.BASIC;
        int currentTier = (int)BiomeExtractorEnt.EnumTiers.BASIC;
        BiomeExtractionSystem.ExtractionTier CurrentTier
        {
            get
            {
                BiomeExtractionSystem.ExtractionTier tier = BES.GetTier(currentTier, lower: true);
                if (tier is not null) CurrentTier = tier;
                return tier;
            }
            set
            {
                if (value is not null) currentTier = value.Tier;
                else currentTier = BES.GetTier(int.MinValue, higher: true).Tier; // set to lowest possible
            }
        }
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.maxStack = 1;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.SetShopValues(ItemRarityColor.Pink5, Item.buyPrice(gold: 10)); //sell at 2
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.altFunctionUse == 2)
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    Point cursor = Main.MouseWorld.ToTileCoordinates();
                    if (TileUtils.TryGetTileEntityAs(cursor.X, cursor.Y, out BiomeExtractorEnt extractor))
                    {
                        CurrentTier = extractor.ExtractionTier;
                        if(currentTier>highestTier)
                        {
                            highestTier = currentTier;
                            Main.NewText(Language.GetTextValue($"{BiomeExtractorsMod.LocDiagnostics}.ScannerCloneExtractor", CurrentTier.Name));
                        } else
                        {
                            Main.NewText(Language.GetTextValue($"{BiomeExtractorsMod.LocDiagnostics}.ScannerChangeTier", CurrentTier.Name));
                        }
                    }
                    else
                    {
                        int newTier = currentTier;
                        BiomeExtractionSystem.ExtractionTier tier = BES.GetClosestHigherTier(newTier); //found closest higher
                        if (tier is not null) newTier = tier.Tier;
                        if (tier is null || newTier > highestTier)
                        {
                            tier = BES.GetTier(int.MinValue, higher: true) ?? //wrap back around
                                throw new IndexOutOfRangeException("Could not find a valid registered tier");
                            newTier = tier.Tier;
                            if (newTier > highestTier)
                                throw new IndexOutOfRangeException("Could not find a valid registered tier");
                        }
                        currentTier = newTier;
                        Main.NewText(Language.GetTextValue($"{BiomeExtractorsMod.LocDiagnostics}.ScannerChangeTier", CurrentTier.Name));
                    }
                    return true;
                }
                else //unneeded, only here for clarity
                {
                    BiomeExtractionSystem.ExtractionTier tier = CurrentTier ??
                        throw new IndexOutOfRangeException("Could not find a valid registered tier");

                    Point16 pCenter = new((int)((player.position.X + player.width * 0.5) / 16.0),
                                           (int)((player.position.Y + player.height * 0.5) / 16.0));
                    ModContent.GetInstance<UISystem>().OpenScannerInterface(pCenter, tier);
                    return true;
                }
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

        public override void SaveData(TagCompound tag)
        {
            tag["current"] = currentTier;
            tag["highest"] = highestTier;
        }

        public override void LoadData(TagCompound tag)
        {
            currentTier = tag.GetAsInt("current");
            highestTier = tag.GetAsInt("highest");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(currentTier);
            writer.Write(highestTier);
        }

        public override void NetReceive(BinaryReader reader)
        {
            currentTier = reader.ReadInt32();
            highestTier = reader.ReadInt32();
        }
    }
}
