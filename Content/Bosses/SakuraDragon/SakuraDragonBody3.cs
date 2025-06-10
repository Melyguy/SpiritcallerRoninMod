using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;

namespace SpiritcallerRoninMod.Content.Bosses.SakuraDragon
{
    public class SakuraDragonBody3 : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 44;
            NPC.damage = 20;
            NPC.defense = 8;
            NPC.lifeMax = 10000;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.dontTakeDamage = true;

            NPC.netAlways = true;
        }

        public override void AI()
        {
            // Validate parent (segment before this one)
            int parentIndex = (int)NPC.ai[1];
            int headIndex = (int)NPC.ai[2];

            if (parentIndex < 0 || parentIndex >= Main.maxNPCs || headIndex < 0 || headIndex >= Main.maxNPCs)
            {
                NPC.active = false;
                return;
            }

            NPC parent = Main.npc[parentIndex];
            NPC head = Main.npc[headIndex];

            NPC.realLife = head.whoAmI;

            // Despawn if head is dead
            if (!head.active || head.life <= 0)
            {
                NPC.active = false;
                return;
            }

            if (!parent.active)
                return;

            // Segment spacing logic
            Vector2 directionToParent = parent.Center - NPC.Center;
            float distanceToParent = directionToParent.Length();

            float desiredSpacing = NPC.width * 0.85f; // Slightly closer than full width to avoid gaps

            if (distanceToParent > desiredSpacing)
            {
                directionToParent.Normalize();
                NPC.Center = parent.Center - directionToParent * desiredSpacing;
            }

            // Rotate to face parent
            NPC.rotation = directionToParent.ToRotation() + MathHelper.PiOver2;

            // Stay stationary to avoid physics interfering
            NPC.velocity = Vector2.Zero;

            // Optional light
            Lighting.AddLight(NPC.Center, 1f, 0f, 0f);
        }

        public override void OnKill()
        {
            SoundEngine.PlaySound(SoundID.Item14, NPC.position);

            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke,
                    Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f), 100, default, 2f);
                Main.dust[dust].noGravity = true;
            }

            for (int i = 0; i < 10; i++)
            {
                int fire = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch,
                    Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f), 150, default, 1.5f);
                Main.dust[fire].noGravity = true;
            }

            for (int i = 0; i < 2; i++)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(2f, 2f),
                    GoreID.Smoke1, 1.5f);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
            float scale = 1f;

            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor,
                NPC.rotation, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool CheckActive() => false;
        public override bool? CanBeHitByItem(Player player, Item item) => false;
        public override bool? CanBeHitByProjectile(Projectile projectile) => false;
    }
}
