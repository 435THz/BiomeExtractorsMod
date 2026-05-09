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
using System;
using System.Linq;

namespace BiomeExtractorsMod.Common.UI
{
    internal class UISystem : ModSystem
    {
        internal UserInterface UIHolder;
        internal ExtractorUI Interface;
        internal AnalyzerUI AnalyzerInterface;
        internal bool switching = false;
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
        private bool analyzer = false;
        internal bool isAnalyzer {get => isScanner && analyzer; }
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
            active = clicked.IsWorking;

            Interface.OnActivate();
            UIHolder?.SetState(Interface);
        }
        internal void OpenScannerInterface(Point16 position, ExtractionTier tier)
        {
            _pools = null;
            SoundEngine.PlaySound(SoundID.MenuOpen);

            Extractor = null;
            this.position = position;
            this.tier = tier;
            this.analyzer = false;
            this.active = true;

            Interface.OnActivate();
            UIHolder?.SetState(Interface);
        }
        internal void SwitchToAnalyzerInterface()
        {
            if(!isAnalyzer) return;
            switching = true;
            AnalyzerInterface.OnActivate();
            UIHolder?.SetState(AnalyzerInterface);
        }
        internal void SwitchToScannerInterface()
        {
            if(!isAnalyzer) return;
            switching = true;
            Interface.OnActivate();
            UIHolder?.SetState(Interface);
        }
        internal void OpenAnalyzerScannerInterface(Point16 position, ExtractionTier tier)
        {
            _pools = null;
            SoundEngine.PlaySound(SoundID.MenuOpen);

            Extractor = null;
            this.position = position;
            this.tier = tier;
            this.analyzer = true;
            this.active = true;

            Interface.OnActivate();
            UIHolder?.SetState(Interface);
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
                analyzer = false;
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
                        if (entry != "" && !entries.Contains(entry)) {
                            if (ModContent.GetInstance<ConfigClient>().DiagnosticPrintPools)
                                entry = $"{entry} ({pool.Name})";
                            entries.Add(entry);
                        }
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

        internal string GeneratePoolsText(int itemId)
        {
            BiomeExtractionSystem system = BiomeExtractionSystem.Instance;
            ScanData scan = new(tier, position, true); //empty dummy scan data. If the checks are set up correctly, the only actually important value should be tier

            List<PoolEntry> entries = system.GetPoolsOfItem((short)itemId);
            Dictionary<string, Dictionary<PoolEntry, string>> entrycollection = new(); //id of ancestor => <pool => subLocKey>
            string obfLoc = $"{BiomeExtractorsMod.LocAnalyzer}.Obfuscated";
            int obf_count = 0;
            
            foreach(PoolEntry pool in entries)
            {
                bool visible = true;
                bool obfuscated = false;
                foreach(Predicate<ScanData> check in pool.WorldChecks)
                {
                    if(!check.Invoke(scan))
                    {
                        visible = false;
                        break;
                    }
                }
                if(visible)
                {
                    foreach(KeyValuePair<string, string> parentEntry in pool.Parents)
                    {
                        List<PoolEntry> ancestors = GetBaseAncestorsOf(Instance.GetPoolEntry(parentEntry.Key), [pool.Name]);

                        string subLoc = parentEntry.Value;

                        foreach(Predicate<ScanData> req in pool.VisibilityRequirements)
                        {
                            if(!req.Invoke(scan))
                            {
                                obfuscated = true;
                                subLoc = obfLoc;
                                break;
                            }
                        }

                        foreach(PoolEntry ancestor in ancestors) {
                            if(entrycollection.ContainsKey(ancestor.Name))
                            {
                                if(!entrycollection[ancestor.Name].ContainsKey(ancestor)) // if the base ancestor is already inside the list, ignore this pool
                                {
                                    if(pool.Name == ancestor.Name)
                                    {
                                        entrycollection[ancestor.Name] = new(); // delete the entire list and only leave this pool in if it's the base ancestor
                                        entrycollection[ancestor.Name].Add(pool, obfuscated? obfLoc : "");
                                    }
                                    else
                                    {
                                        entrycollection[ancestor.Name].Add(pool, subLoc); // add to the list if this pool is not the base ancestor
                                    }
                                }
                            }
                            else
                            {
                                entrycollection.Add(ancestor.Name, new()); // initialize the new list with this pool

                                entrycollection[ancestor.Name].Add(pool, subLoc);
                            }
                        }
                    }
                }

            }

            Dictionary<string, string> string_entries = new(); //ancestor localization key => subset string

            List<string> order = entrycollection.Keys.ToList();
            order.Sort((a, b)=>Instance.GetPoolPriority(a).CompareTo(Instance.GetPoolPriority(b)));
            foreach(KeyValuePair<string, Dictionary<PoolEntry, string>> entry in entrycollection)
            {
                List<string> subCategory_list = [];
                PoolEntry ancestor = Instance.GetPoolEntry(entry.Key);
                if(!string_entries.ContainsKey(ancestor.LocalizationKey)) //if no list exists for this element yet, make one
                    string_entries.Add(ancestor.LocalizationKey, "");

                foreach(string subLocKey in entry.Value.Values)
                {
                    if(!subLocKey.IsWhiteSpace()) {
                        subCategory_list.Add(Language.GetTextValue(subLocKey));
                    }
                }
                if(subCategory_list.Find(str=>str!=obfLoc)==default)
                {
                    string_entries.Remove(ancestor.LocalizationKey);
                    obf_count++;
                }
                string_entries[ancestor.LocalizationKey] = string.Join(" ,", subCategory_list);
            }

            string final_string = "";
            foreach(KeyValuePair<string, string> string_entry in string_entries)
            {
                if(final_string.Length>0) final_string+="\n";
                final_string += Language.GetTextValue(string_entry.Key);
                if(!string_entry.Value.IsWhiteSpace())
                {
                    final_string += $" ({Language.GetTextValue(string_entry.Value)})";
                }
            }
            while(obf_count>0)
            {
                if(final_string.Length>0) final_string+="\n";
                final_string += Language.GetTextValue(obfLoc);
            }
            if(final_string.IsWhiteSpace()) return Language.GetTextValue($"{BiomeExtractorsMod.LocAnalyzer}.NoExtractions");
            return final_string;
        }

        private List<PoolEntry> GetBaseAncestorsOf(PoolEntry pool, List<string> checkd)
        {
            if(checkd.Contains(pool.Name) ) throw new Exception($"Looping parent relationship detected involving pool {pool.Name}. Cannot continue.");
            if(pool.Parents.Count == 0) return [pool];
            checkd.Add(pool.Name);
            List<PoolEntry> ancestors = new();
            foreach(string parent_id in pool.Parents.Keys)
            {
                PoolEntry parent = Instance.GetPoolEntry(parent_id);
                List<PoolEntry> grandparents = GetBaseAncestorsOf(parent, checkd);
                ancestors.AddRange(grandparents);
            }
            return ancestors;
        }
    }
}
