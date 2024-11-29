using BiomeExtractorsMod.Common.Database;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Common.Commands
{
    internal class PrintPoolsCommand : ModCommand
    {
        public override string Command => "printpools";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            BiomeExtractionSystem.PrintPools(caller);
        }
    }
}
