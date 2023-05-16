using BepInEx.Configuration;
using System;
using UnityEngine;

namespace SuisHack.Components
{
	public class SettingsOverride : MonoBehaviour
	{
		private static SettingsOverride instance;

		private static ConfigEntry<AnisotropicFiltering> graphics_Aniso;
		private static ConfigEntry<int> graphics_AnisoValue;
		private static ConfigEntry<AudioSpeakerMode> sound_SpeakerMode;

		private bool showGUI;

		public static void SetupConfig(ConfigFile config)
		{
			graphics_Aniso = config.Bind("Graphics", "Anisotropic filtering", AnisotropicFiltering.ForceEnable, "Changes anisotropic filtering option - options are: Disable / Enable / ForceEnable");
			graphics_Aniso.SettingChanged += (object sender, EventArgs e) => { QualitySettings.anisotropicFiltering = graphics_Aniso.Value; };

			graphics_AnisoValue = config.Bind("Graphics", "Anisotropic filtering samples", 16, "Changes anisotropic filtering samples - should be a value that is a power of 2 with maximum of 16.");
			graphics_AnisoValue.SettingChanged += (object sender, EventArgs e) => { Texture.SetGlobalAnisotropicFilteringLimits(graphics_AnisoValue.Value, graphics_AnisoValue.Value); };

			sound_SpeakerMode = config.Bind("Sounds", "Speaker Mode", AudioSpeakerMode.Stereo, "A speaker mode - by default the game runs in stereo, but 3D sounds can be played with other speaker modes. Available options are: Raw / Mono / Stereo / Quad / Surround / Mode5point1 / Mode7point1 / Prologic");
			sound_SpeakerMode.SettingChanged += (object sender, EventArgs e) =>
			{
				var audioConfig = AudioSettings.GetConfiguration();
				audioConfig.speakerMode = sound_SpeakerMode.Value;
				AudioSettings.Reset(audioConfig);
			};
		}

		public static void Initialize()
		{
			if (instance == null)
			{
				var settingsGO = new GameObject("Settings Override Game Object");
				instance = settingsGO.AddComponent<SettingsOverride>();
				DontDestroyOnLoad(instance.gameObject);
			}
		}

		void Start()
		{
			UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManager_sceneLoaded;
		}

		private void SceneManager_sceneLoaded(UnityEngine.SceneManagement.Scene sceneLoaded, UnityEngine.SceneManagement.LoadSceneMode loadedSceneMode)
		{
			ApplySettings();
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.F11))
			{
				showGUI = !showGUI;
			}
		}

		void OnGUI()
		{
			if (showGUI)
			{
				Cursor.lockState = CursorLockMode.Confined;
				Cursor.visible = true;

				GUILayout.BeginVertical();
				GUILayout.BeginHorizontal(GUI.skin.box);
				{
					GUILayout.BeginHorizontal();
					GUILayout.Label($"Anisotropic filtering ({QualitySettings.anisotropicFiltering}):");
					if (GUILayout.Button("Disable"))
						graphics_Aniso.Value = AnisotropicFiltering.Disable;
					if (GUILayout.Button("Enable"))
						graphics_Aniso.Value = AnisotropicFiltering.Enable;
					if (GUILayout.Button("Force disable"))
						graphics_Aniso.Value = AnisotropicFiltering.ForceEnable;
					GUILayout.EndHorizontal();
				}
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal(GUI.skin.box);
				{
					GUILayout.BeginHorizontal();
					GUILayout.Label($"Sound system({sound_SpeakerMode.Value}):");
					if (GUILayout.Button("Stereo"))
						sound_SpeakerMode.Value = AudioSpeakerMode.Stereo;
					if (GUILayout.Button("Quad"))
						sound_SpeakerMode.Value = AudioSpeakerMode.Quad;
					if (GUILayout.Button("Surround"))
						sound_SpeakerMode.Value = AudioSpeakerMode.Surround;
					if (GUILayout.Button("5.1"))
						sound_SpeakerMode.Value = AudioSpeakerMode.Mode5point1;
					if (GUILayout.Button("7.1"))
						sound_SpeakerMode.Value = AudioSpeakerMode.Mode7point1;
					if (GUILayout.Button("Prologic"))
						sound_SpeakerMode.Value = AudioSpeakerMode.Prologic;
					GUILayout.EndHorizontal();
				}
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal(GUI.skin.box);
				{
					GUILayout.Label("Cheats:");
					if (GameManager.instance != null && GameManager.instance.player)
					{
						var oldValue = Cheat.GodModeCheat.Use;
						GameManager.instance.player.godMode = GUILayout.Toggle(GameManager.instance.player.godMode, "God mode");
						if (GameManager.instance.player.godMode != oldValue)
							Cheat.GodModeCheat.Use = oldValue;
					}

					PlayerBehaviourPatches.UseInterpolation = GUILayout.Toggle(PlayerBehaviourPatches.UseInterpolation, "Character interpolation");
					Cheat.VeryHardDifficultySave.Use = GUILayout.Toggle(Cheat.VeryHardDifficultySave.Use, "Allow saves on very hard");
					Cheat.SecurityGuardCheat.Use = GUILayout.Toggle(Cheat.SecurityGuardCheat.Use, "Disable guard's viewsight");

				}
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			}
		}

		private void ApplySettings()
		{
			QualitySettings.anisotropicFiltering = graphics_Aniso.Value;
			Texture.SetGlobalAnisotropicFilteringLimits(graphics_AnisoValue.Value, graphics_AnisoValue.Value);

			var audioConfig = AudioSettings.GetConfiguration();
			if(audioConfig.speakerMode != sound_SpeakerMode.Value)
			{
				audioConfig.speakerMode = sound_SpeakerMode.Value;
				AudioSettings.Reset(audioConfig);
			}
		}
	}
}
