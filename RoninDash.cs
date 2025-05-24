using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritcallerRoninMod
{
    public class RoninDashPlayer : ModPlayer
    {
        public bool dashTabi;
        private int dashDelay = 0;
        private const int DashCooldown = 30;
        private const float DashSpeed = 14f;

        private int lastDash = 0;
        public bool ignoreNextDash = false;

        public override void ResetEffects()
        {
            // If we remove the accessory, clear ignore flag
            if (!dashTabi)
                ignoreNextDash = false;

            dashTabi = false;
        }

        public override void PostUpdateRunSpeeds()
        {
            if (!dashTabi || dashDelay > 0)
                return;

            if (ignoreNextDash && Player.dash != 0)
            {
                // Skip the first frame where dashType is set to prevent auto-dash
                ignoreNextDash = false;
                lastDash = Player.dash;
                return;
            }

            if (Player.dash != 0 && Player.dash != lastDash)
            {
                lastDash = Player.dash;
                DoDash(Player.dash == 1 ? 1 : -1);
            }
            else if (Player.dash == 0)
            {
                lastDash = 0;
            }
        }

        private void DoDash(int direction)
        {
            Player.velocity.X = direction * DashSpeed;
            dashDelay = DashCooldown;
            Player.dashDelay = DashCooldown;

            // Visual effect
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(Player.position, Player.width, Player.height, DustID.Smoke,
                    Player.velocity.X * 0.5f, Player.velocity.Y * 0.5f);
            }

            // Hitbox damage on enemies
            Rectangle dashHitbox = Player.Hitbox;
            dashHitbox.Inflate(20, 0);
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && !npc.friendly && !npc.dontTakeDamage && npc.Hitbox.Intersects(dashHitbox))
                {
                    Player.ApplyDamageToNPC(npc, 40, 0f, Player.direction, crit: false);
                }
            }
        }

        public override void PostUpdate()
        {
            if (dashDelay > 0)
                dashDelay--;
        }
    }
}
