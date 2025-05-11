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
namespace SpiritcallerRoninMod.Content.Bosses.GenichiroAshina;
[AutoloadBossHead]
public class GenichiroAshina : ModNPC
{
public override void SetStaticDefaults()
{
    Main.npcFrameCount[NPC.type] = 16; // Update total frames to match spritesheet
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
        case 0: // Idle animation (first 4 frames)
            int idleFrame = (int)(NPC.frameCounter / frameSpeed) % 4;
            NPC.frame.Y = idleFrame * 65;
            if (NPC.frameCounter >= frameSpeed * 4)
                NPC.frameCounter = 0;
            break;

        case 1: // Walking animation (frames 4-7)
            int walkFrame = (int)(NPC.frameCounter / frameSpeed) % 4;
            NPC.frame.Y = (walkFrame + 4) * 65;
            if (NPC.frameCounter >= frameSpeed * 4)
                NPC.frameCounter = 0;
            break;

        case 2: // Teleport animation (frames 8-11)
            int teleFrame = (int)(NPC.frameCounter / frameSpeed) % 4;
            NPC.frame.Y = (teleFrame + 8) * 65;
            if (NPC.frameCounter >= frameSpeed * 4)
                NPC.frameCounter = 0;
            break;

        case 3: // Bow animation (frames 12-15)
            int bowFrame = (int)(NPC.frameCounter / frameSpeed) % 4;
            NPC.frame.Y = (bowFrame + 11) * 65;
            if (NPC.frameCounter >= frameSpeed * 4)
                NPC.frameCounter = 0;
            break;
            
        case 4: // Slash animation (frames 8-10)
            int slashFrame = (int)(NPC.frameCounter / frameSpeed) % 3;
            NPC.frame.Y = (slashFrame + 8) * 65;
            if (NPC.frameCounter >= frameSpeed * 3)
                NPC.frameCounter = 0;
            break;
    }
}

public override void SetDefaults()
{
    NPC.width = 44;
    NPC.height = 65;
    NPC.damage = 60; // Increased damage
    NPC.defense = 25;
    NPC.lifeMax = 120000; // Slightly reduced health for faster-paced fight
    NPC.HitSound = SoundID.NPCHit1;
    NPC.DeathSound = SoundID.NPCDeath1;
    NPC.value = Item.buyPrice(0, 20, 0, 0);
    NPC.knockBackResist = 0f;
    NPC.aiStyle = -1;
    NPC.noTileCollide = false;
    Music = MusicID.OtherworldlyEerie; // More intense music
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
			npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ItemID.IceBlock, 10)); //CHANGE THIS LATER!!!
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

    if (!player.active || player.dead)
    {
        NPC.TargetClosest(false);
        NPC.velocity.Y += 1f;
        if (NPC.timeLeft > 10)
            NPC.timeLeft = 10;
        return;
    }

    ManageBlizzard();
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
            SlashAttack(player);
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
                    
                    // Visual effects
                    for (int i = 0; i < 20; i++)
                    {
                        Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.WhiteTorch, 
                            direction.X * 2f, direction.Y * 2f, 100, default, 1.5f);
                    }
                    
                    SoundEngine.PlaySound(SoundID.Item71, NPC.Center);
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
                        Projectile.NewProjectile(
                            NPC.GetSource_FromAI(),
                            NPC.Center,
                            perturbedDirection * speed,
                            ProjectileID.DD2JavelinHostileT3,
                            60,
                            1f,
                            Main.myPlayer
                        );
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
        Projectile.NewProjectile(
            NPC.GetSource_FromAI(),
            slashPosition,
            direction * 12f,
            ModContent.ProjectileType<GenichiroSlash>(),
            45,
            2f,
            Main.myPlayer,
            ai0: scale, // Use ai0 to pass the scale to the projectile
            ai1: 10f
        );
    }
    
    SoundEngine.PlaySound(SoundID.Item71, NPC.Center);
}

private void LeapingSlash(Player player)
{
    // High jump followed by downward slash
    if (NPC.velocity.Y == 0f) // If on ground
    {
        NPC.velocity.Y = -16f; // Big jump
        Vector2 direction = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
        NPC.velocity.X = direction.X * 8f;
    }
    else if (NPC.velocity.Y > 0f) // Falling
    {
        Vector2 direction = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
        Projectile.NewProjectile(
            NPC.GetSource_FromAI(),
            NPC.Center,
            direction * 16f,
            ModContent.ProjectileType<GenichiroSlash>(),
            50,
            3f,
            Main.myPlayer,
            ai0: 2f,
            ai1: 20f
        );
    }
}


}
