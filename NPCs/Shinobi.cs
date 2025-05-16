using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.Creative;
using Terraria.GameContent.Personalities;
using SpiritcallerRoninMod.Content.Biomes;

namespace SpiritcallerRoninMod.NPCs
{
    [AutoloadHead]
    public class Shinobi : ModNPC
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 20;
            NPC.height = 20;
            NPC.aiStyle = 7;
            NPC.defense = 40;
            NPC.lifeMax = 320;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;

            Main.npcFrameCount[NPC.type] = 25;
            NPCID.Sets.ExtraFramesCount[NPC.type] = 0;
            NPCID.Sets.AttackFrameCount[NPC.type] = 1;
            NPCID.Sets.DangerDetectRange[NPC.type] = 200;
            NPCID.Sets.AttackType[NPC.type] = 1;
            NPCID.Sets.AttackTime[NPC.type] = 30;
             NPCID.Sets.AttackAverageChance[NPC.type] = 30;
             NPCID.Sets.HatOffsetY[NPC.type] = 4;
            AnimationType = 22;
        NPC.Happiness
        .SetBiomeAffection<ForestBiome>(AffectionLevel.Love)
        .SetBiomeAffection<SakuraForestBiome>(AffectionLevel.Love)
        .SetBiomeAffection<UndergroundBiome>(AffectionLevel.Like)
        .SetBiomeAffection<DesertBiome>(AffectionLevel.Dislike)
        .SetNPCAffection(NPCID.Merchant, AffectionLevel.Like)
        .SetNPCAffection(NPCID.Guide, AffectionLevel.Love)
        .SetNPCAffection(NPCID.Angler, AffectionLevel.Like)
         .SetNPCAffection(NPCID.Nurse, AffectionLevel.Dislike)
         .SetNPCAffection(NPCID.TaxCollector, AffectionLevel.Hate)
        .SetNPCAffection(NPCID.ArmsDealer, AffectionLevel.Dislike);

        }

        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            for (var i = 0; i < 255; i++){
                Player player = Main.player[i];
                foreach (Item item in player.inventory){
                    if (item.type == ModContent.ItemType<Content.Items.Weapons.KatanaOfEvil>()){
                        return true;
                    }
                    else if(item.type == ModContent.ItemType<Content.Items.Weapons.EvilSealedKatana>()){
                        return true;
                    }
                }

            }
            return false;
        }

        public override List<string> SetNPCNameList()
        {
           return new List<string>()
           {
                "Sekiro",
                "Sekijo",
                "Shura"
           };
        }
        
    

    public override void SetChatButtons(ref string button, ref string button2)
    {
        button = "Shop";
        button2 = "";
    }
    
    public override void OnChatButtonClicked(bool firstButton, ref string shop)
    {
        if (firstButton)
        {
            shop = "Shop";
        }
    }

        /* public override void SetupShop(Chest shop, ref int nextSlot){
             shop.item[nextSlot].SetDefaults(ModContent.ItemType<Content.Items.Weapons.mortalBlade>());
             shop.item[nextSlot].value = 1000000;
             nextSlot++;
             shop.item[nextSlot].SetDefaults(ItemID.Shuriken);
             shop.item[nextSlot].value = 10;
             nextSlot++;
             shop.item[nextSlot].SetDefaults(ItemID.Katana);
             shop.item[nextSlot].value = 50000;
             nextSlot++;
             shop.item[nextSlot].SetDefaults(ItemID.Muramasa);
             shop.item[nextSlot].value = 500000;
             nextSlot++;
             shop.item[nextSlot].SetDefaults(ItemID.SmokeBomb);
             shop.item[nextSlot].value = 40;
             nextSlot++;
             shop.item[nextSlot].SetDefaults(ItemID.GrapplingHook);
             shop.item[nextSlot].value = 500;
             nextSlot++;
             shop.item[nextSlot].SetDefaults(ItemID.MasterNinjaGear);
             shop.item[nextSlot].value = 1000000;
             nextSlot++;
         }*/
        public override void AddShops()
        {
            NPCShop Sekiroshop = new NPCShop(Type, "Shop")
                .Add(ModContent.ItemType<Content.Items.Weapons.mortalBlade>(), Condition.DownedPlantera)
                .Add(ItemID.Shuriken, Condition.Hardmode)
                .Add(ItemID.Katana, Condition.Hardmode)
                .Add(ItemID.Muramasa, Condition.DownedSkeletron)
                .Add(ItemID.SmokeBomb, Condition.Hardmode)
                .Add(ItemID.GrapplingHook, Condition.Hardmode)
                .Add(ItemID.MasterNinjaGear, Condition.DownedPlantera);
            
            Sekiroshop.Register();
        }

    

    public override string GetChat(){
        NPC.FindFirstNPC(ModContent.NPCType<NPCs.Shinobi>());
        switch(Main.rand.Next(6))
        {
            case 0:   
                return "Hesitation Is Defeat...";
            case 1:
                return "A code must be determined by the individual... This is what I've decided.";
            case 2:
                return "One!... The parent is absolute. Their will must be obeyed.";
            case 3:
                return "Two!... The master is absolute. You give your life to keep him safe. You bring him back at any cost.";
            case 4:
                return "Three!... Fear is absolute. There is no shame in losing one battle. But you must take revenge by any means necessary!";        
            default:
                return "Where is Lord Kuro."; 
        }
    }

    public override void TownNPCAttackStrength(ref int damage, ref float knockback){
        damage = 50;
        knockback = 10f;

    
    } 
    public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown){
        cooldown = 5;
        randExtraCooldown = 10;


    }
    public override void TownNPCAttackProj(ref int projType, ref int attackDelay){
        projType = ProjectileID.Shuriken;
        attackDelay = 1;
    }
    public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset){
        multiplier = 7f;
    }
        public override void OnKill()
        {
            Item.NewItem(NPC.GetSource_Death(), NPC.getRect(), ItemID.Shuriken, Main.rand.Next(1, 3), false, 0, false, false);
        }





    }


}

