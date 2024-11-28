using BiomeExtractorsMod.Common.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Commands
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
