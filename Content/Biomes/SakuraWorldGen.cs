using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.WorldBuilding;
using Terraria.GameContent.Generation;
using Terraria.IO;
using SpiritcallerRoninMod.Content.Tiles;
using System;
using SpiritcallerRoninMod.Content.Tiles.Trees;
using SpiritcallerRoninMod.Content.Tiles.Walls;

namespace SpiritcallerRoninMod.Content.Biomes
{
    public class SakuraWorldGen : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int genIndex = tasks.FindIndex(pass => pass.Name.Equals("Micro Biomes"));

            if (genIndex != -1)
            {
                tasks.Insert(genIndex + 1, new PassLegacy("Sakura Grove", delegate (GenerationProgress progress, GameConfiguration config)
                {
                    progress.Message = "Planting Sakura biome...";

                    // Adjust edge buffer based on world size
                    int edgeBuffer = (int)(Main.maxTilesX * 0.05f); // 5% of world width
                    
                    for (int attempt = 0; attempt < 200; attempt++) // Increased attempts
                    {
                        int x = WorldGen.genRand.Next(edgeBuffer, Main.maxTilesX - edgeBuffer);
                        int y = GetSurfaceY(x);

                        if (y <= 0) continue;
                        if (!IsValidLocation(x, y)) continue;

                        PlaceSakuraBiome(x, y);
                        Main.NewText($"[Debug] Sakura biome placed at ({x}, {y})", Color.LightPink);
                        break;
                    }
                }));
            }
        }

        private int GetSurfaceY(int x)
        {
            for (int y = 100; y < Main.maxTilesY - 200; y++)
            {
                if (Main.tile[x, y].HasTile && Main.tileSolid[Main.tile[x, y].TileType])
                    return y;
            }
            return -1;
        }

private bool IsValidLocation(int x, int y)
{
    // Avoid being too close to spawn
    int minSpawnDistance = 200; // Adjust as needed
    if (Math.Abs(x - Main.spawnTileX) < minSpawnDistance)
        return false;

    // Large scan box
     float worldSizeScale = Math.Max(0.6f, (float)Main.maxTilesX / 4200f); // Minimum scale of 0.6
    int width = (int)(100 * worldSizeScale);
    int height = (int)(60 * worldSizeScale);




    for (int i = -width / 2; i < width / 2; i++)
    {
        for (int j = -height / 2; j < height / 2; j++)
        {
            int checkX = x + i;
            int checkY = y + j;
            if (!WorldGen.InWorld(checkX, checkY)) continue;

            Tile tile = Main.tile[checkX, checkY];
            if (!tile.HasTile) continue;

            ushort t = tile.TileType;
            if (t == TileID.SnowBlock || t == TileID.IceBlock || 
                t == TileID.CorruptGrass || t == TileID.CrimsonGrass || t == TileID.HallowedGrass ||
                t == TileID.BlueDungeonBrick || t == TileID.GreenDungeonBrick || t == TileID.PinkDungeonBrick)
                return false;
        }
    }

    return true;
}

        private void PlaceSakuraBiome(int x, int y)
        {
            // Adjust base sizes for small worlds
            float worldSizeScale = Math.Max(0.6f, (float)Main.maxTilesX / 4200f); // Minimum scale of 0.6
            int width = (int)(100 * worldSizeScale);  // Reduced base width to 100
            int height = (int)(60 * worldSizeScale);  // Reduced base height to 60

            for (int i = -width / 2; i < width / 2; i++)
            {
                for (int j = -height / 2; j < height / 2; j++)
                {
                    int newX = x + i;
                    int newY = y + j;
                    if (!WorldGen.InWorld(newX, newY)) continue;
    
                    Tile tile = Main.tile[newX, newY];
                    if (tile.HasTile && (tile.TileType == TileID.Grass || tile.TileType == TileID.Stone || tile.TileType == TileID.Sand || tile.TileType == TileID.JungleGrass || tile.TileType == TileID.Mud || tile.TileType == TileID.Ebonsand || tile.TileType == TileID.Crimsand))
                    {
                        tile.TileType = (ushort)ModContent.TileType<SakuraGrass>();
                        WorldGen.SquareTileFrame(newX, newY, true);
                    }
                }
            }
{
for (int i = -width / 2; i < width / 2; i++)
{
    int newX = x + i;

    // Generate a noisy roof height (adds imperfections)
    float roofNoise = (float)Math.Sin(i * 0.1f) * 4f + WorldGen.genRand.Next(-2, 3);
    int caveTop = y + (int)(height * 1.2f) + (int)roofNoise;
    int caveHeight = (int)(40 * worldSizeScale); // Cave height
    int caveBottom = caveTop + caveHeight;

    for (int j = -6; j <= caveHeight + 6; j++) // Extra buffer for walls/roof
    {
        int newY = caveTop + j;

        if (!WorldGen.InWorld(newX, newY)) continue;

        // Define thickness of roof/floor/walls
        bool isRoof = j < 4;
        bool isFloor = j > caveHeight - 4;
        bool isWall = Math.Abs(i) >= width / 2 - 4;

        if (isRoof || isFloor || isWall)
        {
            Main.tile[newX, newY].TileType = (ushort)ModContent.TileType<SakuraStone>();
                        Tile tile = Main.tile[newX, newY];
            tile.HasTile = true;
        }
        else
        {
             Main.tile[newX, newY].WallType = (ushort)ModContent.WallType<SakuraStoneWall>();
            Tile tile = Main.tile[newX, newY];
            tile.HasTile = false;
        }
    }
}


}


            // Place Sakura trees very close together and in a wider range
            int treeSpacing = (int)(2 * worldSizeScale); // Even closer spacing
            int treeRange = (int)(width / 2);  // Use the full biome width for tree placement

            for (int i = -treeRange; i <= treeRange; i += treeSpacing)
            {
                // Remove the random check to make every spot try to place a tree
                int treeX = x + i;
                int treeY = y - 5;
                PlaceSakuraTree(treeX, treeY);
            }
        }

        private void PlaceSakuraTree(int x, int y)
        {
            while (!Main.tile[x, y].HasTile && y < Main.maxTilesY - 20)
                y++;

            if (y >= Main.maxTilesY - 20) return;

            int saplingX = x;
            int saplingY = y - 1;

            // Ensure the tile is empty
            if (Main.tile[saplingX, saplingY].HasTile) return;

            // Place the sapling
            WorldGen.PlaceObject(saplingX, saplingY, ModContent.TileType<SpiritSapling>());
            WorldGen.SquareTileFrame(saplingX, saplingY, true);

            // Force growth (only works if tile is a sapling)
            if (Main.tile[saplingX, saplingY].TileType == ModContent.TileType<SpiritSapling>())
            {
                WorldGen.GrowTree(saplingX, saplingY);
            }
        }



        public override void Load()
        {
            if (!Main.dedServ)
            {
                Main.NewText("[Debug] SpiritWorldGen loaded", Color.LightSkyBlue);
            }
        }
    }
}
