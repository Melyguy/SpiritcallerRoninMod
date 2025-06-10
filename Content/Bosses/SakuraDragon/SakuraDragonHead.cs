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

namespace SpiritcallerRoninMod.Content.Bosses.SakuraDragon;
[AutoloadBossHead]

    public class SakuraDragonHead : ModNPC
    {
        public bool boomboomBool = false;
        public override void SetDefaults()
        {
            NPC.width = 200;
            NPC.height = 200;
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
                        // Rocket attack logic
NPC.localAI[1]++;
if (NPC.localAI[1] >= 180 && Main.netMode != NetmodeID.MultiplayerClient) // Every 3 seconds
{
    NPC.localAI[1] = 0;

    Player target = Main.player[NPC.target];
    if (target != null && target.active && !target.dead)
    {
        Vector2 shootDirection = target.Center - NPC.Center;
        shootDirection.Normalize();
        shootDirection *= 10f; // rocket speed

        int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, shootDirection,
            ModContent.ProjectileType<Content.Projectiles.SakuraBlastBoss>(), 40, 1f, Main.myPlayer);

        Main.projectile[proj].hostile = true;
        Main.projectile[proj].friendly = false;

        SoundEngine.PlaySound(SoundID.NPCHit56, NPC.position); // rocket launch sound
    }
}



            if (!Main.player.Any(p => p.active && !p.dead )) // or just !p.dead
{
    NPC.TargetClosest(false);
    NPC.velocity.Y -= 0.1f; // Fly upward

    if (NPC.timeLeft > 10)
        NPC.timeLeft = 10; // Despawn soon
    return;
}   
{
}


            if (NPC.ai[0] == 0)
            {
                int previous = NPC.whoAmI;
                for (int i = 0; i < 40; ++i) // 40 segments
                {
                    int type = (i == 39) ? ModContent.NPCType<SakuraDragonTail>() 
                    : (i == 38) ? ModContent.NPCType<SakuraDragonBody3>() 
                    : (i == 37) ? ModContent.NPCType<SakuraDragonBody2>()
                    : (i == 3) ? ModContent.NPCType<SakuraDragonLegs>()
                    : (i == 35) ? ModContent.NPCType<SakuraDragonLegs>()    
                    : ModContent.NPCType<SakuraDragonBody>();
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


if (Main.rand.NextBool(30)) // Very rare, dramatic spark
{
    Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.Next(-2, 2), Main.rand.Next(-2, 2)), GoreID.Smoke1);
}
            
        }
        
private void WormMovement()
{
    Player player = Main.player[NPC.target];
    NPC.TargetClosest();

    float flySpeed = 10f; // Base movement speed
    float turnSpeed = 0.02f; // How quickly it turns (lower = wider turns)

    // Desired direction: to the player
    Vector2 toPlayer = player.Center - NPC.Center;

    // Avoid zero vector
    if (toPlayer.LengthSquared() < 20f * 20f)
    {
        // If very close, keep current direction to glide through
        toPlayer = NPC.velocity.SafeNormalize(Vector2.UnitY);
    }
    else
    {
        toPlayer.Normalize();
    }

    // Use rotation-based turning
    Vector2 desiredVelocity = toPlayer * flySpeed;
    NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, turnSpeed);

    // Add a subtle wave for natural movement
    float time = Main.GameUpdateCount * 0.1f;
    float wave = (float)Math.Sin(time + NPC.whoAmI) * 0.3f;
    Vector2 waveOffset = NPC.velocity.RotatedBy(MathHelper.PiOver2) * wave;
    NPC.velocity += waveOffset * 0.05f;

    // Occasionally perform a charge through the player
    if (Main.rand.NextBool(300))
    {
        Vector2 chargeDir = (player.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 20f;
        NPC.velocity = chargeDir;
        SoundEngine.PlaySound(SoundID.DD2_WyvernDiveDown, NPC.position);
    }

    // Rotate sprite to face movement
    if (NPC.velocity != Vector2.Zero)
        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
}




        
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
            float scale = 2f;
            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, origin, 1f, SpriteEffects.None, 0f);
            return false;
        }
        public override void OnKill()
{
    // Play explosion sound
    SoundEngine.PlaySound(SoundID.Item14, NPC.position); // Explosion sound

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
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<TuskOfThePrototype>(), 2)); // 33% chance
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
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<DesertSpiritBag>()));

			// ItemDropRule.MasterModeCommonDrop for the relic
			npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.Furniture.DesertSpiritRelic>()));

			// ItemDropRule.MasterModeDropOnAllPlayers for the pet
			npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ItemID.SandBlock, 10)); //CHANGE THIS LATER!!!
		}
    



}


