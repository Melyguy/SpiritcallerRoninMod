using System.Numerics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Vector2 = Microsoft.Xna.Framework.Vector2;


namespace SpiritcallerRoninMod.Content.Projectiles;

public class AkaneAoeAttack : ModProjectile
{
    private const int MaxRadius = 200;
    private const int ExpandTime = 30;

    public override void SetDefaults()
    {
        Projectile.width = 10;
        Projectile.height = 10;
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.timeLeft = ExpandTime;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
    }

    public override void AI()
    {
        float progress = 1f - (Projectile.timeLeft / (float)ExpandTime);
        int radius = (int)(MaxRadius * progress);

        Projectile.scale = progress;
        Projectile.width = Projectile.height = radius * 2;
        Projectile.Center = Projectile.position + new Vector2(Projectile.width / 2f, Projectile.height / 2f);

        // Dust or visual effect
        for (int i = 0; i < 5; i++)
        {
            Vector2 dustPos = Projectile.Center + Main.rand.NextVector2CircularEdge(radius, radius);
            Dust.NewDustPerfect(dustPos, DustID.Shadowflame, Vector2.Zero).noGravity = true;
        }

        // Damage players in the radius
        foreach (Player player in Main.player)
        {
            if (player.active && !player.dead && player.Hitbox.Intersects(Projectile.Hitbox))
            {
                player.Hurt(PlayerDeathReason.ByProjectile(Projectile.whoAmI, Projectile.owner), 60, 0);
            }
        }
    }
}
