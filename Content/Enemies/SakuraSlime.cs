using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.Utilities;
using SpiritcallerRoninMod.Content.Biomes;

namespace SpiritcallerRoninMod.Content.Enemies
{
    public class SakuraSlime : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[2];
        }

        public override void SetDefaults()
        {
            NPC.width = 34;
            NPC.height = 16;
            NPC.damage = 11;
            NPC.defense = 3;
            NPC.lifeMax = 50;
            NPC.value = 10000f;
            NPC.aiStyle = 1;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            AIType = NPCID.GreenSlime;
            AnimationType = NPCID.GreenSlime;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome(ModContent.GetInstance<SakuraForestBiome>()) && !NPC.AnyNPCs(Type)) {
                return SpawnCondition.OverworldDaySlime.Chance; 
            }

            return 0f;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 20)
            {
                NPC.frameCounter = 0;
            }
            NPC.frame.Y = (int)NPC.frameCounter / 10 * frameHeight;
        }

        public override void OnKill()
        {
            Item.NewItem(NPC.GetSource_Death(), NPC.getRect(), ItemID.Gel, Main.rand.Next(0, 2));
            Item.NewItem(NPC.GetSource_Death(), NPC.getRect(), ItemID.DynastyWood, Main.rand.Next(0, 4));
        }

    }
}