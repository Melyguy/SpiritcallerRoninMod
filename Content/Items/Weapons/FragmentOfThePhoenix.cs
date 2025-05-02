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
	public class FragmentOfThePhoenix : ModItem
	{
		// Add this field at class level
		private bool alternateSlash;

		public override void SetDefaults() {
			Item.width = 26;
			Item.height = 28;
			Item.scale = 0.16f; // Extreme scale down for 500x500 texture
			
			//Item.holdStyle = ItemHoldStyleID.HoldGuitar;
			//Item.noUseGraphic = false;
			Item.useStyle = ItemUseStyleID.Shoot;
			
			Item.useTime = 4; // Faster attack speed
			Item.useAnimation = 4;
			Item.autoReuse = true;
			
			Item.damage = 250; // Increased damage to reflect combined power
			Item.useTime = 12; // Slightly slower but more powerful
			Item.useAnimation = 12;
			Item.knockBack = 3; // Lower knockback for faster hits
			
			Item.UseSound = SoundID.Item88;
			Item.DamageType = DamageClass.Melee;
			Item.knockBack = 6;
			Item.crit = 6;

			Item.value = Item.buyPrice(gold: 5);
			Item.rare = ItemRarityID.Orange;

			Item.shoot = ModContent.ProjectileType<ImbuedKatanaSlash>(); // Default slash projectile
			Item.shootSpeed = 12f; // Reset to normal speed as we'll multiply in Shoot method
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
			return new Vector2(-260, 6); // Adjusted X to hold more towards the blade
		}

		// Change the alternateSlash from bool to int to track multiple states
		private int slashCounter = 0;

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			// Create spectral afterimages
			for (int i = 0; i < 3; i++) {
				Vector2 offset = new Vector2(player.direction * i * -5, 0);
				// Spectral essence
				Dust.NewDustPerfect(player.MountedCenter + offset, DustID.Chlorophyte, Vector2.Zero, 100, Color.Green, 1.5f);
				// Shroomite trail
				Dust.NewDustPerfect(player.MountedCenter + offset, DustID.BlueTorch, Vector2.Zero, 100, Color.Blue, 1.5f);
			}
			
			Vector2 mousePosition = Main.MouseWorld;
			Vector2 direction = mousePosition - player.MountedCenter;
			direction.Normalize();
			
			int projectileType;
			int extraProjectileType;
			bool shootExtra = true;
			
			// Cycle representing the materials used
			switch (slashCounter) {
				case 0: // Chlorophyte's Nature
					projectileType = ModContent.ProjectileType<PhoenixSlash2>();
					extraProjectileType = ProjectileID.DD2PhoenixBowShot;
					break;
				case 1: // Spectre's Spirit
					projectileType = ModContent.ProjectileType<PhoenixSlash>();
					extraProjectileType = ProjectileID.DD2PhoenixBowShot;
					break;
				case 2: // Shroomite's Precision
					projectileType = ModContent.ProjectileType<PhoenixSlash2>();
					extraProjectileType = ProjectileID.DD2PhoenixBowShot; // Shroomite-like precision
					break;
                case 3: // Shroomite's Precision
					projectileType = ModContent.ProjectileType<PhoenixSlash>();
					extraProjectileType = ProjectileID.DD2PhoenixBowShot; // Shroomite-like precision
					break;
                
				default: // Ectoplasm's Power
					projectileType = ModContent.ProjectileType<PhoenixSlash2>();
					extraProjectileType = ProjectileID.DD2PhoenixBowShot;
					break;
			}
			
			// Enhanced projectile system
			for (int i = 0; i < 2; i++) {
				// Main slash
				Vector2 slashSpeed = direction.RotatedBy(MathHelper.ToRadians(i == 0 ? -10 : 10)) * 24f;
				Projectile.NewProjectile(source, player.MountedCenter, slashSpeed, projectileType, damage, knockback, 
					player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax, 1.8f);
				
				// Material-themed projectiles
				if (shootExtra) {
					// First extra projectile
					Vector2 extraSpeed = direction.RotatedBy(MathHelper.ToRadians(i == 0 ? 5 : -5)) * 20f;
					Projectile.NewProjectile(source, player.MountedCenter, extraSpeed, extraProjectileType, damage * 3/4, knockback, 
						player.whoAmI);
					
					// Spectral homing projectile (represents souls)
					if (Main.rand.NextBool(2)) {
						Vector2 spectralSpeed = direction.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-30, 31))) * 16f;
						Projectile.NewProjectile(source, player.MountedCenter, spectralSpeed, ProjectileID.LostSoulFriendly, 
							damage / 2, knockback / 2, player.whoAmI);
					}
				}
			}

			slashCounter = (slashCounter + 1) % 5;
			return false;
		}

		public override void HoldItem(Player player) {
			// Balanced light and dark effects
			if (Main.rand.NextBool(20)) {
				// Light essence
				Dust.NewDustPerfect(player.Center + new Vector2(player.direction * 20, 0), 
					DustID.HallowedTorch, new Vector2(0, -1f), 100, Color.White, 0.8f);
				// Dark essence
				Dust.NewDustPerfect(player.Center + new Vector2(player.direction * -20, 0), 
					DustID.ShadowbeamStaff, new Vector2(0, -1f), 100, Color.Purple, 0.8f);
			}
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient<KatanaOfTrueVirtue>();
			recipe.AddIngredient<KatanaOfPureEvil>();
			recipe.AddIngredient<KatanaOfOrder>();
			recipe.AddIngredient(ItemID.ChlorophyteBar, 10);
			recipe.AddIngredient(ItemID.ShroomiteBar, 10);
			recipe.AddIngredient(ItemID.SpectreBar, 10);
            recipe.AddIngredient(ItemID.Ectoplasm, 20);
            recipe.AddIngredient(ItemID.SoulofNight, 10);
            recipe.AddIngredient(ItemID.SoulofLight, 10);
            recipe.AddIngredient(ItemID.SoulofFright, 10);
            recipe.AddIngredient(ItemID.SoulofSight, 10);
            recipe.AddIngredient(ItemID.SoulofMight, 10);
            recipe.AddIngredient(ItemID.SoulofFlight, 10);
			recipe.AddIngredient(ItemID.Torch, 200);
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.Register();
            
		}
	}
}