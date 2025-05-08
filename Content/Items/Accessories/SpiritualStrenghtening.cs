using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace SpiritcallerRoninMod.Content.Items.Accessories
{
    public class SpiritualStrengthening : ModItem
    {
        private float AdditiveDamageBonus = 20f;
        private int AdditiveMana = 80;
        private int AdditiveDefense = 15;

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
            // Increase Ronin damage by 5%
            player.GetModPlayer<GlobalPlayer>().SpiritCallerDamage += AdditiveDamageBonus /100f;
            
            // For attack speed, we need to modify the player's attackSpeed stat
            // Increase player's max mana
            player.statManaMax2 += AdditiveMana;
            player.statDefense += AdditiveDefense;
        }

        public override void AddRecipes()
        {
                   CreateRecipe()
				.AddIngredient<SpiritualArmoring>()
                .AddIngredient(ItemID.CobaltBar, 10)
                .AddIngredient(ItemID.SoulofLight, 3)
                .AddTile(TileID.Anvils)
                .Register();

                        CreateRecipe()
				.AddIngredient<SpiritualArmoring>()
                .AddIngredient(ItemID.PalladiumBar, 10)
                .AddIngredient(ItemID.SoulofLight, 3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}