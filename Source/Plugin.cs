using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using SuisHack.FPS_SettingsHack;

namespace SuisHack
{
	[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
	public class Plugin : BaseUnityPlugin
	{
		public static Harmony harmonyInstance { get; private set; }
		public static BepInEx.Logging.ManualLogSource log;

		private readonly ConfigEntry<bool> Config_Input_InvertYAxis;
		private readonly ConfigEntry<bool> Config_Cheat_GodMode;
		private readonly ConfigEntry<bool> Config_Cheat_DisableGuardSight;
		private readonly ConfigEntry<bool> Config_Cheat_DisableStamina;

		public Plugin()
		{
			Config_Input_InvertYAxis = Config.Bind("Input", "Invert mouse Y Axis", false);
			Config_Cheat_GodMode = Config.Bind("Cheats", "GodMode", false);
			Config_Cheat_DisableGuardSight = Config.Bind("Cheats", "DisableGuardsSight", false);
			Config_Cheat_DisableStamina = Config.Bind("Cheats", "DisableStamina", false);
		}

		private void Awake()
		{
			log = Logger;


			// Plugin startup logic
			Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

			harmonyInstance = new Harmony("local.suicidemachine.suihack.harmonyhook");
#if DEBUG
#pragma warning disable CS0618 // Type or member is obsolete
			Harmony.DEBUG = true;
#pragma warning restore CS0618 // Type or member is obsolete
#endif
			FPS_Settings.InjectEarly(harmonyInstance);

			if(Config_Input_InvertYAxis.Value)
				PlayerBehaviourPatches.InjectEarly(harmonyInstance);
			if (Config_Cheat_GodMode.Value)
				Cheat.EnableCheats.InjectEarly(harmonyInstance);
			if (Config_Cheat_DisableGuardSight.Value)
				Cheat.SecurityGuardCheat.InjectEarly(harmonyInstance);
			if (Config_Cheat_DisableStamina.Value)
				Cheat.StaminaCheat.InjectEarly(harmonyInstance);
		}

		private void Start()
		{
			Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
		}
	}
}
