using SpiritcallerRoninMod.Content.Items.Placeable;
using SpiritcallerRoninMod.Content.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritcallerRoninMod.Content.Items.Armor
{
	// The AutoloadEquip attribute automatically attaches an equip texture to this item.
	// Providing the EquipType.Legs value here will result in TML expecting a X_Legs.png file to be placed next to the item's main texture.
	[AutoloadEquip(EquipType.Legs)]
	public class ZuuniteRoninLeggings : ModItem
	{
		public static readonly float RoninDamageIncrease = 1.03f;

		public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(RoninDamageIncrease);

		public override void SetDefaults() {
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ItemRarityID.Green; // The rarity of the item
			Item.defense = 25; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player) {
			player.GetModPlayer<GlobalPlayer>().RoninDamage += RoninDamageIncrease;
		}
		public override void AddRecipes() {
			Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<ZuuniteBar>(), 20);
			recipe.AddTile(ModContent.TileType<ZuuniteAnvil>());
			recipe.Register();
		}
		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

	}
}