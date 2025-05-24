using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace SpiritcallerRoninMod.Content.Items.Accessories
{
    public class SeveredOniHead : ModItem
    {
        private float AdditiveDamageBonus = 20f;
        private float AdditiveAtkSpeed = 20f;

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
                .AddIngredient<OniBottle>()
                .AddIngredient(ItemID.BloodHamaxe, 1)
                .AddIngredient(ItemID.SoulofNight, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}