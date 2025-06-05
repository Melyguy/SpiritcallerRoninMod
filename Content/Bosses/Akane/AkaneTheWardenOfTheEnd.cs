using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using System;
using Terraria.GameContent.ItemDropRules;
using SpiritcallerRoninMod.Content.Items.Weapons;
using SpiritcallerRoninMod.Content.Items.Consumables;
namespace SpiritcallerRoninMod.Content.Bosses.Akane;
[AutoloadBossHead]

    public class AkaneTheWardenOfTheEnd : ModNPC
    {
        public override void SetStaticDefaults()
{
    Main.npcFrameCount[NPC.type] = 40;
}
public override void SetDefaults()
{
    NPC.width = 100;
    NPC.height = 140;
    NPC.damage = 60;
    NPC.defense = 60;
    NPC.lifeMax = 200000;
    NPC.HitSound = SoundID.NPCHit1;
    NPC.DeathSound = SoundID.NPCDeath1;
    NPC.value = Item.buyPrice(0, 20, 0, 0);
    NPC.knockBackResist = 0f;
    NPC.aiStyle = -1;
    NPC.noTileCollide = false;
    Music = MusicID.OtherworldlyBoss1;
    NPC.noGravity = false;
    NPC.boss = true;

}
public bool firstphase = true;

public override void FindFrame(int frameHeight)
{
    NPC.frameCounter++;
    int frameSpeed = 5;

    // Phase check: 0 = phase one, 1 = phase two
    int phaseOffset = (int)NPC.ai[3] == 1 ? 20 : 0;

    switch ((int)NPC.ai[0])
    {
        case 0: // Idle (frames 0–4) or (20–23)
            NPC.frame.Y = ((int)(NPC.frameCounter / frameSpeed) % 3 + phaseOffset + 0) * frameHeight;
            break;

        case 1: // Shooting frame 5-11
            NPC.frame.Y = ((int)(NPC.frameCounter / frameSpeed) % 6 + phaseOffset + 5) * frameHeight;
            break;

        case 2: // Slashing attack (frames 8–11 or 28–31)
            NPC.frame.Y = ((int)(NPC.frameCounter / frameSpeed) % 4 + phaseOffset + 8) * frameHeight;
            break;

        case 3: // Casting/charge (frames 12–15 or 32–35)
            NPC.frame.Y = ((int)(NPC.frameCounter / frameSpeed) % 5 + phaseOffset + 12) * frameHeight;
            break;

        case 4: // Transformed attack/enraged (frames 16–19 or 36–39)
            NPC.frame.Y = ((int)(NPC.frameCounter / frameSpeed) % 4 + phaseOffset + 16) * frameHeight;
            break;

        default:
            NPC.frame.Y = phaseOffset * frameHeight;
            break;
    }

    // Reset counter after one full animation cycle
    if (NPC.frameCounter >= frameSpeed * 7)
        NPC.frameCounter = 0;
}
        public void Movement()
        {
            if(firstphase){
                Player player = Main.player[NPC.target];
                float moveSpeed = 4f; // Increased speed
                float acceleration = 0.3f; // Increased acceleration
    
                // Calculate direction to player
                float xDiff = player.Center.X - NPC.Center.X;
                // Always move towards player
               NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, Math.Sign(xDiff) * moveSpeed, acceleration);
    
                // Set animation state
                //NPC.ai[0] = Math.Abs(NPC.velocity.X) > 0.5f ? 1 : 0;
    
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

        }

    public override void AI()
    {
        Movement();
                NPC.TargetClosest(true);
                Player player = Main.player[NPC.target];
                NPC.spriteDirection = NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;

    
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




    }
