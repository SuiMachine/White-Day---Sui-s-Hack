using HarmonyLib;

namespace SuisHack.Cheat
{
	[HarmonyPatch]
	static class GodModeCheat
	{
		public static bool Use;

		[HarmonyPostfix]
		[HarmonyPatch(typeof(PlayerBehaviour), nameof(PlayerBehaviour.OnAwake))]
		public static void OnAwakePlayerBehaviourPostfix(ref bool ___godMode)
		{
			if(Use)
				___godMode = true;
		}
	}
}
