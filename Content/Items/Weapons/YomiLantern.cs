using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritcallerRoninMod.Content.Items.Weapons
{
	public class YomiLantern : ModItem
	{
		public override void SetDefaults() {
			// DefaultToStaff handles setting various Item values that magic staff weapons use.
			// Hover over DefaultToStaff in Visual Studio to read the documentation!
			// Shoot a black bolt, also known as the projectile shot from the onyx blaster.
			Item.DefaultToStaff(ProjectileID.LostSoulFriendly, 15, 10, 30);
			
			// Adjust the item's physical size
			Item.width = 20;  // Smaller width for an eyeball
			Item.height = 20; // Smaller height for an eyeball
			Item.UseSound = SoundID.NPCHit55;
			
			// Add scale for the held sprite
			Item.scale = 0.5f; // Adjust this value between 0.1 and 1.0 to get the desired size
			
			// Adjust hold style for better positioning
			//Item.holdStyle = ItemHoldStyleID.HoldGuitar;
			Item.noUseGraphic = false;
			
			Item.SetWeaponValues(150, 6, 32);
			Item.SetShopValues(ItemRarityColor.LightRed4, 10000);
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
		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-5, 0); // Adjusted X to hold more towards the blade
		}
		public override void ModifyManaCost(Player player, ref float reduce, ref float mult) {
			// We can use ModifyManaCost to dynamically adjust the mana cost of this item, similar to how Space Gun works with the Meteor armor set.
			// See ExampleHood to see how accessories give the reduce mana cost effect.
			if (player.statLife < player.statLifeMax2 / 2) {
				mult *= 0.5f; // Half the mana cost when at low health. Make sure to use multiplication with the mult parameter.
			}
		}
	}
}