using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using System.Linq;
using SpiritcallerRoninMod.Content.Projectiles;
using Terraria.GameContent.ItemDropRules;
using SpiritcallerRoninMod.Content.Items.Placeable;
using SpiritcallerRoninMod.Content.Items.Consumables;
using SpiritcallerRoninMod.Content.Items.Weapons;
using SpiritcallerRoninMod.Content.Bosses.PrototypeZR1;
using System;

namespace SpiritcallerRoninMod.Content.Bosses.SakuraDragon
{
    [AutoloadBossHead]
    public class SakuraDragonHead : ModNPC
    {
        // AI states for readability
        private enum SakuraDragonAI
        {
            Idle = 0,
            Flamethrower = 1,
            OtherStuff = 2
        }
private bool segmentsSpawned = false;

        private int flameTimer = 0;
        private int flameDuration = 120; // 2 seconds of continuous flamethrower

        public bool boomboomBool = false;

        public override void SetDefaults()
        {
            NPC.width = 100;
            NPC.height = 40;
            NPC.damage = 30;
            NPC.defense = 10;
            NPC.lifeMax = 50000;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.boss = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath60;
            Music = MusicID.OtherworldlyCorruption;
            Lighting.AddLight(NPC.Center, 1f, 0f, 0f); // red glow
        }

        public override void AI()
        {
            Player target = Main.player[NPC.target];
            NPC.TargetClosest();

            // Despawn if no valid player
            if (!Main.player.Any(p => p.active && !p.dead))
            {
                NPC.velocity.Y -= 0.1f; // Fly upward

                if (NPC.timeLeft > 10)
                    NPC.timeLeft = 10; // Despawn soon
                return;
            }

            // Spawn segments once
            if (!segmentsSpawned)
{
    int previous = NPC.whoAmI;
    for (int i = 0; i < 40; ++i)
    {
        int type = (i == 39) ? ModContent.NPCType<SakuraDragonTail>() :
                   (i == 38) ? ModContent.NPCType<SakuraDragonBody3>() :
                   (i == 37) ? ModContent.NPCType<SakuraDragonBody2>() :
                   (i == 3) ? ModContent.NPCType<SakuraDragonLegs>() :
                   (i == 35) ? ModContent.NPCType<SakuraDragonLegs>() :
                   ModContent.NPCType<SakuraDragonBody>();
        int segment = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, type, NPC.whoAmI);
        Main.npc[segment].ai[1] = previous;
        Main.npc[segment].realLife = NPC.whoAmI;
        Main.npc[segment].ai[2] = NPC.whoAmI;
        previous = segment;
    }
    segmentsSpawned = true;
    NPC.ai[0] = (int)SakuraDragonAI.OtherStuff;
}


            // Movement & rotation
            WormMovement();

            // Smoke dust effect every 3 ticks approx
            if (Main.rand.NextBool(3))
            {
                int smoke = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke, NPC.velocity.X * 0.2f, NPC.velocity.Y * 0.2f, 100, default, 1.2f);
                Main.dust[smoke].velocity *= 0.3f;
                Main.dust[smoke].noGravity = true;
            }

