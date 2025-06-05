using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SpiritcallerRoninMod.Content.Projectiles
{
    public class CrescentslashBoss : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // Optional: DisplayName.SetDefault("Crescent Slash");
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = 0;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 3;
            Projectile.damage = 100;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 120;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            // Fancy gradient dust trail
            int[] dustTypes = { DustID.PurpleTorch, DustID.GemAmethyst, DustID.MagicMirror };
            for (int i = 0; i < 2; i++)
            {
                Vector2 offset = Projectile.velocity * i * -0.5f;
                int type = dustTypes[Main.rand.Next(dustTypes.Length)];
                Dust d = Dust.NewDustDirect(Projectile.position + offset, Projectile.width, Projectile.height, type);
                d.velocity *= 0.1f;
                d.scale = 1.5f - (i * 0.3f);
                d.noGravity = true;
            }

            // Opacity flicker
            Projectile.alpha = (int)(100 + 100 * Math.Sin(Main.GameUpdateCount * 0.2f));

            // Scale over time
            float lifeProgress = 1f - (Projectile.timeLeft / 120f);
            Projectile.scale = 1f + 1.5f * lifeProgress;

            // Rotate to face direction
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void Kill(int timeLeft)
        {
            // Dust burst
            for (int i = 0; i < 10; i++)
            {
                int dustType = Main.rand.NextBool() ? DustID.PurpleTorch : DustID.GemAmethyst;
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 150, default, 1.5f);
            }

            // Soul shards
            for (int i = 0; i < 3; i++)
            {
                Vector2 shardVel = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(30)) * 0.8f;
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, shardVel,
                    ProjectileID.SoulDrain, Projectile.damage / 2, 1f, Projectile.owner);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Load main and glow textures
            Texture2D texture = ModContent.Request<Texture2D>("SpiritcallerRoninMod/Content/Projectiles/CrescentslashBoss").Value;

            Vector2 origin = texture.Size() / 2f;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            // Main texture
            Main.EntitySpriteDraw(texture, drawPos, null, lightColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            // Glow overlay

            return false; // We manually draw everything
        }
    }
}
