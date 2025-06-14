﻿using System.Collections;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SpiritcallerRoninMod.Common.Systems
{
	// Acts as a container for "downed boss" flags.
	// Set a flag like this in your bosses OnKill hook:
	//    NPC.SetEventFlagCleared(ref DownedBossSystem.downedForestGuardian, -1);

	// Saving and loading these flags requires TagCompounds, a guide exists on the wiki: https://github.com/tModLoader/tModLoader/wiki/Saving-and-loading-using-TagCompound
	public class DownedBossSystem : ModSystem
	{
		public static bool downedForestGuardian = false;
		public static bool downedDesertSpirit = false;
		public static bool downedCryoWraith = false;
		public static bool downedMagmaGhoul = false;
		public static bool downedOnryo = false;
		public static bool downedStratoshade = false;
		public static bool DownedPrototype = false;
		
		// public static bool downedOtherBoss = false;

		public override void ClearWorld() {
			downedForestGuardian = false;
			downedDesertSpirit = false;
			downedCryoWraith = false;
			downedMagmaGhoul = false;
			downedOnryo = false;
			downedStratoshade = false;
			DownedPrototype = false;

			// downedOtherBoss = false;
		}

		// We save our data sets using TagCompounds.
		// NOTE: The tag instance provided here is always empty by default.
		public override void SaveWorldData(TagCompound tag) {
			if (downedForestGuardian) {
				tag["downedForestGuardian"] = true;
			}
			if (downedDesertSpirit) {
				tag["downedDesertSpirit"] = true;
			}
			if(downedCryoWraith) {
				tag["downedCryoWraith"] = true;
			}
			if(downedMagmaGhoul) {
				tag["downedMagmaGhoul"] = true;	
			}
			if(downedOnryo) {
				tag["downedOnryo"] = true;	
			}
			if(downedStratoshade) {
				tag["downedStratoshade"] = true;	
			}
			if(DownedPrototype) {
				tag["downedPrototype"] = true;	
			}
			// if (downedOtherBoss) {
			//	tag["downedOtherBoss"] = true;
			// }
		}

		public override void LoadWorldData(TagCompound tag) {
			downedForestGuardian = tag.ContainsKey("downedForestGuardian");
			downedForestGuardian = tag.ContainsKey("downedDesertSpirit");
			downedForestGuardian = tag.ContainsKey("downedCryoWraith");
			downedForestGuardian = tag.ContainsKey("downedMagmaGhoul");
			downedForestGuardian = tag.ContainsKey("downedOnryo");
			downedForestGuardian = tag.ContainsKey("downedStratoshade");
			downedForestGuardian = tag.ContainsKey("downedPrototype");
			// downedOtherBoss = tag.ContainsKey("downedOtherBoss");
		}

		public override void NetSend(BinaryWriter writer) {
			// Order of parameters is important and has to match that of NetReceive
			writer.WriteFlags(downedForestGuardian/*, downedOtherBoss*/);
			writer.WriteFlags(downedDesertSpirit/*, downedOtherBoss*/);
			writer.WriteFlags(downedCryoWraith/*, downedOtherBoss*/);
			writer.WriteFlags(downedMagmaGhoul/*, downedOtherBoss*/);
			writer.WriteFlags(downedOnryo/*, downedOtherBoss*/);
			writer.WriteFlags(downedStratoshade/*, downedOtherBoss*/);
			writer.WriteFlags(DownedPrototype/*, downedOtherBoss*/);

			// WriteFlags supports up to 8 entries, if you have more than 8 flags to sync, call WriteFlags again.

			// If you need to send a large number of flags, such as a flag per item type or something similar, BitArray can be used to efficiently send them. See Utils.SendBitArray documentation.
		}

		public override void NetReceive(BinaryReader reader) {
			// Order of parameters is important and has to match that of NetSend
			reader.ReadFlags(out downedForestGuardian/*, out downedOtherBoss*/);
			reader.ReadFlags(out downedDesertSpirit/*, out downedOtherBoss*/);
			reader.ReadFlags(out downedCryoWraith/*, out downedOtherBoss*/);
			reader.ReadFlags(out downedMagmaGhoul/*, out downedOtherBoss*/);
			reader.ReadFlags(out downedOnryo/*, out downedOtherBoss*/);
			reader.ReadFlags(out downedStratoshade/*, out downedOtherBoss*/);
			reader.ReadFlags(out DownedPrototype/*, out downedOtherBoss*/);
			// ReadFlags supports up to 8 entries, if you have more than 8 flags to sync, call ReadFlags again.
		}
	}
}