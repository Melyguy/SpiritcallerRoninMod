using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace SpiritcallerRoninMod.Content.Items.Accessories
{
    public class SpiritualArmoring : ModItem
    {
        private float AdditiveDamageBonus = 15f;
        private int AdditiveMana = 60;
        private int AdditiveDefense = 4;

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 50);
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(AdditiveDamageBonus, AdditiveMana, AdditiveDefense);
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GlobalPlayer>().SpiritCallerDamage += AdditiveDamageBonus /100f;
            player.statManaMax2 += AdditiveMana;
            player.statDefense += AdditiveDefense;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
				.AddIngredient<SpiritualAwakening>()
                .AddIngredient(ItemID.CrimtaneBar, 10)
                .AddIngredient(ItemID.FallenStar, 3)
                .AddTile(TileID.Anvils)
                .Register();

                        CreateRecipe()
				.AddIngredient<SpiritualAwakening>()
                .AddIngredient(ItemID.DemoniteBar, 10)
                .AddIngredient(ItemID.FallenStar, 3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}