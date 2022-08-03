using HarmonyLib;

namespace SuisHack.Cheat
{
	[HarmonyPatch]
	public static class InventoryCheat
	{
		public static bool EnableSaveCheat;

		[HarmonyPrefix]
		[HarmonyPatch(typeof(PlayerInfo), nameof(PlayerInfo.HasItem))]
		public static bool HasItemPrefix(ref bool __result, int itemID)
		{
			if (EnableSaveCheat && itemID == 40040)
			{
				__result = true;
				return false;
			}
			return true;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(PlayerInfo), nameof(PlayerInfo.GetItemQuantity))]
		public static bool GetItemQuantityPrefix(ref int __result, int itemID)
		{
			if(EnableSaveCheat && itemID == 40040)
			{
				__result = 25;
				return false;
			}
			return true;
		}
	}
}
