using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace SpiritcallerRoninMod.Content.Items.Accessories
{
    public class CursedBladeChunk : ModItem
    {
        private float AdditiveDamageBonus = 10f;
        private float AdditiveCrit = 5f;
        private float AdditiveAtkSpeed = 10f;

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 50);
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(AdditiveDamageBonus, AdditiveAtkSpeed, AdditiveCrit);
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // Increase Ronin damage by 5%
            player.GetModPlayer<GlobalPlayer>().RoninDamage += AdditiveDamageBonus /100f;
            
            // For attack speed, we need to modify the player's attackSpeed stat
            player.GetCritChance(DamageClass.Generic) += AdditiveCrit / 100f;
            player.GetAttackSpeed(DamageClass.Generic)  += AdditiveAtkSpeed /100f;
var modPlayer = player.GetModPlayer<RoninDashPlayer>();
    modPlayer.dashTabi = true;
    modPlayer.ignoreNextDash = true; // Mark this frame to ignore the first dash detection
    player.dashType = 1; // Enable vanilla dash detection
        }

        public override void AddRecipes()
        {
            CreateRecipe()
				.AddIngredient<CrimsonCrescent>()
                .AddIngredient(ItemID.DemoniteBar, 15)
                .AddIngredient(ItemID.SoulofNight, 15)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
            CreateRecipe()
				.AddIngredient<CrimsonCrescent>()
                .AddIngredient(ItemID.CrimtaneBar, 15)
                .AddIngredient(ItemID.SoulofNight, 15)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}