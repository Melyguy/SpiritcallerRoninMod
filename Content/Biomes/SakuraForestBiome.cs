using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritcallerRoninMod.Content.Projectiles;
using SpiritcallerRoninMod.Content.Items;
using Terraria.Localization;
using SpiritcallerRoninMod.Content.Tiles;
using Terraria.GameContent.Personalities;
using System.Collections.Generic;
using SpiritcallerRoninMod.Content.Enemies;

namespace SpiritcallerRoninMod.Content.Biomes;


public class SakuraForestBiome : ModBiome
{
    // Keep high priority
    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

    // Add these required overrides
    public override int Music => MusicID.OtherworldlyJungle; // Temporary until you have custom music
    
    // Specify biome depth for proper detection
    public override ModWaterStyle WaterStyle => null;
    public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => null;

    public override bool IsBiomeActive(Player player)
    {
        // Increase detection range and reduce required tiles
        int sakuraTileCount = 0;
        Point playerCenter = player.Center.ToTileCoordinates();

        // Check a larger area (60x60 tiles)
        for (int x = playerCenter.X - 30; x < playerCenter.X + 30; x++)
        {
            for (int y = playerCenter.Y - 30; y < playerCenter.Y + 30; y++)
            {
                if (WorldGen.InWorld(x, y))
                {
                    Tile tile = Main.tile[x, y];
                    if (tile.HasTile && tile.TileType == ModContent.TileType<SakuraGrass>())
                        sakuraTileCount++;
                }
            }
        }
        
        // Reduce required tiles to make detection easier
        return sakuraTileCount >= 100;
    }
    
}
