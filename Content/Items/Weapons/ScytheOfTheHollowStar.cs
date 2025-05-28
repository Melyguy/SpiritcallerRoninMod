using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using SpiritcallerRoninMod.Content.Projectiles;

namespace SpiritcallerRoninMod.Content.Items.Weapons
{
    public class ScytheOfTheHollowStar : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.damage = 100;
            Item.DamageType = DamageClass.Magic; // Replace with your custom SpiritcallerDamageClass
            Item.width = 58;
            Item.height = 58;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6.5f;
            Item.value = Item.sellPrice(platinum: 1);
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item71;
            Item.autoReuse = true;
            Item.noMelee = false;
            Item.mana = 12;

            Item.shoot = ModContent.ProjectileType<HollowStarSlash>();
            Item.shootSpeed = 15f;
        }

        public override bool? UseItem(Player player)
        {
            // Fires a void slash toward the cursor
            Vector2 direction = Vector2.Normalize(Main.MouseWorld - player.Center) * Item.shootSpeed;
            Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, direction,
                Item.shoot, Item.damage, Item.knockBack, player.whoAmI);
            return true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<WailingEclipse>(), 1);
            recipe.AddIngredient(ItemID.FragmentNebula, 10);
            recipe.AddIngredient(ItemID.LunarBar, 8);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
    }
}
