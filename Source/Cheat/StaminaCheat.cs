using HarmonyLib;

namespace SuisHack.Cheat
{
	[HarmonyPatch]
	class StaminaCheat
	{
		public static bool Use;

		[HarmonyPrefix]
		[HarmonyPatch(typeof(GameManager), nameof(GameManager.UpdateBreath))]
		static void UpdateBreathDetour()
		{
			if (Use)
				Global.playerInfo.breath = 100f;
		}
	}
}
