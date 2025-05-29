using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace SpiritcallerRoninMod.Content.Projectiles
{
    public class Crescentslash : ModProjectile
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

            // Scale over time: from 1.0 to 1.5
            float lifeProgress = 1f - (Projectile.timeLeft / 120f); // 0 at spawn, 1 at end
            Projectile.scale = 1f + 1.5f * lifeProgress;
                Projectile.alpha = Main.rand.Next(50, 180);

            // Make the rotation follow the velocity direction
            Projectile.rotation = Projectile.velocity.ToRotation();
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
