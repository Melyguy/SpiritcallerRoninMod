using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.GameContent;
using SpiritcallerRoninMod.Content.Tiles;
using ReLogic.Content;

namespace SpiritcallerRoninMod.Content.Tiles.Trees
{
    public class SpiritTree : ModTree
    {
        public override void SetStaticDefaults()
        {
            GrowsOnTileId = new int[] { ModContent.TileType<SakuraGrass>() };
        }

        public override TreePaintingSettings TreeShaderSettings => new()
        {
            UseSpecialGroups = true,
            SpecialGroupMinimalHueValue = 0.05f,
            SpecialGroupMaximumHueValue = 0.20f,
            SpecialGroupMaximumSaturationValue = 0.8f,// Add this line
        };

        public override int CreateDust() => DustID.PinkTorch;
        public override int DropWood() => ItemID.DynastyWood;

        public override Asset<Texture2D> GetTexture() =>
            ModContent.Request<Texture2D>("SpiritcallerRoninMod/Content/Tiles/Trees/SpiritTree");

        public override Asset<Texture2D> GetTopTextures() =>
            ModContent.Request<Texture2D>("SpiritcallerRoninMod/Content/Tiles/Trees/SpiritTreeTop");

        public override Asset<Texture2D> GetBranchTextures() =>
            ModContent.Request<Texture2D>("SpiritcallerRoninMod/Content/Tiles/Trees/SpiritTreeBranches");

        public override void SetTreeFoliageSettings(Tile tile, ref int xoffset, ref int treeFrame, ref int floorY, ref int topTextureFrameWidth, ref int topTextureFrameHeight)
        {
            topTextureFrameWidth = 116;
            topTextureFrameHeight = 96;
            xoffset = 58; // Centers the tree top texture horizontally (116/2)
            treeFrame = 0; // The vertical frame index for the tree top texture
        }
    public virtual int SaplingGrowthType(ref int style)
    {
        style = 0;
        return ModContent.TileType<SpiritSapling>();
    }
    }
}
