using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;

namespace SpiritcallerRoninMod.Content.Bosses.PrototypeZR1

{
    public class PrototypeZR1Body : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.width = 98;
            NPC.height = 136;
            NPC.damage = 20;
            NPC.defense = 8;
            NPC.lifeMax = 10000;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.dontTakeDamage = true; // This segment can't be damaged directly
            Lighting.AddLight(NPC.Center, 1f, 0f, 0f); // red glow
        }

        public override void AI()
        {


            // Parent = previous segment; RealLife = head
            NPC parent = Main.npc[(int)NPC.ai[1]];
            NPC head = Main.npc[(int)NPC.ai[2]];
            NPC.realLife = head.whoAmI;
// Segment spacing logic
            Vector2 directionToParent = parent.Center - NPC.Center;
            float distanceToParent = directionToParent.Length();

            float desiredSpacing = NPC.width * 0.85f; // Slightly closer than full width to avoid gaps

            if (distanceToParent > desiredSpacing)
            {
                directionToParent.Normalize();
                NPC.Center = parent.Center - directionToParent * desiredSpacing;
            }
            // Follow parent segment
            Vector2 toParent = parent.Center - NPC.Center;
            float distance = toParent.Length();
            if (distance > NPC.width)
            {
                NPC.Center = Vector2.Lerp(NPC.Center, parent.Center, 0.1f);
            }

            // Rotate to face the segment ahead
            if (parent.active)
            {
                Vector2 diff = parent.Center - NPC.Center;
                NPC.rotation = diff.ToRotation() + MathHelper.PiOver2;
            }

            // Despawn if head dies
            if (!head.active || head.life <= 0)
            {
                NPC.active = false;
            }


            // Timer for laser shooting
NPC.localAI[0]++; // Increment timer every tick

if (NPC.localAI[0] >= 600f) // Every 20 seconds
{
    NPC.localAI[0] = 0f; // Reset timer

    if (Main.netMode != NetmodeID.MultiplayerClient) // Server-only logic
    {
        Vector2 laserDirection = Vector2.UnitY.RotatedBy(NPC.rotation - MathHelper.PiOver2); // Shoots forward
        float laserSpeed = 12f;

        // Replace ProjectileID.DeathLaser with your custom projectile if you have one
        int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(),
                                            NPC.Center,
                                            laserDirection * laserSpeed,
                                            ProjectileID.DeathLaser,
                                            25, 1f, Main.myPlayer);
        Main.projectile[proj].hostile = true;
        Main.projectile[proj].friendly = false;
    }

    // Optional: Sound effect
    SoundEngine.PlaySound(SoundID.Item33, NPC.position); // Laser sound
}

            // Emit smoke and sparks to show wear and tear
if (Main.rand.NextBool(3)) // Roughly every 3 ticks
{
    // Smoke dust
    int smoke = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke, NPC.velocity.X * 0.2f, NPC.velocity.Y * 0.2f, 100, default, 1.2f);
    Main.dust[smoke].velocity *= 0.3f;
    Main.dust[smoke].noGravity = true;
}

if (Main.rand.NextBool(5)) // Slightly rarer sparks
{
    // Electric sparks
    int spark = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric, NPC.velocity.X, NPC.velocity.Y, 150, default, 1.1f);
    int flameboom = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, NPC.velocity.X, NPC.velocity.Y, 150, default, 1.1f);
    Main.dust[spark].noGravity = true;
    Main.dust[spark].velocity *= 1.2f;
}
if (Main.rand.NextBool(30)) // Very rare, dramatic spark
{
    Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.Next(-2, 2), Main.rand.Next(-2, 2)), GoreID.Smoke1);
}


        }
public override void OnKill()
{
    // Play explosion sound
    SoundEngine.PlaySound(SoundID.Item14, NPC.position); // Explosion sound

    // Create explosion dust
    for (int i = 0; i < 20; i++)
    {
        int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke,
                                     Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f),
                                     100, default, 2f);
        Main.dust[dustIndex].noGravity = true;
    }

    // Add some fire/explosive dust
    for (int i = 0; i < 10; i++)
    {
        int fireDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch,
                                    Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f),
                                    150, default, 1.5f);
        Main.dust[fireDust].noGravity = true;
    }

    // Optional: Add gore
    for (int i = 0; i < 2; i++)
    {
        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(2f, 2f),
                     GoreID.Smoke1, 1.5f);
    }
}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
             Vector2 drawPos = NPC.Center - screenPos;
             float scale = 1.5f;
            spriteBatch.Draw(
        texture,
        drawPos,
        NPC.frame,
        drawColor,
        NPC.rotation,
        origin,
        1f,
        SpriteEffects.None,
        0f
    );

    // Draw glowmask
    Texture2D glowTexture = ModContent.Request<Texture2D>("SpiritcallerRoninMod/Content/Bosses/PrototypeZR1/PrototypeZR1Head_Glow").Value;
    spriteBatch.Draw(
        glowTexture,
        drawPos,
        NPC.frame,
        Color.White, // Glow is unaffected by lighting
        NPC.rotation,
        origin,
        1f,
        SpriteEffects.None,
        0f
    );
            return false;
        }

        // Prevent segment from being a valid target
        public override bool CheckActive() => false;

        public override bool? CanBeHitByItem(Player player, Item item) => false;
        public override bool? CanBeHitByProjectile(Projectile projectile) => false;
    }
}
