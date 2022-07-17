using HarmonyLib;

namespace SuisHack.Cheat
{
	[HarmonyPatch]
	class SecurityGuardCheat
	{
		internal static void InjectEarly(Harmony harmonyInstance)
		{
			{
				var originalMethod = typeof(SecurityGuardBehaviour).GetMethod("UpdateCheckActiveAiSight", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
				var detourMethod = typeof(SecurityGuardCheat).GetMethod(nameof(UpdateCheckActiveAiSightDetour), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
				harmonyInstance.Patch(originalMethod, postfix: new HarmonyMethod(detourMethod));
			}
		}

		public static void UpdateCheckActiveAiSightDetour(ref bool ___ActiveSearchingSystem, ref float ___Hearability, ref float ___targetVisibility)
		{
			___ActiveSearchingSystem = false;
			___Hearability = 0f;
			___targetVisibility = 0f;
		}
	}
}
