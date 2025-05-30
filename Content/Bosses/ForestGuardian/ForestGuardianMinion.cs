using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritcallerRoninMod.Content.Bosses.ForestGuardian
{
	// The minions spawned when the body spawns
	// Please read ForestGuardianBody.cs first for important comments, they won't be explained here again
	public class ForestGuardianMinion : ModNPC
	{
		// This is a neat trick that uses the fact that NPCs have all NPC.ai[] values set to 0f on spawn (if not otherwise changed).
		// We set ParentIndex to a number in the body after spawning it. If we set ParentIndex to 3, NPC.ai[0] will be 4. If NPC.ai[0] is 0, ParentIndex will be -1.
		// Now combine both facts, and the conclusion is that if this NPC spawns by other means (not from the body), ParentIndex will be -1, allowing us to distinguish
		// between a proper spawn and an invalid/"cheated" spawn
		public int ParentIndex {
			get => (int)NPC.ai[0] - 1;
			set => NPC.ai[0] = value + 1;
		}

		public bool HasParent => ParentIndex > -1;

		public float PositionOffset {
			get => NPC.ai[1];
			set => NPC.ai[1] = value;
		}

		public const float RotationTimerMax = 360;
		public ref float RotationTimer => ref NPC.ai[2];

		// Helper method to determine the body type
		public static int BodyType() {
			return ModContent.NPCType<ForestGuardian>();  // Changed from ForestGuardianMinion to ForestGuardianBody
		}

		public override void SetStaticDefaults() {
			Main.npcFrameCount[Type] = 1;

			// By default enemies gain health and attack if hardmode is reached. this NPC should not be affected by that
			NPCID.Sets.DontDoHardmodeScaling[Type] = true;
			// Enemies can pick up coins and be respawned automatically, let's prevent it for this NPC since we don't want this enemy to respawn outside of a boss fight.
			NPCID.Sets.CantTakeLunchMoney[Type] = true;
			// Automatically group with other bosses
			NPCID.Sets.BossBestiaryPriority.Add(Type);

			// Specify the debuffs it is immune to. Most NPCs are immune to Confused.
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

			// Optional: If you don't want this NPC to show on the bestiary (if there is no reason to show a boss minion separately)
			// Make sure to remove SetBestiary code as well
			// NPCID.Sets.NPCBestiaryDrawModifiers bestiaryData = new NPCID.Sets.NPCBestiaryDrawModifiers() {
			//	Hide = true // Hides this NPC from the bestiary
			// };
			// NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, bestiaryData);
		}

		public override void SetDefaults() {
			NPC.width = 30;
			NPC.height = 30;
			NPC.damage = 7;
			NPC.defense = 0;
			NPC.lifeMax = 50;
			NPC.HitSound = SoundID.NPCHit9;
			NPC.DeathSound = SoundID.NPCDeath11;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.knockBackResist = 0.8f;
			NPC.alpha = 255; // This makes it transparent upon spawning, we have to manually fade it in in AI()
			NPC.netAlways = true;

			NPC.aiStyle = -1;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			// Makes it so whenever you beat the boss associated with it, it will also get unlocked immediately
			int associatedNPCType = BodyType();
			bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement("Mods.ExampleMod.Bestiary.ForestGuardianMinion")
			});
		}

		public override Color? GetAlpha(Color drawColor) {
			if (NPC.IsABestiaryIconDummy) {
				// This is required because we have NPC.alpha = 255, in the bestiary it would look transparent
				return NPC.GetBestiaryEntryColor();
			}
			return Color.White * NPC.Opacity;
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot) {
			cooldownSlot = ImmunityCooldownID.Bosses; // use the boss immunity cooldown counter, to prevent ignoring boss attacks by taking damage from other sources
			return true;
		}

		public override void OnKill() {
			// Boss minions typically have a chance to drop an additional heart item in addition to the default chance
			Player closestPlayer = Main.player[Player.FindClosest(NPC.position, NPC.width, NPC.height)];

			if (Main.rand.NextBool(2) && closestPlayer.statLife < closestPlayer.statLifeMax2) {
				Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ItemID.Heart);
			}
		}

		public override void HitEffect(NPC.HitInfo hit) {
			if (NPC.life <= 0) {
				// If this NPC dies, spawn some visuals

				int dustType = 59; // Some blue dust, read the dust guide on the wiki for how to find the perfect dust

				for (int i = 0; i < 20; i++) {
					Vector2 velocity = NPC.velocity + new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));
					Dust dust = Dust.NewDustPerfect(NPC.Center, dustType, velocity, 26, Color.White, Main.rand.NextFloat(1.5f, 2.4f));

					dust.noLight = true;
					dust.noGravity = true;
					dust.fadeIn = Main.rand.NextFloat(0.3f, 0.8f);
				}
			}
		}

		// Add these fields at the top of the class with other fields
		private const int CHARGE_COOLDOWN = 180; // 3 seconds between charges
		private const float CHARGE_SPEED = 16f; // Speed of the charge attack
		private const int CHARGE_DURATION = 45; // How long the charge lasts
		private int chargeTimer = 0;
		private int chargeCooldown = 60; // Start with a shorter cooldown for first attack
		private bool isCharging = false;

		public override void AI() {
			if (Despawn()) {
				return;
			}

			FadeIn();

			// Only move in formation when not charging
			if (!isCharging) {
				MoveInFormation();
				HandleChargeAttack();
			}
			else {
				UpdateCharge();
			}
		}

		private void HandleChargeAttack() {
			chargeCooldown--;
			if (chargeCooldown <= 0) {
				// Start a charge attack
				isCharging = true;
				chargeTimer = CHARGE_DURATION;
				chargeCooldown = CHARGE_COOLDOWN;

				// Target the nearest player
				Player target = Main.player[Player.FindClosest(NPC.position, NPC.width, NPC.height)];
				if (target.active && !target.dead) {
					Vector2 direction = (target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
					NPC.velocity = direction * CHARGE_SPEED;
				}
			}
		}

		private void UpdateCharge() {
			chargeTimer--;
			if (chargeTimer <= 0) {
				isCharging = false;
				NPC.velocity *= 0.5f; // Slow down after charge
			}
			else {
				// Create dust trail while charging
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenTorch, 
					NPC.velocity.X * 0.2f, NPC.velocity.Y * 0.2f, 100, default, 1.5f);
			}
		}
		private bool Despawn() {
			if (Main.netMode != NetmodeID.MultiplayerClient &&
				(!HasParent || !Main.npc[ParentIndex].active || Main.npc[ParentIndex].type != BodyType())) {
				// * Not spawned by the boss body (didn't assign a position and parent) or
				// * Parent isn't active or
				// * Parent isn't the body
				// => invalid, kill itself without dropping any items
				NPC.active = false;
				NPC.life = 0;
				NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
				return true;
			}
			return false;
		}

		private void FadeIn() {
			// Fade in (we have NPC.alpha = 255 in SetDefaults which means it spawns transparent)
			if (NPC.alpha > 0) {
				NPC.alpha -= 10;
				if (NPC.alpha < 0) {
					NPC.alpha = 0;
				}
			}
		}

		private void MoveInFormation() {
			NPC parentNPC = Main.npc[ParentIndex];

			// This basically turns the NPCs PositionIndex into a number between 0f and TwoPi to determine where around
			// the main body it is positioned at
			float rad = (float)PositionOffset * MathHelper.TwoPi;

			// Add some slight uniform rotation to make the eyes move, giving a chance to touch the player and thus helping melee players
			RotationTimer += 0.5f;
			if (RotationTimer > RotationTimerMax) {
				RotationTimer = 0;
			}

			// Since RotationTimer is in degrees (0..360) we can convert it to radians (0..TwoPi) easily
			float continuousRotation = MathHelper.ToRadians(RotationTimer);
			rad += continuousRotation;
			if (rad > MathHelper.TwoPi) {
				rad -= MathHelper.TwoPi;
			}
			else if (rad < 0) {
				rad += MathHelper.TwoPi;
			}

			float distanceFromBody = parentNPC.width + NPC.width;

			// offset is now a vector that will determine the position of the NPC based on its index
			Vector2 offset = Vector2.One.RotatedBy(rad) * distanceFromBody;

			Vector2 destination = parentNPC.Center + offset;
			Vector2 toDestination = destination - NPC.Center;
			Vector2 toDestinationNormalized = toDestination.SafeNormalize(Vector2.Zero);

			float speed = 8f;
			float inertia = 20;

			Vector2 moveTo = toDestinationNormalized * speed;
			NPC.velocity = (NPC.velocity * (inertia - 1) + moveTo) / inertia;
		}
	}
}