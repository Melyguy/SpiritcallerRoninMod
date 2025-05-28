using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using SpiritcallerRoninMod.Content.Projectiles;
using System.Collections.Generic;
using System.Linq;

namespace SpiritcallerRoninMod.Content.Items.Weapons
{
    public class WailingEclipse : ModItem
    {
        private bool alternateSlash = false;

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.DamageType = DamageClass.Magic; // Your custom damage class can replace this
            Item.width = 55;
            Item.height = 48;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(silver: 75);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item71;
            Item.autoReuse = true;
            Item.noMelee = false; // Scythe does melee
            Item.shoot = ModContent.ProjectileType<EclipseCrescentProjectile>(); // Optional wisp
            Item.shootSpeed = 20f;
            Item.mana = 5;
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
                ModContent.ProjectileType<EclipseSlash>() : 
                ModContent.ProjectileType<EclipseSlash>();

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
			recipe.AddIngredient<ReapersHunger>();
            recipe.AddIngredient(ItemID.ChlorophyteBar, 15);
            recipe.AddIngredient(ItemID.ShroomiteBar, 15);
            recipe.AddIngredient(ItemID.BeetleHusk, 5); // or CrimtaneBar
            recipe.AddIngredient(ItemID.SpectreBar, 20);
            recipe.AddIngredient(ItemID.BrokenHeroSword, 1); // or Tissue Sample
            recipe.AddTile(TileID.Anvils); // Or Crimson Altar
            recipe.Register();
        
            
        }
    }
}
