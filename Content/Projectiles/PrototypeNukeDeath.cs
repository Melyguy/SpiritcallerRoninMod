using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using System;

namespace SpiritcallerRoninMod.Content.Projectiles
{
    public class PrototypeNukeDeath : ModProjectile
    {
        private float expansionSpeed = 1.5f;
        private float maxScale = 30f;
        private bool hasExploded = false;
        private bool isFading = false;

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 0;
            Projectile.damage = 300; // Heavy damage
            Projectile.DamageType = DamageClass.Default;
        }

public override void AI()
{
    Projectile.velocity = Vector2.Zero;

    // Expansion phase
    if (!isFading && Projectile.scale < maxScale)
    {
        Projectile.scale += expansionSpeed * 1f; // Slow but sharp scaling

        float radius = (32f * Projectile.scale) / 2f;

        // Damage players inside the scaled radius
        foreach (Player player in Main.player)
        {
            if (player.active && !player.dead && player.immuneTime <= 0)
            {
                if (Vector2.Distance(player.Center, Projectile.Center) < radius)
                {
                    player.Hurt(PlayerDeathReason.ByProjectile(player.whoAmI, Projectile.whoAmI), Projectile.damage, 0);
                    player.immuneTime = 40;
                    Main.instance.CameraModifiers.Add(
                        new PunchCameraModifier(player.Center, Main.rand.NextVector2CircularEdge(1f, 1f), 4f, 4f, 10, 1000f, "NukeTouch")
                    );
                }
            }
        }

        // Dust around edge
        for (int i = 0; i < 4; i++)
        {
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(radius, radius);
            int dust = Dust.NewDust(pos, 1, 1, DustID.Smoke, 0f, 0f, 100, default, 2f);
            Main.dust[dust].noGravity = true;
        }

        if (Projectile.scale >= maxScale)
        {
            isFading = true;
            Projectile.hostile = false;
        }
    }

    // Fade-out
    if (isFading)
    {
        Projectile.alpha += 10;
        if (Projectile.alpha >= 255 || Projectile.timeLeft < 10)
        {
            Projectile.Kill();
        }
    }

    // Lighting effect
    float lightStrength = 1f - (Projectile.alpha / 255f);
    Lighting.AddLight(Projectile.Center, lightStrength * 2.5f, lightStrength * 2f, lightStrength);
}



        public override bool CanHitPlayer(Player target)
        {
            return !isFading; // Only hit during expansion
        }

        public override void Kill(int timeLeft)
        {
            if (!hasExploded)
            {
                hasExploded = true;

                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

                // Dust burst
                for (int i = 0; i < 60; i++)
                {
                    int dust = Dust.NewDust(Projectile.Center, 1, 1, DustID.Torch, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-5, 5), 100, default, 2f);
                    Main.dust[dust].noGravity = true;
                }

                // Gore burst
                for (int i = 0; i < 10; i++)
                {
                    Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextVector2Circular(4f, 4f), GoreID.Smoke1);
                }
            }
        }
    }
}
