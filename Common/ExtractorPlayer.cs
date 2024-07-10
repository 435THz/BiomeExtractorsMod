using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Common.UI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace BiomeExtractorsMod.Common
{
    internal class ExtractorPlayer : ModPlayer
    {
        internal static ExtractorPlayer LocalPlayer => Main.LocalPlayer.GetModPlayer<ExtractorPlayer>();
        private static readonly string VectorX = "ExtractorWindowX";
        private static readonly string VectorY = "ExtractorWindowY";
        private static Vector2 Defaultpos => new(((Main.screenWidth / Main.UIScale) - ExtractorUI.PanelWidth) / 2, 18.625f);
        private Vector2? _windowpos = null;
        internal Vector2 ExtractorWindowPos {
            get => ModContent.GetInstance<ConfigClient>().SaveWindowPos && _windowpos is not null ? (Vector2)_windowpos : Defaultpos;
            set => _windowpos = ModContent.GetInstance<ConfigClient>().SaveWindowPos ? value : Defaultpos;
        }
        
        internal bool IsInRectangleRange(Point16 center)
        {
            Point16 tl = center + Point16.NegativeOne;
            int pCenterX = (int)((Player.position.X + Player.width * 0.5) / 16.0);
            int pCenterY = (int)((Player.position.Y + Player.height * 0.5) / 16.0);

            Rectangle r = new(tl.X * 16, tl.Y * 16, 48, 48);
            r.Inflate(-1, -1);
            
            Point point = r.ClosestPointInRect(Player.Center).ToTileCoordinates();
            return pCenterX >= point.X - Player.tileRangeX && pCenterX <= point.X + Player.tileRangeX + 1 && pCenterY >= point.Y - Player.tileRangeY && pCenterY <= point.Y + Player.tileRangeY + 1;
        }

        public override void UpdateDead()
        {
            if (Player.whoAmI == Main.myPlayer)
                ModContent.GetInstance<UISystem>().CloseInterface();
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add(VectorX, ExtractorWindowPos.X);
            tag.Add(VectorY, ExtractorWindowPos.Y);
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey(VectorX) && tag.ContainsKey(VectorY))
            {
                float x = tag.GetFloat(VectorX);
                float y = tag.GetFloat(VectorY);
                ExtractorWindowPos = new Vector2(x, y);
            }
            else ExtractorWindowPos = Defaultpos;
        }

        public static void SendUpdateRequest()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModPacket statePacket = ModContent.GetInstance<BiomeExtractorsMod>().GetPacket(1);
                statePacket.Write((byte)ClientMessageType.UPDATE_REQUEST);
                statePacket.Send();
            }
        }
    }
}
