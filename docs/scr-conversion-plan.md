# Stunt Car Racer → EliteSharp-style C# conversion plan

Source: `C:\code\github\fluffyfreak\stuntcarracer` (C++, DirectX9/DXUT + SDL2)
Target: `C:\code\github\aphawkins\elite-the-sharp-kind` (this repo), as a new sibling
game alongside Elite, sharing the `src/useful/*` libraries.

## Goal

Hand-port Stunt Car Racer (SCR) from C++/DirectX9 to cross-platform C#
(.NET, SDL2), following the architecture already established by the Elite
conversion in this repo: hardware access hidden behind interfaces, a software
renderer as the primary rendering path, and maximum reuse of the `Useful.*`
libraries rather than SCR-specific duplicates.

## Non-goals for this pass

- No DirectX9/DXUT code is ported. `Common/DXUT*.cpp/h` is device/window/GUI
  plumbing for the original Win32+D3D9 host and has a direct replacement
  already in this repo (`Useful.SDL`). Skip it entirely — if a piece of DXUT
  turns out to contain actual game logic (not device/window management),
  flag it rather than silently porting the DXUT abstraction around it.
- No XBOX controller support yet (`XBOXController.cpp/h`) — keyboard input
  only for the first pass, added later via `Useful.Controls`.
- No sound, no menus/UI chrome, no AI opponents, no multi-track support.
- No attempt at bit-exact physics replication — behavioural fidelity
  ("feels like the original"), not numerical fidelity, is the bar.

## Scope of this first pass

Get one track playable: a single car, driven by the local player, racing
against the clock (no opponent car) on one hard-coded track, rendered via a
software rasterizer through SDL2, with keyboard-only input. This proves the
architecture and the shared-rendering approach before scaling to full
feature parity (all tracks, sound, AI opponents, menus, gamepad).

Concretely, convert:

| SCR source | Role |
|---|---|
| `3D Engine.cpp/h` | 3D transform/projection pipeline — game-specific projection lives in `StuntCarRacerLib`; shared fill primitives go into `Useful.Graphics` |
| `Track.cpp/h` | Track geometry/segment data and collision |
| `Car.cpp/h`, `Car Behaviour.cpp/h` | Car physics/handling |
| `Backdrop.cpp/h` | Sky/scenery — can be deferred/stubbed if it blocks nothing else |
| `wavefunctions.cpp/h` | ~~Small math utility~~ Nothing to port: despite the name it is DirectSound WAV plumbing (Win32 resource loading, RIFF chunk parsing, `IDirectSoundBuffer8` writes), fully superseded by `Useful.Audio` |
| `StuntCarRacer.cpp/h` | Main loop / game state — becomes `StuntCarRacerMain`, structured like `EliteMain` and hosted by the `StuntCarRacer` SDL app |

Deferred (do not convert this pass): `Opponent Behaviour.cpp/h`,
`XBOXController.cpp/h`, all of `Sounds/`, all `Common/DXUT*`.

## Repository layout

Mirror the existing `src/elite/*` split, as a new top-level game under
`src/scr/`. (Note: the live Elite app project is `src/elite/apps/EliteSharp` —
the `EliteSharp.SDL`/`EliteSharp.WinForms`/`EliteSharp.Renderer` folders on
disk are stale and not in the solution.)

```
src/scr/
  apps/
    StuntCarRacer/            (mirrors src/elite/apps/EliteSharp)
  libs/
    StuntCarRacerLib/         (mirrors src/elite/libs/EliteSharpLib)
  test/
    StuntCarRacerLib.Tests/   (mirrors src/elite/test/EliteSharpLib.Tests)
    StuntCarRacerLib.Fakes/   (mirrors src/elite/test/EliteSharpLib.Fakes)
```

`StuntCarRacerLib` references `Useful`, `Useful.Abstraction`, `Useful.Assets`,
`Useful.Audio`, `Useful.Controls`, `Useful.Graphics` the same way
`EliteSharpLib` does — same project reference style, same analyzer settings
(`Directory.Build.props` applies), wired into `EliteSharp.slnx` under
`/src/scr/`.

Do not create a parallel copy of any `Useful.*` library. If SCR needs
something a `Useful.*` library doesn't have yet, extend that library (see
below) rather than adding a SCR-local equivalent.

## Shared-library strategy (highest priority)

**Before writing any SCR-specific code, check whether the equivalent already
exists in `src/useful/*`, and extend the shared library instead of
duplicating it.** Specifically:

- **Math**: Elite already uses `System.Numerics.Vector2/Vector3/Matrix4x4`
  natively. SCR's C++ vector/matrix math should be re-expressed in terms of
  `System.Numerics` types, not a bespoke SCR math class. Check
  `wavefunctions.cpp` against this before porting it verbatim.
