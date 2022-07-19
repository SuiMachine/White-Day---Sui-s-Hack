using HarmonyLib;
using System;
using UnityEngine;

namespace SuisHack.FPS_SettingsHack
{
	//[HarmonyPatch]
	public static class PlayerBehaviourPatches
	{
		private static System.Reflection.PropertyInfo info;

		public static void Initialize()
		{
			info = typeof(PlayerBehaviour).GetProperty("standingState", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetProperty);
			Plugin.log.LogMessage($"{info}");
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(PlayerBehaviour), "UpdateMoveControlStandAlone")]
		public static bool UpdateMoveControlStandAlone(ref Vector3 __result, ref PlayerBehaviour __instance, ref bool moving, ref bool running)
		{

			float axis = Input.GetAxis(Global.config.JoystickAxisTrigger);

			float movementX;
			float movementY;
			if (axis < 0.5f)
			{
				movementX = (!Global.config.JoyPadVertivalReversal) ? (-Input.GetAxis("JoystickAxis5") / 2f) : (Input.GetAxis("JoystickAxis5") / 2f);
				movementY = (!Global.config.JoyPadHorizontalReversal) ? (Input.GetAxis("JoystickAxis4") / 2f) : (-Input.GetAxis("JoystickAxis4") / 2f);
			}
			else
			{
				movementX = (!Global.config.JoyPadVertivalReversal) ? (-Input.GetAxis("JoystickAxis5") * Global.config.JoypadVertivalSpeed / 2f) : (Input.GetAxis("JoystickAxis5") * Global.config.JoypadVertivalSpeed / 2f);
				movementY = (!Global.config.JoyPadHorizontalReversal) ? (Input.GetAxis("JoystickAxis4") * Global.config.JoypadHorizontalSpeed / 2f) : (-Input.GetAxis("JoystickAxis4") * Global.config.JoypadHorizontalSpeed / 2f);
			}

			var mouseX = Global.config.JoyPadVertivalReversal ? -Input.GetAxis("Mouse X") : Input.GetAxis("Mouse X");
			var mouseY = Global.config.JoyPadHorizontalReversal ? Input.GetAxis("Mouse Y") : -Input.GetAxis("Mouse Y");
			Vector2 combinedRotation = new Vector3(mouseX + movementX, mouseY + movementY);
			combinedRotation *= Global.config.controlSensitivity * 2f;
			__instance.transform.rotation = Quaternion.Euler(0f, combinedRotation.x, 0f) * __instance.transform.rotation;

			float clampedRotation = Mathf.Clamp(combinedRotation.y, -__instance.maxAngularVelocity, __instance.maxAngularVelocity);

			GameManager.instance.mainCamera.localRotation.x = (__instance.standingState != PlayerBehaviour.StandingState.Standing) ? Mathf.Clamp(GameManager.instance.mainCamera.localRotation.x - clampedRotation - movementX, -30f, 60f) : Mathf.Clamp(GameManager.instance.mainCamera.localRotation.x - clampedRotation - movementX, -40f, 80f);
			GameManager.instance.mainCamera.localRotation.y = 0f;
			Vector2 desiredMovement = Vector2.zero;
			if (GameHelper.GetKey(Global.config.keyLeft))
				desiredMovement.x -= 1f;
			if (GameHelper.GetKey(Global.config.keyRight))
				desiredMovement.x += 1f;
			if (GameHelper.GetKey(Global.config.keyForward))
				desiredMovement.y += 1f;
			if (GameHelper.GetKey(Global.config.keyBackward))
				desiredMovement.y -= 1f;

			desiredMovement.x += Input.GetAxis(Global.config.joypadHorizontal);
			desiredMovement.y += -Input.GetAxis(Global.config.joypadVertical);
			desiredMovement.Normalize();

			Vector3 returnMovement = Vector3.zero;
			moving = desiredMovement.sqrMagnitude > 0.25f;
			if (moving)
			{
				running = moving && (GameHelper.GetKey(Global.config.keyRun) || GameHelper.GetJoypad(Global.config.joypadRun));

				if (__instance.bindedObject != null || Global.playerInfo.breathless || desiredMovement.y < -0.7071f)
					running = false;
				if (__instance.standingState == PlayerBehaviour.StandingState.Hiding && running)
				{
					info.SetValue(__instance, PlayerBehaviour.StandingState.Standing, null);
				}
				if (__instance.standingState != PlayerBehaviour.StandingState.Standing)
					running = false;
				if (__instance.standingState == PlayerBehaviour.StandingState.Standing)
				{
					//This was really ugly in dnSpy so I reworked it to more readable form
					float movementMultiplier;

					//Player walking
					if (!running)
					{
						movementMultiplier = __instance.walkSpeed;
					}
					else
					{
						//Player running, but at low speed, so still walking... ehh?
						if (desiredMovement.y <= 0.7071f)
						{
							movementMultiplier = __instance.walkSpeed + (__instance.runSpeed - __instance.walkSpeed) * 0.5f;
						}
						else
						{
							movementMultiplier = __instance.runSpeed;
						}
					}

					returnMovement += (__instance.transform.right * desiredMovement.x + __instance.transform.forward * desiredMovement.y) * movementMultiplier;
				}
				else
				{
					returnMovement += (__instance.transform.right * desiredMovement.x + __instance.transform.forward * desiredMovement.y) * (__instance.walkSpeed * 0.5f);
				}
			}
			__result = returnMovement;

			return false;
		}
	}
}
