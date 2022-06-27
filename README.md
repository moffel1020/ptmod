# ptmod
Mod loader for Parkour Tag, including console and map editor
## Installing
1. Download the [latest release](https://github.com/moffel1020/ptmod/releases)
2. Extract the .zip
3. Put the files inside your Parkour Tag folder

Your parkour tag folder should look like this:  
![image](https://user-images.githubusercontent.com/100964263/170320121-b6c7500c-8402-41f3-a5af-8ac9ac3f2ced.png)

## Usage
To play the game with mods just launch the game like you normally would  
If you want to play without mods go into doorstop_config.ini in the game folder, and set enabled=false

### Map Editor
Editing and saving existing maps is not supported, so make a new map or load one from the top right menu. To save maps go to the upper right corner in edit mode, fill in the desired map name, and press the save map button. Alternatively you can save and load maps by typing 'loadmap \<name\>' and 'savemap \<name\>' in console.

#### controls
- Q: freecam (make sure to stand still before activating, otherwise you keep your momentum)
  - WASD: movement
  - Left shift: move down 
  - Space: move up
- E: enable/disable edit mode
  - Left click: place object
  - Right click: edit selected object
  - X and Z: cycle through building objects (bottom right of your screen)

#### console commands

- savemap \<name\>      save map by name
- loadmap \<name\>      load map by name
- camspeed \<speed\>    set speed of freecam
- highlight \<R\> \<G\> \<B\>     change the color of highlighted objects, rgb values are between 0 and 1
- reach \<value\>       set from how far you can edit objects, default is 35

#### multiplayer
When in a private match lobby, load a map from the upper right menu as you usually would. Both players must have the exact same map(with the same file name) installed for this to work. If everything goes right you should be able to play custom maps in multiplayer. You cannot (yet?) edit maps with eachother in multiplayer.

### Console
- use command "help" to show a list of available commands
- F1: show/hide
- left alt: unlock mouse cursor
- up/down arrow keys: cycle through input history


![image](https://user-images.githubusercontent.com/100964263/169693782-3e9a0fa3-0844-49b0-a29f-7f862a23e8bf.png)