            // Occasional spark gore
            if (Main.rand.NextBool(30))
            {
                Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.Next(-2, 2), Main.rand.Next(-2, 2)), GoreID.Smoke1);
            }

            // Attack timer logic, triggers every 3 seconds (~180 ticks)
            NPC.localAI[1]++;
            if (NPC.localAI[1] >= 180 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.localAI[1] = 0;

                if (target != null && target.active && !target.dead)
                {
                    if (Main.rand.NextBool()) // 50% chance energyball
                    {
                        Vector2 shootDirection = target.Center - NPC.Center;
                        shootDirection.Normalize();
                        shootDirection *= 10f;

                        int proj = Projectile.NewProjectile(
                            NPC.GetSource_FromAI(),
                            NPC.Center,
                            shootDirection,
                            ModContent.ProjectileType<Content.Projectiles.SakuraBlastBoss>(),
                            40,
                            1f,
                            Main.myPlayer
                        );

                        Main.projectile[proj].hostile = true;
                        Main.projectile[proj].friendly = false;

                        SoundEngine.PlaySound(SoundID.NPCHit56, NPC.position);
                    }
                    else
                    {
                        // Start flamethrower phase
                        NPC.ai[0] = (int)SakuraDragonAI.Flamethrower;
                        flameTimer = 0;
                        SoundEngine.PlaySound(SoundID.Item34, NPC.position); // flame start sound
                    }
                }
            }

            // Flamethrower continuous attack
            if (NPC.ai[0] == (int)SakuraDragonAI.Flamethrower)
            {
                flameTimer++;

                if (flameTimer % 4 == 0) // Spawn flame projectile every 4 ticks (~15/sec)
                {
                    FlamethrowerAttack();
                }

                if (flameTimer >= flameDuration)
                {
                    NPC.ai[0] = (int)SakuraDragonAI.Idle; // Return to idle or another state
                    flameTimer = 0;
                }

                // Optional: Slow movement while flamethrowing
                NPC.velocity *= 0.9f;
            }

            // Rotate sprite to face velocity
            if (NPC.velocity != Vector2.Zero)
            {
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
            }
        }

        private void WormMovement()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest();

            float flySpeed = 10f; // Base movement speed
            float turnSpeed = 0.02f; // How quickly it turns (lower = wider turns)

            Vector2 toPlayer = player.Center - NPC.Center;

            if (toPlayer.LengthSquared() < 20f * 20f)
            {
                toPlayer = NPC.velocity.SafeNormalize(Vector2.UnitY);
            }
            else
            {
                toPlayer.Normalize();
            }

            Vector2 desiredVelocity = toPlayer * flySpeed;
            NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, turnSpeed);

            float time = Main.GameUpdateCount * 0.1f;
            float wave = (float)Math.Sin(time + NPC.whoAmI) * 0.3f;
            Vector2 waveOffset = NPC.velocity.RotatedBy(MathHelper.PiOver2) * wave;
            NPC.velocity += waveOffset * 0.05f;

            if (Main.rand.NextBool(300))
            {
                Vector2 chargeDir = (player.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 20f;
                NPC.velocity = chargeDir;
                SoundEngine.PlaySound(SoundID.DD2_WyvernDiveDown, NPC.position);
            }
        }

        public void FlamethrowerAttack()
        {
            if (!Main.player[NPC.target].active || Main.player[NPC.target].dead)
                return;

            Vector2 direction = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitX);
            float speed = 12f;

            Vector2 velocity = direction.RotatedByRandom(MathHelper.ToRadians(5f)) * speed;

            int flame = Projectile.NewProjectile(
                NPC.GetSource_FromAI(),
                NPC.Center,
                velocity,
                ProjectileID.EyeFire,
                20,
                1f,
                Main.myPlayer
            );

            Main.projectile[flame].hostile = true;
            Main.projectile[flame].friendly = false;
            Main.projectile[flame].tileCollide = false;
            Main.projectile[flame].timeLeft = 30; // Short life for fast fading
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
            SoundEngine.PlaySound(SoundID.Item14, NPC.position); // Explosion sound
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<ZuuniteBar>(), 1, 30, 45));
            notExpertRule.OnSuccess(ItemDropRule.Common(ItemID.ChlorophyteBar, 1, 15, 30));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<TuskOfThePrototype>(), 2));
            notExpertRule.OnSuccess(ItemDropRule.Common(ItemID.LivingWoodWand, 3));
            notExpertRule.OnSuccess(ItemDropRule.Common(ItemID.GoldCoin, 1, 3, 5));

            npcLoot.Add(notExpertRule);

            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<DesertSpiritBag>()));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.Furniture.DesertSpiritRelic>()));
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ItemID.SandBlock, 10)); // Change later
        }
    }
}
