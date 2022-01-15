using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

namespace SuisHack.Input
{
	[HarmonyPatch(typeof(PlayerBehaviour))]
	public static class MouseHack
	{
		[HarmonyPatch("UpdateMoveControlStandAlone")]
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> row, ILGenerator generator)
		{
			var codes = new List<CodeInstruction>(row);

			var match = new CodeMatcher(codes).MatchForward(false,
				new CodeMatch(OpCodes.Ldstr, "Mouse Y"),
				new CodeMatch(OpCodes.Call, AccessTools.Method(AccessTools.TypeByName("UnityEngine.Input"), "GetAxis"))
				);

			if (!match.IsValid)
			{
				Plugin.log.LogError("Failed to find MouseY in UpdateMoveControlStandAlone. Below is code dump:");
				int i = 0;
				foreach (var code in codes)
				{
					Plugin.log.LogInfo(i.ToString("0000") + ": " + code.opcode + " " + code.operand != null ? code.operand : "");
					i++;
				}
				return codes.AsEnumerable();
			}

			Plugin.log.LogInfo("Successfully located code");
			match.Advance(2);
			match.InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_R4, -1),
				new CodeInstruction(OpCodes.Mul),
				new CodeInstruction(OpCodes.Ldloc_2));
			match.RemoveInstruction();
			codes = match.Instructions();

			return codes.AsEnumerable();
		}
	}
}
