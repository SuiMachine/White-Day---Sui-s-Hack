using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SuisHack.FPS_SettingsHack
{
	[HarmonyPatch]
	class FPS_Settings
	{
		static readonly Dictionary<int, int> indexToRefreshRate = new Dictionary<int, int>();

		[HarmonyPostfix]
		[HarmonyPatch(typeof(UILanguageSetting), "OnEnable")]
		static void AddOptions(ref MenuListGroup ___menuListGroup)
		{
			if (___menuListGroup.selectAbles != null && ___menuListGroup.selectAbles.Count > 9)
			{
				var leftRight = (LeftRightSelectable)___menuListGroup.selectAbles[9];

				var updateRates = new List<int>() { 30, 60, 120, 144 };
				foreach(var resolution in Screen.resolutions)
				{
					if (!updateRates.Contains(resolution.refreshRate))
					{
						var refreshRateToAdd = resolution.refreshRate;
						if (Plugin.Config_Increase_RefreshRateByOne.Value)
							refreshRateToAdd++;

						updateRates.Add(refreshRateToAdd);
					}
				}

				indexToRefreshRate.Clear();

				leftRight.selectData = new LeftRightSelectable.SelectData[updateRates.Count];
				for (int i = 0; i < updateRates.Count; i++)
				{
					leftRight.selectData[i] = new LeftRightSelectable.SelectData() { text = updateRates[i].ToString(), index = i };
					indexToRefreshRate.Add(i, updateRates[i]);
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
			//var desiredFramerate = indexToRefreshRate[(___menuListGroup.selectAbles[9] as LeftRightSelectable).SelectIndex];
			Global.config.frameRates = indexToRefreshRate[(___menuListGroup.selectAbles[9] as LeftRightSelectable).SelectIndex];
			Global.SetFrame();
			return false;
		}
	}
}
