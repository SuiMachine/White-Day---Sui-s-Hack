using HarmonyLib;
using UnityEngine;
using static PlayerBehaviour;

namespace SuisHack.Input
{
	[HarmonyPatch]
	class MouseHack
	{
		[HarmonyPostfix]
		[HarmonyPatch(typeof(PlayerBehaviour), "UpdateMoveControlStandAlone")]
		static bool UpdateMoveControlStandAloneDetoured(ref Vector3 __result, ref bool moving, ref bool running)
		{
			__result = Vector3.zero;
			return true;
		}

		/*		[HarmonyPrefix]
				[HarmonyPatch(typeof(PlayerBehaviour), "UpdateMoveControlStandAlone")]
				static bool UpdateMoveControlStandAloneDetoured(ref Vector3 __result, ref bool moving, ref bool running,
					ref Transform ___transform, ref float ___maxAngularVelocity, ref StandingState ___standingState, ref GameObject ___bindedObject, ref float ___walkSpeed, ref float ___runSpeed)
				{
					if (UnityReflections.InputGetAxis == null)
					{
						Plugin.log.LogError("Failed!");
						return true;
					}

					float axis = UnityReflections.InputGetAxis(Global.config.JoystickAxisTrigger);
					float num;
					float num2;
					if (axis < -0.5f)
					{
						num = ((!Global.config.JoyPadVertivalReversal) ? ((0f - UnityReflections.InputGetAxis("JoystickAxis5")) / 2f) : (UnityReflections.InputGetAxis("JoystickAxis5") / 2f));
						num2 = ((!Global.config.JoyPadHorizontalReversal) ? (UnityReflections.InputGetAxis("JoystickAxis4") / 2f) : ((0f - UnityReflections.InputGetAxis("JoystickAxis4")) / 2f));
					}
					else
					{
						num = ((!Global.config.JoyPadVertivalReversal) ? ((0f - UnityReflections.InputGetAxis("JoystickAxis5")) * Global.config.JoypadVertivalSpeed / 2f) : (UnityReflections.InputGetAxis("JoystickAxis5") * Global.config.JoypadVertivalSpeed / 2f));
						num2 = ((!Global.config.JoyPadHorizontalReversal) ? (UnityReflections.InputGetAxis("JoystickAxis4") * Global.config.JoypadHorizontalSpeed / 2f) : ((0f - UnityReflections.InputGetAxis("JoystickAxis4")) * Global.config.JoypadHorizontalSpeed / 2f));
					}
					Vector2 cameraInput = new(UnityReflections.InputGetAxis("Mouse X") + num2, UnityReflections.InputGetAxis("Mouse Y") + num);
					cameraInput *= Global.config.controlSensitivity * 2f * -1f;
					___transform.rotation = Quaternion.Euler(0f, cameraInput.x, 0f) * ___transform.rotation;
					float num3 = Mathf.Clamp(cameraInput.y, 0f - ___maxAngularVelocity, ___maxAngularVelocity);
					GameManager.instance.mainCamera.localRotation.x = ((___standingState != 0) ? Mathf.Clamp(GameManager.instance.mainCamera.localRotation.x - num3 - num, -30f, 60f) : Mathf.Clamp(GameManager.instance.mainCamera.localRotation.x - num3 - num, -40f, 80f));
					GameManager.instance.mainCamera.localRotation.y = 0f;
					Vector2 zero3 = Vector2.zero;
					if (GameHelper.GetKey(Global.config.keyLeft))
					{
						zero3.x -= 1f;
					}
					if (GameHelper.GetKey(Global.config.keyRight))
					{
						zero3.x += 1f;
					}
					if (GameHelper.GetKey(Global.config.keyForward))
					{
						zero3.y += 1f;
					}
					if (GameHelper.GetKey(Global.config.keyBackward))
					{
						zero3.y -= 1f;
					}
					zero3.x += UnityReflections.InputGetAxis(Global.config.joypadHorizontal);
					zero3.y += 0f - UnityReflections.InputGetAxis(Global.config.joypadVertical);
					zero3.Normalize();
					moving = zero3.sqrMagnitude > 0.25f;

					Vector3 movementInput = Vector3.zero;
					if (moving)
					{
						running = moving && (GameHelper.GetKey(Global.config.keyRun) || GameHelper.GetJoypad(Global.config.joypadRun));
						if (___bindedObject != null || Global.playerInfo.breathless || zero3.y < -0.7071f)
						{
							running = false;
						}
						if (___standingState == StandingState.Hiding && running)
						{
							___standingState = StandingState.Standing;
						}
						if (___standingState != 0)
						{
							running = false;
						}
						if (___standingState == StandingState.Standing)
						{
							movementInput += (___transform.right * zero3.x + ___transform.forward * zero3.y) * ((!running) ? ___walkSpeed : ((!(zero3.y > 0.7071f)) ? (___walkSpeed + (___runSpeed - ___walkSpeed) * 0.5f) : ___runSpeed));
						}
						else
						{
							movementInput += (___transform.right * zero3.x + ___transform.forward * zero3.y) * (___walkSpeed * 0.5f);
						}
					}
					__result = movementInput;
					return false;
				}*/
	}

	/* Just gonna leave it in case I want to play with transpliers ever again... but fuck IL
	 * [HarmonyPatch(typeof(PlayerBehaviour))]
		[HarmonyPatch("UpdateMoveControlStandAlone")]
		public static class MouseHack
		{
			static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
			{
				var codes = new List<CodeInstruction>(instructions);

				var match = new CodeMatcher(instructions).MatchForward(false,
					new CodeMatch(OpCodes.Ldstr, "Mouse Y"),
					new CodeMatch(OpCodes.Call, AccessTools.Method(AccessTools.TypeByName("UnityEngine.Input"), "GetAxis"))
					);

				if(!match.IsValid)
				{
					Plugin.log.LogError("Failed to find MouseY in UpdateMoveControlStandAlone. Below is code dump:");
					int i = 0;
					foreach(var code in codes)
					{
						Plugin.log.LogInfo(i.ToString("0000") + ": " + code.opcode + " " + code.operand != null ? code.operand : "");
						i++;
					}
					return codes.AsEnumerable();
				}
				else
					Plugin.log.LogInfo("Successfully located code");

				var result = match.Pos;

				return codes.AsEnumerable();
			}
		}*/
}
