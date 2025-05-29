using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using SpiritcallerRoninMod.Content.Projectiles;
using System.Collections.Generic;
using System.Linq;

namespace SpiritcallerRoninMod.Content.Items.Weapons
{
    public class SpectreFang : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.Magic; // Your custom damage class can replace this
            Item.width = 55;
            Item.height = 48;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(silver: 75);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item71;
            Item.autoReuse = true;
            Item.noMelee = false; // Scythe does melee
            Item.shoot = ModContent.ProjectileType<SoulGhastProjectile>(); // Optional wisp
            Item.shootSpeed = 15f;
            Item.mana = 5;
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
        public override bool? UseItem(Player player)
        {
            // 1 in 3 chance to release a soul wisp
            if (Main.rand.NextBool(5))
            {
                Vector2 velocity = Vector2.Normalize(Main.MouseWorld - player.Center) * Item.shootSpeed;
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, velocity, Item.shoot, Item.damage / 2, 0f, player.whoAmI);
            }

            return base.UseItem(player);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
			recipe.AddIngredient<SoulSplinterHarvester>();
            recipe.AddIngredient(ItemID.MythrilBar, 15); // or CrimtaneBar
            recipe.AddIngredient(ItemID.CursedFlame, 5);
            recipe.AddIngredient(ItemID.SoulofNight, 10); // or Tissue Sample
            recipe.AddTile(TileID.Anvils); // Or Crimson Altar
            recipe.Register();
            
            recipe = CreateRecipe();
			recipe.AddIngredient<SoulSplinterHarvester>();
            recipe.AddIngredient(ItemID.MythrilBar, 15); // or CrimtaneBar
            recipe.AddIngredient(ItemID.Ichor, 5);
            recipe.AddIngredient(ItemID.SoulofNight, 10); // or Tissue Sample
            recipe.AddTile(TileID.Anvils); // Or Crimson Altar
            recipe.Register();
            
            recipe = CreateRecipe();
			recipe.AddIngredient<SoulSplinterHarvester>();
            recipe.AddIngredient(ItemID.OrichalcumBar, 15); // or CrimtaneBar
            recipe.AddIngredient(ItemID.CursedFlame, 5);
            recipe.AddIngredient(ItemID.SoulofNight, 10); // or Tissue Sample
            recipe.AddTile(TileID.Anvils); // Or Crimson Altar
            recipe.Register();
            
            recipe = CreateRecipe();
			recipe.AddIngredient<SoulSplinterHarvester>();
            recipe.AddIngredient(ItemID.OrichalcumBar, 15); // or CrimtaneBar
            recipe.AddIngredient(ItemID.Ichor, 5);
            recipe.AddIngredient(ItemID.SoulofNight, 10); // or Tissue Sample
            recipe.AddTile(TileID.Anvils); // Or Crimson Altar
            recipe.Register();
            
        }
    }
}
