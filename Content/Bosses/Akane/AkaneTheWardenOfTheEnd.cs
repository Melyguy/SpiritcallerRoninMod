using SpiritcallerRoninMod.Common.Systems;
using SpiritcallerRoninMod.Content.Items;
using SpiritcallerRoninMod.Content.Items.Consumables;
using SpiritcallerRoninMod.Content.Items.Weapons;
using SpiritcallerRoninMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritcallerRoninMod.Content.Items.Armor;
namespace SpiritcallerRoninMod.Content.Bosses.Akane;
[AutoloadBossHead]
public class AkaneTheWardenOfTheEnd : ModNPC
{
public override void SetStaticDefaults()
{
    Main.npcFrameCount[NPC.type] = 40; // Update total frames to match spritesheet
    NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
    {
        CustomTexturePath = null,
        Position = new Vector2(0f, 0f),
        PortraitPositionXOverride = null,
        PortraitPositionYOverride = null
    };
}
		public static int secondStageHeadSlot = -1;
public override void FindFrame(int frameHeight)
{
    NPC.frameCounter++;
    int frameSpeed = 5;

    // Phase check: 0 = phase one, 1 = phase two
    int phaseOffset = (int)NPC.ai[3] == 1 ? 20 : 0;

    switch ((int)NPC.ai[0])
    {
        case 0: // Idle (frames 0–3) or (20–23)
            NPC.frame.Y = ((int)(NPC.frameCounter / frameSpeed) % 4 + phaseOffset + 0) * frameHeight;
            break;

        case 1: // Walking/prep (frames 4–7 or 24–27)
            NPC.frame.Y = ((int)(NPC.frameCounter / frameSpeed) % 4 + phaseOffset + 4) * frameHeight;
            break;

        case 2: // Slashing attack (frames 8–11 or 28–31)
            NPC.frame.Y = ((int)(NPC.frameCounter / frameSpeed) % 4 + phaseOffset + 8) * frameHeight;
            break;

        case 3: // Casting/charge (frames 12–15 or 32–35)
            NPC.frame.Y = ((int)(NPC.frameCounter / frameSpeed) % 4 + phaseOffset + 12) * frameHeight;
            break;

        case 4: // Transformed attack/enraged (frames 16–19 or 36–39)
            NPC.frame.Y = ((int)(NPC.frameCounter / frameSpeed) % 4 + phaseOffset + 16) * frameHeight;
            break;

        default:
            NPC.frame.Y = phaseOffset * frameHeight;
            break;
    }

    // Reset counter after one full animation cycle
    if (NPC.frameCounter >= frameSpeed * 4)
        NPC.frameCounter = 0;
}

		public override void Load() {
			// We want to give it a second boss head icon, so we register one
			string texture = BossHeadTexture + "_SecondStage"; // Our texture is called "ClassName_Head_Boss_SecondStage"
			secondStageHeadSlot = Mod.AddBossHeadTexture(texture, -1); // -1 because we already have one registered via the [AutoloadBossHead] attribute, it would overwrite it otherwise
		}

		public override void BossHeadSlot(ref int index) {
			int slot = secondStageHeadSlot;
			if (SecondPhase && slot != -1) {
				// If the boss is in its second stage, display the other head icon instead
				index = slot;
			}
		}

