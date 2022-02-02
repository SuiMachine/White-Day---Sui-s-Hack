using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace SuisHack.FPS_SettingsHack
{
	class MouseInvert
	{
		public static void InjectEarly(Harmony harmonyInstance)
		{
			{
				var originalMethod = typeof(PlayerBehaviour).GetMethod("UpdateMoveControlStandAlone", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				var detourMethod = typeof(MouseInvert).GetMethod(nameof(UpdateMoveControlStandAloneTranspiler), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
				harmonyInstance.Patch(originalMethod, transpiler: new HarmonyMethod(detourMethod));
			}
		}

		static IEnumerable<CodeInstruction> UpdateMoveControlStandAloneTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			var codes = new List<CodeInstruction>(instructions);
			var inputType = AccessTools.TypeByName("UnityEngine.Input");

			var codeMatcher = new CodeMatcher(codes).MatchForward(true,
				new CodeMatch(OpCodes.Ldstr, "Mouse Y"),
				new CodeMatch(OpCodes.Call, AccessTools.Method(inputType, "GetAxis", new Type[] { typeof(string) }))
				);


			if(codeMatcher.Pos == 0 || codeMatcher.Pos == instructions.Count())
			{
				Plugin.log.LogError("Failed to find proper mouse input signature");
				return codes.AsEnumerable();
			}

			Plugin.log.LogWarning("Found mouse Y at:" + codeMatcher.Pos);
			codeMatcher.Advance(1).RemoveInstruction();
			codeMatcher.InsertAndAdvance(
				new CodeInstruction(OpCodes.Ldc_R4, -1f),
				new CodeInstruction(OpCodes.Mul),
				new CodeInstruction(OpCodes.Ldloc_2)
				);
			codes = codeMatcher.Instructions();

			return codes.AsEnumerable();
		}
	}
}
