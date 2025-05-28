using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace SpiritcallerRoninMod.Content.Projectiles
{
    public class SoulGhastProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.light = 0.3f;
            Projectile.alpha = 100;
            AIType = ProjectileID.MagicMissile;
        }

        public override void AI()
        {
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ghost, 0, 0, 150, default, 1.2f);
        }
    }
}
