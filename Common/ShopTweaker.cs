using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BiomeExtractorsMod.Common.Shop
{
    internal class ShopTweaker : GlobalNPC
    {
        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType == NPCID.Mechanic)
            {
                shop.Add(new Item(ItemID.Extractinator));
            }
        }
    }
}
