using BiomeExtractorsMod.Common.Database;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Calamity.Common
{
    [JITWhenModsEnabled("CalamityMod")]
    internal abstract class BiomeChecker
    {
        internal static bool IsSubmerged(ScanData scan) =>
            IsSubmerged(new Point((int)scan.X, (int)scan.Y));
        internal static bool IsSubmerged(Point point)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    Tile tile = Main.tile[point + new Point(i, j)];
                    if (tile.LiquidAmount < 255) return false;
                    if (tile.LiquidType != LiquidID.Water) return false;
                }
            }
            return true;
        }

        internal static bool IsInAbyssArea(ScanData scan) =>
            IsInAbyssArea(new Point((int)scan.X, (int)scan.Y));
        internal static bool IsInAbyssArea(Point point)
        {
            int maxTilesX = Main.maxTilesX;
            int center = maxTilesX / 2;
            int abyss_startX = Abyss.AtLeftSideOfWorld ? center - (center - 135) + 35 : center + (center - 135) - 35;
            bool in_abyss_areaX;

            if (Abyss.AtLeftSideOfWorld)
            {
                in_abyss_areaX = point.X < abyss_startX + 140;
            }
            else
            {
                in_abyss_areaX = point.X > abyss_startX - 140;
            }

            if (in_abyss_areaX)
            {
                int abyss_startY = Main.remixWorld ? SulphurousSea.YStart : (SulphurousSea.YStart + (int)Main.worldSurface) / 2 + 90;
                if (Main.remixWorld)
                {
                    return point.Y < abyss_startY;
                }
                else
                {
                    return in_abyss_areaX && point.Y >= abyss_startY && point.Y <= Main.UnderworldLayer;
                }
            }

            return false;
        }
    }
}
