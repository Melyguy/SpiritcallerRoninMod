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
using System;
using SpiritcallerRoninMod.Content.Tiles;
using SpiritcallerRoninMod.Content.Items.Placeable;
using SpiritcallerRoninMod.Content.Items.Placeable.Furniture;
using SpiritcallerRoninMod.Common.Systems;

namespace SpiritcallerRoninMod.Content.Bosses.PrototypeZR1;
[AutoloadBossHead]

    public class PrototypeZR1Head : ModNPC
    {
        public bool boomboomBool = false;
                // AI states for readability
        private enum SakuraDragonAI
        {
            Idle = 0,
            Flamethrower = 1,
            OtherStuff = 2
        }
        
        private int flameTimer = 0;
        private int flameDuration = 120; // 2 seconds of continuous flamethrower
        public override void SetDefaults()
        {
            NPC.width = 173;
            NPC.height = 104;
            NPC.damage = 100;
            NPC.defense = 200;
            NPC.lifeMax = 150000;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.boss = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.Zombie104;
            Music = MusicID.OtherworldlyDungeon;
            Lighting.AddLight(NPC.Center, 1f, 0f, 0f); // red glow
        }

        public override void AI()
        {
            
            Player target = Main.player[NPC.target];
                        // Rocket attack logic
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
                            ModContent.ProjectileType<Content.Projectiles.PrototypeRocket>(),
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
                        NPC.ai[1] = (int)SakuraDragonAI.Flamethrower;
                        flameTimer = 0;
                        SoundEngine.PlaySound(SoundID.Item34, NPC.position); // flame start sound
                    }
                }
            }

            // Flamethrower continuous attack
            if (NPC.ai[1] == (int)SakuraDragonAI.Flamethrower)
            {
                flameTimer++;

                if (flameTimer % 4 == 0) // Spawn flame projectile every 4 ticks (~15/sec)
                {
                    FlamethrowerAttack();
                }

                if (flameTimer >= flameDuration)
                {
                    NPC.ai[1] = (int)SakuraDragonAI.Idle; // Return to idle or another state
                    flameTimer = 0;
                }

                // Optional: Slow movement while flamethrowing
                //NPC.velocity *= 0.9f;
            }   



            if (!Main.player.Any(p => p.active && !p.dead )) // or just !p.dead
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


           if (NPC.localAI[3] == 0f) // Only run once
{
    NPC.localAI[3] = 1f;
    int previous = NPC.whoAmI;
    for (int i = 0; i < 40; ++i)
    {
        int type = (i == 39) ? ModContent.NPCType<PrototypeZR1Tail>() : ModContent.NPCType<PrototypeZR1Body>();
        int segment = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, type, NPC.whoAmI);
        Main.npc[segment].ai[1] = previous;
        Main.npc[segment].realLife = NPC.whoAmI;
        Main.npc[segment].ai[2] = NPC.whoAmI;
        previous = segment;
    }
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

            float flySpeed = 15f; // Base movement speed
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
                Vector2 chargeDir = (player.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 40f;
                NPC.velocity = chargeDir;
                SoundEngine.PlaySound(SoundID.Item92, NPC.position);
            }
        }
public void FlamethrowerAttack()
        {
            if (!Main.player[NPC.target].active || Main.player[NPC.target].dead)
                return;

            Vector2 direction = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitX);
            float speed = 12f;

            Vector2 velocity = direction.RotatedByRandom(MathHelper.ToRadians(10f)) * speed;

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
            Main.projectile[flame].owner = 255; // <-- Mark it as NPC-owned (not by a player)
            Main.projectile[flame].usesLocalNPCImmunity = true;
            Main.projectile[flame].localNPCHitCooldown = -1;

        }

        
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
             Vector2 drawPos = NPC.Center - screenPos;
            float scale = 2f;
                // Draw base texture
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
        public override void OnKill()
{
    // Play explosion sound
    SoundEngine.PlaySound(SoundID.Item14, NPC.position); // Explosion sound
NPC.SetEventFlagCleared(ref DownedBossSystem.DownedPrototype, -1);
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

public override void ModifyNPCLoot(NPCLoot npcLoot) {
			// Do NOT misuse the ModifyNPCLoot and OnKill hooks: the former is only used for registering drops, the latter for everything else

			// The order in which you add loot will appear as such in the Bestiary. To mirror vanilla boss order:
			// 1. Trophy
			// 2. Classic Mode ("not expert")
			// 3. Expert Mode (usually just the treasure bag)
			// 4. Master Mode (relic first, pet last, everything else inbetween)

			// Trophies are spawned with 1/10 chance

			// All the Classic Mode drops here are based on "not expert", meaning we use .OnSuccess() to add them into the rule, which then gets added
			LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());

			// Add your new drops here
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<ZuuniteBar>(), 1, 30, 45)); // 100% drop chance
			
			// Add some materials with different drop chances
			notExpertRule.OnSuccess(ItemDropRule.Common(ItemID.ChlorophyteBar, 1, 15, 30)); // Drops 15-30 Wood
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<ZuuniteAnvilItem>(), 1)); // 33% chance
			notExpertRule.OnSuccess(ItemDropRule.Common(ItemID.LivingWoodWand, 3)); // 33% chance
			
			// You can also add coins
			notExpertRule.OnSuccess(ItemDropRule.Common(ItemID.GoldCoin, 1, 3, 5)); // Drops 3-5 Gold Coins

			// Notice we use notExpertRule.OnSuccess instead of npcLoot.Add so it only applies in normal mode
			// Boss masks are spawned with 1/7 chance
			//notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<ForestGuardianMask>(), 7));

			// This part is not required for a boss and is just showcasing some advanced stuff you can do with drop rules to control how items spawn
			// We make 12-15 ExampleItems spawn randomly in all directions, like the lunar pillar fragments. Hereby we need the DropOneByOne rule,
			// which requires these parameters to be defined
			
			var parameters = new DropOneByOne.Parameters() {
				ChanceNumerator = 1,
				ChanceDenominator = 1,
				MinimumStackPerChunkBase = 1,
				MaximumStackPerChunkBase = 1,
				MinimumItemDropsCount = 1,
				MaximumItemDropsCount = 1,
			};
			

			// Finally add the leading rule
			npcLoot.Add(notExpertRule);

			// Add the treasure bag using ItemDropRule.BossBag (automatically checks for expert mode)
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<PrototypeBag>()));

			// ItemDropRule.MasterModeCommonDrop for the relic
			npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.Furniture.PrototypeRelic>()));

			// ItemDropRule.MasterModeDropOnAllPlayers for the pet
			npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ItemID.SandBlock, 10)); //CHANGE THIS LATER!!!
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
    for (int i = 0; i < 100; i++)
    {
        Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke  , Main.rand.NextFloat(-10, 10), Main.rand.NextFloat(-10, 10), Scale: 2f);
    }

    // Spawn projectile or damaging explosion
    Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<PrototypeNukeDeath>(), 200, 10f);
}



}


