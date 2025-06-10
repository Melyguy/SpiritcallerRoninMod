using SpiritcallerRoninMod.Content.Biomes;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace SpiritcallerRoninMod.Content.Enemies
{
    public class SakuraWisp : ModNPC
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            NPC.width = 28;
            NPC.height = 44;
            NPC.damage = 40;
            NPC.defense = 10;
            NPC.lifeMax = 100;
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.value = 10; // No money drops
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = 14; // Floating AI, similar to Wraith
            AIType = NPCID.Wraith;
            AnimationType = 0; // No animation;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome(ModContent.GetInstance<SakuraForestBiome>()) && !NPC.AnyNPCs(Type)) {
                return SpawnCondition.OverworldDaySlime.Chance; // Spawn with 1/10th the chance of a regular zombie.
            }

            return 0f;
        }



    }
}
