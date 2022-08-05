using HarmonyLib;

namespace SuisHack.Components
{
	[HarmonyPatch]
	public static class PlayerBehaviourPatches
	{
		public static bool UseInterpolation = true;

		[HarmonyPostfix]
		[HarmonyPatch(typeof(PlayerBehaviour), "OnAwake")]
		public static void OnAwakePostifx(ref PlayerBehaviour __instance)
		{
			__instance.gameObject.AddComponent<PlayerBehaviourCustomUpdate>();
			//Plugin.log.LogMessage("Added!");
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(PlayerBehaviour), "UpdateMoveControl")]
		public static bool UpdateMoveControlPrefix()
		{
			if (UseInterpolation)
				return false;
			else
				return true;
		}
	}
}
