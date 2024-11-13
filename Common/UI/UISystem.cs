using BiomeExtractorsMod.Content.TileEntities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using BiomeExtractorsMod.Common.Database;
using BiomeExtractorsMod.Common.Collections;
using BiomeExtractorsMod.Common.Configs;
using static BiomeExtractorsMod.Common.Database.BiomeExtractionSystem;
using Terraria.Localization;
using BiomeExtractorsMod.Content.Items;
using BiomeExtractorsMod.Common.Players;

namespace BiomeExtractorsMod.Common.UI
{
    internal class UISystem : ModSystem
    {
        internal UserInterface UIHolder;
        internal ExtractorUI Interface;
        internal BiomeExtractorEnt Extractor;
        private List<PoolEntry> _pools;
        internal List<PoolEntry> PoolList { get
            {
                _pools ??= ModContent.GetInstance<BiomeExtractionSystem>().CheckValidBiomes(tier, position, isScanner);
                return _pools;
            }
        }

        internal Point16 position;
        internal ExtractionTier tier;
        internal bool isScanner { get => Extractor == null; }
        internal bool active;

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
            if (UIHolder?.CurrentState != null) UIHolder.Update(gameTime);
            if (Extractor is null)
            {
                if (Main.LocalPlayer.HeldItem.type != ModContent.GetInstance<BiomeScanner>().Type) CloseInterface();
            }
            else if (!Extractor.IsTileValidForEntity(position.X, position.Y) || !ExtractorPlayer.LocalPlayer.IsInRectangleRange(position)) CloseInterface();
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
            position = Point16.Zero;
            tier = null;
            active = false;
        }

        internal void OpenExtractorInterface(BiomeExtractorEnt clicked)
        {
            _pools = null;
            SoundEngine.PlaySound(SoundID.MenuOpen);

            Extractor = clicked;
            position = clicked.Position;
            tier = clicked.ExtractionTier;
            active = clicked.Active;

            UIHolder?.SetState(Interface);
            Interface.OnActivate();
        }
        internal void OpenScannerInterface(Point16 position, ExtractionTier tier)
        {
            _pools = null;
            SoundEngine.PlaySound(SoundID.MenuOpen);

            Extractor = null;
            this.position = position;
            this.tier = tier;
            this.active = true;

            UIHolder?.SetState(Interface);
            Interface.OnActivate();
        }

        internal WeightedList<ItemEntry> GetDropList()
        {
            BiomeExtractionSystem system = ModContent.GetInstance<BiomeExtractionSystem>();
            if (Extractor is not null) return Extractor.GetDropList();
            else return system.JoinPools(PoolList);
        }

        internal void CloseInterface()
        {
            if (UIHolder.CurrentState is not null)
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
                UIHolder.SetState(null);
                Extractor = null;
                position = Point16.Zero;
                tier = null;
                active = false;
            }
        }

        internal string GetWindowTitle()
        {
            if (Extractor != null) return Extractor.LocalName;
            return tier.Name;
        }

        internal string GetExtractorStatus()
        {
            if (Extractor is not null) return Extractor.GetStatus();
            else return GenerateStatus();
        }

        private string GenerateStatus()
        {
            string su = $"{BiomeExtractorsMod.LocDiagnostics}.Scanner";
            if (PoolList.Count > 0)
            {
                List<string> entries = [];
                List<string> backup = [];
                string s = "";
                for (int i = 0; i < PoolList.Count; i++)
                {
                    PoolEntry pool = PoolList[i];
                    backup.Add(pool.Name);
                    if (pool.IsLocalized())
                    {
                        string key = pool.LocalizationKey;
                        string entry = Language.GetTextValue(key);
                        if (entry != "" && !entries.Contains(entry)) entries.Add(entry);
                        else if (ModContent.GetInstance<ConfigClient>().DiagnosticPrintPools)
                            entries.Add(pool.Name);
                    }
                    else if (ModContent.GetInstance<ConfigClient>().DiagnosticPrintPools)
                    {
                        entries.Add(pool.Name);
                    }
                }

                List<string> list = entries;
                if (entries.Count == 0) list = backup;
                for (int i = 0; i < list.Count; i++)
                {
                    s += list[i];
                    if (i < list.Count - 1) s += ", ";
                }
                return Language.GetTextValue($"{su}Success") + " " + s;
            }
            else return Language.GetTextValue($"{su}Fail");
        }
    }
}
