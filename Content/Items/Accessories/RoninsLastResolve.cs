using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace SpiritcallerRoninMod.Content.Items.Accessories
{
    public class RoninsLastResolve : ModItem
    {
        private float AdditiveDamageBonus = 5f;
        private float AdditiveAtkSpeed = 10f;
        private float LowHealthThreshold = 0.25f; // 25% health threshold
        private float CritMultiplier = 2f; // Double crit chance

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 50);
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(AdditiveDamageBonus, AdditiveAtkSpeed, (int)(LowHealthThreshold * 100));

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GlobalPlayer>().RoninDamage += AdditiveDamageBonus /100f;
            player.GetAttackSpeed(DamageClass.Generic) += AdditiveAtkSpeed /100f;

            // Add critical strike bonus when health is low
            if ((float)player.statLife / player.statLifeMax2 <= LowHealthThreshold)
            {
                player.GetCritChance(DamageClass.Generic) *= CritMultiplier;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Bone, 40)
                .AddIngredient(ItemID.Shuriken, 20)
                .AddIngredient(ItemID.CrimtaneBar, 5)
                .AddTile(TileID.Anvils)
                .Register();
                        CreateRecipe()
                .AddIngredient(ItemID.Bone, 40)
                .AddIngredient(ItemID.Shuriken, 20)
                .AddIngredient(ItemID.DemoniteBar, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}