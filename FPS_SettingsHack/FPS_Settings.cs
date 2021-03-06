using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace SuisHack.FPS_SettingsHack
{
	[HarmonyPatch]
	class FPS_Settings
	{
		static Dictionary<int, int> indexToRefreshRate = new Dictionary<int, int>();


		public static void InjectEarly(Harmony harmonyInstance)
		{
			{
				var originalMethod = typeof(UILanguageSetting).GetMethod("OnEnable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				var detourMethod = typeof(FPS_Settings).GetMethod(nameof(addOptions), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
				harmonyInstance.Patch(originalMethod, postfix: new HarmonyMethod(detourMethod));
			}

			{
				var originalMethod = typeof(UILanguageSetting).GetMethod("OnValueChangeFrame", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
				var detourMethod = typeof(FPS_Settings).GetMethod(nameof(OnValueChangeFrameDetour), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
				harmonyInstance.Patch(originalMethod, prefix: new HarmonyMethod(detourMethod));
			}
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(UILanguageSetting), "OnEnable")]
		static void addOptions(ref MenuListGroup ___menuListGroup)
		{
			if (___menuListGroup.selectAbles != null && ___menuListGroup.selectAbles.Count > 9)
			{
				var leftRight = (LeftRightSelectable)___menuListGroup.selectAbles[9];
				var screenDisplayRefreshRates = new int[] { 30, 60, 120, 144, 165, 240 }; //Screen.resolutions.Select(x => x.refreshRate).Distinct().OrderBy(x => x).ToArray();
				indexToRefreshRate.Clear();

				leftRight.selectData = new LeftRightSelectable.SelectData[screenDisplayRefreshRates.Length];
				for (int i = 0; i < screenDisplayRefreshRates.Length; i++)
				{
					leftRight.selectData[i] = new LeftRightSelectable.SelectData() { text = screenDisplayRefreshRates[i].ToString(), index = i };
					indexToRefreshRate.Add(i, screenDisplayRefreshRates[i]);
				}


				if (indexToRefreshRate.ContainsValue(Global.config.frameRates))
				{
					var values = indexToRefreshRate.ToArray();
					for (int i = 0; i < values.Length; i++)
					{
						var value = values[i].Value;
						if (value == Global.config.frameRates)
						{
							leftRight.SelectIndex = i;
							break;
						}
					}
				}

			}
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(UILanguageSetting), "OnValueChangeFrame")]
		static bool OnValueChangeFrameDetour(ref bool ___mModify, ref MenuListGroup ___menuListGroup)
		{
			___mModify = true;
			var desiredFramerate = indexToRefreshRate[(___menuListGroup.selectAbles[9] as LeftRightSelectable).SelectIndex];
			Global.config.frameRates = indexToRefreshRate[(___menuListGroup.selectAbles[9] as LeftRightSelectable).SelectIndex];
			Global.SetFrame();
			return false;
		}
	}
}
