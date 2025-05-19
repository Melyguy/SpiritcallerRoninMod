using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritcallerRoninMod.Content.Items.Armor
{
	// The AutoloadEquip attribute automatically attaches an equip texture to this item.
	// Providing the EquipType.Body value here will result in TML expecting a X_Body.png file to be placed next to the item's main texture.
	[AutoloadEquip(EquipType.Body)]
	public class CobaltRoninChestplate : ModItem
	{
		public static readonly float RoninDamageIncrease = 15f;

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
			recipe.AddIngredient(ItemID.Wood, 15);
			recipe.AddIngredient(ItemID.CobaltBar, 30);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();

		}
	}
}