    public bool SecondPhase = false;
public override void SetDefaults()
{
    NPC.width = 100;
    NPC.height = 140;
    NPC.damage = 60; // Increased damage
    NPC.defense = 50;
    NPC.lifeMax = 200000; // Slightly reduced health for faster-paced fight
    NPC.HitSound = SoundID.NPCHit1;
    NPC.DeathSound = SoundID.NPCDeath1;
    NPC.value = Item.buyPrice(0, 20, 0, 0);
    NPC.knockBackResist = 0f;
    NPC.aiStyle = -1;
    NPC.noTileCollide = false;
    Music = MusicID.OtherworldlyBoss1; // More intense music
    NPC.noGravity = false;
    NPC.boss = true;

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
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<PikeOfTheWarden>(), 1)); // 100% drop chance
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<WardensHalo>(), 2)); // 100% drop chance
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<WardensHiddenTreasure>(), 3)); // 100% drop chance
			
			// Add some materials with different drop chances
			notExpertRule.OnSuccess(ItemDropRule.Common(ItemID.Ectoplasm, 1, 15, 30)); // Drops 15-30 Wood
			notExpertRule.OnSuccess(ItemDropRule.Common(ItemID.AbigailsFlower, 3)); // 33% chance
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SheathedKatana>(), 3)); // 33% chance
			
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
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<AkaneBag>()));

			// ItemDropRule.MasterModeCommonDrop for the relic
			npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.Furniture.AkaneRelic>()));

			// ItemDropRule.MasterModeDropOnAllPlayers for the pet
			npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<Items.Weapons.blackmortalBlade>())); //CHANGE THIS LATER!!!
		}

		public override void OnKill()
		{
		    // Reset weather
		    Main.windSpeedTarget = 0f;
		    Main.raining = false;
		    Main.rainTime = 0;
		    
			// The first time this boss is killed, spawn ExampleOre into the world. This code is above SetEventFlagCleared because that will set downedForestGuardian to true.

			// This sets downedForestGuardian to true, and if it was false before, it initiates a lantern night
			NPC.SetEventFlagCleared(ref DownedBossSystem.downedCryoWraith, -1);

			// Since this hook is only ran in singleplayer and serverside, we would have to sync it manually.
			// Thankfully, vanilla sends the MessageID.WorldData packet if a BOSS was killed automatically, shortly after this hook is ran

			// If your NPC is not a boss and you need to sync the world (which includes ModSystem, check DownedBossSystem), use this code:
			/*
			if (Main.netMode == NetmodeID.Server) {
				NetMessage.SendData(MessageID.WorldData);
			}
			*/
		}
        		public override void HitEffect(NPC.HitInfo hit) {
			// If the NPC dies, spawn gore and play a sound
			if (Main.netMode == NetmodeID.Server) {
				// We don't want Mod.Find<ModGore> to run on servers as it will crash because gores are not loaded on servers
				return;
			}

			if (NPC.life <= 0) {
				// These gores work by simply existing as a texture inside any folder which path contains "Gores/"


				var entitySource = NPC.GetSource_Death();

				SoundEngine.PlaySound(SoundID.NPCDeath59, NPC.Center);

				// This adds a screen shake (screenshake) similar to Deerclops
				PunchCameraModifier modifier = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 20f, 6f, 20, 1000f, FullName);
				Main.instance.CameraModifiers.Add(modifier);
			}
		}
        
            // Add this method after your other methods
            private void ManageBlizzard()
            {
                // Force blizzard weather while the boss is alive
                Main.windSpeedTarget = 0.8f * NPC.direction; // Strong wind in the direction the boss is facing
                Main.maxRaining = 1f; // Maximum rain intensity
                Main.raining = true;
                Main.rainTime = 2; // Keep rain active
                Main.cloudAlpha = 1f; // Full cloud coverage
                
                // Create snow particles
                if (Main.rand.NextBool(2)) // 50% chance each frame
                {
                    int snowDust = Dust.NewDust(NPC.position, Main.screenWidth, 10, DustID.Snow, 
                        Main.windSpeedTarget * 5f, 2f, 0, default, 1.2f);
                    Main.dust[snowDust].noGravity = true;
                    Main.dust[snowDust].velocity.X *= 2f;
                }
            }   


private bool TileExists(int x, int y)
{
    if (!WorldGen.InWorld(x, y, 2)) return false;

    Tile tile = Main.tile[x, y];
    if (tile == null || !tile.HasTile)
        return false;

    // Check if it's a fully solid tile (no slope, no half block)
    return Main.tileSolid[tile.TileType] &&
           !tile.IsHalfBlock &&
           tile.Slope == SlopeType.Solid;
}

private bool IsOnGround()
{
    int startX = (int)(NPC.Left.X / 16f);
    int endX = (int)(NPC.Right.X / 16f);
    int y = (int)((NPC.Bottom.Y + 2f) / 16f);

    for (int x = startX; x <= endX; x++)
    {
        if (TileExists(x, y)) return true;
    }
    return false;
}



