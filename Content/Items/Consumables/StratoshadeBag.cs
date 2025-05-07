
using SpiritcallerRoninMod.Content.Bosses.Stratoshade;
using SpiritcallerRoninMod.Content.Bosses.DesertSpirit;
using SpiritcallerRoninMod.Content.Bosses.ForestGuardian;
using SpiritcallerRoninMod.Content.Items.Weapons;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritcallerRoninMod.Content.Items.Consumables
{
	// Basic code for a boss treasure bag
	public class StratoshadeBag : ModItem
	{
		public override void SetStaticDefaults() {
			// This set is one that every boss bag should have.
			// It will create a glowing effect around the item when dropped in the world.
			// It will also let our boss bag drop dev armor..
			ItemID.Sets.BossBag[Type] = true;
			ItemID.Sets.PreHardmodeLikeBossBag[Type] = true; // ..But this set ensures that dev armor will only be dropped on special world seeds, since that's the behavior of pre-hardmode boss bags.

			Item.ResearchUnlockCount = 3;
		}

		public override void SetDefaults() {
			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;
			Item.width = 24;
			Item.height = 24;
			Item.rare = ItemRarityID.Purple;
			Item.expert = true; // This makes sure that "Expert" displays in the tooltip and the item name color changes
		}

		public override bool CanRightClick() {
			return true;
		}

		public override void ModifyItemLoot(ItemLoot itemLoot) {
			// Guaranteed expert drops
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<LightningLantern>(), 1));
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<StratoshadeStaff>(), 2));
			
			// Chance-based drops (33% chance each)
			itemLoot.Add(ItemDropRule.Common(ItemID.EnchantedBoomerang, 3));
			itemLoot.Add(ItemDropRule.Common(ItemID.SoulofFlight, 3, 5, 15));
			
			// Wood drops (5-15 pieces, guaranteed)
			itemLoot.Add(ItemDropRule.Common(ItemID.Cloud, 1, 5, 15));
			
			// Add coins based on NPC value
			itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<Stratoshade>()));
		}
	}
}