- **Rendering — this is the one substantial gap.** `Useful.Graphics.IGraphics`
  already has 2D-projected primitives including `DrawPolygonFilled` and
  `DrawTriangleFilled`, implemented in software by `SoftwareGraphics.cs`.
  This is flat-colour fill only, operating on already-projected 2D points —
  no texture, depth, or perspective-correct sampling. SCR needs
  textured/shaded polygon fill for road segments and cars. Extend
  `IGraphics`/`SoftwareGraphics` rather than building a separate SCR
  renderer. The 3D→2D projection step itself (from `3D Engine.cpp`) is
  game-specific and belongs in `StuntCarRacerLib`, calling into the extended
  `IGraphics`, the same way Elite's `EliteDraw.cs` projects ship vertices and
  then calls `IGraphics.DrawPolygonFilled`.
- **Assets**: use `Useful.Assets.AssetLocator`/`AssetManifest.json`
  (string-keyed) for SCR's tracks/bitmaps, mirroring
  `EliteSharpLib/Assets/`.
- **Input**: use `Useful.Controls.IKeyboard` as-is for the keyboard-only
  first pass; don't write an SCR-specific input layer.
- **Window/app loop**: the `StuntCarRacer` app is modelled directly on
  `EliteSharp`'s `SDLProgram.cs` using `Useful.SDL.SoftwareAbstraction`.
- **Audio/Controls (deferred)**: when sound and gamepad support are added in
  a later pass, they should likewise extend `Useful.Audio`/`Useful.Controls`
  rather than adding SCR-specific equivalents.

If a genuine SCR-only need doesn't generalize to Elite (e.g. track-segment
collision), it's fine to keep in `StuntCarRacerLib` — the bar is "does this
belong in a shared library," not "force everything to be shared."

## Assets

Convert `Bitmap/`, `Tracks/`, `Sounds/` (sounds deferred, but note the format
for later) into `StuntCarRacerLib/Assets/` mirroring `EliteSharpLib/Assets/`.
Track and bitmap data will likely need format conversion (binary C++ struct
dumps → a plain C# data format or a small loader) — treat the original file
formats as read-once inputs to a one-time conversion step, not as a live
binary format the C# code must parse identically forever.

## Conversion order (each step a separate reviewable commit)

1. ✅ Project skeleton (`src/scr/*`), wired into the solution, builds clean,
   empty SDL window opens with a placeholder scene. *(done)*
2. ✅ Assess `wavefunctions.cpp/h` — outcome: skip. It is DirectSound WAV
   loading (resource loading + RIFF parsing), not math; `Useful.Audio`
   already covers this role and sound is deferred anyway. *(done — no code)*
3. ✅ Port `Track.cpp/h` data model (geometry only, no rendering yet) with unit
   tests in `StuntCarRacerLib.Tests`. *(done — `StuntCarRacerLib/Tracks/`;
   the Amiga template tables in `AmigaTrackData.cs` are script-generated from
   the original `Track.cpp`. Deferred from Track.cpp to later steps:
   `MoveDrawBridge`/`ResetDrawBridge` (needs game state + opponent, step 4+)
   and all vertex-buffer/`DrawTrack` rendering code (step 5/6).)*
4. ✅ Port `Car Behaviour.cpp/h` physics, testable independent of rendering.
   *(done — `StuntCarRacerLib/Cars/`: `CarPhysics` (partial classes) plus the
   fixed-point trig from `3D Engine.cpp` (`AmigaTrig`/`TrigCoefficients`).
   Deferred: sound effect triggers (grounded/creak/smash/sparks/engine
   buffers), car-to-car collision and slipstream (need opponent),
   `LimitViewpointY` (camera, step 6), XBOX controller input, action
   replay/Amiga recording debug code. `Car.cpp` itself is D3D9 car mesh
   rendering and belongs to step 5/6.)*
5. ✅ Port `3D Engine.cpp/h`'s projection math into `StuntCarRacerLib`.
   *(done — `StuntCarRacerLib/Rendering/`: `ScrPalette`, `SceneCamera`
   (cockpit view), `Scene3D` (view transform + near-plane clip + FOCUS
   projection, from the software pipeline the D3D remake had disabled) and
   `TrackRenderer` (flat-shaded road/side quads with painter's sorting).
   Deviation from plan: no `Useful.Graphics` extension was needed —
   `DrawPolygonFilled` covers flat-shaded rendering, which matches the Amiga
   original's look. Textured road lines and the chase-view camera are
   deferred; view handedness/signs need visual verification in step 6.)*
6. Wire projection + track + car into a render loop in the app, get one
   track's geometry drawing and the car driving on it, keyboard-controlled.
7. Only then revisit `Backdrop.cpp` and any remaining polish for this scope.

## Validation

- Unit tests for physics/track/math logic in `StuntCarRacerLib.Tests`,
  following existing conventions in `EliteSharpLib.Tests`/`Useful.*.Tests`.
- Manual comparison against the original SCR build for "does it feel right"
  on the single chosen track — steering response, jump/landing behaviour,
  track segment transitions.
- No requirement to match original frame-by-frame physics output; this is a
  behavioural port, not a bit-exact one.
