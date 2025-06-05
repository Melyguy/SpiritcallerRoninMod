using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using SpiritcallerRoninMod.Content.Items.Placeable;

namespace SpiritcallerRoninMod.Content.Items.Accessories
{
    public class MastersTsuba : ModItem
    {
        private float AdditiveDamageBonus = 30f;
        private float AdditiveAtkSpeed = 25f;

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
                .AddIngredient(ModContent.ItemType<ZuuniteBar>(), 20)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}