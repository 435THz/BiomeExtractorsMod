using BiomeExtractorsMod.Common.Configs;
using BiomeExtractorsMod.Content.TileEntities;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using System.Linq;
using BiomeExtractorsMod.Common.Systems;
using Terraria.DataStructures;
using BiomeExtractorsMod.Content.Tiles;

namespace BiomeExtractorsMod
{
    internal enum ClientMessageType {UPDATE_REQUEST, KILL_REQUEST}
    internal enum ServerMessageType {EXTRACTOR_REGISTER, EXTRACTOR_UPDATE, EXTRACTOR_REMOVE}


    public class BiomeExtractorsMod : Mod
    {
        public static BiomeExtractorsMod Instance => ModContent.GetInstance<BiomeExtractorsMod>();
        static string GetModName(SupportedMods mod) => Enum.GetName(typeof(SupportedMods), mod);
        enum SupportedMods
        {
            MagicStorage
        }

        static readonly private Dictionary<SupportedMods, bool> supportedModsLoaded = [];
        static internal bool MS_loaded => supportedModsLoaded[SupportedMods.MagicStorage] && ModContent.GetInstance<ConfigCompat>().MS;

        private static readonly object LocBase = "Mods.BiomeExtractorsMod";

        private static string LocItemsCategory => $"{LocBase}.Items";
        public static string LocExtractorPrefix => $"{LocItemsCategory}.BiomeExtractorItem";
        private static string LocConfigCategory => $"{LocBase}.Config";
        public static string LocClientConfig => $"{LocConfigCategory}.ConfigClient";
        public static string LocCommonConfig => $"{LocConfigCategory}.ConfigCommon";
        public static string LocCompatConfig => $"{LocConfigCategory}.ConfigCompat";

        private static string LocTextCategory => $"{LocBase}.Text";
        public static string LocDiagnostics => $"{LocTextCategory}.Diagnostics";
        public static string LocPoolNames => $"{LocTextCategory}.PoolNames";
        public static object LocArticles => $"{LocTextCategory}.Articles";
        private static string LocMapCategory => $"{LocBase}.Map";
        public static string LocMapTileName => $"{LocMapCategory}.TileName";
        public static string LocIconInactive => $"{LocMapCategory}.IconInactive";
        public static string LocIconNoOutput => $"{LocMapCategory}.IconNoOutput";
        public static string LocIconError => $"{LocMapCategory}.IconError";

        public override void Load()
        {
            foreach(SupportedMods mod in Enum.GetValues(typeof(SupportedMods)))
            supportedModsLoaded[mod] = ModLoader.HasMod(GetModName(mod));
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ServerMessageType msgType = (ServerMessageType)reader.ReadByte();
                switch(msgType)
                {
                    case ServerMessageType.EXTRACTOR_REGISTER:
                        Point16 toAdd = new(reader.ReadInt16(), reader.ReadInt16());
                        int tier = reader.ReadInt32();
                        ExtractorState state = (ExtractorState)reader.ReadByte();
                        IconMapSystem.AddExtractorData(toAdd, tier, state);
                        break;
                    case ServerMessageType.EXTRACTOR_UPDATE:
                        Point16 toUpdate = new(reader.ReadInt16(), reader.ReadInt16());
                        ExtractorState newState = (ExtractorState)reader.ReadByte();
                        IconMapSystem.UpdateExtractorData(toUpdate, newState);
                        break;
                    case ServerMessageType.EXTRACTOR_REMOVE:
                        Point16 toRemove = new(reader.ReadInt16(), reader.ReadInt16());
                        IconMapSystem.RemoveExtractorData(toRemove);
                        break;
                }
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                ClientMessageType msgType = (ClientMessageType)reader.ReadByte();
                switch (msgType)
                {
                    case ClientMessageType.KILL_REQUEST:
                        int x = reader.ReadInt32(), y = reader.ReadInt32();
                        if(TileUtils.TryGetTileEntityAs(x, y, out BiomeExtractorEnt ent))
                            ent.Kill(x, y);
                        break;
                    case ClientMessageType.UPDATE_REQUEST:
                        IEnumerable<BiomeExtractorEnt> extractors = TileEntity.ByPosition
                            .Where(pair => pair.Value is BiomeExtractorEnt)
                            .Select(pair => pair.Value as BiomeExtractorEnt);
                        List<BiomeExtractorEnt> toSend = extractors.ToList();
                        foreach (BiomeExtractorEnt extractor in toSend)
                            extractor.SendUpdatePacket(ServerMessageType.EXTRACTOR_UPDATE);
                        break;
                }
            }
        }
    }
}