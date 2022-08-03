using HarmonyLib;
using SuisHack.Components;

namespace SuisHack.Cheat
{
	class EnableCheats
	{
		internal static void InjectEarly(Harmony harmonyInstance)
		{
			harmonyInstance.Patch(typeof(PlayerBehaviour).GetMethod(nameof(PlayerBehaviour.OnAwake)), postfix: new HarmonyMethod(typeof(EnableCheats).GetMethod(nameof(OnAwakePlayerBehaviourPostfix))));
		}

		public static void OnAwakePlayerBehaviourPostfix(ref bool ___godMode)
		{
			___godMode = true;
		}
	}
}
