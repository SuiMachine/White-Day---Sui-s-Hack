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
			var foundMassUsageMethod = false;
			var startIndex = -1;
			var endIndex = -1;

			var codes = new List<CodeInstruction>(instructions);
			var inputType = AccessTools.TypeByName("UnityEngine.Input");

			var codeMatcher = new CodeMatcher(codes).MatchForward(true,
				new CodeMatch(OpCodes.Ldstr, "Mouse X"),
				new CodeMatch(OpCodes.Call, AccessTools.Method(inputType, "GetAxis", new Type[] { typeof(string) })),
				new CodeMatch(OpCodes.Ldloca_S, 4),
				new CodeMatch(OpCodes.Add),
				new CodeMatch(OpCodes.Ldstr, "Mouse Y"),
				new CodeMatch(OpCodes.Call, AccessTools.Method(inputType, "GetAxis", new Type[] { typeof(string) }))
				);

			if(codeMatcher.Pos == 0 || codeMatcher.Pos == instructions.Count())
			{
				Plugin.log.LogError("Failed to find proper mouse input signature");
				return codes.AsEnumerable();
			}

			codeMatcher.InsertAndAdvance(new CodeInstruction[]
			{
				new CodeInstruction(OpCodes.Nop)
			});
			/*
			 	IL_012b: ldc.r4 -1
	IL_0130: mul
	IL_0131: ldloc.2
			 */


			var pos = codeMatcher.Pos;
			Plugin.log.LogWarning(pos);




/*			for (var i = 0; i < codes.Count; i++)
			{
				if (codes[i].opcode == OpCodes.Ret)
				{
					if (foundMassUsageMethod)
					{
						Plugin.log.LogError("END " + i);

						endIndex = i; // include current 'ret'
						break;
					}
					else
					{
						Plugin.log.LogError("START " + (i + 1));

						startIndex = i + 1; // exclude current 'ret'

						for (var j = startIndex; j < codes.Count; j++)
						{
							if (codes[j].opcode == OpCodes.Ret)
								break;
							var strOperand = codes[j].operand as string;
							if (strOperand == "TooBigCaravanMassUsage")
							{
								foundMassUsageMethod = true;
								break;
							}
						}
					}
				}
			}

			if (startIndex > -1 && endIndex > -1)
			{
				// we cannot remove the first code of our range since some jump actually jumps to
				// it, so we replace it with a no-op instead of fixing that jump (easier).
				codes[startIndex].opcode = OpCodes.Nop;
				codes.RemoveRange(startIndex + 1, endIndex - startIndex - 1);
			}*/

			return codes.AsEnumerable();
		}
	}
}
