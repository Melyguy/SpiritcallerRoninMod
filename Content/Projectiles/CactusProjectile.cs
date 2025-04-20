using Terraria;
using Terraria.ModLoader;

namespace SpiritcallerRoninMod.Content.Projectiles
{
    public class CactusProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120; // Changed from 600 to 120 (2 seconds)
            Projectile.aiStyle = 14;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return target.friendly ? false : null;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false; // This prevents the projectile from damaging players
        }
    }
}