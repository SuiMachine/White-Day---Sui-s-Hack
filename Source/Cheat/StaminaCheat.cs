using HarmonyLib;

namespace SuisHack.Cheat
{
	class StaminaCheat
	{
		internal static void InjectEarly(Harmony harmonyInstance)
		{
			var originalMethod = typeof(GameManager).GetMethod("UpdateBreath", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
			var detourMethod = typeof(StaminaCheat).GetMethod(nameof(UpdateBreathDetour), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
			harmonyInstance.Patch(originalMethod, prefix: new HarmonyMethod(detourMethod));
		}

		static void UpdateBreathDetour()
		{
			Global.playerInfo.breath = 100f;
		}
	}
}
