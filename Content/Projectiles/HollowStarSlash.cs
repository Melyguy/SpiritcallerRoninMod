using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace SpiritcallerRoninMod.Content.Projectiles
{
    public class HollowStarSlash : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 120;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            // Trail effect
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemAmethyst, Projectile.velocity.X * 0.3f, Projectile.velocity.Y * 0.3f);

            // Slight rotation
            Projectile.rotation += 0.3f;
        }

        public override void Kill(int timeLeft)
        {
            // Spawn soul shards
            for (int i = 0; i < 3; i++)
            {
                Vector2 shardVel = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(30)) * 0.8f;
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, shardVel,
                    ProjectileID.SoulDrain, Projectile.damage / 2, 1f, Projectile.owner);
            }

            // Final explosion
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleTorch, 0f, 0f, 150, default, 1.2f);
            }
        }
    }
}
