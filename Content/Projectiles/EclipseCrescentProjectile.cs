using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace SpiritcallerRoninMod.Content.Projectiles
{
    public class EclipseCrescentProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = -1; // Custom AI style
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.light = 0.5f; // Increased light emission
            Projectile.alpha = 100;
        }

        public override void AI()
        {
            // Rotation effect
            Projectile.rotation += 0.4f; // Controls spin speed
            
            // Pulsing glow effect
            float pulseRate = 6f;
            float glowIntensity = 0.3f + (float)System.Math.Sin(Main.GameUpdateCount / pulseRate) * 0.2f;
            Lighting.AddLight(Projectile.Center, 0.5f * glowIntensity, 0.5f * glowIntensity, 1f * glowIntensity); // Blue-tinted light
            
            // Dust trail effect
            for (int i = 0; i < 2; i++) // Spawn 2 dust particles per frame
            {
                Dust dust = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Torch, // Blue torch dust for eclipse theme
                    Projectile.velocity.X * 0.2f,
                    Projectile.velocity.Y * 0.2f,
                    100,
                    Color.Orange,
                    1.2f
                );
                dust.noGravity = true;
                dust.velocity *= 0.3f;
            }
            
            // Update projectile movement
            Projectile.velocity *= 0.98f; // Slight slowdown over time
        }
    }
}
