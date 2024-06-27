using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Content.Tiles;
using MagicStorage.Components;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.CrossMod
{
    [JITWhenModsEnabled("MagicStorage")]
    internal abstract class MagicStorageHook
    {
        private static EnvironmentAccess EnvAccessTile { get => TileLoader.GetTile(ModContent.TileType<EnvironmentAccess>()) as EnvironmentAccess; }

        private static TEEnvironmentAccess GetMSAccess(Point pos)
        {
            TileUtils.TryGetTileEntityAs(pos.X, pos.Y, out TEEnvironmentAccess entity);
            if (entity == null) return null;
            return entity;
        }

        private static TEStorageHeart GetMSStorageHeart(Point startFrom)
        {
            TEEnvironmentAccess start = GetMSAccess(startFrom);
            if (start == null || EnvAccessTile == null) return null;
            return EnvAccessTile.GetHeart(start.Position.X, start.Position.Y);
        }

        internal static bool IsOutputValid(Point pos)
        {
            return GetMSStorageHeart(pos) != null;
        }
        internal static bool IsTileEnvAccess(Point pos)
        {
            return GetMSAccess(pos) != null;
        }

        internal static Point GetAccessPosition(Point pos)
        {
            return GetMSAccess(pos).Position.ToPoint();
        }

        internal static bool AddItemToStorage(Item newItem, Point access)
        {
            TEStorageHeart heart = GetMSStorageHeart(access);
            if (heart == null) return false;

            int limit = DepositableAmount(heart, newItem);
            newItem.stack = Utils.Clamp(newItem.stack, 0, limit);
            if(newItem.stack == 0) return false;
            heart.DepositItem(newItem);
            return true;
        }

        private static int DepositableAmount(TEStorageHeart storage, Item newItem)
        {
            int limit = ModContent.GetInstance<ConfigCompat>().MaxMS;
            int stackLimit = ModContent.GetInstance<ConfigCompat>().MaxMSStacks;
            limit = Math.Min(limit, newItem.maxStack * stackLimit);

            int amount = 0;
            int remainingToStack = 0;
            foreach (Item item in storage.GetStoredItems())
            {
                if (item.type == newItem.type)
                {
                    amount += item.stack;
                    remainingToStack += newItem.maxStack - item.stack;
                    if (amount >= limit) return 0;
                }
            }
            int availableSpace = GetFreeSlots(storage) * newItem.maxStack + remainingToStack;
            limit = Math.Min(limit, availableSpace);

            return limit - amount;
        }

        private static int GetFreeSlots(TEStorageHeart storage)
        {
            int numItems = 0;
            int capacity = 0;
            if (storage is not null)
            {
                foreach (TEAbstractStorageUnit abstractStorageUnit in storage.GetStorageUnits())
                {
                    if (abstractStorageUnit is TEStorageUnit storageUnit)
                    {
                        numItems += storageUnit.NumItems;
                        capacity += storageUnit.Capacity;
                    }
                }
            }
            return capacity - numItems;
        }
    }
}
