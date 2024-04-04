using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Content.Tiles;
using MagicStorage.Components;
using MagicStorage.Items;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.CrossMod
{
    internal abstract class MagicStorageHook
    {
        internal static Mod MS
        { get 
            {
                ModLoader.TryGetMod("MagicStorage", out Mod mod);
                return mod;
            }
        }

        private static TEEnvironmentAccess GetMSAccess(Point pos)
        {
            if (MS == null) return null;
            TileUtils.TryGetTileEntityAs(pos.X, pos.Y, out TEEnvironmentAccess entity);
            if (entity == null) return null;
            return entity;
        }

        private static TEStorageHeart GetMSStorageHeart(Point startFrom)
        {
            if (MS == null) return null;
            TEEnvironmentAccess start = GetMSAccess(startFrom);
            if (start == null) return null;
            return start.GetHeart();
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

            int limit = DepositableAmount(heart, newItem.type);
            newItem.stack = Utils.Clamp(newItem.stack, 0, limit);
            if(newItem.stack == 0) return false;
            heart.DepositItem(newItem);
            return true;
        }

        private static int DepositableAmount(TEStorageHeart storage, int itemID)
        {
            int limit = ModContent.GetInstance<ExtractorCompat>().MaxMS;
            if (limit < 1) return int.MaxValue;

            int amount = 0;
            foreach (Item item in storage.GetStoredItems())
            {
                if (item.type == itemID)
                {
                    amount += item.stack;
                    if (amount >= limit) return 0;
                }
            }
            return limit - amount;
        }
    }
}
