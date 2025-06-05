using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritcallerRoninMod
{
public class RoninPlayer : ModPlayer
{
 public int Focus = 0;
    public const int MaxFocus = 100;

    public bool HasRoninSet = false;
        private int debugTimer;

        public override void ResetEffects()
    {
        HasRoninSet = false; // Always reset — set again if the player has armor
    }

    public override void PostUpdate()
    {
            Player player = this.Player;

        if (HasRoninSet && !player.controlUseItem && player.itemAnimation == 0 && player.itemTime == 0)
        {


            Focus = Math.Min(Focus + 1, MaxFocus); // Passive regen only if armor is worn
        }
    }

    public bool ConsumeFocus(int amount)
    {
        if (HasRoninSet && Focus >= amount)
        {
            Focus -= amount;
            return true;
        }
        return false;
    }

    public override void ModifyDrawInfo(ref Terraria.DataStructures.PlayerDrawSet drawInfo)
    {
        if (HasRoninSet)
        {
            DrawFocusBar();
        }
    }

    private void DrawFocusBar()
    {
        Vector2 screenPosition = Main.screenPosition;
        Vector2 playerCenter = Player.Center;
        Vector2 barPosition = playerCenter - screenPosition + new Vector2(-20, 40);

        int barWidth = 40;
        int barHeight = 5;

        // Background
        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)barPosition.X, (int)barPosition.Y, barWidth, barHeight), Color.Gray * 0.6f);
        int fillWidth = (int)((Focus / (float)MaxFocus) * barWidth);
        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)barPosition.X, (int)barPosition.Y, fillWidth, barHeight), Color.LimeGreen);
        
        
    }

   
}
}

