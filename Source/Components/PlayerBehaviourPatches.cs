using HarmonyLib;

namespace SuisHack.Components
{
	[HarmonyPatch]
	public static class PlayerBehaviourPatches
	{
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
			return false;
		}
	}
}