private void Movement(Player player)
{
    float normalSpeed = 3f;
    float normalAcceleration = 0.2f;

    ref float ai_jumpCooldown = ref NPC.ai[0];
    ref float ai_stuckTimer = ref NPC.ai[1];
    ref float ai_prevX = ref NPC.ai[2];

    bool fleeing = player.dead;

    int direction = fleeing 
        ? -Math.Sign(player.Center.X - NPC.Center.X)
        : Math.Sign(player.Center.X - NPC.Center.X);
    if (direction == 0) direction = 1;

if (player.dead)
{
    NPC.EncourageDespawn(10);
    NPC.noTileCollide = false;
    NPC.noGravity = false;
    
    float fleeSpeed = 6f;
    float fleeAcceleration = 0.3f;

    int fleeDirection = -Math.Sign(player.Center.X - NPC.Center.X);
    if (fleeDirection == 0)
        fleeDirection = 1;

    NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, fleeDirection * fleeSpeed, fleeAcceleration);
    NPC.direction = fleeDirection;
    NPC.spriteDirection = -fleeDirection;

    bool onground = IsOnGround();
    Point tileAhead = (NPC.Bottom + new Vector2(fleeDirection * 16f, 0)).ToTileCoordinates();

    bool obstacleAhead = TileExists(tileAhead.X, tileAhead.Y - 1);    // tile right in front (head level)
    bool spaceAbove = !TileExists(tileAhead.X, tileAhead.Y - 2);      // space 1 block above front tile
    bool spaceTwoAbove = !TileExists(tileAhead.X, tileAhead.Y - 3);   // space 2 blocks above front tile
    

    // Check tile ahead at feet level


if (onground && Math.Abs(NPC.velocity.Y) < 0.1f && ai_jumpCooldown <= 0f)
{
    if (obstacleAhead && spaceAbove)
    {
        NPC.velocity.Y = -10f; // Slightly stronger jump
        ai_jumpCooldown = 30f;
    }
    else if (obstacleAhead && !spaceAbove && spaceTwoAbove)
    {
        NPC.velocity.Y = -20f; // Higher jump for 2-tile obstacles
        ai_jumpCooldown = 30f;
    }
}


    if (!onground)
    {
        NPC.velocity.Y += 0.4f;    // gravity
        if (NPC.velocity.Y > 10f)
            NPC.velocity.Y = 10f;
    }

    if (ai_jumpCooldown > 0)
        ai_jumpCooldown--;

    return;
}



    // === Normal grounded movement ===

    NPC.noTileCollide = false;
    NPC.noGravity = false;

    float moveSpeed = normalSpeed;
    float acceleration = normalAcceleration;

    NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, direction * moveSpeed, acceleration);
    NPC.direction = direction;
    NPC.spriteDirection = direction;

    bool onGround = IsOnGround();

    if (!onGround)
    {
        NPC.velocity.Y += 0.4f;
        if (NPC.velocity.Y > 10f) NPC.velocity.Y = 10f;
    }

    Point ahead = (NPC.Bottom + new Vector2(direction * 16f, 0)).ToTileCoordinates();
    bool tileInFront = TileExists(ahead.X, ahead.Y - 1);
    bool stepPossible = !TileExists(ahead.X, ahead.Y);

if (onGround && Math.Abs(NPC.velocity.Y) < 0.1f && ai_jumpCooldown <= 0f)
{
    if (tileInFront && !TileExists(ahead.X, ahead.Y - 2))
    {
        NPC.position.Y -= 16f;
        NPC.velocity.Y = -2f; // step-up only
    }
    else if (tileInFront && TileExists(ahead.X, ahead.Y - 2) && !TileExists(ahead.X, ahead.Y - 4))
    {
        NPC.velocity.Y = -11f; // higher jump for 2-tile obstacle
        SoundEngine.PlaySound(SoundID.Item16, NPC.Center);
        ai_jumpCooldown = 30f;
    }
}


    if (onGround && tileInFront && TileExists(ahead.X, ahead.Y - 2) && !TileExists(ahead.X, ahead.Y - 4))
    {
        if (ai_jumpCooldown <= 0f)
        {
            NPC.velocity.Y = -10f;
            SoundEngine.PlaySound(SoundID.Item16, NPC.Center);
            ai_jumpCooldown = 30f;
        }
    }

    if (Math.Abs(NPC.Center.X - ai_prevX) < 0.2f && onGround)
    {
        ai_stuckTimer++;
        if (ai_stuckTimer > 30)
        {
            NPC.velocity.Y = -6f;
            ai_stuckTimer = 0f;
        }
    }
    else ai_stuckTimer = 0f;

    ai_prevX = NPC.Center.X;

    if (ai_jumpCooldown > 0f)
        ai_jumpCooldown--;
}





