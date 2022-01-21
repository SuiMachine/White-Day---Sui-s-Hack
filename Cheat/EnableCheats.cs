using HarmonyLib;

namespace SuisHack.Cheat
{
	[HarmonyPatch]
	class EnableCheats
	{
		[HarmonyPostfix]
		[HarmonyPatch(typeof(PlayerBehaviour), nameof(PlayerBehaviour.OnAwake))]
		public static void OnAwakePlayerBehaviourPostfix(ref bool ___godMode)
		{
			___godMode = true;
		}
	}
}
