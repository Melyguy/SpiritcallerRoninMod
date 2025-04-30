using SpiritcallerRoninMod.Common.Systems;
using SpiritcallerRoninMod.Content.Items;
using SpiritcallerRoninMod.Content.Items.Consumables;
using SpiritcallerRoninMod.Content.Items.Weapons;
using SpiritcallerRoninMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;


namespace SpiritcallerRoninMod.Content.Bosses.DesertSpirit;
public class DesertSpiritHand : ModNPC
{
    private const int AttackCooldown = 180;
    private const int AimDuration = 30;
    private const float OrbitDistance = 120f;
    private const float TornadoOffset = 100f; // Distance to place tornados from player
    
    // AI States
    public ref float AttackTimer => ref NPC.ai[2];
    public ref float IsAttacking => ref NPC.ai[3];

    public override void SetDefaults()
    {
        NPC.width = 30;
        NPC.height = 30;
        NPC.damage = 30;
        NPC.defense = 5;
        NPC.lifeMax = 1500;
        NPC.knockBackResist = 0f;
        NPC.noGravity = true;
        NPC.noTileCollide = false;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath3;
        NPC.aiStyle = -1;
    }

    public override void AI()
    {
        int headIndex = (int)NPC.ai[0];
        if (!Main.npc[headIndex].active || Main.npc[headIndex].type != ModContent.NPCType<DesertSpirit>())
        {
            NPC.active = false;
            return;
        }

        Player player = Main.player[Main.npc[headIndex].target];
        Vector2 toPlayer = player.Center - NPC.Center;
        
        AttackTimer++;
        
        if (AttackTimer >= AttackCooldown)
        {
            // Reset timer and start attack
            AttackTimer = 0;
            IsAttacking = 1f;
            
            // Telegraph the attack with a sound
            SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
        }

        if (IsAttacking == 1f)
        {
            if (AttackTimer == AimDuration)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // Calculate offset based on which hand this is (left or right)
                    float sideOffset = NPC.ai[1] * TornadoOffset; // Uses the hand's side indicator (-1 or 1)
                    
                    // Calculate position beside the player
                    Vector2 targetPos = player.Center + new Vector2(sideOffset, 0);
                    Vector2 shootVelocity = (targetPos - NPC.Center).SafeNormalize(Vector2.Zero) * 8f;
                    
                    int damage = NPC.damage / 2;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, shootVelocity, 
                        ProjectileID.SandnadoHostile, damage, 2f, Main.myPlayer);
                    
                    SoundEngine.PlaySound(SoundID.Item60, NPC.Center);
                }
                IsAttacking = 0f;
            }
            else
            {
                // Aim at the target position beside the player
                float sideOffset = NPC.ai[1] * TornadoOffset;
                Vector2 targetPos = player.Center + new Vector2(sideOffset, 0);
                NPC.rotation = (targetPos - NPC.Center).ToRotation() + MathHelper.PiOver2;
                NPC.velocity *= 0.9f;
            }
        }
        else
        {
            // Normal rotation around head
            float angleOffset = NPC.ai[1] == -1 ? -1.5f : 1.5f;
            Vector2 center = Main.npc[headIndex].Center;
            Vector2 desiredPosition = center + new Vector2(
                (float)Math.Cos(Main.GameUpdateCount * 0.05f + angleOffset) * OrbitDistance,
                (float)Math.Sin(Main.GameUpdateCount * 0.05f + angleOffset) * OrbitDistance
            );
            NPC.Center = Vector2.Lerp(NPC.Center, desiredPosition, 0.1f);
            NPC.velocity *= 0.9f;
        }

        // Update rotation to face movement or aim direction
        if (IsAttacking == 1f && AttackTimer < AimDuration)
        {
            // Face the player while aiming
            NPC.rotation = toPlayer.ToRotation() + MathHelper.PiOver2;
        }
        else if (NPC.velocity != Vector2.Zero)
        {
            // Face movement direction
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
        }
    }
}
