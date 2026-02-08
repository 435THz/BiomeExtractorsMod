using BiomeExtractorsMod.Common.Database;
using BiomeExtractorsMod.Content.Items;
using BiomeExtractorsMod.Calamity.Content.Tiles;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using BiomeExtractorsMod.Calamity.Common;

namespace BiomeExtractorsMod.Calamity.Content.Items
{
    [ExtendsFromMod("CalamityMod")]
    [JITWhenModsEnabled("CalamityMod")]
    internal class AuricExtractorItem : BiomeExtractorItem
    {
        protected internal override int TileId => ModContent.TileType<AuricExtractorTile>();
        protected internal override ExtractorUpgradeKit UpgradeItemToCraftThis => ModContent.GetInstance<AuricUpgradeKit>();

        public override void SetDefaults()
        {
            base.SetDefaults();
            BiomeExtractionSystem.Instance.AddTier(ExtractionTiers.AURIC, $"{BiomeExtractorsMod.LocArticles}.Auric", BiomeExtractorsMod.LocExtractorSuffix("Auric"), delegate { return CalamityConfigs.Instance.AuricExtractorRate; }, delegate { return CalamityConfigs.Instance.AuricExtractorChance; }, delegate { return CalamityConfigs.Instance.AuricExtractorAmount; }, delegate { return Mod.Assets.Request<Texture2D>("Calamity/Content/MapIcons/AuricExtractorIcon"); });
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.value = Item.buyPrice(gold: 75); // sell at 15
        }
    }
}
