using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using SpiritcallerRoninMod.Content.Projectiles;
using System.Collections.Generic;
using System.Linq;
using SpiritcallerRoninMod.Content.Tiles.Furniture;

namespace SpiritcallerRoninMod.Content.Items.Weapons
{
    public class GravebornCrescent : ModItem
    {
        private bool alternateSlash = false;
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.damage = 150;
            Item.DamageType = DamageClass.Magic; // Replace with your custom SpiritcallerDamageClass
            Item.width = 58;
            Item.height = 58;
			Item.scale = 1.2f; // Extreme scale down for 500x500 texture
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6.5f;
            Item.value = Item.sellPrice(platinum: 1);
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item71;
            Item.UseSound = SoundID.Item124;
            Item.autoReuse = true;
            Item.noMelee = false;
            Item.mana = 12;

            Item.shoot = ModContent.ProjectileType<Crescentslash>();
            Item.shootSpeed = 20f;
        }
                    public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var linetochange = tooltips.FirstOrDefault(x => x.Name == "Damage" && x.Mod == "Terraria");
            if (linetochange != null)
            {
                string[] splittext = linetochange.Text.Split(' ');
                linetochange.Text = splittext.First() + " SpiritCaller " + splittext.Last();
            }
        }
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage += player.GetModPlayer<GlobalPlayer>().SpiritCallerDamage;
        }

               public override bool? UseItem(Player player)
        {
            // 1 in 2 chance to release a soul wisp
            if (Main.rand.NextBool(2))
            {
                Vector2 velocity = Vector2.Normalize(Main.MouseWorld - player.Center) * Item.shootSpeed;
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, velocity, Item.shoot, Item.damage / 2, 0f, player.whoAmI);
            }

            // Create a slash projectile
            float adjustedItemScale = player.GetAdjustedItemScale(Item);
            int projectileType = alternateSlash ? 
                ModContent.ProjectileType<HollowStarMelee>() : 
                ModContent.ProjectileType<HollowStarMelee>();

            // Create two slashes for a combo effect
            for (int i = 0; i < 2; i++)
            {
                Vector2 perturbedSpeed = new Vector2(player.direction, 0f).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-15, 15)));
                Projectile.NewProjectile(
                    player.GetSource_ItemUse(Item),
                    player.Center,
                    perturbedSpeed,
                    projectileType,
                    Item.damage,
                    Item.knockBack,
                    player.whoAmI,
                    player.direction * player.gravDir,
                    player.itemAnimationMax,
                    adjustedItemScale
                );
            }

            // Toggle the slash type for next shot
            alternateSlash = !alternateSlash;

            return base.UseItem(player);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<ScytheOfTheHollowStar>(), 1);
            recipe.AddIngredient(ItemID.FragmentNebula, 30);
            recipe.AddIngredient(ItemID.LunarBar, 20);
            recipe.AddIngredient(ItemID.SoulofNight, 10);
            recipe.AddIngredient(ItemID.SoulofMight, 10);
            recipe.AddIngredient(ItemID.SoulofFright, 10);
            recipe.AddIngredient(ItemID.SoulofSight, 10);

			recipe.AddTile(ModContent.TileType<ZuuniteAnvil>());
            recipe.Register();
        }
    }
}
