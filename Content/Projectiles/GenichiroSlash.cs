using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritcallerRoninMod.Content.Projectiles
{
    public class GenichiroSlash : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4; // Set the number of frames
        }

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 20;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1f;
            
            // Add these for better animation control
            Projectile.aiStyle = -1;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            // Update animation frame
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            
            // Create white/yellow dust effects
            if (Main.rand.NextBool(2))
            {
                // Main slash dust (white)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 
                    DustID.WhiteTorch, Projectile.velocity.X * 0.2f, 
                    Projectile.velocity.Y * 0.2f, 100, default, 1.2f);
                    
                // Secondary yellow/golden sparkles
                if (Main.rand.NextBool(2))
                {
                    Dust sparkle = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height,
                        DustID.GoldCoin, Projectile.velocity.X * 0.1f,
                        Projectile.velocity.Y * 0.1f, 150, default, 0.8f);
                    sparkle.noGravity = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            
            Rectangle sourceRectangle = new Rectangle(0, Projectile.frame * frameHeight, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Vector2 position = Projectile.Center - Main.screenPosition;
            
            // Make the slash more white/yellow tinted
            Color drawColor = Color.Lerp(Color.White, Color.Yellow, 0.3f) * Projectile.Opacity;
            
            Main.spriteBatch.Draw(
                texture,
                position,
                sourceRectangle,
                drawColor,
                Projectile.rotation,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0f
            );
            
            return false;
        }
    }
}