using BiomeExtractorsMod.Common.UI;
using BiomeExtractorsMod.Content.TileEntities;
using BiomeExtractorsMod.Content.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Common
{
    internal class ExtractorPlayer : ModPlayer
    {
        public static ExtractorPlayer LocalPlayer => Main.LocalPlayer.GetModPlayer<ExtractorPlayer>();

        public bool IsInExtractorRange(BiomeExtractorEnt extractor)
        {
            Point16 tl = TileUtils.GetTopLeftTileInMultitile(extractor.Position.X, extractor.Position.Y);
            int num = (int)(((double)Player.position.X + (double)Player.width * 0.5) / 16.0);
            int num2 = (int)(((double)Player.position.Y + (double)Player.height * 0.5) / 16.0);

            Rectangle r = new Rectangle(tl.X * 16, tl.Y * 16, 48, 48);
            r.Inflate(-1, -1);
            
            Point point = r.ClosestPointInRect(Player.Center).ToTileCoordinates();
            return num >= point.X - Player.tileRangeX && num <= point.X + Player.tileRangeX + 1 && num2 >= point.Y - Player.tileRangeY && num2 <= point.Y + Player.tileRangeY + 1;
        }

        public override void UpdateDead()
        {
            if (Player.whoAmI == Main.myPlayer)
                ModContent.GetInstance<UISystem>().CloseInterface();
        }
    }
}
