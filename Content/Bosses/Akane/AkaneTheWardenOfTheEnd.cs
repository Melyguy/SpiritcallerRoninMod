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
    Main.npcFrameCount[NPC.type] = 20; // Update total frames to match spritesheet
    NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
    {
        CustomTexturePath = null,
        Position = new Vector2(0f, 0f),
        PortraitPositionXOverride = null,
        PortraitPositionYOverride = null
    };
}

public override void FindFrame(int frameHeight)
{
    NPC.frameCounter++;
    int frameSpeed = 5;

    switch ((int)NPC.ai[0])
    {
        case 0: // Idle (frames 0–3)
            NPC.frame.Y = ((int)(NPC.frameCounter / frameSpeed) % 4) * frameHeight;
            if (NPC.frameCounter >= frameSpeed * 4)
                NPC.frameCounter = 0;
            break;

        case 1: // Walking or preparing (frames 4–7)
            NPC.frame.Y = ((int)(NPC.frameCounter / frameSpeed) % 4 + 4) * frameHeight;
            if (NPC.frameCounter >= frameSpeed * 4)
                NPC.frameCounter = 0;
            break;

        case 2: // Slashing attack (frames 8–11)
            NPC.frame.Y = ((int)(NPC.frameCounter / frameSpeed) % 4 + 8) * frameHeight;
            if (NPC.frameCounter >= frameSpeed * 4)
                NPC.frameCounter = 0;
            break;

        case 3: // Charging or casting (frames 12–15)
            NPC.frame.Y = ((int)(NPC.frameCounter / frameSpeed) % 4 + 12) * frameHeight;
            if (NPC.frameCounter >= frameSpeed * 4)
                NPC.frameCounter = 0;
            break;

        case 4: // Enraged/Transformed (frames 16–19)
            NPC.frame.Y = ((int)(NPC.frameCounter / frameSpeed) % 4 + 16) * frameHeight;
            if (NPC.frameCounter >= frameSpeed * 4)
                NPC.frameCounter = 0;
            break;

        default: // fallback to idle
            NPC.frame.Y = 0;
            break;
    }
}


    public bool SecondPhase = false;
public override void SetDefaults()
{
    NPC.width = 120;
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
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<AshinaHelmet>(), 1)); // 100% drop chance
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<AshinaChestPlate>(), 2)); // 100% drop chance
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<AshinaLeggings>(), 3)); // 100% drop chance
			
			// Add some materials with different drop chances
			notExpertRule.OnSuccess(ItemDropRule.Common(ItemID.WoodenArrow, 1, 15, 30)); // Drops 15-30 Wood
			notExpertRule.OnSuccess(ItemDropRule.Common(ItemID.WoodenBow, 3)); // 33% chance
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SheathedKatana>(), 3)); // 33% chance
			
			// You can also add coins
			notExpertRule.OnSuccess(ItemDropRule.Common(ItemID.GoldCoin, 1, 3, 5)); // Drops 3-5 Gold Coins

			// Notice we use notExpertRule.OnSuccess instead of npcLoot.Add so it only applies in normal mode
			// Boss masks are spawned with 1/7 chance
			//notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<ForestGuardianMask>(), 7));

			// This part is not required for a boss and is just showcasing some advanced stuff you can do with drop rules to control how items spawn
			// We make 12-15 ExampleItems spawn randomly in all directions, like the lunar pillar fragments. Hereby we need the DropOneByOne rule,
			// which requires these parameters to be defined
			int itemType = ModContent.ItemType<EvilSealingSheath>();
			int itemType2 = ModContent.ItemType<CactusSpirit>();
			
			var parameters = new DropOneByOne.Parameters() {
				ChanceNumerator = 1,
				ChanceDenominator = 1,
				MinimumStackPerChunkBase = 1,
				MaximumStackPerChunkBase = 1,
				MinimumItemDropsCount = 1,
				MaximumItemDropsCount = 1,
			};
			

			notExpertRule.OnSuccess(new DropOneByOne(itemType, parameters));

			// Finally add the leading rule 
			npcLoot.Add(notExpertRule);

			// Add the treasure bag using ItemDropRule.BossBag (automatically checks for expert mode)
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<GenichiroBag>()));

			// ItemDropRule.MasterModeCommonDrop for the relic
			npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.Furniture.GenichiroRelic>()));

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

            private void Movement(Player player)
{
    float moveSpeed = 4f; // Increased speed
    float acceleration = 0.3f; // Increased acceleration
    
    // Calculate direction to player
    float xDiff = player.Center.X - NPC.Center.X;
    
    // Always move towards player
    NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, Math.Sign(xDiff) * moveSpeed, acceleration);
    
    // Set animation state
    NPC.ai[0] = Math.Abs(NPC.velocity.X) > 0.5f ? 1 : 0;
    
    // Ground and jump logic
    bool onGround = false;
    Point tileCoords = (NPC.Bottom + new Vector2(0, 8f)).ToTileCoordinates();
    
    if (WorldGen.InWorld(tileCoords.X, tileCoords.Y, 2))
    {
        Tile tile = Main.tile[tileCoords.X, tileCoords.Y];
        onGround = tile != null && tile.HasTile && Main.tileSolid[tile.TileType];
    }
    
    if (onGround)
    {
        Point frontCoords = new Point(tileCoords.X + Math.Sign(NPC.velocity.X), tileCoords.Y - 2);
        bool tileInFront = WorldGen.InWorld(frontCoords.X, frontCoords.Y, 2) && 
                          Main.tile[frontCoords.X, frontCoords.Y].HasTile && 
                          Main.tileSolid[Main.tile[frontCoords.X, frontCoords.Y].TileType];
        
        if (tileInFront && NPC.velocity.Y == 0f)
        {
            NPC.velocity.Y = -10f;
            SoundEngine.PlaySound(SoundID.Item16, NPC.Center);
        }
    }
    else
    {
        NPC.velocity.Y += 0.4f;
        if (NPC.velocity.Y > 10f)
            NPC.velocity.Y = 10f;
    }
}

