using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

namespace SuisHack.Input
{
	[HarmonyPatch(typeof(PlayerBehaviour))]
	[HarmonyPatch("UpdateMoveControlStandAlone")]
	public static class MouseHack
	{
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> row, ILGenerator generator)
		{
			var codes = new List<CodeInstruction>(row);

			var match = new CodeMatcher(row).MatchForward(false,
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

			Label l1 = generator.DefineLabel();
			Label l2 = generator.DefineLabel();

			/*			List<CodeInstruction> patch = new List<CodeInstruction>()
						{
							new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Global), "get_config")),
							new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Config), "JoyPadVertivalReversal")),
							new CodeInstruction(OpCodes.Brtrue_S, l1),
							new CodeInstruction(OpCodes.Ldc_R4, 1)
							new CodeInstruction(OpCodes.Brtrue_S, l2),
							new CodeInstruction(OpCodes.Ldc_R4, -1).WithLabels(l1),
							new CodeInstruction(OpCodes.Mul).WithLabels(l2),
							new CodeInstruction(OpCodes.Ldloc_2).WithLabels()
						};
						match.InsertAndAdvance(patch);
						match.RemoveInstruction();*/

			//match.CreateLabel(out l1);
			match.InsertAndAdvance(new CodeInstruction(OpCodes.Nop));
			//match.CreateLabel(out l2);


			return match.Instructions();
		}
	}
}
