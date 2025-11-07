Three Space Engineers targets:
- Client: The game client itself. The code is built by the Pulsar plugin loader on the player's machine into a DLL. Patches are executed on loading the game.
- Dedicated Server: The original server to run multiplayer games on a server, which itself does not have graphics. Players can only connect to the server.
- Torch: Improved version of the Dedicated Server, which has better plugin support, configurability and logging. It has an outdated custom patch mechanism, which this plugin does not use.

Harmony, also called HarmonyLib: Patching library, which allows to change the IL code of the game at runtime (after the DLLs are loaded). Documentation: https://harmony.pardeike.net/api/index.html

Folder structure:
- Shared: Shared project with the patches relevant on all three targets
- ClientPlugin: Game client specific initialization and logging
- DedicatedPlugin: Dedicated server specific initialization and logging
- TorchPlugin: Torch server specific initialization and logging

General instructions:
- Do not touch the configuration mechanism and the configuration dialog.
- Do not introduce whitespace only changes on code lines which are not changed.
- Follow the variable and class naming convention used in the code.
