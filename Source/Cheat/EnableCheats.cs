using HarmonyLib;
using SuisHack.FPS_SettingsHack;

namespace SuisHack.Cheat
{
	[HarmonyPatch]
	class EnableCheats
	{
		internal static void InjectEarly(Harmony harmonyInstance)
		{
			harmonyInstance.Patch(typeof(PlayerBehaviour).GetMethod(nameof(PlayerBehaviour.OnAwake)), postfix: new HarmonyMethod(typeof(EnableCheats).GetMethod(nameof(OnAwakePlayerBehaviourPostfix))));
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(PlayerBehaviour), nameof(PlayerBehaviour.OnAwake))]
		public static void OnAwakePlayerBehaviourPostfix(ref bool ___godMode)
		{
			___godMode = true;
		}
	}
}
