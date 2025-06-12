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

                    int edgeBuffer = (int)(Main.maxTilesX * 0.05f);

                    for (int attempt = 0; attempt < 300; attempt++)
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
                    if (
                        t == TileID.CorruptGrass || t == TileID.CrimsonGrass || t == TileID.HallowedGrass ||
                        t == TileID.BlueDungeonBrick || t == TileID.GreenDungeonBrick || t == TileID.PinkDungeonBrick || t == TileID.Cloud || t == TileID.Sand)
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
                    if (tile.HasTile && (tile.TileType == TileID.Grass || tile.TileType == TileID.SnowBlock || tile.TileType == TileID.JungleGrass || tile.TileType == TileID.Mud  || tile.TileType == TileID.CorruptGrass ))
                    {
                        tile.TileType = (ushort)ModContent.TileType<SakuraGrass>();
                        WorldGen.SquareTileFrame(newX, newY, true);
                    }
                    if (tile.HasTile && (tile.TileType == TileID.Stone || tile.TileType == TileID.Ebonstone || tile.TileType == TileID.IceBlock))
                    {
                        tile.TileType = (ushort)ModContent.TileType<SakuraStone>();
                        WorldGen.SquareTileFrame(newX, newY, true);
                    }
                }
            }

            // === Underground Structure + Roof ===
            for (int i = -width / 2; i < width / 2; i++)
            {
                int newX = x + i;
                float roofNoise = (float)Math.Sin(i * 0.1f) * 4f + WorldGen.genRand.Next(-2, 3);
                int caveTop = y + (int)(height * 1.2f) + (int)roofNoise;
                int caveHeight = (int)(40 * worldSizeScale);
                int caveBottom = caveTop + caveHeight;

                for (int j = -6; j <= caveHeight + 6; j++)
                {
                    int newY = caveTop + j;
                    if (!WorldGen.InWorld(newX, newY)) continue;

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

            // === Place Dynasty Ruins Inside Underground ===
            int caveY = y + (int)(height * 1.2f);
            int ruinsCaveHeight = (int)(40 * worldSizeScale); // Renamed variable
            PlaceDynastyRuinsInCave(x, caveY, width, ruinsCaveHeight); // <<<<<< INSERTION POINT

            // === Sakura Trees ===
            int treeSpacing = (int)(2 * worldSizeScale);
            int treeRange = (int)(width / 2);

            for (int i = -treeRange; i <= treeRange; i += treeSpacing)
            {
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

            if (Main.tile[saplingX, saplingY].HasTile) return;

            WorldGen.PlaceObject(saplingX, saplingY, ModContent.TileType<SpiritSapling>());
            WorldGen.SquareTileFrame(saplingX, saplingY, true);

            if (Main.tile[saplingX, saplingY].TileType == ModContent.TileType<SpiritSapling>())
            {
                WorldGen.GrowTree(saplingX, saplingY);
            }
        }

        // === Dynasty Ruin Logic ===
        private void PlaceDynastyRuinsInCave(int x, int yStart, int caveWidth, int caveHeight)
        {
            int ruinCount = WorldGen.genRand.Next(40, 50);

            for (int i = 0; i < ruinCount; i++)
            {
                int ruinWidth = WorldGen.genRand.Next(6, 12);
                int ruinHeight = WorldGen.genRand.Next(4, 7);
                int ruinX = x + WorldGen.genRand.Next(-caveWidth / 2 + 10, caveWidth / 2 - 10);
                // Calculate ruinY to place the bottom of the ruin at the cave floor
                // We subtract ruinHeight to ensure the entire ruin is within the cave, starting from the floor
                int ruinY = yStart + caveHeight - WorldGen.genRand.Next(ruinHeight + 2, ruinHeight + 5); // Added a small random offset above the floor



                GenerateDynastyRuin(ruinX, ruinY, ruinWidth, ruinHeight);
            }
        }

        private void GenerateDynastyRuin(int x, int y, int width, int height)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int tileX = x + i;
                    int tileY = y + j;

                    if (!WorldGen.InWorld(tileX, tileY)) continue;

                    bool isEdge = i == 0 || j == 0 || i == width - 1 || j == height - 1;
                    bool isBroken = WorldGen.genRand.NextBool(6);

                    if (isEdge && !isBroken)
                    {
                        Main.tile[tileX, tileY].TileType = TileID.DynastyWood;
                                                Tile tile = Main.tile[tileX, tileY];
                    tile.HasTile = true;
                    }
                    else
                    {
                        Main.tile[tileX, tileY].WallType = WallID.WhiteDynasty;
                        Tile tile = Main.tile[tileX, tileY];
                    tile.HasTile = false;
                    }

                    WorldGen.SquareTileFrame(tileX, tileY);
                }
            }

            for (int i = -1; i <= width; i++)
            {
                int roofX = x + i;
                int roofY = y - 1;
                if (WorldGen.InWorld(roofX, roofY) && WorldGen.genRand.NextBool(3))
                {
                    Main.tile[roofX, roofY].TileType = TileID.BlueDynastyShingles;
                    Tile tile = Main.tile[roofX, roofY];
                    tile.HasTile = true;
                    WorldGen.SquareTileFrame(roofX, roofY);
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
