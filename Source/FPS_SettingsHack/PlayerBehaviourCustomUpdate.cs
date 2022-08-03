using System.Reflection;
using UnityEngine;

namespace SuisHack.FPS_SettingsHack
{
	public class PlayerBehaviourCustomUpdate : MonoBehaviour
	{
		PlayerBehaviour playerBehaviourRef;
		FieldInfo forcedMoving;
		FieldInfo layerMask;
		PropertyInfo standingState;
		PropertyInfo maxAngularVelocity;


		private static MethodInfo UpdateControlStandAloneMethodRef;
		private static MethodInfo UpdateControlQuickTurnRef;

		public void Awake()
		{
			playerBehaviourRef = GetComponent<PlayerBehaviour>();
			forcedMoving = typeof(PlayerBehaviour).GetField("forcedMoving", BindingFlags.Instance | BindingFlags.NonPublic);
			layerMask = typeof(PlayerBehaviour).GetField("layerMask", BindingFlags.Instance | BindingFlags.NonPublic);
			standingState = typeof(PlayerBehaviour).GetProperty("standingState", BindingFlags.Instance | BindingFlags.NonPublic);
			maxAngularVelocity = typeof(PlayerBehaviour).GetProperty("maxAngularVelocity", BindingFlags.Instance | BindingFlags.NonPublic);
			UpdateControlStandAloneMethodRef = typeof(PlayerBehaviour).GetMethod("UpdateControlStandAlone", BindingFlags.NonPublic | BindingFlags.Instance);
			UpdateControlQuickTurnRef = typeof(PlayerBehaviour).GetMethod("UpdateControlQuickTurn", BindingFlags.NonPublic | BindingFlags.Instance);
		}

		void Update()
		{
			if (!GameManager.isLoadingComplete || playerBehaviourRef == null)
			{
				return;
			}

			var forcedMoving = (bool)this.forcedMoving.GetValue(playerBehaviourRef);
			if (!playerBehaviourRef.ignoreInput)
			{
				if (!playerBehaviourRef.isDead && !forcedMoving && !Cursor.visible && !GameManager.instance.UI.IsActivePanel && GameManager.isLoadingComplete && !GameManager.instance.isCinemaState && GameManager.instance.mainCamera.camera.enabled && GameManager.instance.UI.virtualPad.gameObject.activeInHierarchy && !GameManager.instance.player.isDead)
				{
					UpdateControlStandAloneMethodRef.Invoke(playerBehaviourRef, null);
					UpdateControlQuickTurnRef.Invoke(playerBehaviourRef, null);

					Vector2 rotationMouse = Vector2.zero;
					float axis = Input.GetAxis(Global.config.JoystickAxisTrigger);
					float num;
					float num2;
					if (axis < -0.5f)
					{
						num = ((!Global.config.JoyPadVertivalReversal) ? (-Input.GetAxis("JoystickAxis5") / 2f) : (Input.GetAxis("JoystickAxis5") / 2f));
						num2 = ((!Global.config.JoyPadHorizontalReversal) ? (Input.GetAxis("JoystickAxis4") / 2f) : (-Input.GetAxis("JoystickAxis4") / 2f));
					}
					else
					{
						num = ((!Global.config.JoyPadVertivalReversal) ? (-Input.GetAxis("JoystickAxis5") * Global.config.JoypadVertivalSpeed / 2f) : (Input.GetAxis("JoystickAxis5") * Global.config.JoypadVertivalSpeed / 2f));
						num2 = ((!Global.config.JoyPadHorizontalReversal) ? (Input.GetAxis("JoystickAxis4") * Global.config.JoypadHorizontalSpeed / 2f) : (-Input.GetAxis("JoystickAxis4") * Global.config.JoypadHorizontalSpeed / 2f));
					}
					rotationMouse.Set(Input.GetAxis("Mouse X") + num2, Input.GetAxis("Mouse Y") + num);
					rotationMouse *= Global.config.controlSensitivity * 2f;
					playerBehaviourRef.transform.rotation = Quaternion.Euler(0f, rotationMouse.x, 0f) * playerBehaviourRef.transform.rotation;
					float fmaxAngularVelocity = (float)maxAngularVelocity.GetValue(playerBehaviourRef, null);
					float horizontalRotation = Mathf.Clamp(rotationMouse.y, -fmaxAngularVelocity, fmaxAngularVelocity);
					GameManager.instance.mainCamera.localRotation.x = (((PlayerBehaviour.StandingState)standingState.GetValue(playerBehaviourRef, null) != PlayerBehaviour.StandingState.Standing) ? Mathf.Clamp(GameManager.instance.mainCamera.localRotation.x - horizontalRotation - num, -30f, 60f) : Mathf.Clamp(GameManager.instance.mainCamera.localRotation.x - horizontalRotation - num, -40f, 80f));
					GameManager.instance.mainCamera.localRotation.y = 0f;
				}
			}
		}

		void FixedUpdate()
		{
			if(!GameManager.isLoadingComplete || playerBehaviourRef == null)
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
				if(playerBehaviourRef.enableMoving)
				{
					playerBehaviourRef.charController.Move(movement * Time.fixedDeltaTime);
				}
				if(playerBehaviourRef.bindedObject != null && playerBehaviourRef.bindingCollision)
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

/*			playerBehaviourRef.isMoving = isMoving;
			playerBehaviourRef.isRun = isRun;*/
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
					this.standingState.SetValue(playerBehaviourRef, PlayerBehaviour.StandingState.Standing, null);
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
