using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace SpiritcallerRoninMod.Content.Tiles.Walls
{
    public class SakuraStoneWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true; // allows housing
            DustType = DustID.PinkTorch; // dust when broken
            AddMapEntry(new Color(75, 0, 130)); // color on map
        }
    }
}
