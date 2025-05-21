using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using SpiritcallerRoninMod.Content.Tiles;
using SpiritcallerRoninMod.Content.Tiles.Walls;

namespace SpiritcallerRoninMod.Content.Biomes
{
    public class UndergroundSakuraBiome : ModBiome
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override int Music => MusicID.OtherworldlyJungle; // Replace with your custom music if desired

        public override bool IsBiomeActive(Player player)
        {
            int sakuraStoneCount = 0;
            Point playerCenter = player.Center.ToTileCoordinates();

            // Check a 60x60 area around the player
            for (int x = playerCenter.X - 30; x < playerCenter.X + 30; x++)
            {
                for (int y = playerCenter.Y - 30; y < playerCenter.Y + 30; y++)
                {
                    if (WorldGen.InWorld(x, y))
                    {
                        Tile tile = Main.tile[x, y];
                        if ((tile.HasTile && tile.TileType == ModContent.TileType<SakuraStone>()) ||
                            tile.WallType == ModContent.WallType<SakuraStoneWall>())
                        {
                            sakuraStoneCount++;
                        }
                    }
                }
            }

            // Only active below surface
            bool isUnderground = player.position.Y / 16f > Main.worldSurface + 30;
            return sakuraStoneCount >= 100 && isUnderground;
        }
    }
}