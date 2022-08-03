# White Day - Sui's Hack
 A hack for White Day: A Labyrinth Named School.
 
# Features
 * Adds 120, 144, 165 fps cap options added to game's settings together. If your monitor has a different refresh rate that option will also appear in the settings menu (read notes).
 * Movement interpolation to prevent player from being unable to move when using high FPS.
 * Enabling Vertical axis inversion now also affects the mouse input.
 
# Cheats (disabled by default):
 * Option to enable god mode that is hidden in game's libraries.
 * Option to hook code for security guard (janitor) view sight, so he ignores you. 
 * Option to hook code responsible for stemina (breath), so you don't get tired from running.
 * Option to hook code responsible for inventory (save file pens).

# Requirements
* The official copy of the game
* [BepInEx UnityMono x64 5.x](https://github.com/BepInEx/BepInEx/releases/) and HarmonyX (bundled with the release)

# Installation
* Download [Sui's Hack](https://github.com/SuiMachine/White-Day---Sui-s-Hack/releases).
* Extract the zip archive and move it to the game's directory.
* If you want to configure the plugin, after starting the game once with it, go to ``<game folder>/BepInEx/config`` and edit ``SuisHack.cfg``.

# Experimental warning
* Movement interpolation may break things.

# Notes
* Some monitors report refresh rates as float values. For example Samsung's Odyssey G7 instead of reporting 240Hz to the system, reports 239.958 Hz. Unity expects integer values, so this will become 239 fps. If this happens, you can use ``Increase refresh rate by one`` setting in ``BepInEx/config/SuisHack.cfg``.