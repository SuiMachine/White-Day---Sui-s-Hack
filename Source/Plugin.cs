using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace SuisHack
{
	[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
	public class Plugin : BaseUnityPlugin
	{
		public static Harmony HarmonyInstance { get; private set; }
		public static BepInEx.Logging.ManualLogSource log;

		public static ConfigEntry<bool> Config_Increase_RefreshRateByOne;

		public Plugin()
		{
			Config_Increase_RefreshRateByOne = Config.Bind("General", "Increase refresh rate by one", false, new ConfigDescription("This is a workaround for an issue related to refresh rate, where some monitors instead of using - let's say - 240Hz refresh as advertised, report something dumb like 239.958 Hz (I am looking at you Samsung!). As Unity expects integers are refresh rate, this then ends up being 239 Hz, so this option increases it by 1 as a workaround."));

			Cheat.InventoryCheat.UseUnlimitedSavePens = Config.Bind("Cheats", "SavePens", false).Value;
			Cheat.VeryHardDifficultySave.Use = Config.Bind("Cheats", "AllowSavingOnVeryHardDifficulty", false).Value;
			Cheat.GodModeCheat.Use = Config.Bind("Cheats", "GodMode", false).Value;
			Cheat.SecurityGuardCheat.Use = Config.Bind("Cheats", "DisableGuardsSight", false).Value;
			Cheat.StaminaCheat.Use = Config.Bind("Cheats", "DisableStamina", false).Value;

			Components.SettingsOverride.SetupConfig(Config);
		}

		private void Awake()
		{
			log = Logger;

			// Plugin startup logic
			Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

			HarmonyInstance = new Harmony("local.suicidemachine.suihack.harmonyhook");
#if DEBUG
#pragma warning disable CS0618 // Type or member is obsolete
			Harmony.DEBUG = true;
#pragma warning restore CS0618 // Type or member is obsolete
#endif
			HarmonyInstance.PatchAll();

			Components.SettingsOverride.Initialize();
			Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} finished loading loaded!");
		}
	}
}
