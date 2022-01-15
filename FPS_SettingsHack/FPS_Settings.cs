using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SuisHack.FPS_SettingsHack
{
	[HarmonyPatch]
	class FPS_Settings
	{
		static Dictionary<int, int> indexToRefreshRate = new Dictionary<int, int>();

		[HarmonyPostfix]
		[HarmonyPatch(typeof(UILanguageSetting), "OnEnable")]
		static void addOptions(ref MenuListGroup ___menuListGroup)
		{
			if (___menuListGroup.selectAbles != null && ___menuListGroup.selectAbles.Count > 9)
			{
				var leftRight = (LeftRightSelectable)___menuListGroup.selectAbles[9];
				var screenDisplayRefreshRates = new int[] { 30, 60, 120, 144, 165, 240, 99999 }; //Screen.resolutions.Select(x => x.refreshRate).Distinct().OrderBy(x => x).ToArray();
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

/*		[HarmonyPrefix]
		[HarmonyPatch(typeof(SystemManager), nameof(SystemManager.SetFrameRate))]
		static bool DetouredGlobal(int frameRate)
		{			
			if (frameRate == -1)
			{
				QualitySettings.vSyncCount = 1;
				Application.targetFrameRate = 0;
			}
			else
			{
				QualitySettings.vSyncCount = 0;
				Application.targetFrameRate = frameRate;
			}
			return false;
		}*/
	}
}