public override void AI()
{
    NPC.TargetClosest(true);
    Player player = Main.player[NPC.target];

    if(!SecondPhase && NPC.life <= NPC.lifeMax * 0.8f){
        SecondPhase = true;
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
        CombatText.NewText(NPC.getRect(), Color.Yellow, "I cannot yield! ", true);
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

    switch ((int)NPC.ai[0])
    {
        case 0: // Idle/Movement
        case 1: // Walking
            Movement(player);
            if (NPC.ai[1]++ >= 30)
            {
                NPC.ai[1] = 0;
                NPC.ai[0] = Main.rand.Next(2, 5); // Randomly choose between attacks
            }
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
            private void FireIceProjectilesAtPlayer(Player player)
            {
                Vector2 direction = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
                float speed = 30f;

                for (int i = -1; i <= 1; i++)
                {
                    Vector2 perturbedDirection = direction.RotatedBy(MathHelper.ToRadians(15f * i));
                    Projectile.NewProjectile(
                        NPC.GetSource_FromAI(), 
                        NPC.Center, 
                        perturbedDirection * speed, 
                        ProjectileID.CultistBossIceMist, 
                        40, 
                        1f, 
                        Main.myPlayer);
                }
            }

            private void SlashAttack(Player player)
            {
                // Set animation state to slash
                NPC.ai[0] = 4;
                
                // Calculate direction to player
                Vector2 direction = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
                
                // Only perform the slash on the correct animation frame
                if (NPC.frameCounter == 10)
                {
                    // Dash towards player
                    float dashSpeed = 12f;
                    NPC.velocity = direction * dashSpeed;
                    
                    // Create a single large slash projectile
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
                                        // Visual effects
                    for (int i = 0; i < 20; i++)
                    {
                        Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.WhiteTorch, 
                            direction.X * 2f, direction.Y * 2f, 100, default, 1.5f);
                    }
                    }
                    else{
                    Projectile.NewProjectile(
                        NPC.GetSource_FromAI(),
                        slashPosition,
                        direction * 8f,
                        ModContent.ProjectileType<GenichiroSlashLightning>(),
                        45,
                        2f,
                        Main.myPlayer,
                        ai0: 2.5f, // Much larger scale
                        ai1: 10f	
                    );	
                // Replace the existing Thunder sound with a combination of these:
                SoundEngine.PlaySound(SoundID.Item94, NPC.Center); // Electric magic sound
                SoundEngine.PlaySound(SoundID.Item92, NPC.Center); // Lightning zap sound
                SoundEngine.PlaySound(SoundID.NPCHit53, NPC.Center); // Electric hit sound
                    // Visual effects for lightning
                    for (int i = 0; i < 30; i++)
                    {
                        // Create electric dust particles
                        Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.YellowStarDust, 
                            direction.X * 3f, direction.Y * 3f, 0, new Color(0, 236, 255), 2f);
                        
                        // Add some spark effects
                        if (Main.rand.NextBool(2))
                        {
                            Dust sparkDust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, 
                                DustID.YellowStarDust, direction.X * 4f, direction.Y * 4f);
                            sparkDust.noGravity = true;
                            sparkDust.scale = 2f;
                        }
                    }
                    }

                    

                    
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
                            ProjectileID.DD2BetsyFireball,
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
                        ModContent.ProjectileType<GenichiroSlashLightning>(),
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
                            DustID.YellowStarDust, // Electric dust type for lightning effect
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
                                DustID.YellowStarDust,
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

private void SpawnGuys(Player player)
{
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

    SoundEngine.PlaySound(SoundID.Item20, NPC.position);
}


}
