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

		private bool showGUI;

		public static void SetupConfig(ConfigFile config)
		{
			graphics_Aniso = config.Bind("Graphics", "Anisotropic filtering", AnisotropicFiltering.ForceEnable, "Changes anisotropic filtering option - options are: Disable / Enable / ForceEnable");
			graphics_Aniso.SettingChanged += (object sender, EventArgs e) => { QualitySettings.anisotropicFiltering = graphics_Aniso.Value; };

			graphics_AnisoValue = config.Bind("Graphics", "Anisotropic filtering samples", 16, "Changes anisotropic filtering samples - should be a value that is a power of 2 with maximum of 16.");
			graphics_AnisoValue.SettingChanged += (object sender, EventArgs e) => { Texture.SetGlobalAnisotropicFilteringLimits(graphics_AnisoValue.Value, graphics_AnisoValue.Value); };
		}

		public static void Initialize()
		{
			if(instance == null)
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
			if(Input.GetKeyDown(KeyCode.F11))
			{
				showGUI = !showGUI;
			}			
		}

		void OnGUI()
		{
			if(showGUI)
			{
				GUILayout.BeginHorizontal();
				GUILayout.BeginVertical(GUI.skin.box);

				{
					GUILayout.BeginHorizontal();
					GUILayout.Label($"Anisotropic filtering ({QualitySettings.anisotropicFiltering}):");
					if (GUILayout.Button("Disable"))
						graphics_Aniso.Value = AnisotropicFiltering.Disable;
					if(GUILayout.Button("Enable"))
						graphics_Aniso.Value = AnisotropicFiltering.Enable;
					if (GUILayout.Button("Force disable"))
						graphics_Aniso.Value = AnisotropicFiltering.ForceEnable;
					GUILayout.EndHorizontal();
				}
				GUILayout.EndVertical();
				GUILayout.EndHorizontal();
			}
		}

		private void ApplySettings()
		{
			QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
			Texture.SetGlobalAnisotropicFilteringLimits(graphics_AnisoValue.Value, graphics_AnisoValue.Value);
		}
	}
}
