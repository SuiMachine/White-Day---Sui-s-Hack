using BepInEx;
using HarmonyLib;
using SuisHack.FPS_SettingsHack;

namespace SuisHack
{
	[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
	public class Plugin : BaseUnityPlugin
	{
		public static Harmony harmonyInstance { get; private set; }
		public static BepInEx.Logging.ManualLogSource log;

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
			Cheat.EnableCheats.InjectEarly(harmonyInstance);
			Cheat.SecurityGuardCheat.InjectEarly(harmonyInstance);
			FPS_Settings.InjectEarly(harmonyInstance);
		}

		private void Start()
		{
			Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
		}
	}
}
