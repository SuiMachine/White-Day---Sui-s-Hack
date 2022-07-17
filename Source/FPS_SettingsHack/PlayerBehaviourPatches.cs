using HarmonyLib;
using System;
using UnityEngine;

namespace SuisHack.FPS_SettingsHack
{
	[HarmonyPatch]
	public static class PlayerBehaviourPatches
	{
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

			var mouseX = Global.config.JoyPadVertivalReversal ? Input.GetAxis("Mouse X") : -Input.GetAxis("Mouse X");
			var mouseY = Global.config.JoyPadHorizontalReversal ? Input.GetAxis("Mouse Y") : -Input.GetAxis("Mouse Y");
			Vector2 combinedRotation = new Vector3(mouseX + movementX, mouseY + movementY);
			combinedRotation *= Global.config.controlSensitivity * 2f;
			__instance.transform.rotation = Quaternion.Euler(0f, combinedRotation.x, 0f) * __instance.transform.rotation;

			float clampedRotation = Mathf.Clamp(combinedRotation.y, -__instance.maxAngularVelocity, __instance.maxAngularVelocity);

			GameManager.instance.mainCamera.localRotation.x = (__instance.standingState != PlayerBehaviour.StandingState.Standing) ? Mathf.Clamp(GameManager.instance.mainCamera.localRotation.x - clampedRotation - movementX, -30f, 60f) : Mathf.Clamp(GameManager.instance.mainCamera.localRotation.x - clampedRotation - movementX, -40f, 80f);
			//258 here

			return false;
		}
	}
}
