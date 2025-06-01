using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using System.Linq;
using SpiritcallerRoninMod.Content.Projectiles;

namespace SpiritcallerRoninMod.Content.Bosses.PrototypeZR1;
[AutoloadBossHead]

    public class PrototypeZR1Head : ModNPC
    {
        public bool boomboomBool = false;
        public override void SetDefaults()
        {
            NPC.width = 200;
            NPC.height = 200;
            NPC.damage = 30;
            NPC.defense = 10;
            NPC.lifeMax = 40000;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.boss = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.Zombie104;
            Music = MusicID.OtherworldlyCrimson;
        }

        public override void AI()
        {
            if (!Main.player.Any(p => p.active && !p.dead && p.ZoneUnderworldHeight)) // or just !p.dead
{
    NPC.TargetClosest(false);
    NPC.velocity.Y -= 0.1f; // Fly upward
    CombatText.NewText(NPC.Hitbox, Color.Gray, "ThReAt ElIImiNATed. DISENGAGING.");

    if (NPC.timeLeft > 10)
        NPC.timeLeft = 10; // Despawn soon
    return;
}   
            if(!boomboomBool && NPC.life <= NPC.lifeMax * 0.10f){
                boomboomBool = true;
                TriggerNukeSequence();
                CombatText.NewText(NPC.Hitbox, Color.Red, "WARNING: CORE MELTDOWN DETECTED");
                CombatText.NewText(NPC.Hitbox, Color.Yellow, "WARNING: CORE MELTDOWN DETECTED");
                CombatText.NewText(NPC.Hitbox, Color.Red, "WARNING: CORE MELTDOWN DETECTED");
                CombatText.NewText(NPC.Hitbox, Color.Yellow, "WARNING: CORE MELTDOWN DETECTED");
            }
            if (NPC.ai[0] == 99) // Nuke phase
{
    
    NPC.dontTakeDamage = true;
    NPC.chaseable = false; // Optional: disables targeting/reticles
    NPC.localAI[0]++;

    // Warning effects
    if (NPC.localAI[0] % 20 == 0)
    {
        Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke, Scale: 1.5f);
    }

    if (NPC.localAI[0] >= 120) // 2 seconds later
    {
        NukeExplosion();
        NPC.life = 0; // Kill boss if part of the effect
        NPC.checkDead(); // Force death
    }
}


            if (NPC.ai[0] == 0)
            {
                int previous = NPC.whoAmI;
                for (int i = 0; i < 40; ++i) // 40 segments
                {
                    int type = (i == 19) ? ModContent.NPCType<PrototypeZR1Body>() : ModContent.NPCType<PrototypeZR1Body>();
                    int segment = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, type, NPC.whoAmI);
                    Main.npc[segment].ai[1] = previous;
                    Main.npc[segment].realLife = NPC.whoAmI;
                    Main.npc[segment].ai[2] = NPC.whoAmI;
                    previous = segment;
                }
                NPC.ai[0] = 1;
            }
            if (NPC.velocity != Vector2.Zero)
            {
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
            }
            WormMovement();
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
        
        private void WormMovement()
{
    Player player = Main.player[NPC.target];
    NPC.TargetClosest();

    Vector2 moveTo = player.Center - NPC.Center;
    float baseSpeed = 8f;
    float speed = baseSpeed;
    float turnSpeed = 0.1f;

    // Occasionally slow down or speed up for glitch effect
    if (Main.rand.NextBool(120))
        speed *= 0.5f;
    else if (Main.rand.NextBool(120))
        speed *= 1.3f;

    // Apply movement vector
    moveTo.Normalize();
    moveTo *= speed;
    NPC.velocity = (NPC.velocity * (1f - turnSpeed)) + (moveTo * turnSpeed);

    // Add erratic rotation with jitter
    float targetRotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
    float rotationJitter = MathHelper.ToRadians(Main.rand.NextFloat(-4f, 4f));
    NPC.rotation = MathHelper.Lerp(NPC.rotation, targetRotation + rotationJitter, 0.2f);

    // Occasionally flicker transparency to simulate short-circuiting
    if (Main.rand.NextBool(20))
        NPC.alpha = 200;
    else
        NPC.alpha = 0;

    // Emit sparks or smoke
    if (Main.rand.NextBool(4))
    {
        int dustType = Main.rand.NextBool(2) ? DustID.Electric : DustID.Smoke;
        Dust.NewDust(NPC.position, NPC.width, NPC.height, dustType,
            Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
    }

    // Optional: play mechanical glitch sound occasionally
    if (Main.rand.NextBool(300))
                SoundEngine.PlaySound(SoundID.Item94, NPC.Center); // Electric magic sound
}

        
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);

            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, origin, 1f, SpriteEffects.None, 0f);
            return false;
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
private void TriggerNukeSequence()
{
    // Optional: Pause or change behavior
    NPC.ai[0] = 99; // Enter nuke state
    NPC.velocity *= 0f; // Stop moving

    // Screen shake or warning sound
    SoundEngine.PlaySound(SoundID.Roar, NPC.position);

    // Start a countdown using a timer in AI slots (e.g., NPC.ai[1])
    NPC.localAI[0] = 0; // Used as nuke timer
}
private void NukeExplosion()
{
    // Big visual and sound
    SoundEngine.PlaySound(SoundID.Item14, NPC.position); // Explosion
    for (int i = 0; i < 50; i++)
    {
        Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, Main.rand.NextFloat(-10, 10), Main.rand.NextFloat(-10, 10), Scale: 2f);
    }

    // Spawn projectile or damaging explosion
    Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<PrototypeNukeDeath>(), 200, 10f);
}



}


