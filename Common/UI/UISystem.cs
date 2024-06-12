using BiomeExtractorsMod.Content.TileEntities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace BiomeExtractorsMod.Common.UI
{
    internal class UISystem : ModSystem
    {
        internal UserInterface UIHolder;
        internal ExtractorUI Interface;
        internal BiomeExtractorEnt Extractor;

        private GameTime _lastUpdateUiGameTime;
        
        public override void Load()
        {
            if (!Main.dedServ)
            {
                UIHolder = new UserInterface();

                Interface = new ExtractorUI();
                Interface.Activate(); // Activate calls Initialize() on the UIState if not initialized and calls OnActivate, then calls Activate on every child element.
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;
            if (UIHolder?.CurrentState != null)
            {
                UIHolder.Update(gameTime);
            }
            if (Extractor is not null)
            {
                if (!Extractor.IsTileValidForEntity(Extractor.Position.X, Extractor.Position.Y) ||
                    !ExtractorPlayer.LocalPlayer.IsInExtractorRange(Extractor)) CloseInterface();
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "BiomeExtractorsMod: ExtractorInterface",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && UIHolder?.CurrentState != null)
                        {
                            UIHolder.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }

        public override void Unload()
        {
            Extractor = null;
            Interface = null;
        }

        internal void OpenInterface(BiomeExtractorEnt clicked)
        {
            SoundEngine.PlaySound(SoundID.MenuOpen);
            Extractor = clicked;
            UIHolder?.SetState(Interface);
            Interface = new ExtractorUI(); //TODO swap with update call
        }

        internal void CloseInterface()
        {
            if (UIHolder.CurrentState is not null)
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
                UIHolder.SetState(null);
            }
            Extractor = null;
        }

        internal string GetExtractorName()
        {
            return Extractor.LocalName;
        }

        internal string GetExtractorStatus()
        {
            return Extractor.GetStatus();
        }
    }
}