public override void AI()
{
    
    NPC.TargetClosest(true);
    Player player = Main.player[NPC.target];

    if(!SecondPhase && NPC.life <= NPC.lifeMax * 0.8f){
        SecondPhase = true;
        NPC.ai[3] = 1;
        NPC.damage = 100;
        NPC.defense += 15; // Increase defense
        NPC.knockBackResist = 0f; // Become immune to knockback
        NPC.velocity *= 1.5f; // Speed boost
        ManageBlizzard();
        Music = MusicID.OtherworldlyBoss2; // More intense music
// Change display name for second phase
NPC.GivenName = "Soulfractured Akane";
        
        // Visual transformation effects
        for (int i = 0; i < 50; i++) {
            Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric, speed.X * 5f, speed.Y * 5f);
        }
        
        // Screen shake effect
        Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Main.rand.NextVector2CircularEdge(1f, 1f), 15f, 6f, 20, 1000f, "Genichiro Phase 2"));
        
        // Play transformation sound
        SoundEngine.PlaySound(SoundID.Thunder, NPC.Center);

        // Display text above boss when transforming
        CombatText.NewText(NPC.getRect(), Color.Pink, "Seems i have to deal with you myself! ", true);
    }

    if (!player.active || player.dead)
    {
        NPC.TargetClosest(false);
        NPC.velocity.Y += 1f;
        if (NPC.timeLeft > 10)
            NPC.timeLeft = 10;
        return;
    }

NPC.spriteDirection = NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;

if (player.dead) {
    NPC.velocity.Y -= 0.04f;
    NPC.EncourageDespawn(10);
    return;
}

ref float ai_attackTimer = ref NPC.ai[3]; // new attack timer

// Always move while on ground or unless overridden
if (NPC.ai[0] == 0 || NPC.ai[0] == 1)
{
    Movement(player);
}

// Always count up attack timer (unless doing rapid attack)
if (NPC.ai[0] == 0 || NPC.ai[0] == 1)
{
    ai_attackTimer++;
    if (ai_attackTimer >= 60f) // Attack every ~1 second
    {
        ai_attackTimer = 0;
        NPC.ai[0] = Main.rand.Next(2, 5); // Choose attack
        NPC.ai[1] = 0;
    }
}

