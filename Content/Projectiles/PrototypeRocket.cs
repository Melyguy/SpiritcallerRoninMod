using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;

namespace SpiritcallerRoninMod.Content.Projectiles
{
    public class PrototypeRocket : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 30;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300; // Lasts 5 seconds
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.aiStyle = 0;
            Projectile.damage = 25;
        }

        public override void AI()
        {
            // Rotate toward velocity
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // Lighting effect
            Lighting.AddLight(Projectile.Center, 1f, 0.5f, 0.1f); // orange glow

            // Fire exhaust
            if (Main.rand.NextBool(2))
            {
                Vector2 firePos = Projectile.Center + new Vector2(0, Projectile.height / 2).RotatedBy(Projectile.rotation);
                int fire = Dust.NewDust(firePos, 4, 4, DustID.Torch,
                    -Projectile.velocity.X * 0.2f, -Projectile.velocity.Y * 0.2f, 150, default, 1.5f);
                Main.dust[fire].noGravity = true;
                Main.dust[fire].velocity *= 0.5f;
            }

            // Homing logic
            float homingSpeed = 10f;
            float lerpFrames = 20f;
            Player target = Main.player[Player.FindClosest(Projectile.Center, 0, 0)];
            if (target.active && !target.dead)
            {
                Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * homingSpeed;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / lerpFrames);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hit)
        {
            // Instead of calling explosion directly, trigger it safely by setting timeLeft
            if (Projectile.timeLeft > 1)
                Projectile.timeLeft = 1;
        }

        public override void OnKill(int timeLeft)
        {
            // Explosion sound
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            // Smoke and fire dust
            for (int i = 0; i < 30; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke,
                    Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f), 100, default, 1.5f);
                Main.dust[dust].noGravity = true;
            }

            for (int i = 0; i < 20; i++)
            {
                int fire = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch,
                    Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-6f, 6f), 150, default, 1.5f);
                Main.dust[fire].noGravity = true;
            }

            // Optional AoE damage
            int explosionRadius = 60;
            foreach (Player player in Main.player)
            {
                if (player.active && !player.dead && player.Distance(Projectile.Center) < explosionRadius)
                {
                    player.Hurt(PlayerDeathReason.ByProjectile(Projectile.owner, Projectile.whoAmI), Projectile.damage, 0);
                }
            }
        }
    }
}
