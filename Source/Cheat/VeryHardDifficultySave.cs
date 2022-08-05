using HarmonyLib;
using UnityEngine;
using Utils;

namespace SuisHack.Cheat
{
	[HarmonyPatch]
	public static class VeryHardDifficultySave
	{
		public static bool Use = false;
		private static LevelofDifficulty TempDifficulty;

		[HarmonyPrefix]
		[HarmonyPatch(typeof(SavePanelBehaviour), nameof(SavePanelBehaviour.OnClickSave))]
		public static void OnClickSavePrefix()
		{
			TempDifficulty = Global.playerInfo.difficulty;
			if (Use)
			{
				Global.playerInfo.difficulty = LevelofDifficulty.Hard;
			}
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(SavePanelBehaviour), nameof(SavePanelBehaviour.OnClickSave))]
		public static void OnClickSavePostfix()
		{
			if (Use)
			{
				Global.playerInfo.difficulty = TempDifficulty;
			}
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(EventTrigger), "CoroutineQuery")]
		public static bool CoroutineTriggerEventCallerPrefix(EventTrigger __instance, EventNodeData data)
		{
			if(Use && (EventQueryType)data.node.intParam1 == EventQueryType.IsDifficultyOverThen)
			{
				GameObject target = null;
				EventQueryType intParam = (EventQueryType)data.node.intParam1;
				switch (intParam)
				{
					case EventQueryType.IsMainCameraView:
					case EventQueryType.IsSaveEnable:
					case EventQueryType.HasItemQuantity:
					case EventQueryType.IsDifficultyOverThen:
					case EventQueryType.HasThisMagicPen:
					case EventQueryType.IsBreathless:
					case EventQueryType.IsObserverCameraView:
					case EventQueryType.IsSubScenario:
					case EventQueryType.IsEnabledSubScenario:
						break;
					default:
						if (intParam != EventQueryType.HasItem)
						{
							target = EventTrigger.GetTargetObject(__instance, data.node);
							if (target == null)
							{
								WDLogger.LogError("ERROR : Cannot find target object.", new object[] { __instance.gameObject });
								return true;
							}
						}
						break;
				}
				data.nextNodeIndex = 1;
				return false;
			}
			return true;
		}
	}
}