switch ((int)NPC.ai[0])
{
    case 0: // Idle
    case 1: // Walking (merged above)
        break;

    case 2: // Bow Attack
        ArrowAtk(player);
        if (NPC.ai[1]++ >= 45)
        {
            NPC.ai[1] = 0;
            NPC.ai[0] = 0;
        }
        break;

    case 3: // Basic Slash
        SpawnGuys(player);
        if (NPC.ai[1]++ >= 30)
        {
            NPC.ai[1] = 0;
            NPC.ai[0] = 0;
        }
        break;

    case 4: // Rapid Slash
        RapidSlash(player);
        if (NPC.ai[1]++ >= 20)
        {
            NPC.ai[1] = 0;
            NPC.ai[0] = 0;
        }
        break;
}


    // Failsafe
    if (NPC.ai[1] > 60 && NPC.ai[0] != 0)
    {
        NPC.ai[0] = 0;
        NPC.ai[1] = 0;
    }
}
                        private void ArrowAtk(Player player)
            {
                // Set animation state to use last 4 frames (arrow animation)
                NPC.ai[0] = 2; // Attack animation state
            
                Vector2 direction = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
                float speed = 30f;
            
                // Only shoot when the bow is fully drawn (frame 14)
                if (NPC.frameCounter == 10) // This is when the bow is fully drawn
                {
                    for (int i = -1; i <= 1; i++)
                    {
                        Vector2 perturbedDirection = direction.RotatedBy(MathHelper.ToRadians(15f * i));
                        if(!SecondPhase){
                        Projectile.NewProjectile(
                            NPC.GetSource_FromAI(),
                            NPC.Center,
                            perturbedDirection * speed,
                            ProjectileID.CultistBossIceMist,
                            60,
                            1f,
                            Main.myPlayer
                        );

                        }
                        else{
                        Projectile.NewProjectile(
                            NPC.GetSource_FromAI(),
                            NPC.Center,
                            perturbedDirection * speed,
                            ModContent.ProjectileType<CrescentslashBoss>(),
                            60,
                            1f,
                            Main.myPlayer
                        );		
                        }
                    }
                    
                    // Visual and sound effects
                    for (int i = 0; i < 10; i++)
                    {
                        Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Snow, 
                            direction.X * 2f, direction.Y * 2f, 100, default, 1.2f);
                    }
                    SoundEngine.PlaySound(SoundID.Item38, NPC.position); // Bow sound instead of teleport sound
                }
            }
            private void RapidSlash(Player player)
{
    // Quick multi-slash combo
    Vector2 direction = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
    float dashSpeed = 15f;
    NPC.velocity = direction * dashSpeed;
    
    // Create multiple quick slashes with increasing scale
    for (int i = 0; i < 3; i++)
    {
        float scale = 1f + (i * 0.5f); // Each slash is 50% larger than the previous
        Vector2 slashPosition = NPC.Center + direction * 40f;
                    if(!SecondPhase){
                    Projectile.NewProjectile(
                        NPC.GetSource_FromAI(),
                        slashPosition,
                        direction * 8f,
                        ModContent.ProjectileType<GenichiroSlash>(),
                        45,
                        2f,
                        Main.myPlayer,
                        ai0: 2.5f, // Much larger scale
                        ai1: 10f
                    );
    SoundEngine.PlaySound(SoundID.Item71, NPC.Center);
                    }
                    else{
                    Projectile.NewProjectile(
                        NPC.GetSource_FromAI(),
                        slashPosition,
                        direction * 8f,
                        ModContent.ProjectileType<HollowStarSlashBoss>(),
                        45,
                        2f,
                        Main.myPlayer,
                        ai0: 2.5f, // Much larger scale
                        ai1: 10f	
                    );	
                SoundEngine.PlaySound(SoundID.Item94, NPC.Center); // Electric magic sound
                SoundEngine.PlaySound(SoundID.Item92, NPC.Center); // Lightning zap sound
                SoundEngine.PlaySound(SoundID.NPCHit53, NPC.Center); // Electric hit sound
                    for (int x = 0; x < 15; x++) // Reduced particle count for smaller effect
                    {
                        // Create small lightning spark particles
                        Vector2 randomOffset = Main.rand.NextVector2Circular(20f, 20f);
                        Dust lightningDust = Dust.NewDustPerfect(
                            NPC.Center + randomOffset,
                            DustID.PurpleCrystalShard, // Electric dust type for lightning effect
                            direction * Main.rand.NextFloat(1f, 3f),
                            0,
                            new Color(255, 255, 220), // Yellowish white color for lightning
                            0.5f); // Smaller scale for spark effect
                        
                        lightningDust.noGravity = true;
                        lightningDust.fadeIn = 1f;
                        
                        // Create trailing sparks
                        if (Main.rand.NextBool(2))
                        {
                            Dust trailDust = Dust.NewDustPerfect(
                                NPC.Center + randomOffset,
                                DustID.PurpleCrystalShard,
                                direction * Main.rand.NextFloat(0.5f, 2f),
                                0,
                                new Color(255, 255, 0),
                                0.3f);
                            trailDust.noGravity = true;
                            trailDust.fadeIn = 0.5f;
                        }
                    }
                    }
    }
    
}
            private void TeleportNearPlayer(Player player)
            {
                Vector2 newPos = player.Center + Main.rand.NextVector2Circular(200, 200);
                NPC.Center = newPos;
                SoundEngine.PlaySound(SoundID.Zombie90, NPC.position); // Teleport sound
                for (int i = 0; i < 20; i++) 
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Ice, 0f, 0f);
                }
            }
private void SpawnGuys(Player player)
{
    if(!SecondPhase) {
    	  Vector2 dir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
            float speed = 20f;

            // (Your existing orb code…)

            // Spawn Nimbus Cloud “enemy” NPCs:
            const int cloudCount = 2;
            const float spawnHeight = 250f;       // how many pixels above the player
            const float horizontalRadius = 100f;  // horizontal spread
            for (int i = 0; i < cloudCount; i++)
            {
                Vector2 spawnPos = player.Center
                                + new Vector2(
                                    Main.rand.NextFloat(-horizontalRadius, horizontalRadius),
                                    -spawnHeight
                                );
                int npcID = ModContent.NPCType<GuardOfTheEnd>(); // Spawn Angry Nimbus enemy
                int index = NPC.NewNPC(
                    NPC.GetSource_FromAI(),
                    (int)spawnPos.X, 
                    (int)spawnPos.Y, 
                    npcID,
                    ai0: 0, 
                    ai1: 0
                );
                // Optionally tweak its velocity
                Main.npc[index].velocity = new Vector2(0f, 2f);
            }

                if (NPC.frameCounter == 10)
                {
                    SoundEngine.PlaySound(SoundID.Item20, NPC.position);
                }
    else if(SecondPhase){
            TeleportNearPlayer(player);

    }
    
        
    
    }

}


}
