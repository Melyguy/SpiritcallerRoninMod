using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace SpiritcallerRoninMod{
    public class GlobalPlayer : ModPlayer{
        public float ShogunDamage = 0f;
        public float RoninDamage = 0f;
        public float SpiritCallerDamage = 0f;

        public override void PostUpdate(){
            
        }
        public override void ResetEffects()
        {
            ShogunDamage = 0f;
            RoninDamage = 0f;
            SpiritCallerDamage = 0f;
        }
        

    }
}


