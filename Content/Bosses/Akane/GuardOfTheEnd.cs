using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritcallerRoninMod.Content.Bosses.Akane
{
    public class GuardOfTheEnd : ModNPC
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
            NPC.lifeMax = 50;
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.value = 0; // No money drops
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = 14; // Floating AI, similar to Wraith
            AIType = NPCID.Wraith;
            AnimationType = 0; // No animation;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            // Customize where it spawns — maybe only at night or in special biomes
            return spawnInfo.Player.ZoneOverworldHeight && !Main.dayTime ? 0.05f : 0f;
        }



    }
}
