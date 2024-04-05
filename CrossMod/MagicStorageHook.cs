using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Content.Tiles;
using MagicStorage.Components;
using Microsoft.Xna.Framework;
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

            int limit = DepositableAmount(heart, newItem.type);
            newItem.stack = Utils.Clamp(newItem.stack, 0, limit);
            if(newItem.stack == 0) return false;
            heart.DepositItem(newItem);
            return true;
        }

        private static int DepositableAmount(TEStorageHeart storage, int itemID)
        {
            int limit = ModContent.GetInstance<ExtractorCompat>().MaxMS;
            if (limit < 0) return int.MaxValue;

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
