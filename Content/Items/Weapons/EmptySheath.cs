using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritcallerRoninMod.Content.Items.Weapons
{ 
	// This is a basic item template.
	// Please see tModLoader's ExampleMod for every other example:
	// https://github.com/tModLoader/tModLoader/tree/stable/ExampleMod
	public class EmptySheath : ModItem
	{
		// The Display Name and Tooltip of this item can be edited in the 'Localization/en-US_Mods.KatanaMod.hjson' file.
		public override void SetDefaults()
		{
			Item.damage = 5;
			Item.DamageType = DamageClass.Melee;
			Item.width = 60;
			Item.height = 60;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Thrust;
			Item.knockBack = 6;
			Item.value = Item.buyPrice(copper: 50);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Wood, 15);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}
