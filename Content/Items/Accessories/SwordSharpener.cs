using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace SpiritcallerRoninMod.Content.Items.Accessories
{
    public class SwordSharpener : ModItem
    {
        private float AdditiveDamageBonus = 5f;
        private float AdditiveAtkSpeed = 5f;

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 50);
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(AdditiveDamageBonus, AdditiveAtkSpeed);
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // Increase Ronin damage by 5%
            player.GetModPlayer<GlobalPlayer>().RoninDamage += AdditiveDamageBonus /100f;
            
            // For attack speed, we need to modify the player's attackSpeed stat
            player.GetAttackSpeed(DamageClass.Generic)  += AdditiveAtkSpeed /100f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.StoneBlock, 10)
                .AddIngredient(ItemID.IronBar, 5)
                .AddTile(TileID.Anvils)
                .Register();
                        CreateRecipe()
                .AddIngredient(ItemID.StoneBlock, 10)
                .AddIngredient(ItemID.LeadBar, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}