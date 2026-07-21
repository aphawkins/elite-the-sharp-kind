# Elite - The Sharp Kind

![Elite - The Sharp Kind - Screenshot](images/elite-screenshot.png)

A C# port of the classic BBC home computer game 'Elite'.  It is meant to look, feel and play the same as the original 8bit and 16bit versions of the game.

Currently the objective of this port is authenticity, object oriented code and cross platform compatibility using dotnet.
Framerate is fixed at 13.5 fps, which using the current engine implementation, runs at approximately the same speed as the original.
Performance, or maximum FPS, are a secondary objective, which may come later.

Part of [The Sharp Kind](../README.md), alongside [Stunt Car Racer - The Sharp Kind](scr-readme.md).

## Getting Started

The program has been tested to run on the following platforms and architectures:
- Windows 10 (x64)
- Ubuntu 24.04 (x64)
- Raspberry Pi 4 (ARM64)

To build and run from source, install the .NET SDK and run:

``` bash
dotnet run --project src/elite/apps/EliteSharp
```

CI also publishes self-contained single-file builds (win-x64 and linux-x64) that do not require the .NET runtime to be installed.

## Controls

Press Y or N on the intro screen
Press Space on the ship parade screen
Use Left/Right cursor keys to scroll through ships on the ship parade screen

| Key | Function |
| --- | -------- |
| F1  | Front View (in flight) <br/> Launch (when docked) |
| F2  | Rear View |
| F3  | Left View |
| F4  | Right View (in flight) <br/> Equip ship (when docked) |
| F5 | Display Galactic Chart |
| F6 | Short Range Chart |
| F7 | Show information on selected planet |
| F8 | Stock market |
| F9 | Commander information |
| F10 | Inventory |
| F11 | Options |
| P | Pause game |
| R | Resume game |

### Flight Controls
| Key | Function |
| --- | -------- |
| A | Fire lasers |
| S or Up Arrow | Dive |
| X or Down Arrow | Climb |
| &lt; or Left Arrow | Roll Left |
| &gt; or Right Arrow | Roll Right |
| / | Slow Down |
| Space | Speed up |
| C | Activate docking computer, if fitted |
| D | De-activate docking computer if switched on |
| E | Active ECM, if fitted |
| H | Hyperspace |
| J | Warp Jump |
| M | Fire missile |
| T | Target a missile |
| U | Un-target missile |
| TAB | Detonate energy bomb, if fitted |
| CTRL+H | Galactic Hyperspace, if fitted |
| ESC | Launch escape capsule, if fitted |

### Chart Screens
| Key | Function |
| --- | -------- |
| D | Select a planet and show distance to it |
| F | Find planet by name |
| O | Return cursor to current planet |
| Cursor Keys | Move cross hairs around |

### Equipment Screen
| Key | Function |
| --- | -------- |
| Arrow keys | Navigate options |
| Enter | Buy item |

### Stock Market
| Key | Function |
| --- | -------- |
| S or Up Arrow | Select previous item |
| X or Down Arrow | Select next item |
| &lt; or Left Arrow | Sell item |
| &gt; or Right Arrow | Buy item |

### Options Screen
| Key | Function |
| --- | -------- |
| Arrow keys | Navigate options |
| Enter | Change option |

### Game Settings Screen
From the Options Screen (F11) you can enter the Game Settings Screen. From here you can change a number of settings that control how the game looks and plays.  Use the cursor keys to select an option and the Enter/Return key to change it. The options can be saved as default for future games by pressing Enter while on the Save Settings option (NB this is not necessary if you want to change the settings just for the current game).

## Configuration

Game settings are held in the `elitesharp.cfg` file, stored in JSON format, in the user's application data directory (`%AppData%\TheSharpKind` on Windows, `~/.config/TheSharpKind` on Linux/macOS) — shared with [Stunt Car Racer - The Sharp Kind](scr-readme.md). Commander saves (`.cmdr` files) and logs (`logs\elite-*.log`, daily rolling, 7 days retained) live in the same directory. If the config file is missing or invalid the game falls back to defaults. `elitesharp.cfg` can take the following values:

``` json
{
    "Fps": 60,                             // Maximum render frame rate. The game speed is independent, fixed at 13.5 updates per second.
    "MusicOn": true,                       // Play music.
    "EffectsOn": true,                     // Play sound effects.
    "ShipWireframe": false,                // Use wireframe ship graphics.  false (Solid) or true (Wireframe)
    "ShipRenderMode": "ZBuffer",           // Depth-sort strategy for filled ship rendering (ignored when ShipWireframe is true).  Painter or ZBuffer
    "PlanetStyle": "Fractal",              // The render style of the planets.  Wireframe or Solid or Striped or Fractal
    "SunStyle": "Gradient",                // The render style of the sun.  Solid or Gradient
    "PlanetDescriptions": "TreeGrubs",     // Description style used for the planets.  TreeGrubs (BBC) or HoopyCasinos (MSX)
    "InstantDock": false                   // When the docking computer is engaged, instantly dock (true) or let the auto pilot fly in (false)
}
```

## SDL - Development Setup

Elite can be developed using either Visual Studio 2022 or VSCode.
For all platforms, ensure that the dotnet SDK is installed.
Start the EliteSharp project, which supports most platforms, to get straight into the trading and combat!

### Windows (x64)
Nuget packages are in the projects that copy the necessary libraries.
These are: SDL2.dll, SDL2_ttf.dll, SDL2_mixer.dll

### Ubuntu (x64 & ARM64)
The following packages will need to be installed to get the necessary SDL libraries:
``` bash
sudo apt-get install libsdl2-dev
sudo apt-get install libsdl2-gfx-dev
sudo apt-get install libsdl2-mixer-dev
sudo apt-get install libsdl2-ttf-dev
```

## Credits

'Elite - The Sharp Kind' re-engineered in C# by Andy Hawkins 2023.
- Converted into C#/.NET from C.J.Pinder's C version.
- Forked from fesh0r/newkind 06 Dec 2022

'Elite - The New Kind' re-engineered in C by C.J.Pinder 1999-2001.
- christian@newkind.co.uk  |  www.newkind.co.uk
- Reverse engineered from the BBC disk version of Elite.
- Additional material by C.J.Pinder.
- Face information for the ships. Adapted from the Elite ship data published by Ian Bell.
- Alterations to vertex ordering by Thomas Harte. <T.Harte@excite.com>
- Routines for drawing anti-aliased lines and circles by T.Harte.
- Check for hidden surface supplied by T.Harte.

The original Elite code is (C) I.Bell & D.Braben 1984.

Gabriel Gambetta - Computer Graphics from Scratch
https://gabrielgambetta.com/computer-graphics-from-scratch/
