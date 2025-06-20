using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritcallerRoninMod.Content.Items.Armor
{
	// The AutoloadEquip attribute automatically attaches an equip texture to this item.
	// Providing the EquipType.Body value here will result in TML expecting a X_Body.png file to be placed next to the item's main texture.
	[AutoloadEquip(EquipType.Body)]
	public class ShadowCallerRobe : ModItem
	{
		public static readonly int MaxManaIncrease = 20;
		public static readonly float SpiritCallerDamageIncrease = 10f;

		public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MaxManaIncrease, SpiritCallerDamageIncrease);

		public override void SetDefaults() {
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ItemRarityID.Green; // The rarity of the item
			Item.defense = 12; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player) {
			player.buffImmune[BuffID.OnFire] = true; // Make the player immune to Fire
			player.statManaMax2 += MaxManaIncrease; // Increase how many mana points the player can have by 20
			player.GetModPlayer<GlobalPlayer>().SpiritCallerDamage += SpiritCallerDamageIncrease / 100f;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DemoniteBar, 20);
            recipe.AddIngredient(ItemID.ShadowScale, 15);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();

            recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.CrimtaneBar, 20);
            recipe.AddIngredient(ItemID.TissueSample, 15);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}