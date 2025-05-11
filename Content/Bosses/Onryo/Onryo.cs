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
using SpiritcallerRoninMod.Content.Items.Accessories;
namespace SpiritcallerRoninMod.Content.Bosses.Onryo;
[AutoloadBossHead]
public class Onryo : ModNPC
{
public override void SetStaticDefaults()
{
    Main.npcFrameCount[NPC.type] = 16; // Same as Lunatic Cultist
}
public override void FindFrame(int frameHeight)
{
    if (NPC.ai[0] == 0f) // Idle
    {
        NPC.frameCounter += 1.0;
        if (NPC.frameCounter > 8)
        {
            NPC.frameCounter = 0.0;
            NPC.frame.Y = frameHeight * (4 + ((NPC.frame.Y / frameHeight - 5 + 1) % 4)); // Cycles through frames 5-8
        }
    }
    else if (NPC.ai[0] == 1f) // Casting
    {
        NPC.frameCounter += 1.0;
        if (NPC.frameCounter > 6)
        {
            NPC.frameCounter = 0.0;
            NPC.frame.Y = frameHeight * (4 + (int)(Main.rand.Next(0, 4))); // frames 4–7 randomly
        }
    }
        else if (NPC.ai[0] == 2f) // Teleporting
    {
        NPC.frameCounter += 1.0;
        if (NPC.frameCounter > 3)
        {
            NPC.frameCounter = 0.0;
            NPC.frame.Y = frameHeight * (13 + (NPC.frame.Y / frameHeight + 1) % 3); // Cycles through frames 13-15
        }
    }
        else if (NPC.ai[0] == 3f) // Casting
    {
        NPC.frameCounter += 1.0;
        if (NPC.frameCounter > 6)
        {
            NPC.frameCounter = 0.0;
            NPC.frame.Y = frameHeight * (10 + (int)(Main.rand.Next(0, 4))); // frames 4–7 randomly
        }
    }
}

public override void SetDefaults()
{
    NPC.width = 40;
    NPC.height = 80;
    NPC.damage = 50;
    NPC.defense = 30;
    NPC.lifeMax = 70000;
    NPC.HitSound = SoundID.NPCHit54; // More ghostly hit sound
    NPC.DeathSound = SoundID.NPCDeath52; // Haunting death sound
    NPC.value = Item.buyPrice(0, 20, 0, 0);
    NPC.knockBackResist = 0f;
    NPC.aiStyle = -1; // Custom AI
    NPC.noTileCollide = true;
        Music = MusicID.OtherworldlyCorruption;
    NPC.noGravity = true;
    NPC.boss = true;
    NPC.alpha = 100; // Make her semi-transparent
    NPC.HitSound = SoundID.NPCHit54 with { Pitch = 0.2f }; // Higher pitched, feminine sound
    NPC.DeathSound = SoundID.NPCDeath6; // More ghostly death sound
    Music = MusicID.OtherworldlyEerie; // More ethereal music fitting for a spirit
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
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<YomiLantern>(), 1)); // 100% drop chance
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<OnryoScream>(), 2)); // 100% drop chance
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SpiritualBell>(), 4)); // 100% drop chance
			//notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<CryoWraithTalon>(), 3)); // 100% drop chance
			
			// Add some materials with different drop chances
			notExpertRule.OnSuccess(ItemDropRule.Common(ItemID.IceBlock, 1, 15, 30)); // Drops 15-30 Wood
			notExpertRule.OnSuccess(ItemDropRule.Common(ItemID.BloodbathDye, 3)); // 33% chance
			notExpertRule.OnSuccess(ItemDropRule.Common(ItemID.BloodOrange, 3)); // 33% chance
			
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
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<OnryoBag>()));

			// ItemDropRule.MasterModeCommonDrop for the relic
			npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.Furniture.OnryoRelic>()));

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
			NPC.SetEventFlagCleared(ref DownedBossSystem.downedOnryo, -1);

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

            private void Movement(Player player) {
    float hoverY = -100f;
    float hoverX = 150f * -NPC.direction;
    
    // Ghostly floating motion
    float floatSpeed = 0.03f;
    hoverY += (float)Math.Sin(Main.GameUpdateCount * floatSpeed) * 30f;
    
    Vector2 targetPos = player.Center + new Vector2(hoverX, hoverY);
    
    // Ethereal movement - smoother and more ghost-like
    float speed = MathHelper.Clamp(NPC.Distance(targetPos) / 100f, 0.5f, 2f) * 6f;
    Vector2 moveDirection = targetPos - NPC.Center;
    
    if (moveDirection != Vector2.Zero) {
        moveDirection.Normalize();
        NPC.velocity = Vector2.Lerp(NPC.velocity, moveDirection * speed, 0.04f);
    }
    
    // Add ghostly trailing effect
    if (Main.rand.NextBool(2)) {
        int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.CrimsonTorch);
        Main.dust[dust].noGravity = true;
        Main.dust[dust].velocity *= 0.1f;
        Main.dust[dust].scale = 1.5f;
    }
}

            public override void AI()
            {
                NPC.TargetClosest(true);
                Player player = Main.player[NPC.target];

                // Add blizzard effect
                ManageBlizzard();

                // Add movement
                Movement(player);

                // Make the boss face the player
                NPC.spriteDirection = NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;

                switch ((int)NPC.ai[0])
                {
                    case 0: // Idle float
                        if (NPC.ai[1]++ > 120) 
                        {
                            NPC.ai[1] = 0;
                            // Randomly choose between all attack states (1, 2, or 3)
                            NPC.ai[0] = Main.rand.Next(1, 6); // This will give us 1, 2, or 3
                        }
                        break;

                    case 1: // Casting projectile
                        if (NPC.ai[1]++ > 60) 
                        {
                            NPC.ai[1] = 0;
                            FireIceProjectilesAtPlayer(player);
                            NPC.ai[0] = 0f; // Loop back to idle
                        }
                        break;
                    
                    case 2: // Teleporting
                        if (NPC.ai[1]++ > 80) 
                        {
                            NPC.ai[1] = 0; // Fixed: was setting to 2
                            TeleportNearPlayer(player);
                            NPC.ai[0] = 0f; // Loop back to idle
                        }
                        break;

                    case 3: // Ice Waves
                        if (NPC.ai[1]++ > 100) 
                        {
                            NPC.ai[1] = 0; // Fixed: was setting to 1
                            UnleashSpikes(player);
                            NPC.ai[0] = 0f; // Loop back to idle
                        }
                        break;
                    case 4: // Hands?
                        if (NPC.ai[1]++ > 80) 
                        {
                            NPC.ai[1] = 0; // Fixed: was setting to 1
                            HandAttack(player);
                            NPC.ai[0] = 0f; // Loop back to idle
                        }
                        break;
                    case 5: // Snowballs
                        if (NPC.ai[1]++ > 70) 
                        {
                            NPC.ai[1] = 0; // Fixed: was setting to 1
                            SnowballsOfDeath(player);
                            NPC.ai[0] = 0f; // Loop back to idle
                        }
                        break;
                }
            }
            private void TeleportNearPlayer(Player player)
            {
                Vector2 newPos = player.Center + Main.rand.NextVector2Circular(200, 200);
                NPC.Center = newPos;
                SoundEngine.PlaySound(SoundID.Item8 with { Volume = 0.5f, Pitch = -0.5f }, NPC.position); // Ghostly teleport
                for (int i = 0; i < 20; i++) 
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Ice, 0f, 0f);
                }
            }


            private void FireIceProjectilesAtPlayer(Player player)
            {
                Vector2 direction = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
                float speed = 15f; // Slower speed for better shockwave effect
                
                // Create a wider arc of projectiles
                for (int i = -4; i <= 4; i++)
                {
                    Vector2 perturbedDirection = direction.RotatedBy(MathHelper.ToRadians(15f * i));
                    // Create the projectile
                    Projectile.NewProjectile(
                        NPC.GetSource_FromAI(), 
                        NPC.Center, 
                        perturbedDirection * speed, 
                        ProjectileID.ShadowBeamHostile, // More ghost-like projectile
                        35, 
                        1f, 
                        Main.myPlayer);
                }
                
                // Add visual and sound effects
                SoundEngine.PlaySound(SoundID.NPCDeath52 with { Volume = 1.5f, Pitch = -0.5f }, NPC.position); // Deeper, louder sound
                
                // Add dust effect for the scream
                for (int i = 0; i < 50; i++)
                {
                    Vector2 dustSpeed = Main.rand.NextVector2CircularEdge(1f, 1f) * 8f;
                    Dust.NewDust(NPC.Center, 0, 0, DustID.Shadowflame, dustSpeed.X, dustSpeed.Y, 100, default, 2f);
                }
            }
            private void UnleashSpikes(Player player)
            {
                Vector2 direction = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
                float speed = 10f;

                for (int i = 0; i < 5; i++ ){
                    Vector2 perturbedDirection = direction.RotatedBy(MathHelper.ToRadians(15f * i));
                    Projectile.NewProjectile(
                        NPC.GetSource_FromAI(),
                        NPC.Center,
                        perturbedDirection * speed,
                        ProjectileID.BloodShot,
                        40,
                        1f,
                        Main.myPlayer 	
                    );
                    
                SoundEngine.PlaySound(SoundID.NPCHit55, NPC.position); // Teleport sound
                }

            }
            private void HandAttack(Player player)
            {
                Vector2 direction = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
                float speed = 10f;

                for (int i = 0; i < 5; i++ ){
                    Vector2 perturbedDirection = direction.RotatedBy(MathHelper.ToRadians(15f * i));
                    Projectile.NewProjectile(
                        NPC.GetSource_FromAI(),
                        NPC.Center,
                        perturbedDirection * speed,
                        ProjectileID.InsanityShadowHostile,
                        40,
                        1f,
                        Main.myPlayer 	
                    );
                    
                SoundEngine.PlaySound(SoundID.Zombie105, NPC.position); // Teleport sound
                }

            }
            private void SnowballsOfDeath(Player player)
            {
                Vector2 direction = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
                float speed = 30f;

                for (int i = -1; i <= 1; i++ ){
                    Vector2 perturbedDirection = direction.RotatedBy(MathHelper.ToRadians(15f * i));
                    Projectile.NewProjectile(
                        NPC.GetSource_FromAI(),
                        NPC.Center,
                        perturbedDirection * speed,
                        ProjectileID.LostSoulHostile,
                        60,
                        1f,
                        Main.myPlayer
                    );	
                    SoundEngine.PlaySound(SoundID.Zombie88, NPC.position); // Teleport sound
                }		
            }


}
