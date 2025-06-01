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
    private float expansionSpeed = 0.3f;
    private float maxScale = 75f;
    private bool exploded = false;

    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 180;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.alpha = 100;
        Projectile.scale = 0.1f;
        Projectile.damage = 100; // Set base damage
        Projectile.DamageType = DamageClass.Default;
    }

    public override void AI()
    {
        // Stop any movement
        Projectile.velocity = Vector2.Zero;

        // Expand with acceleration
        if (Projectile.scale < maxScale)
        {
            Projectile.scale += expansionSpeed * (1f + Projectile.scale * 0.1f);
            
            // Calculate current radius
            float radius = (Projectile.width * Projectile.scale) / 2f;
            
            // Check for player collision using circular detection
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && !player.dead && player.immuneTime <= 0)
                {
                    // Calculate distance from player center to projectile center
                    float distance = Vector2.Distance(player.Center, Projectile.Center);
                    
                    // If player is within the radius, apply damage
                    if (distance <= radius)
                    {
                        int damage = (int)(Projectile.damage * (1f + Projectile.scale * 0.2f));
                        player.Hurt(PlayerDeathReason.ByProjectile(player.whoAmI, Projectile.whoAmI), damage, 0);
                        player.immuneTime = 20;
                        
                        // Add screen shake when player is hit
                        Main.instance.CameraModifiers.Add(new PunchCameraModifier(player.Center, Main.rand.NextVector2CircularEdge(1f, 1f), 3f, 3f, 5, 1000f, "NukeHit"));
                    }
                }
            }
        }

        // More intense particle effects
        for (int i = 0; i < 3; i++) // Spawn more particles
        {
            Vector2 dustPos = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width/2, Projectile.height/2);
            int dustType = Main.rand.NextFromList(DustID.Smoke, DustID.Torch, DustID.Torch);
            var dust = Dust.NewDustPerfect(dustPos, dustType, Vector2.Zero, Scale: 2.5f);
            dust.noGravity = true;
            dust.velocity = (dustPos - Projectile.Center) * 0.1f;
        }

        // Stronger lighting
        float lightIntensity = 2f + (float)Math.Sin(Main.GameUpdateCount * 0.1f) * 0.5f;
        Lighting.AddLight(Projectile.Center, 2f * lightIntensity, 1.5f * lightIntensity, 0.5f * lightIntensity);

        // Screen shake while expanding
        if (Projectile.scale < maxScale * 0.8f)
        {
            Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2CircularEdge(1f, 1f), 5f, 6f, 20, 1000f, "Nuke"));
        }

        if (!exploded && Projectile.timeLeft <= 3)
        {
            exploded = true;
            Explode();
        }
    }

    private void Explode()
    {
        // Multiple explosion sounds layered
        SoundEngine.PlaySound(SoundID.Item62, Projectile.Center);
        SoundEngine.PlaySound(SoundID.Item14, Projectile.Center); // Thunder sound
        SoundEngine.PlaySound(SoundID.Item69, Projectile.Center); // Bass boom

        // Massive screen shake on explosion
        Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, (Main.rand.NextFloat() * 6.28318f).ToRotationVector2(), 20f, 20f, 40, 2000f, "NukeExplosion"));

        // More dramatic particle effects
        for (int i = 0; i < 120; i++)
        {
            int dustType = Main.rand.NextFromList(DustID.Smoke, DustID.Torch, DustID.Torch, DustID.Vortex);
            Vector2 velocity = Main.rand.NextVector2CircularEdge(15f, 15f);
            var dust = Dust.NewDustPerfect(Projectile.Center, dustType, velocity, 0, default, 3f);
            dust.noGravity = true;
        }

        // Gore effects for more impact
        for (int i = 0; i < 30; i++)
        {
            Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextVector2CircularEdge(8f, 8f), Main.rand.Next(61, 64));
        }

        // Player damage check with screen shake on hit
        foreach (Player player in Main.player)
        {
            // Enhanced explosion damage
            // Fixed explosion damage check
            float explosionRadius = Projectile.width * Projectile.scale / 2;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player currentPlayer = Main.player[i];
                if (player.active && !player.dead && player.immuneTime <= 0 && 
                    Vector2.Distance(player.Center, Projectile.Center) < explosionRadius)
                {
                    int explosionDamage = Projectile.damage * 2;
                    player.Hurt(PlayerDeathReason.ByProjectile(player.whoAmI, Projectile.whoAmI), explosionDamage, 0);
                    player.AddBuff(BuffID.OnFire, 180);
                    player.immuneTime = 30; // Longer immunity frames for explosion
                }
            }
        }
    }

    public override bool CanHitPlayer(Player target)
    {
        return true; // Ensure the projectile can hit players
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        // Scale damage based on projectile scale
        modifiers.SourceDamage *= 1f + Projectile.scale * 0.5f;
        
        // Add critical hit chance
        if (Main.rand.NextBool(10)) // 10% chance for critical hit
        {
            modifiers.SetCrit();
        }
    }
}
}
