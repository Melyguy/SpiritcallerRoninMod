using System.Collections.Generic;
using System.Linq;
using KatanaMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace KatanaMod.Content.Items.Weapons
{
	/// <summary>
	///     Star Wrath/Starfury style weapon. Spawn projectiles from sky that aim towards mouse.
	///     See Source code for Star Wrath projectile to see how it passes through tiles.
	///     For a detailed sword guide see <see cref="ExampleSword" />
	/// </summary>
	public class DemoniteInfusedKatana : ModItem
	{
		// Add this field at class level
		private bool alternateSlash;

		public override void SetDefaults() {
			Item.width = 50;
			Item.height = 50;
			
			//Item.holdStyle = ItemHoldStyleID.HoldGuitar;
			//Item.noUseGraphic = false;
			Item.useStyle = ItemUseStyleID.Shoot;
			
			Item.useTime = 10; // Faster attack speed
			Item.useAnimation = 10;
			Item.autoReuse = true;
			
			Item.damage = 40;
			Item.knockBack = 3; // Lower knockback for faster hits
			
			Item.UseSound = SoundID.Item60;
			Item.DamageType = DamageClass.Melee;
			Item.knockBack = 6;
			Item.crit = 6;

			Item.value = Item.buyPrice(gold: 5);
			Item.rare = ItemRarityID.Pink;

			Item.shoot = ModContent.ProjectileType<ImbuedKatanaSlash>(); // Default slash projectile
			Item.shootSpeed = 16f;

			// If you want melee speed to only affect the swing speed of the weapon and not the shoot speed (not recommended)
			// Item.attackSpeedOnlyAffectsWeaponAnimation = true;

			// Normally shooting a projectile makes the player face the projectile, but if you don't want that (like the beam sword) use this line of code
			// Item.ChangePlayerDirectionOnShoot = false;
		}
        // This method gets called when firing your weapon/sword.
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var linetochange = tooltips.FirstOrDefault(x => x.Name == "Damage" && x.Mod == "Terraria");
            if (linetochange != null)
            {
                string[] splittext = linetochange.Text.Split(' ');
                linetochange.Text = splittext.First() + " Ronin " + splittext.Last();
            }
        }
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage += player.GetModPlayer<GlobalPlayer>().RoninDamage;
        }
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-20,5); // X=0 for no horizontal offset, Y=-20 to move the hold point up
		}
        

		// Add this field at class level
		// Remove the dashCooldown field
		// private int dashCooldown = 0;

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			float adjustedItemScale = player.GetAdjustedItemScale(Item);
			
			// Create afterimages
			for (int i = 0; i < 3; i++) {
				Vector2 offset = new Vector2(player.direction * i * -5, 0);
				Dust.NewDustPerfect(player.MountedCenter + offset, DustID.BlueTorch, Vector2.Zero, 100, Color.White, 1.5f);
			}
			
			// Alternate between slash types
			int projectileType = alternateSlash ? 
				ModContent.ProjectileType<DemoniteInfusedSlash2 >() : 
				ModContent.ProjectileType<DemoniteInfusedSlash>();
			
			// Multiple slashes
			for (int i = 0; i < 2; i++) {
				Vector2 perturbedSpeed = new Vector2(player.direction, 0f).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-15, 15)));
				Projectile.NewProjectile(source, player.MountedCenter, perturbedSpeed, projectileType, damage, knockback, 
					player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax, adjustedItemScale);
			}

			// Toggle the slash type for next shot
			alternateSlash = !alternateSlash;

			return false; // Don't fire the original projectile
		}

		public override void HoldItem(Player player) {
			// Add floating dust effect when holding
			if (Main.rand.NextBool(20)) {
				Dust.NewDustPerfect(player.Center + new Vector2(player.direction * 20, 0), 
					DustID.BlueTorch, new Vector2(0, -1f), 100, Color.White, 0.8f);
			}
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient<EvilSealingSheath>();
			recipe.AddIngredient(ItemID.DemoniteBar, 10);
            recipe.AddIngredient(ItemID.FallenStar, 10);
            recipe.AddIngredient(ItemID.RottenChunk, 10);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		
			recipe = CreateRecipe();
			recipe.AddIngredient<CrimtaneInfusedKatana>();
            recipe.AddIngredient(ItemID.FallenStar, 30);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}