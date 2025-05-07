using SpiritcallerRoninMod.Content.Bosses.Stratoshade;
using SpiritcallerRoninMod.Content.Items.Weapons;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritcallerRoninMod.Content.Items.Consumables
{
	// This is the item used to summon a boss, in this case the modded Minion Boss from Example Mod. For vanilla boss summons, see comments in SetStaticDefaults
	public class OverchargedSoul : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 3;
			ItemID.Sets.SortingPriorityBossSpawns[Type] = 12; // This helps sort inventory know that this is a boss summoning Item.

			// If this would be for a vanilla boss that has no summon item, you would have to include this line here:
			// NPCID.Sets.MPAllowedEnemies[NPCID.Plantera] = true;

			// Otherwise the UseItem code to spawn it will not work in multiplayer
		}

		public override void SetDefaults() {
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 20;
			Item.value = 100;
			Item.rare = ItemRarityID.Blue;
			Item.useAnimation = 30;
			Item.useTime = 30;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.consumable = true;
		}

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup) {
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossSpawners;
		}

		public override bool CanUseItem(Player player) {
			bool isInForest = player.ZoneSkyHeight;
			if (!isInForest) {
				Main.NewText("I can only be summoned in the Sky.", 250, 150, 50);
				return false;
			}
			return !NPC.AnyNPCs(ModContent.NPCType<Stratoshade>()); // Removed extra parenthesis
		}

		public override bool? UseItem(Player player) {
			if (player.whoAmI == Main.myPlayer) {
				// If the player using the item is the client
				// (explicitly excluded serverside here)
				SoundEngine.PlaySound(SoundID.Zombie105, player.position);

				int type = ModContent.NPCType<Stratoshade>();

				if (Main.netMode != NetmodeID.MultiplayerClient) {
					// If the player is not in multiplayer, spawn directly
					NPC.SpawnOnPlayer(player.whoAmI, type);
				}
				else {
					// If the player is in multiplayer, request a spawn
					// This will only work if NPCID.Sets.MPAllowedEnemies[type] is true, which we set in MinionBossBody
					NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
				}
			}

			return true;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ItemID.HellstoneBar, 30)
				.AddIngredient(ItemID.Obsidian, 30)
                .AddIngredient(ItemID.ThrowingKnife, 1)
                .AddIngredient(ItemID.SoulofNight, 10)
                .AddIngredient(ItemID.SoulofLight, 10)
				.AddTile(TileID.DemonAltar)
				.Register();
		}
	}
}