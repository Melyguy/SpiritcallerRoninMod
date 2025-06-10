using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritcallerRoninMod.Content.Projectiles;
using SpiritcallerRoninMod.Content.Items;
using Terraria.Localization;

namespace SpiritcallerRoninMod.Content.Tiles;
public class RustStone : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileMergeDirt[Type] = true;
        DustType = DustID.BrownMoss;
        AddMapEntry(new Color(128, 128, 128));
        // Optionally: SoundType, drop item, etc.
        RegisterItemDrop(ItemID.StoneBlock);
    }
}
