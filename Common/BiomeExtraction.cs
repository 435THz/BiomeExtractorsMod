using BiomeExtractorsMod.Content.TileEntities;
using System;
using Terraria;
using Terraria.ID;

namespace BiomeExtractorsMod.Common
{
    public static class BiomeExtraction
    {
        internal static bool AddToChest(Item newItem, Chest chest)
        {
            int starting = newItem.stack;
            for (int inventoryIndex = 0; inventoryIndex < Chest.maxItems; inventoryIndex++)
            {
                Item item = chest.item[inventoryIndex];
                if (item.type == ItemID.None)
                {
                    item.SetDefaults(newItem.type);
                    newItem.stack -= item.stack;
                }
                if (item.type == newItem.type) {
                    int diff = item.maxStack - item.stack;
                    int transfer = Math.Min(diff, newItem.stack);
                    newItem.stack -= transfer;
                    item.stack += transfer;
                }
                if (newItem.stack < 1)
                {
                    return true;
                }
            }
            return starting != newItem.stack;
        }

            internal static Item GenerateItem(BiomeExtractorEnt entity)
        {
            Item item = new();
            item.SetDefaults(ItemID.StoneBlock);
            SceneMetrics metric = new();
            SceneMetricsScanSettings settings = new()
            {
                BiomeScanCenterPositionInWorld = entity.Position.ToVector2(),
                ScanOreFinderData = false
            };
            metric.ScanAndExportToMain(settings);
            if (metric.EnoughTilesForDesert) //TODO S C R E A M
                item.SetDefaults(ItemID.SandBlock);
            item.stack = 1;
            return item;
        }
    }
}