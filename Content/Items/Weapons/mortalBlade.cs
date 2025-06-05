using System.Collections.Generic;
using System.Linq;
using SpiritcallerRoninMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritcallerRoninMod.Content.Items.Weapons
{
	/// <summary>
	///     Star Wrath/Starfury style weapon. Spawn projectiles from sky that aim towards mouse.
	///     See Source code for Star Wrath projectile to see how it passes through tiles.
	///     For a detailed sword guide see <see cref="ExampleSword" />
	/// </summary>
	public class mortalBlade : ModItem
	{
		// Add this field at class level
		private bool alternateSlash;

		public override void SetDefaults() {
			Item.width = 100;
			Item.height = 100;
			
			//Item.holdStyle = ItemHoldStyleID.HoldGuitar;
			//Item.noUseGraphic = false;
			Item.useStyle = ItemUseStyleID.Swing;
			
			Item.useTime = 10; // Faster attack speed
			Item.useAnimation = 10;
			Item.autoReuse = true;
			
			Item.damage = 200;
			Item.knockBack = 3; // Lower knockback for faster hits
			
			Item.UseSound = SoundID.Item60;
			Item.DamageType = DamageClass.Melee;
			Item.knockBack = 6;
			Item.crit = 6;

			Item.value = Item.buyPrice(platinum: 1);
			Item.rare = ItemRarityID.Pink;

			Item.shoot = ModContent.ProjectileType<ImbuedKatanaSlash>(); // Default slash projectile
			Item.shootSpeed = 16f;

			// If you want melee speed to only affect the swing speed of the weapon and not the shoot speed (not recommended)
			// Item.attackSpeedOnlyAffectsWeaponAnimation = true;

			// Normally shooting a projectile makes the player face the projectile, but if you don't want that (like the beam sword) use this line of code
			// Item.ChangePlayerDirectionOnShoot = false;
		}
        // This method gets called when firing your weapon/sword.
		/*public override Vector2? HoldoutOffset()
		{
			return new Vector2(-20,5); // X=0 for no horizontal offset, Y=-20 to move the hold point up
		}*/
        

		// Add this field at class level
		// Remove the dashCooldown field
		// private int dashCooldown = 0;

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			float adjustedItemScale = player.GetAdjustedItemScale(Item);
			
			// Create afterimages
			
			// Alternate between slash types
			int projectileType =  ModContent.ProjectileType<CrimtaneKatanaSlash>();
			
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



		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
	}
}