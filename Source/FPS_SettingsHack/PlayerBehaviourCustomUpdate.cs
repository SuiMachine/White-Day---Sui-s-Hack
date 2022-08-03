using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace SuisHack.FPS_SettingsHack
{
	public class PlayerBehaviourCustomUpdate : MonoBehaviour
	{
		PlayerBehaviour playerBehaviourRef;
		FieldInfo forcedMoving;
		FieldInfo layerMask;
		MethodInfo maxAngularVelocity;
		MethodInfo isMoving;
		MethodInfo isRun;
		MethodInfo UpdateControlStandAloneMethodRef;
		MethodInfo UpdateControlQuickTurnRef;
		MethodInfo standingStateGetter;
		MethodInfo standingStateSetter;



		public void Awake()
		{
			playerBehaviourRef = GetComponent<PlayerBehaviour>();
			forcedMoving = typeof(PlayerBehaviour).GetField("forcedMoving", BindingFlags.Instance | BindingFlags.NonPublic);
			layerMask = typeof(PlayerBehaviour).GetField("layerMask", BindingFlags.Instance | BindingFlags.NonPublic);
			UpdateControlStandAloneMethodRef = typeof(PlayerBehaviour).GetMethod("UpdateControlStandAlone", BindingFlags.NonPublic | BindingFlags.Instance);
			UpdateControlQuickTurnRef = typeof(PlayerBehaviour).GetMethod("UpdateControlQuickTurn", BindingFlags.NonPublic | BindingFlags.Instance);

			standingStateGetter = AccessTools.PropertyGetter(typeof(PlayerBehaviour), "standingState");
			standingStateSetter = AccessTools.PropertySetter(typeof(PlayerBehaviour), "standingState");
			maxAngularVelocity = AccessTools.PropertyGetter(typeof(PlayerBehaviour), "maxAngularVelocity");
			isMoving = AccessTools.PropertySetter(typeof(CharacterBehaviour), "isMoving");
			isRun = AccessTools.PropertySetter(typeof(CharacterBehaviour), "isRun");
		}

		void Update()
		{
			if (!GameManager.isLoadingComplete || playerBehaviourRef == null)
			{
				return;
			}

			if (!playerBehaviourRef.ignoreInput)
			{
				var forcedMoving = (bool)this.forcedMoving.GetValue(playerBehaviourRef);

				if (!playerBehaviourRef.isDead && !forcedMoving && !Cursor.visible && !GameManager.instance.UI.IsActivePanel && GameManager.isLoadingComplete && !GameManager.instance.isCinemaState && GameManager.instance.mainCamera.camera.enabled && GameManager.instance.UI.virtualPad.gameObject.activeInHierarchy && !GameManager.instance.player.isDead)
				{
					UpdateControlStandAloneMethodRef.Invoke(playerBehaviourRef, null);
					UpdateControlQuickTurnRef.Invoke(playerBehaviourRef, null);

					Vector2 rotation = Vector2.zero;
					float axis = Input.GetAxis(Global.config.JoystickAxisTrigger);
					float rotationGamepadY;
					float rotationGamepadX;
					if (axis < -0.5f)
					{
						rotationGamepadY = ((!Global.config.JoyPadVertivalReversal) ? (-Input.GetAxis("JoystickAxis5") / 2f) : (Input.GetAxis("JoystickAxis5") / 2f));
						rotationGamepadX = ((!Global.config.JoyPadHorizontalReversal) ? (Input.GetAxis("JoystickAxis4") / 2f) : (-Input.GetAxis("JoystickAxis4") / 2f));
					}
					else
					{
						rotationGamepadY = ((!Global.config.JoyPadVertivalReversal) ? (-Input.GetAxis("JoystickAxis5") * Global.config.JoypadVertivalSpeed / 2f) : (Input.GetAxis("JoystickAxis5") * Global.config.JoypadVertivalSpeed / 2f));
						rotationGamepadX = ((!Global.config.JoyPadHorizontalReversal) ? (Input.GetAxis("JoystickAxis4") * Global.config.JoypadHorizontalSpeed / 2f) : (-Input.GetAxis("JoystickAxis4") * Global.config.JoypadHorizontalSpeed / 2f));
					}
					rotation.Set(Input.GetAxis("Mouse X") + rotationGamepadX, Input.GetAxis("Mouse Y") + rotationGamepadY);
					rotation *= Global.config.controlSensitivity * 2f;
					playerBehaviourRef.transform.rotation = Quaternion.Euler(0f, rotation.x, 0f) * playerBehaviourRef.transform.rotation;
					var fMaxAngularVelocity = (float)maxAngularVelocity.Invoke(playerBehaviourRef, new object[] { });
					float num3 = Mathf.Clamp(rotation.y, -fMaxAngularVelocity, fMaxAngularVelocity);

					var standingValue = (PlayerBehaviour.StandingState)standingStateGetter.Invoke(playerBehaviourRef, new object[] { });

					GameManager.instance.mainCamera.localRotation.x = ((standingValue != PlayerBehaviour.StandingState.Standing) ? Mathf.Clamp(GameManager.instance.mainCamera.localRotation.x - num3 - rotationGamepadY, -30f, 60f) : Mathf.Clamp(GameManager.instance.mainCamera.localRotation.x - num3 - rotationGamepadY, -40f, 80f));
					GameManager.instance.mainCamera.localRotation.y = 0f;
				}
			}
		}

		void FixedUpdate()
		{
			if (!GameManager.isLoadingComplete || playerBehaviourRef == null)
			{
				return;
			}

			bool isMoving = false;
			bool isRun = false;

			var forcedMoving = (bool)this.forcedMoving.GetValue(playerBehaviourRef);

			Vector3 movement = (forcedMoving ? Vector3.zero : Physics.gravity);
			if (!playerBehaviourRef.ignoreInput)
			{
				if (!playerBehaviourRef.isDead && !forcedMoving && !Cursor.visible && !GameManager.instance.UI.IsActivePanel && GameManager.isLoadingComplete && !GameManager.instance.isCinemaState && GameManager.instance.mainCamera.camera.enabled && GameManager.instance.UI.virtualPad.gameObject.activeInHierarchy && !GameManager.instance.player.isDead)
				{
					movement += UpdateMoveControlStandAlone(ref isMoving, ref isRun);
				}
				if (playerBehaviourRef.enableMoving)
				{
					playerBehaviourRef.charController.Move(movement * Time.fixedDeltaTime);
				}
				if (playerBehaviourRef.bindedObject != null && playerBehaviourRef.bindingCollision)
				{
					float num = 0.2f;
					float num2 = 0.6f;
					if (Physics.SphereCast(base.transform.position + Vector3.up * 0.5f, num, base.transform.forward, out RaycastHit raycastHit, num2, (LayerMask)layerMask.GetValue(playerBehaviourRef)))
					{
						Vector3 vector = raycastHit.point + raycastHit.normal * num - (base.transform.position + base.transform.forward * num2);
						vector.y = 0f;
						float magnitude = vector.magnitude;
						playerBehaviourRef.charController.Move(raycastHit.normal * magnitude * Vector3.Dot(base.transform.forward, -raycastHit.normal));
					}
				}
			}

			this.isMoving.Invoke(playerBehaviourRef, new object[] { isMoving });
			this.isRun.Invoke(playerBehaviourRef, new object[] { isRun });
		}

		private Vector3 UpdateMoveControlStandAlone(ref bool moving, ref bool running)
		{
			Vector3 fullMovement = Vector3.zero;

			Vector2 rawInput = Vector2.zero;
			if (GameHelper.GetKey(Global.config.keyLeft))
				rawInput.x -= 1f;
			if (GameHelper.GetKey(Global.config.keyRight))
				rawInput.x += 1f;
			if (GameHelper.GetKey(Global.config.keyForward))
				rawInput.y += 1f;
			if (GameHelper.GetKey(Global.config.keyBackward))
				rawInput.y -= 1f;
			rawInput.x += Input.GetAxis(Global.config.joypadHorizontal);
			rawInput.y += -Input.GetAxis(Global.config.joypadVertical);
			rawInput.Normalize();
			moving = rawInput.sqrMagnitude > 0.25f;
			if (moving)
			{
				running = moving && (GameHelper.GetKey(Global.config.keyRun) || GameHelper.GetJoypad(Global.config.joypadRun));
				if (playerBehaviourRef.bindedObject != null || Global.playerInfo.breathless || rawInput.y < -0.7071f)
				{
					running = false;
				}
				if (playerBehaviourRef.standingState == PlayerBehaviour.StandingState.Hiding && running)
				{
					this.standingStateGetter.Invoke(playerBehaviourRef, new object[] { PlayerBehaviour.StandingState.Standing });
				}
				if (playerBehaviourRef.standingState != PlayerBehaviour.StandingState.Standing)
				{
					running = false;
				}
				if (playerBehaviourRef.standingState == PlayerBehaviour.StandingState.Standing)
				{
					fullMovement += (base.transform.right * rawInput.x + base.transform.forward * rawInput.y) * ((!running) ? playerBehaviourRef.walkSpeed : ((rawInput.y <= 0.7071f) ? (playerBehaviourRef.walkSpeed + (playerBehaviourRef.runSpeed - playerBehaviourRef.walkSpeed) * 0.5f) : playerBehaviourRef.runSpeed));
				}
				else
				{
					fullMovement += (base.transform.right * rawInput.x + base.transform.forward * rawInput.y) * (playerBehaviourRef.walkSpeed * 0.5f);
				}
			}

			return fullMovement;
		}
	}
}
