using BiomeExtractorsMod.Common.Database;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Common.Hooks
{
    public abstract class ExtractionSystemExtension : ModSystem
    {
        private static readonly Dictionary<Type, ExtractionSystemExtension> Extensions = [];
        protected static BiomeExtractionSystem BES => BiomeExtractionSystem.Instance;

        protected virtual bool CanLoad() => true;

        internal static void LoadExtensions()
        {
            foreach (var extension in Extensions.Values)
                extension.LoadDatabase();
        }

        public override sealed void OnModLoad()
        {
            if (CanLoad())
                Extensions[GetType()] = this;
        }

        public override sealed void OnModUnload()
        {
            Extensions.Remove(GetType());
        }

        public abstract void LoadDatabase();
    }
}
