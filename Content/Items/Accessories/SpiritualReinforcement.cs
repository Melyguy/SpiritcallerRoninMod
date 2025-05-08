using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace SpiritcallerRoninMod.Content.Items.Accessories
{
    public class SpiritualReinforcement : ModItem
    {
        private float AdditiveDamageBonus = 10f;
        private int AdditiveMana = 40;

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 50);
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(AdditiveDamageBonus, AdditiveMana);
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // Increase Ronin damage by 5%
            player.GetModPlayer<GlobalPlayer>().SpiritCallerDamage += AdditiveDamageBonus /100f;
            
            // For attack speed, we need to modify the player's attackSpeed stat
            // Increase player's max mana
            player.statManaMax2 += AdditiveMana;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
				.AddIngredient<SpiritualAwakening>()
                .AddIngredient(ItemID.IronBar, 10)
                .AddIngredient(ItemID.FallenStar, 3)
                .AddTile(TileID.Anvils)
                .Register();

                        CreateRecipe()
				.AddIngredient<SpiritualAwakening>()
                .AddIngredient(ItemID.LeadBar, 10)
                .AddIngredient(ItemID.FallenStar, 3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}