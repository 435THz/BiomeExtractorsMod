using Terraria.ModLoader;

namespace BiomeExtractorsMod
{
    public class BiomeExtractorsMod : Mod
	{
        static internal bool MS_loaded;

        public override void Load()
        {
            MS_loaded = ModLoader.HasMod("MagicStorage");
        }
    }
}