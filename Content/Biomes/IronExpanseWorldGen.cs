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
    public class IronExpanseWorldGen : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int genIndex = tasks.FindIndex(pass => pass.Name.Equals("Micro Biomes"));

            if (genIndex != -1)
            {
                tasks.Insert(genIndex + 1, new PassLegacy("Iron Expanse", delegate (GenerationProgress progress, GameConfiguration config)
                {
                    progress.Message = "Planting iron expanse...";

                    int edgeBuffer = (int)(Main.maxTilesX * 0.05f);

                    for (int attempt = 0; attempt < 200; attempt++)
                    {
                        int midX = Main.maxTilesX / 2;
                        int rightStart = midX + (int)(Main.maxTilesX * 0.1f); // start searching 10% past the middle
                        int rightEnd = Main.maxTilesX - edgeBuffer;

                        int x = WorldGen.genRand.Next(rightStart, rightEnd);


                        int y = GetSurfaceY(x);

                        if (y <= 0) continue;
                        if (!IsValidLocation(x, y)) continue;

                        PlaceSakuraBiome(x, y);
                        Main.NewText($"[Debug] Iron expanse placed at ({x}, {y})", Color.LightPink);
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
            int minSpawnDistance = 200;
            if (Math.Abs(x - Main.spawnTileX) < minSpawnDistance)
                return false;

            float worldSizeScale = Math.Max(0.6f, (float)Main.maxTilesX / 4200f);
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
                        t == TileID.HallowedGrass ||
                        t == TileID.BlueDungeonBrick || t == TileID.GreenDungeonBrick || t == TileID.PinkDungeonBrick)
                        return false;
                }
            }

            return true;
        }

        private void PlaceSakuraBiome(int x, int y)
        {
            float worldSizeScale = Math.Max(0.6f, (float)Main.maxTilesX / 4200f);
            int width = (int)(100 * worldSizeScale);
            int height = (int)(60 * worldSizeScale);

            for (int i = -width / 2; i < width / 2; i++)
            {
                for (int j = -height / 2; j < height / 2; j++)
                {
                    int newX = x + i;
                    int newY = y + j;
                    if (!WorldGen.InWorld(newX, newY)) continue;

                    Tile tile = Main.tile[newX, newY];
                    if (tile.HasTile && (tile.TileType == TileID.Grass || tile.TileType == TileID.JungleGrass || tile.TileType == TileID.Mud || tile.TileType == TileID.Sand || tile.TileType == TileID.Ebonsand || tile.TileType == TileID.Crimsand  || tile.TileType == TileID.Sandstone || tile.TileType == TileID.SnowBlock || tile.TileType == TileID.IceBlock || tile.TileType == TileID.CorruptGrass || tile.TileType == TileID.CrimsonGrass     ))
                    {
                        tile.TileType = (ushort)ModContent.TileType<RustGrass>();
                        WorldGen.SquareTileFrame(newX, newY, true);
                    }
                    if (tile.HasTile && (tile.TileType == TileID.Stone  || tile.TileType == TileID.LivingWood))
                    {
                        tile.TileType = (ushort)ModContent.TileType<RustStone>();
                        WorldGen.SquareTileFrame(newX, newY, true);
                    }
                }
            }
            // Generate factory ruins after biome block placement
                PlaceFactoryRuins(x, y);

            


        }

        private void PlaceFactoryRuins(int originX, int originY)
{
    int width = WorldGen.genRand.Next(20, 35); // random width
    int height = WorldGen.genRand.Next(10, 18); // random height
    int floorCount = WorldGen.genRand.Next(3, 5); // one or two floor ruins

    for (int f = 0; f < floorCount; f++)
    {
        int floorY = originY - (f * (height)); // offset for each floor

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int x = originX + i - width / 2;
                int y = floorY + j;

                if (!WorldGen.InWorld(x, y)) continue;

                Tile tile = Main.tile[x, y];
                if (i == 0 || i == width - 1 || j == 0 || j == height - 1 || WorldGen.genRand.NextBool(20))
                {
                    // Wall
                    tile.HasTile = true;
                    tile.TileType = (ushort)ModContent.TileType<RustStoneBrick>();
                }
                else
                {
                    // Hollow space
                    tile.HasTile = false;
                    tile.WallType = (ushort)ModContent.WallType<RustStoneWall>();
                }

                WorldGen.SquareTileFrame(x, y, true);
            }
        }

        // Optionally: add "broken" gaps
        for (int k = 0; k < 3; k++)
        {
            int holeX = originX + WorldGen.genRand.Next(-width / 2 + 2, width / 2 - 2);
            int holeY = floorY + WorldGen.genRand.Next(2, height - 2);
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int x = holeX + dx;
                    int y = holeY + dy;
                    if (WorldGen.InWorld(x, y))
                    {
                        Tile tile = Main.tile[x, y];
                        tile.HasTile = false;
                        WorldGen.SquareTileFrame(x, y, true);
                    }
                }
            }
        }
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
