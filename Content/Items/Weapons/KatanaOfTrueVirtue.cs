using System.Collections.Generic;
using System.Linq;
using SpiritcallerRoninMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritcallerRoninMod.Content.Items.Placeable;

namespace SpiritcallerRoninMod.Content.Items.Weapons
{
	/// <summary>
	///     Star Wrath/Starfury style weapon. Spawn projectiles from sky that aim towards mouse.
	///     See Source code for Star Wrath projectile to see how it passes through tiles.
	///     For a detailed sword guide see <see cref="ExampleSword" />
	/// </summary>
	public class KatanaOfTrueVirtue : ModItem
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
			
			Item.useTime = 7; // Extremely fast attack speed
			Item.useAnimation = 7;
			Item.autoReuse = true;
			
			Item.damage = 90; // Higher base damage against evil
			Item.useTime = 10; // Slightly slower but more impactful
			Item.useAnimation = 10;
			Item.knockBack = 8; // Increased knockback to represent purifying force
			
			Item.UseSound = SoundID.Item29; // Divine sound (similar to Excalibur)
			Item.rare = ItemRarityID.Yellow; // More prestigious rarity
			Item.DamageType = DamageClass.Melee;
			Item.knockBack = 6;
			Item.crit = 6;

			Item.value = Item.buyPrice(gold: 5);
			Item.rare = ItemRarityID.Yellow;

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
			// Create balanced light/dark afterimages
			for (int i = 0; i < 3; i++) {
				Vector2 offset = new Vector2(player.direction * i * -5, 0);
				// Light dust
				Dust.NewDustPerfect(player.MountedCenter + offset, DustID.HallowedWeapons, Vector2.Zero, 100, Color.Red, 1.8f);
				// Dark dust
				Dust.NewDustPerfect(player.MountedCenter + offset, DustID.ShadowbeamStaff, Vector2.Zero, 100, Color.Purple, 1.7f);
			}
			
			// Get mouse position and calculate direction
			Vector2 mousePosition = Main.MouseWorld;
			Vector2 direction = mousePosition - player.MountedCenter;
			direction.Normalize();
			
			// Declare the projectile type variables
			int projectileType;
			int extraProjectileType;
			bool shootExtra = true;
			
			// Four-part cycle representing different aspects of order
			switch (slashCounter) {
				case 0: // Corruption's Strength
					projectileType = ModContent.ProjectileType<VirtueSlash2>();
					extraProjectileType = ProjectileID.RainbowRodBullet;
					break;
				default: // Crimson's Power
					projectileType = ModContent.ProjectileType<VirtueSlash>();
					extraProjectileType = ProjectileID.RainbowRodBullet;
					break;
			}
			
			// Balanced dual projectile system
			for (int i = 0; i < 2; i++) {
				// Main slash with perfect aim
				Vector2 slashSpeed = direction.RotatedBy(MathHelper.ToRadians(i == 0 ? -10 : 10)) * 24f;
				Projectile.NewProjectile(source, player.MountedCenter, slashSpeed, projectileType, damage, knockback, 
					player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax, 1.5f);
					
				// Complementary projectile
				if (shootExtra) {
					Vector2 extraSpeed = direction.RotatedBy(MathHelper.ToRadians(i == 0 ? 5 : -5)) * 20f;
					int oppositeDamage = (int)(damage * 0.75f); // More balanced damage ratio
					Projectile.NewProjectile(source, player.MountedCenter, extraSpeed, extraProjectileType, oppositeDamage, knockback, 
						player.whoAmI);
				}
			}

			slashCounter = (slashCounter + 1) % 2;
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
			recipe.AddIngredient<EvilSealedKatana>();
            recipe.AddIngredient(ModContent.ItemType<ZuuniteBar>(), 15);
			recipe.AddIngredient(ItemID.HallowedBar, 10);
            recipe.AddIngredient(ItemID.FallenStar, 20);
            recipe.AddIngredient(ItemID.SoulofLight, 20);
            recipe.AddIngredient(ItemID.SoulofFright, 10);
            recipe.AddIngredient(ItemID.SoulofSight, 10);
            recipe.AddIngredient(ItemID.SoulofMight, 10);
            recipe.AddIngredient(ItemID.LightShard, 1);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
            
		}
	}
}