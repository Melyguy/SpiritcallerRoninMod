using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace SpiritcallerRoninMod.Content.Items.Accessories;
public class SilentStepTabi : ModItem
{
    public override void SetStaticDefaults()
    {

    }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 24;
            Item.accessory = true;
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Orange;
            Item.defense = 1; // optional
        }

public override void UpdateAccessory(Player player, bool hideVisual)
{
    var modPlayer = player.GetModPlayer<RoninDashPlayer>();
    modPlayer.dashTabi = true;
    modPlayer.ignoreNextDash = true; // Mark this frame to ignore the first dash detection
    player.dashType = 1; // Enable vanilla dash detection
}
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Silk, 5)
                .AddIngredient(ItemID.DynastyWood, 5)
                .AddTile(TileID.Anvils)
                .Register();
                CreateRecipe()
                .AddIngredient(ItemID.Silk, 5)
                .AddIngredient(ItemID.DynastyWood, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }


}

