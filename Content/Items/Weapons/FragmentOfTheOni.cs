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
	public class FragmentOfTheOni : ModItem
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
			
			Item.useTime = 2; // Faster attack speed
			Item.useAnimation = 2;
			Item.autoReuse = true;
			
			Item.damage = 320; // Increased damage for demonic power
			Item.useTime = 15; // Slightly slower for more impactful hits
			Item.useAnimation = 15;
			Item.knockBack = 8; // Increased knockback for oni strength
			
			Item.UseSound = SoundID.Item119; // More demonic sound
			Item.DamageType = DamageClass.Melee;
			Item.crit = 10; // Increased crit for savage attacks
			
			Item.value = Item.buyPrice(gold: 5);
			Item.rare = ItemRarityID.Red;

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
			// Create oni essence effects
			for (int i = 0; i < 3; i++) {
				Vector2 offset = new Vector2(player.direction * i * -5, 0);
				// Demonic essence
				Dust.NewDustPerfect(player.MountedCenter + offset, DustID.Shadowflame, Vector2.Zero, 100, Color.Purple, 1.5f);
				// Blood essence
				Dust.NewDustPerfect(player.MountedCenter + offset, DustID.Blood, Vector2.Zero, 100, Color.Red, 1.5f);
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
					projectileType = ModContent.ProjectileType<CrimtaneKatanaSlash>();
					extraProjectileType = ProjectileID.CursedFlameFriendly;
					break;
				case 1: // Spectre's Spirit
					projectileType = ModContent.ProjectileType<CrimtaneKatanaSlash2>();
					extraProjectileType = ProjectileID.GoldenShowerFriendly;
					break;
                
				default: // Ectoplasm's Power
					projectileType = ModContent.ProjectileType<CrimtaneKatanaSlash2>();
					extraProjectileType = ProjectileID.InfernoFriendlyBolt;
					break;
			}
			
			// Enhanced projectile system
			for (int i = 0; i < 2; i++) {
				// Main slash
				Vector2 slashSpeed = direction.RotatedBy(MathHelper.ToRadians(i == 0 ? -10 : 10)) * 24f;
				Projectile.NewProjectile(source, player.MountedCenter, slashSpeed, projectileType, damage, knockback, 
					player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax, 1.9f);
				
				// Material-themed projectiles
				if (shootExtra) {
					// First extra projectile
					Vector2 extraSpeed = direction.RotatedBy(MathHelper.ToRadians(i == 0 ? 5 : -5)) * 20f;
					Projectile.NewProjectile(source, player.MountedCenter, extraSpeed, extraProjectileType, damage * 3/4, knockback, 
						player.whoAmI);
					
					// Spectral homing projectile (represents souls)
				}
			}

			slashCounter = (slashCounter + 1) % 3;
			return false;
		}

		public override void HoldItem(Player player) {
			if (Main.rand.NextBool(20)) {
				// Demonic aura
				Dust.NewDustPerfect(player.Center + new Vector2(player.direction * 20, 0), 
					DustID.Shadowflame, new Vector2(0, -1f), 100, Color.Purple, 1f);
				// Blood essence
				Dust.NewDustPerfect(player.Center + new Vector2(player.direction * -20, 0), 
					DustID.Blood, new Vector2(0, -1f), 100, Color.Red, 1f);
			}
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient<KatanaOfPureEvil>();
			recipe.AddIngredient<KatanaOfOrder>();
			recipe.AddIngredient<MoltenKatana>();
			recipe.AddIngredient(ItemID.ChlorophyteBar, 10);
			recipe.AddIngredient(ItemID.HallowedBar, 10);
			recipe.AddIngredient(ItemID.SpectreBar, 10);
            recipe.AddIngredient(ItemID.Ectoplasm, 30);
            recipe.AddIngredient(ItemID.SoulofNight, 20);
            recipe.AddIngredient(ItemID.SoulofFright, 30);
            recipe.AddIngredient(ItemID.SoulofSight, 10);
            recipe.AddIngredient(ItemID.SoulofMight, 10);
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.Register();
            
		}
	}
}