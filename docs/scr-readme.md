# Stunt Car Racer - The Sharp Kind

A C# port of the classic Geoff Crammond racing game 'Stunt Car Racer'.  It is converted from the C++/DirectX9 Windows remake of the Amiga version, and is meant to feel and play the same as the original.

The port shares the `Useful` libraries with 'Elite - The Sharp Kind': hardware access is hidden behind interfaces, with a software renderer drawing through SDL2.  The physics uses the original Amiga fixed-point algorithms and track data.

Race the clock and an AI opponent over four laps of elevated track.  The car takes damage from heavy landings and collisions, boost is limited, and falling off the track costs time - after too long off the track the car is dropped back on.

## Status

In development.  The game starts at the track selection menu with all eight original tracks; gamepad support and the outside camera view are still to come.

## Controls

The keys follow the original PC remake (RETURN to accelerate, steering on S/D).

At the track menu and preview:

| Key | Function |
| --- | -------- |
| 1-8 | Choose a track |
| S | Select the track / start the race |
| M | Back to the track menu (from the preview) |
| Esc | Quit |

During the race:

| Key | Function |
| --- | -------- |
| S | Steer left |
| D | Steer right |
| Enter | Accelerate (+ boost) |
| Space | Brake / reverse (+ boost) |
| B | Brake / reverse (no boost) |
| N | Change the scenery type |
| M | Back to the track menu (after GAME OVER) |
| Esc | Quit |

Notes:
- Boost drains the reserve shown on the dashboard; when it is empty, Enter still accelerates but without the extra power.
- There is no accelerate-without-boost key, exactly as the original keyboard controls.
- Steering only works while the wheels are touching the road.

## Dashboard

| Display | Meaning |
| ------- | ------- |
| Opponent: ... | The opponent's name, shown for four seconds at race start |
| Lap | Current lap (the race is four laps) |
| Boost | Remaining boost reserve |
| Opponent Distance | Distance to the opponent (negative when they are behind) |
| Speed | Current speed |
| Damage | Accumulated car damage |

## Credits

'Stunt Car Racer - The Sharp Kind' re-engineered in C# by Andy Hawkins 2026.
- Converted from the C++/DirectX9 Windows remake: sourceforge.net/projects/stuntcarremake (fork: github.com/fluffyfreak/stuntcarracer).
- The remake uses the original Amiga track data, sound samples and car physics algorithms.

The original Stunt Car Racer is (C) Geoff Crammond / MicroStyle / MicroProse 1989, now believed to be copyright Infogrames or Interactive Game Group.
