using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritcallerRoninMod.Content.Projectiles;
using SpiritcallerRoninMod.Content.Items;
using Terraria.Localization;

namespace SpiritcallerRoninMod.Content.Tiles;


public class RustGrass : ModTile
{
    public override void SetStaticDefaults()

    {
        Main.tileSolid[Type] = true;
        Main.tileMergeDirt[Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileLighted[Type] = true;
        Main.tileBlendAll[Type] = true; // Allow blending with all tiles
        Main.tileMerge[Type][TileID.Grass] = true; // Allow merging with regular grass
        Main.tileMerge[TileID.Grass][Type] = true; // Allow regular grass to merge with this
        AddMapEntry(new Color(128, 200, 128));

        RegisterItemDrop(ItemID.DirtBlock);

        RegisterItemDrop(ItemID.DirtBlock);
    
        DustType = DustID.BrownMoss; // Pink dust for particle effects
    }
}
