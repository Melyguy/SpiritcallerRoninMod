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

namespace SpiritcallerRoninMod.Content.Bosses.DesertSpirit;
public class DesertSpirit : ModNPC
{
    private const float SpinSpeed = 0.04f;
    private const float ChargeSpeed = 13f;
    private const float IdleSpeed = 7f;
    private const float IdleHeight = -300f; // Increased height
    private const float IdleWidth = 400f;   // Wider figure-8 pattern
    
    // AI States
    public ref float AttackTimer => ref NPC.ai[0];
    public ref float MovementTimer => ref NPC.ai[1];
    public ref float AttackPhase => ref NPC.ai[2];

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.SkeletronHead];
    }

    public override void SetDefaults()
    {
        NPC.width = 60;
        NPC.height = 60;
        NPC.damage = 40;
        NPC.defense = 10;
        NPC.lifeMax = 5000;
        NPC.knockBackResist = 0f;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath3;
			NPC.value = Item.buyPrice(gold: 10);
        NPC.boss = true;
        NPC.aiStyle = -1; // Custom AI
        Music = MusicID.Boss5;
		NPC.npcSlots = 10f; // Take up open spawn slots, preventing random NPCs from spawning during the fight
    }
    		public override void ModifyNPCLoot(NPCLoot npcLoot) {
			// Do NOT misuse the ModifyNPCLoot and OnKill hooks: the former is only used for registering drops, the latter for everything else

			// The order in which you add loot will appear as such in the Bestiary. To mirror vanilla boss order:
			// 1. Trophy
			// 2. Classic Mode ("not expert")
			// 3. Expert Mode (usually just the treasure bag)
			// 4. Master Mode (relic first, pet last, everything else inbetween)

			// Trophies are spawned with 1/10 chance
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Placeable.Furniture.ForestGuardianTrophy>(), 10));

			// All the Classic Mode drops here are based on "not expert", meaning we use .OnSuccess() to add them into the rule, which then gets added
			LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());

			// Add your new drops here
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<CactusSpirit>(), 1)); // 100% drop chance
			
			// Add some materials with different drop chances
			notExpertRule.OnSuccess(ItemDropRule.Common(ItemID.SandBlock, 1, 15, 30)); // Drops 15-30 Wood
			notExpertRule.OnSuccess(ItemDropRule.Common(ItemID.LeafWand, 3)); // 33% chance
			notExpertRule.OnSuccess(ItemDropRule.Common(ItemID.LivingWoodWand, 3)); // 33% chance
			
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
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<DesertSpiritBag>()));

			// ItemDropRule.MasterModeCommonDrop for the relic
			npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.Furniture.DesertSpiritRelic>()));

			// ItemDropRule.MasterModeDropOnAllPlayers for the pet
			npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ItemID.SandBlock, 10)); //CHANGE THIS LATER!!!
		}

		public override void OnKill() {
			// The first time this boss is killed, spawn ExampleOre into the world. This code is above SetEventFlagCleared because that will set downedForestGuardian to true.

			// This sets downedForestGuardian to true, and if it was false before, it initiates a lantern night
			NPC.SetEventFlagCleared(ref DownedBossSystem.downedDesertSpirit, -1);

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

				SoundEngine.PlaySound(SoundID.Roar, NPC.Center);

				// This adds a screen shake (screenshake) similar to Deerclops
				PunchCameraModifier modifier = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 20f, 6f, 20, 1000f, FullName);
				Main.instance.CameraModifiers.Add(modifier);
			}
		}
    public override void AI()
    {
        // Spawn hands once
        if (NPC.localAI[0] == 0f)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                int handLeft = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DesertSpiritHand>(), ai0: NPC.whoAmI, ai1: -1);
                int handRight = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DesertSpiritHand>(), ai0: NPC.whoAmI, ai1: 1);
                
                if (Main.netMode == NetmodeID.Server) {
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, handLeft);
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, handRight);
                }
            }
            NPC.localAI[0] = 1f;
        }

        if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active) {
            NPC.TargetClosest();
        }

        Player player = Main.player[NPC.target];

        if (player.dead) {
            NPC.velocity.Y -= 0.04f;
            NPC.EncourageDespawn(10);
            return;
        }

        AttackTimer++;
        MovementTimer++;

        Vector2 toPlayer = player.Center - NPC.Center;
        float distance = toPlayer.Length();

        // Switch between idle hovering and charging
        if (AttackTimer >= 180) {
            AttackTimer = 0;
            AttackPhase = AttackPhase == 0 ? 1 : 0;
            if (AttackPhase == 1) {
                SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
            }
        }

        if (AttackPhase == 0) {
            // Idle hovering phase - move in a wider figure-8 pattern above the player
            float targetRotation = MovementTimer * SpinSpeed;
            Vector2 offset = new Vector2(0, IdleHeight);
            offset = offset.RotatedBy(targetRotation);
            offset.X *= (float)Math.Sin(targetRotation * 0.5f) * IdleWidth / 200f;

            Vector2 desiredPosition = player.Center + offset;
            Vector2 toDestination = desiredPosition - NPC.Center;
            
            // Slower approach when far away for smoother movement
            float approachSpeed = distance > 600f ? 0.1f : 0.05f;
            NPC.velocity = Vector2.Lerp(NPC.velocity, toDestination.SafeNormalize(Vector2.Zero) * IdleSpeed, approachSpeed);
        }
        else {
            // Charge attack phase - add telegraph
            if (AttackTimer < 30) {
                // Brief pause before charging
                NPC.velocity *= 0.9f;
            }
            else if (distance > 50f) {
                NPC.velocity = Vector2.Lerp(NPC.velocity, toPlayer.SafeNormalize(Vector2.Zero) * ChargeSpeed, 0.1f);
            }
        }
    }
}
