using HarmonyLib;

namespace SuisHack.Cheat
{
	[HarmonyPatch]
	static class SecurityGuardCheat
	{
		public static bool Use;

		[HarmonyPostfix]
		[HarmonyPatch(typeof(SecurityGuardBehaviour), "UpdateCheckActiveAiSight")]
		public static void UpdateCheckActiveAiSightDetour(ref bool ___ActiveSearchingSystem, ref float ___Hearability, ref float ___targetVisibility)
		{
			if(Use)
			{
				___ActiveSearchingSystem = false;
				___Hearability = 0f;
				___targetVisibility = 0f;
			}
		}
	}
}
