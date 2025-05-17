using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritcallerRoninMod.Content.Items.Armor
{
	// The AutoloadEquip attribute automatically attaches an equip texture to this item.
	// Providing the EquipType.Body value here will result in TML expecting a X_Body.png file to be placed next to the item's main texture.
	[AutoloadEquip(EquipType.Body)]
	public class HellstoneRoninChestplate : ModItem
	{
		public static readonly float RoninDamageIncrease = 7.5f;

		public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(RoninDamageIncrease);

		public override void SetDefaults() {
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ItemRarityID.Green; // The rarity of the item
			Item.defense = 15; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player) {
			
			player.GetModPlayer<GlobalPlayer>().RoninDamage += RoninDamageIncrease / 100f;// Increase how many minions the player can have by one
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
public override void AddRecipes() {
		Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.AshWood, 15);
			recipe.AddIngredient(ItemID.HellstoneBar, 30);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();

		}
	}
}