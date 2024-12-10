using BiomeExtractorsMod.Calamity.Common;
using BiomeExtractorsMod.Content.TileEntities;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Calamity.Content.TileEntities
{
    [ExtendsFromMod("CalamityMod")]
    [JITWhenModsEnabled("CalamityMod")]
    public abstract class BiomeExtractorEntAbyss : BiomeExtractorEnt
    {
        protected internal bool PressureLock { get; private set; } = false;

        public override void Update()
        {
            Point point = Position.ToPoint() + new Point(1, 1);
            bool Locked = PressureLock;
            PressureLock = !BiomeChecker.IsInAbyssArea(point) || !BiomeChecker.IsSubmerged(point);
            if (Locked && !PressureLock)
                UpdatePoolList(); //must update status if back in water

            if (!PressureLock)
                base.Update();
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(PressureLock);
            base.NetSend(writer);
        }

        public override void NetReceive(BinaryReader reader)
        {
            PressureLock = reader.ReadBoolean();
            base.NetReceive(reader);
        }
    }
}
