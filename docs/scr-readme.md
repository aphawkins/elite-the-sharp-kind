# Stunt Car Racer - The Sharp Kind

A C# port of the classic Geoff Crammond racing game 'Stunt Car Racer'.  It is converted from the C++/DirectX9 Windows remake of the Amiga version, and is meant to feel and play the same as the original.

The port shares the `Useful` libraries with 'Elite - The Sharp Kind': hardware access is hidden behind interfaces, with a software renderer drawing through SDL2.  The physics uses the original Amiga fixed-point algorithms and track data.

Race the clock and an AI opponent over four laps of elevated track.  The car takes damage from heavy landings and collisions, boost is limited, and falling off the track costs time - after too long off the track the car is dropped back on.

## Status

In development.  The game starts at the track selection menu with all eight original tracks, framed with the remake's title artwork.  Gamepad/joystick support and the outside camera view are still to come.

## Controls

The keys follow the current remake's scheme: arrow keys to steer/accelerate/brake, Space to boost.

At the track menu:

| Key | Function |
| --- | -------- |
| 1-8 | Choose a track |
| S | Preview the selected track |
| Esc | Quit |

At the track preview:

| Key | Function |
| --- | -------- |
| S | Start the race |
| M | Back to the track menu |
| Esc | Quit |

During the race:

| Key | Function |
| --- | -------- |
| Left arrow | Steer left |
| Right arrow | Steer right |
| Up arrow | Accelerate |
| Down arrow | Brake / reverse |
| Space | Boost |
| N | Change the scenery type |
| M | Back to the track menu (after GAME OVER) |
| Esc | Quit |

Notes:
- Boost (Space) only does anything while accelerating or braking, and only while the reserve shown on the dashboard isn't empty; accelerate and brake work on their own without it.
- Steering only works while the wheels are touching the road.

## Dashboard

The cockpit is drawn as a set of sprites over the track view: front wheels that bounce with the suspension and spin with road speed, an engine that flares when boosting, a damage crack across the top beam that grows with accumulated damage (and leaves a hole once it fully cracks), a speed gauge, and the lap number, boost reserve and opponent distance shown in the dashboard's read-out panels.

## Credits

'Stunt Car Racer - The Sharp Kind' re-engineered in C# by Andy Hawkins 2026.
- Converted from the C++/DirectX9 Windows remake: sourceforge.net/projects/stuntcarremake (fork: github.com/ptitSeb/stuntcarremake, itself forked from github.com/fluffyfreak/stuntcarracer).
- The remake uses the original Amiga track data, sound samples and car physics algorithms.

The original Stunt Car Racer is (C) Geoff Crammond / MicroStyle / MicroProse 1989, now believed to be copyright Infogrames or Interactive Game Group.
