# Changelog

All notable changes to this project are documented in this file. The format
is based on [Keep a Changelog](https://keepachangelog.com/); the project does
not yet cut versioned releases, so everything sits under Unreleased.
Completed items from the [backlog](docs/backlog-roadmap.md) move here.

## [Unreleased]

### Fixed (Fuel scooping rate, 2026-08-04)

- **Fuel scooping gained roughly 40x too much fuel per tick.** The original
  scoops `speed/80` light years per tick (0.1-0.5 LY across the speed
  range); `UpdateCabinTemp` was adding `speed/2` directly, 40x the intended
  rate against a 7 LY tank.

### Fixed (Scooping with a full hold, 2026-08-04)

- **A full cargo hold no longer damages the ship when scooping.** The
  original has two distinct outcomes: can't scoop at all (no fuel scoop, or
  the canister is above the ship) takes full collision damage; a scoop
  attempt that fails only because the hold is full just destroys the
  canister with a sound, no damage. `ScoopItem` collapsed both cases into
  one branch that always applied collision damage.

### Fixed (Legal status band, 2026-08-04)

- **A legal status of exactly 50 showed "Offender" instead of "Fugitive".**
  The original's `CPY #50` branches on carry, so 50 and above is Fugitive;
  `LegalStatusBand.For` used `bounty > 50`, misclassifying exactly 50.

### Fixed (Pack-hunter spawns, 2026-08-04)

- **Pack-hunter spawns were missing Cobra Mk III and used the wrong
  probability shape.** The original picks from 8 ship types via the AND of
  two random bytes reduced to 0-7, deliberately biased toward smaller
  indices (Sidewinder common, Cobra Mk III rare). `CreatePackHunter` had
  only 7 options, chosen with a flat `Random(7)`, and Cobra Mk III wasn't
  reachable at all.

### Fixed (Hyperspace misjump chance, 2026-08-04)

- **Hyperspace misjump chance was roughly a third too low.** The original
  triggers a misjump into witchspace when the random byte is `>= 253`
  (253, 254 or 255); `CompleteHyperspace` used `> 253`, matching only 254
  or 255 - a `>=` had become a `>`.

### Fixed (Docking Computer price, 2026-08-04)

- **Docking Computer cost 1500 Cr, should be 1000 Cr.** The original `PRXS`
  price table has it at 1000 Cr; the port's equipment stock had it at 1500,
  the same value as the Extra Energy Unit one row above - a copy/paste slip.

### Fixed (Missiles arrive, 2026-08-03)

- **A missile detonates when it gets there.** It was asking whether the
  distance to its target was under 256 where both the BBC 6502 and The New
  Kind ask whether each axis is - a sphere inscribed in the original's box,
  throwing away the corners and up to 443 units of reach. A missile that had
  arrived flew straight through, turned round and came back, which is why they
  seemed to take several passes to kill anything.
- Both games' reference sources are now written down in
  [reference-sources.md](docs/reference-sources.md) - which checkout to read
  and which one wins when they disagree (the BBC assembly for Elite,
  ptitSeb's fork for Stunt Car Racer). The two readmes link to it rather than
  carrying the detail, being for people who want to play the games.

### Fixed (The M key fires a missile again, 2026-08-03)

- **A bare `M` fires a missile again.** With `ELITE_DEBUG_MISSIONS` set, the
  Ctrl-M mission jump was reading the `M` key before checking Ctrl. A key
  press is read once and consumed, so the jump ate every bare `M` and then
  declined to run, leaving nothing for the missile. It tests Ctrl first now,
  and without consuming it - `Ctrl-H`'s galactic hyperspace was taking Ctrl
  away the same way and no longer does.
- `IKeyboard` gained **`IsHeld(ConsoleModifiers)`** for guards of the form
  "Ctrl decides what this key does", and its documentation now says which of
  the two to reach for.
- **The test keyboard models the one-shot contract.** `FakeKeyboard.IsPressed`
  answered every call, so no test could see two handlers act on one press.
  It consumes now, like the real keyboard.

### Changed (Renditions, 2026-08-03)

- Everything the game draws is now a **rendition**: a plugin holding the
  screens, the HUD, the way planets, suns, stars and ships are drawn, and the
  artwork, palette, fonts and models they are drawn with. The two that ship
  live in a **`Renditions` folder beside the executable** and are found at
  startup, the same way missions are. Nothing in the game knows which
  renditions exist, so adding one is a folder.
- **They need not be machines.** A rendition names itself and declares its own
  resolution and colour limits, so the next one can be futuristic, underwater
  or psychedelic at whatever size it likes - it is not limited to standing in
  for 8-bit or 16-bit hardware.
- **`engine.tier` is now `engine.rendition`**, and takes a name rather than
  `8Bit`/`16Bit`. Existing config files are read and upgraded the first time
  they are saved, so a commander keeps the look they had.
- The Engine Settings screen offers the renditions actually installed, so it
  is no longer possible to select one that is not there.
- **Fixed:** a police Viper showed purple on the 16-bit scanner and fired a
  cyan beam. A ship is one colour everywhere now, so the 16-bit beam is
  purple to match. 8-bit police stay cyan, where that palette's only purple
  is already the missile.
- No other visible change: both renditions draw exactly what they drew before.

### Changed (Screens become plugins, 2026-08-03)

- The 8-bit and 16-bit screens are no longer written into the game. Each tier
  is an assembly of its own — **`EliteSharp.Views.EightBit`** and
  **`EliteSharp.Views.SixteenBit`** — referencing the contracts and nothing
  else, and the game finds them at startup in a **`Views` folder beside the
  executable**, the same way it finds a mission. Adding a tier is now an
  assembly rather than a second branch in every screen registration.
- **The `Views` folder is required.** Delete it, or the pack for the tier
  configured in `engine.tier`, and the game refuses to start and says which
  tier it could not find — there is nothing to draw with. A pack missing a
  screen is refused the same way before the first frame, naming every screen it
  does not draw.
- The mission contracts assembly is now **`EliteSharp.Abstractions`**, one
  plugin-facing assembly subdivided by namespace, so a plugin of any kind
  references one thing. A mission plugin built against the old
  `EliteSharp.Missions.Abstractions` needs recompiling; nothing about what a
  mission *does* changed.
- No visible change to either tier: both draw exactly what they drew before.

### Changed (Missions become plugins, 2026-08-02)

- The two missions are no longer written into the game. They live in
  **`EliteSharp.Missions.Classic`**, an assembly that references the mission
  contracts and nothing else, and the game finds them at startup in a
  **`Missions` folder beside the executable** — the same way it would find
  anyone else's. Adding a mission is now a new assembly rather than edits to
  two enums, two controllers, two `Screen` values and four views.
- **Fixed:** the rumours about the stolen Constrictor were printed for whatever
  system was selected on the chart, so Reesdice was named from anywhere in the
  first galaxy. A rumour is somebody in the station talking, and is now only
  said about the system the commander is standing on.
- The two mission briefing screens are one screen, which lays itself out from
  what a briefing contains — a headline, how many paragraphs, whether somebody
  is pictured — rather than from which mission sent it. On the **8-bit tier the
  Constrictor debrief moves down the screen**, matching the Thargoid debrief;
  one rule cannot draw the two layouts they had drifted into. 16-bit is
  unchanged.
- Commander files hold only the mission stages actually reached, so a fresh
  commander writes none. A file naming a mission that nothing provides is
  refused and says which name it could not place — including when a mission's
  plugin has been removed.
- `ELITE_DEBUG_MISSIONS`' Ctrl-M jump now moves the commander to a real system
  rather than overwriting two of the docked planet's six seed bytes, so the
  chart, the planet name and the data screen agree afterwards.

### Changed (The 16-bit tier becomes a real 12-bit machine, 2026-08-01)

- The 2026-07-30 rename of the 16-bit palette to web colour names is backed
  out: the 29 ramp names (`LighterGrey`, `Gold`, `Lilac`, `RedOrange`…)
  return across `palette.json`, `palette.mtl`, the ~380 `usemtl` lines in
  the 31 `SixteenBit/*.obj` models and every `Palette["..."]` lookup on the
  16-bit path. `FractalPlanet` goes back to the plain shared lookup, since
  the names it wanted exist in both palettes again.
- In its place the tier gets the constraint that actually describes the
  hardware: a **12-bit DAC, 4096 colours**, four bits per channel. Every
  palette entry is now a value that hardware could produce — channels
  widened by replication (`0xE` → `0xEE`), never by a left shift (`0xE0`),
  which can never reach white. Most of the old ramp values were shifted
  nibbles, so restoring them properly expanded changes the expansion rather
  than the colour.
- `AssetColourBudget.ChannelBits`/`IsOnGrid` and `AssetSet.Load` enforce it
  at startup for both the named palette and every bitmap pixel, failing
  with the offending asset and colour named. Separate from the existing
  `PaletteNamesEveryColour` rule, which stays false for 16-bit — the tier
  is direct-colour, so a bitmap need not use a *named* colour, only a
  *producible* one. The 8-bit tier keeps 8 bits per channel and is
  unaffected.
- 36 colours across the 16-bit bitmaps were repainted onto the grid, mostly
  `elitetext.bmp`'s anti-aliased grey ramp and `scanner.bmp`. Stunt Car
  Racer shares the tier, so its `menu.bmp` was repainted too; its palette
  was already compliant.

### Added (a way into Elite's mission screens, 2026-08-01)

- The five mission briefings were only reachable by playing the missions -
  the Constrictor brief wants a combat rating of Above Average, the Thargoid
  sequence the third galaxy and two particular planets - so checking their
  layout meant hours of play or a hand-edited save. With the new
  `ELITE_DEBUG_MISSIONS` environment variable set (to any value), Ctrl-M now
  cycles them: Constrictor brief, Constrictor debrief, the two Thargoid
  briefs, the Thargoid debrief, and round again.
- Gated at runtime rather than by `#if DEBUG`, following
  `ELITE_DEBUG_COMMANDER` and the conditional-compilation removal of issue
  #7, so the screens can be checked in a Release build too. Unset, Ctrl-M
  does nothing. Ctrl-modified because a bare M would fire from the
  commander-name screens, which take typed letters, and F12 - the obvious
  key - is already `GameHost`'s frame dump.
- The environment variables are now documented in tables rather than only in
  code comments, each where it belongs: the shared `GAME_KEY_SCRIPT` and
  `GAME_FRAME_DUMP_DIR` in the [main readme](README.md#environment-variables)
  alongside the shared engine config, and each game's own in its readme
  (`ELITE_LOG_LEVEL`, `ELITE_DEBUG_COMMANDER`, `ELITE_DEBUG_MISSIONS`;
  `SCR_LOG_LEVEL`), cross-linked both ways. Elite's key table gained the F12
  frame-dump row with them.
- Each jump sets the state the screen's own `Reset` tests for and lets that
  `Reset` run rather than forcing the screen in, so what is drawn is the
  screen the game draws - the briefing text chosen the usual way, and the
  Constrictor spawned in front of the player for the screen that shows it.
  They are cheats and leave the commander mid-mission: the last two also
  overwrite the docked planet's seed with the system the briefing expects.
  Documented in [docs/elite-readme.md](docs/elite-readme.md).
- `MissionJumpTests` drives the real game through `HeadlessGameHarness` and
  asserts each stage lands on the right screen showing the right briefing,
  so a drift between the jumps and the controllers' entry conditions fails
  the build. Verified live with `run-elite`, on and off.

### Fixed (the 100-polygon-per-frame cap, 2026-08-01)

- `PainterRenderer`, `ZBufferRenderer` and `WireframeRenderer` each held a
  fixed 100-entry polygon array and opened `Submit` with
  `if (_totalPolys == MAXPOLYS) { return; }` - no log, no counter. The cap
  came from The New Kind's `poly_chain` but was reachable here: with 20
  universe objects against models running to 29 faces, four Cobras and a
  station is already ~91 polygons before lasers or explosion debris, and
  whatever arrived after the hundredth was silently not drawn.
- The three now share `PolygonBuffer`, which starts at 128 entries and
  doubles on demand, so nothing is dropped. The buffer is reused frame to
  frame, so the growth settles at the busiest scene seen and stops
  allocating. `WireframeRenderer` was under the same cap and is fixed with
  them, so its hidden-line pass no longer loses surfaces from the depth
  prepass either.
- Covered by `PolygonRendererCapacityTests`, which submits 500 polygons in a
  frame and asserts every one reaches `IGraphics` - failing against the old
  cap - plus a second frame to check the count resets rather than the buffer
  shrinking.

### Fixed (camera-space backface culling for Elite's ships, 2026-07-31)

- `ShipBase.DrawModelFaces` decided backface culling from the *projected*
  outline, whose behind-camera points had been through `ProjectPoint`'s
  `if (vec.Z <= 0) { vec.Z = 1; }` depth clamp. A face straddling the camera
  plane therefore had meaningless screen X/Y feeding the winding test, so it
  could be culled or kept incorrectly - and the near-plane clip that runs
  afterwards cannot undo a cull already taken. Visible as faces flickering
  on and off as a ship passed close by.
- The cull now runs in camera space, against the face's own normal rotated
  into view: `dot(normal, pointOnFace) <= 0`. The normals are precomputed
  once per model by `FindFaceRoots`, which was already deriving plane
  normals for the decal-rooting pass, so this costs no extra per-frame work.
  The clamp remains only for the laser aim, where it is harmless.
- 2-point detail lines are culled too. The old winding test resolved
  `point2` back to `PointIndices[0]` for a face of fewer than three points,
  degenerating to `0 <= 0` so every detail line passed every frame whichever
  way it faced. A line has no normal of its own, so it now takes the normal
  of the face whose plane it lies in - the root `FindFaceRoots` already
  finds. This removes the stray stub that poked off the far side of a hull
  silhouette, where no geometry existed to depth-test it away.

### Added (hidden-line removal for wireframe, 2026-07-31)

- Wireframe still showed the missile's fins, and the wings on the Krait,
  Cougar and Cobras, through the hull. No cull can fix that: those are
  double-sided plates, two coplanar triangles wound opposite ways, so
  exactly one of each pair faces the camera from any angle and survives
  backface culling even when it sits behind the body. Culling removes
  surfaces facing *away*; it cannot remove surfaces facing you from behind
  something else, which on a convex hull is the same thing and on a hull
  with fins is not.
- `WireframeRenderer` now buffers the frame and draws it in two passes: a
  new `IGraphics.FillDepth` writes every surface into the depth buffer
  without drawing it, then the outlines are drawn depth-tested against it.
  Threaded through the existing depth-fill path in both backends with a
  `writeColor` flag rather than duplicating the triangle walk. 2-point
  detail lines are excluded from the first pass - they are not surfaces and
  occlude nothing.
- An edge lies exactly on the surface it bounds, so it ties with the depth
  that surface just wrote, and the two rasterisers sample the same plane at
  slightly different sub-pixel positions - the tie is decided by about a
  pixel's worth of depth gradient. No depth bias resolves this: one large
  enough for a near edge-on face (whose gradient is huge) is also larger
  than the gap between a wing and the hull behind it, so any value that
  kept an edge visible also let hidden ones leak. A fixed 1% and then a
  slope-scaled offset were both tried and both left visible dashing.
- Settled by identity instead of magnitude: `FillDepth` tags each pixel it
  wins with a surface id, and `DrawLineDepth` takes the id of the surface
  its edge belongs to, drawing over that surface however the depths
  compare. No bias at all. Zero means "no surface", which never matches, so
  `ZBufferRenderer` and the textured paths are unaffected. Ids are cleared
  with `ClearDepth`.
- Verified against a filled reference render of the same missile poses: the
  wireframe's visible edges match the visible face boundaries of the solid
  image exactly, with nothing crossing the body. `VisualDumpTests` gained
  the missile dumps, wireframe and solid, since this is the model that
  exercises it hardest.

### Fixed (wireframe hidden-surface gaps: unrooted lines and lasers, 2026-07-31)

- Wireframe mode still showed hull detail lines and laser bolts through the
  hull. `WireframeRenderer` has no depth buffer at all and does no ordering
  - deliberately, since outlines are order-independent - so the backface
  cull is its *only* hidden-surface mechanism, and two things were escaping
  it. In solid mode the per-pixel depth test had been masking both.
- A detail line lying in no other face's plane has no root, so it had no
  normal to be culled against and drew unconditionally. It is now culled by
  the normals its two ends *share*: the model already records, per vertex,
  the faces that vertex belongs to (the `pn` entries, previously read only
  by the explosion debris effect), and the faces a line runs along are those
  common to both its ends. The line draws when any of them faces the camera,
  which is how the original decided a line's visibility. A line whose ends
  share none - a model carrying no such data - still draws, so this cannot
  silently remove detail.
- The laser bypassed the face loop entirely and was never culled. The bolt
  springs from a mount on the hull, so it now draws only when that vertex is
  visible by the same test, and a ship firing away from the viewer no longer
  paints a bolt through its own hull.
- Both rules fall back to drawing when the model carries no normals for the
  vertices in question, which is what the existing `ShipBase` transform and
  laser tests rely on; four new theory cases cover the culled and kept
  directions for each. Verified live via `run-elite` in wireframe (the stray
  stub on the Cobra Mk3 is gone, silhouettes are clean) and re-checked in
  solid mode for lost detail.

### Fixed (real per-vertex depth for Elite's z-buffer, 2026-07-31)

- Elite's z-buffered mode never actually got per-vertex depth.
  `ZBufferRenderer.Submit` filled every entry of a polygon's `Depths` array
  with the single flat whole-face key it was handed, so
  `DrawPolygonFilledDepth` interpolated a constant across each triangle and
  the per-pixel test resolved at whole-polygon granularity - no better than
  the painter's chain on the interpenetrating and steeply-angled faces a
  depth buffer exists to solve. SCR was already feeding real per-vertex
  depth straight to `IGraphics`; Elite was the side that wasn't.
- `IPolygonRenderer.Submit` (and `IEliteDraw.DrawPolygonFilled` behind it)
  now takes the camera-space depth of each point alongside the flat key.
  `ShipBase.BuildFacePolygon` emits the near-plane-clipped `Z` per vertex,
  which is the value it was already computing and discarding.
  `PainterRenderer` and `WireframeRenderer` ignore the new argument; the
  flat key stays because the painter's chain still sorts by it.
- `ZBufferRenderer`'s O(n²) insertion sort into a back-to-front linked chain
  is gone. It sorted whole polygons and *then* depth-tested every pixel;
  with a real per-pixel test the sort was dead work. Polygons now draw in
  submission order. `PainterRenderer` keeps its chain, which is the whole of
  what it does.
- Decals needed a bias. Cockpit windows and engine plates lie exactly in the
  plane of the hull face beneath, so per-vertex depth makes them tie pixel
  for pixel rather than merely share a key, and the rasteriser's scanline
  interpolation of inverse depth from floored pixel positions makes that tie
  inexact - the hull speckles through. Faces that `FindFaceRoots` roots to
  an earlier plane are now pulled 1% of their depth nearer. 0.1% was
  measured live as too small to cover the interpolation error on a near
  edge-on decal; 1% covers it and stays far inside the front-to-back spread
  of a single ship.
- `PainterAndZBufferRenderIdenticallyForNonDecalGeometry` became
  `PainterAndZBufferAgreeOnSilhouetteForNonDecalGeometry`. The two
  strategies are no longer pixel-identical on a convex decal-free model:
  the z-buffer awards a shared edge to the nearer face even when it is drawn
  first, where the painter's later face simply paints over it. The
  silhouette is still exactly equal, and the seam pixels are ~1% of the
  covered area.
- Lines are depth-tested now too, via a new `IGraphics.DrawLineDepth`
  implemented in both backends (`SoftwareGraphics.DrawLineIntDepth`,
  `SDLGraphics.DrawLineDepthToLayer` — Bresenham interpolating inverse
  depth by fraction of the major axis, mirroring the span fills).
  `ZBufferRenderer` routes its 2-point submissions through it. Previously
  every line — model detail lines and the laser bolt — went to the plain
  `DrawLine` and wrote pixels unconditionally, so hull detail on the far
  side of a ship drew straight through the hull. Dropping the back-to-front
  chain removed the accidental ordering that had been partly masking this,
  which is why it is fixed here rather than filed. `PainterRenderer` keeps
  plain `DrawLine`, which is correct for it — the chain is its ordering.
- Detail lines take the decal bias for the same reason decals do, and the
  laser bolt takes it at its mount: the bolt's far end is a screen-space
  viewport-boundary point with no camera depth of its own, so the whole
  line tests at the mount's biased depth — it cannot be swallowed by the
  hull it springs from, but anything genuinely in front still hides it.
- Verified live via `run-elite` against the same poses rendered in
  painter's mode: decal panels render solid at every angle including near
  edge-on, no faces flicker or drop out, and no detail line draws across a
  hull face any more.
- Left open, and filed in [backlog-issues.md](docs/backlog-issues.md): a
  2-point face never gets backface-culled, because the winding test
  resolves its third point back to its first and degenerates to `0 <= 0`.
  Depth testing hides such a line where it crosses the hull, but the part
  projecting outside the silhouette has nothing to occlude it and still
  shows as a stray line off the hull edge.

### Changed (shared near-plane clipping, 2026-07-31)

- `Scene3D.ClipPolygonToNearPlane` moved out of SCR into
  `Useful.Graphics.NearPlaneClip.Clip`
  ([NearPlaneClip.cs](src/useful/libs/Useful.Graphics/NearPlaneClip.cs)). Both
  overloads (plain, and the one interpolating texture coordinates) were
  already pure static Sutherland-Hodgman with no SCR dependencies; the plane
  distance is now a parameter rather than a constant, because the two games
  work in different camera units. SCR passes its own `Scene3D.NearPlane`
  (0.5 track units) and is otherwise unchanged. The three clipping tests
  moved from `Scene3DTests` to a new `NearPlaneClipTests`, joined by two
  covering the texture-coordinate overload and a non-default plane.
- Elite adopted it: `ShipBase` used to force `vec.Z = 1` on any transformed
  point at or behind the camera, which does not clip the geometry so much as
  drag the offending vertex onto the camera plane - a face crossing that
  plane was drawn with a smeared, wrong-shaped outline. Ship faces are now
  clipped properly: `TransformModelPoints` keeps the camera-space points
  alongside the projected ones, and each face is clipped against a near
  plane of 1 unit and projected from the clipped result. A face entirely
  behind the plane is now dropped rather than drawn as garbage.
- Two things deliberately keep the old clamp. The backface-winding test and
  the laser aim both read projected points directly and need *some* value
  for a point behind the camera, so their behaviour is unchanged; and a
  2-point detail line is not a polygon, so feeding it to a cyclic clipper
  would walk its single edge twice. Faces of three points or more - which is
  every filled surface - go through the clipper.
- Verified live in both games via `run-elite`/`run-scr`: Elite's ship parade
  across a full close approach (the case that crosses the camera plane) and
  the flight views, and an SCR race where the road still reaches the bottom
  of the viewport uncut.

### Added (startup diagnostics shared by both games, 2026-07-31)

- `GameApp.Run` ([GameApp.cs](src/useful/libs/Useful.App/GameApp.cs)) now logs
  two facts right after "Starting {title}", once per process, so a bug
  report's log file carries what fixing it usually needs first without
  asking the reporter for their machine spec or config: the build's
  informational version (from `AssemblyInformationalVersionAttribute`, i.e.
  MinVer's git-derived version), OS description, .NET runtime, process
  architecture and logical processor count (`LogMessages.SystemInfo`); and
  the engine settings actually in effect - backend, tier, window scale, fps,
  graphic style, depth sort, and the sound toggles
  (`LogMessages.EngineSettings`). Both are logged from the one shared call
  site both `SDLProgram.cs`s already route through, so Elite and SCR get it
  identically with no per-game code.
- Both lines are a single JSON object (`System.Text.Json`-serialized) rather
  than a prose sentence, so the two facts can be machine-parsed back out of
  the log file, e.g.
  `{"backend":"Software","tier":"SixteenBit","windowScale":1,"fps":60,...}`.
  The message template uses Serilog's `:l` (literal) format specifier on the
  JSON string - without it the sink would additionally quote-and-escape the
  whole blob, same as it already does for any plain string property.
  Verified live in both `elite-.log` and `scr-.log` via `run-elite`/`run-scr`.

### Removed (last conditional compilation site in Elite, 2026-07-31)

- `SaveFile`'s `#if DEBUG` (issue #7) picked `CommanderFactory.Max()` as the
  starting commander in debug builds and `.Jameson()` otherwise, so trying
  the debug commander meant a debug build. It's now a runtime check against
  a new `ELITE_DEBUG_COMMANDER` environment variable (presence-checked, same
  convention as `Useful.Abstraction.GameHost`'s `GAME_KEY_SCRIPT`/
  `GAME_FRAME_DUMP_DIR`), so `Jameson()` is the default in every build and
  `Max()` is an opt-in without recompiling. The other site the backlog item
  named, a commented-out `////#if QHD` block in `SDLProgram.cs`, had already
  been deleted by an earlier change; the item was stale on that half.
- `SaveFile` also logs the outcome at startup (new
  `LogMessages.DebugCommanderEnvVar`, Information level so it shows with
  the default log config): `elite-.log` now records whether
  `ELITE_DEBUG_COMMANDER` was set and which commander it started with,
  e.g. `ELITE_DEBUG_COMMANDER is set; starting commander is MAX.` -
  verified live against both branches with `run-elite`.

### Fixed (a missing sound effect no longer throws, 2026-07-31)

- `AudioController.PlayEffect` looked its sample up with an unguarded
  dictionary indexer, so an effect name absent from the sample dictionary
  threw `KeyNotFoundException` out of whatever gameplay code asked for the
  sound. It now uses `TryGetValue` and logs a Warning no-op instead, via a
  new `Useful.Audio.LogMessages.MissingSfxSample` following the same
  `[LoggerMessage]` pattern as `Useful.Graphics`.
- The logger reaches it through a second constructor taking `ILogger?`
  (the existing three-argument one delegates with `null`, so tests and any
  other caller are unchanged); Elite's DI registration passes a real
  logger. The practical effect is that a test constructing an
  `AudioController` only has to populate the effects it actually cares
  about — `EffectsOn` defaults true, so previously every effect the code
  under test happened to trigger had to be present.

### Fixed (sdl-drive key names, 2026-07-31)

- `drive.ps1`'s `ConvertTo-VirtualKeyCode` had no entry for `Return`, so a
  `key:Return` step threw "Unknown key name" (hit while driving SCR's
  menus). `Return` is now an alias of the existing `Enter`, and `Tab` and
  `Back`/`Backspace` were added alongside it as the other obvious
  omissions.

### Fixed (one word-wrap, two off-by-ones, 2026-07-31)

- The line breaking that `BaseView8Bit` and `BaseView16Bit` each had a
  copy of is now `Views/TextWrap.Split`, which returns rows rather than
  drawing them. `DrawTextPretty` draws those rows left-aligned; the 8-bit
  options screen centres the same rows and stacks them upwards from the
  bottom of the viewport, which is why it kept a wrapper of its own and no
  longer has to. Splitting rather than sharing the drawing is what let one
  helper serve both.
- Text that already fitted was broken anyway: the break scan clamped to
  the last index of the string instead of noticing the remainder fit, so a
  40-character string on a 40-character row came out as two rows.
- A row ending in a comma or period could run one character past its
  width. The scan started at the first character *past* the row rather
  than the last one on it, so a break there put the punctuation on a row
  already full. Narrowing it shifts wrapped 16-bit mission text slightly.
- A space at the break is dropped rather than kept on the row it ends. It
  drew nothing left-aligned, but it pushed a centred row half a character
  off centre.

### Changed (ViewLayout becomes the viewport, and 8-bit text gets a grid, 2026-07-31)

- The screen border was a pixel short on the right: `DrawRectangle` draws
  its far edge at `position + size - 1`, so passing `ScreenWidth - 1` lost
  the last column. It now touches both edges.
- `ViewLayout` describes one region, the viewport — everything above the
  HUD — starting at the screen origin. `BorderWidth`, `ScannerTop`,
  `ScannerLeft`, `ScannerRight` and `Offset` are gone; `Offset` duplicated
  `ScannerLeft` exactly, and both tiers' scanners being full-width meant
  the scanner members were screen edges under another name. See
  [decisions.md](docs/decisions.md).
- The border is drawn over the viewport's outer pixel rather than reserved
  out of it, so its width no longer moves any view, and each screen draws
  it in its own `Draw` instead of `EliteMain.Update` doing it once a tick.
- The 8-bit viewport is exactly 40x25 of its 8x8 character cells, and
  8-bit text is positioned with `Row(n)`/`Column(n)`, `DrawTextCentreOnGrid`
  and `SnapToGrid` rather than pixel constants. Row 0 and the outermost
  columns belong to the border, so the chrome band is row 1, the header
  rule row 2, and screen content starts at row 3. The header's yellow
  verticals down to the scanner are gone on both tiers.
- The FPS overlay is an engine config option, `engine.graphics.showFps`
  (off by default), rather than a `#if DEBUG` gate that Release could
  never show and Debug could never hide. It, the hyperspace countdown and
  the in-flight message are per-tier chrome on `IBaseView` now, not
  `EliteMain`'s own drawing.
- `ShortRangeChartViewBase` — the last screen combining controller and
  view — is split into `ShortRangeChartController`/`ShortRangeChartModel`
  with per-tier views. Its model carries screen positions rather than
  galaxy space, because the row packing decides which planets get a name
  and the blob sizes depend on the `CarryFlag` side effect of naming;
  keeping both in the controller is what the backlog entry preferred.

### Changed (FastColor's implicit conversions removed, 2026-07-30)

- Phase 4, closing out the colour-handling unification: the implicit
  `uint`↔`FastColor` conversions added in phase 1 are gone, so the
  boundary is now explicit wherever one still exists. Rather than cast at
  each call site — which would have put the conversion noise inside the
  render inner loops the earlier phases had deliberately kept clean — the
  type flows end to end instead: `FastBitmap.GetPixel` returns `FastColor`
  (the backing store stays `uint[]` for memalloc) and the redundant
  `SetPixel(.., in uint)` overload goes; `BitmapFont.Ink`/`Background`,
  `SoftwareGraphics`' private helpers and the ~110 Elite and SCR colour
  fields phase 3 had left on `uint` all become `FastColor`. Alpha tests
  read `color.A != 0` rather than masking with `0xFF000000`.
- Two places keep `uint` deliberately: `AssetSet`'s colour-budget
  bookkeeping does set arithmetic and sorts with `Order()`, which
  `FastColor` does not support, and `SDLGraphics` keys its text-texture
  cache on the raw ARGB.
- `PlanetRenderer.Landscape` was doing double duty — the fractal
  midpoint-displacement pass filled it with heights, `ColorLandscape`
  overwrote it in place with colours, and the renderer drew it as pixels.
  That cannot survive typing, since heights are averaged arithmetically
  and colours are not, so `FractalPlanet` now owns a private `uint[,]`
  height map and `Landscape` is the `FastColor` buffer it always was at
  draw time. Behaviour is unchanged; the per-seed determinism test still
  passes.
- The two `FastColorTests` covering the deleted operators now exercise
  `FromUInt32`/`ToUInt32` instead, so the round trip stays covered.

### Changed (The 8-bit palette becomes sixteen web colours, 2026-07-30)

- `Palette/EightBit/palette.json` is now sixteen CSS colour names at their
  exact web values, replacing the 29 inherited ramp names. The rationale,
  including why `DimGray` displaced `Magenta` and why these keep the CSS
  `Gray` spelling against the repo's UK standard, is in
  [decisions.md](docs/decisions.md).
- The 14 8-bit bitmaps were repainted onto those sixteen colours
  (maintainer). Transparency is carried by a 32bpp alpha channel at
  `00000000` — indexed and 24bpp BMP cannot express it, since
  `BitmapReader` forces those opaque, so the sprites that need a
  transparent background are 32bpp and the rest stay indexed.
- Every `Palette["..."]` lookup on the 8-bit path moved with it: `Gold` to
  `Yellow`, `LightRed` to `Red`, `LighterRed` to `Orange`, `LightGrey` to
  `LightGray`, across 13 views.

### Added (Models are tier-scoped, 2026-07-30)

- `Assets/Models/` splits into `EightBit/` and `SixteenBit/`. Geometry is
  resolution-independent, but colour is not: `ModelReader` resolves each
  `usemtl` through the active palette, and 13 of the 21 material names the
  ships use stopped existing at 8-bit.
- The 8-bit copies' ~380 `usemtl` lines were rewritten onto the web names.
  The grey ramp maps onto five neutral steps by luminance — `LighterGrey`
  to `White` (255), `LightGrey` to `LightGray` (211), `Grey` to `DarkGray`
  (169), `DarkGrey` to `Gray` (128), `DarkerGrey` to `DimGray` (105) —
  deliberately stopping short of `Black`, which would render 48 faces as
  holes against space.
- `AssetLocator.ModelPaths` uses `TierPath` like the other tier-varying
  categories; `Models` is no longer tier-neutral.
- Each tier's folder gains a `palette.mtl` generated from its own
  `palette.json`. The game ignores it, but the `.obj` files declare
  `mtllib palette.mtl` and would otherwise open with missing materials in
  external tools.

### Added (8-bit bitmaps must only use palette colours, 2026-07-30)

- Loading an asset set now checks that every opaque colour in its bitmaps
  is one the palette names, and fails startup naming the offending files
  and colours. Previously nothing tied art and palette together, and only
  the combined colour count would notice, and only once it crossed the cap.
- The rule applies to `EightBit` and not `SixteenBit`, following the
  hardware each tier stands in for: 8-bit machines were indexed-colour, so
  an unnamed bitmap colour is one the machine could not have shown, while
  16-bit is direct-colour and its palette only names colours the geometry
  draws with. `AssetColourBudget.PaletteNamesEveryColour` carries the
  distinction, beside `MaxColours`.
- It is a subset test, not equality, and transparent pixels are excluded —
  as they are from the colour cap.

### Changed (Nothing that draws is shared between tiers, 2026-07-30)

- `ViewLayout` holds the tier's screen metrics, derived from a screen size,
  the scanner art's size and the coordinate scale rather than stored, so no
  two can disagree. `IEliteDraw` exposes it and no longer carries its own
  `Top`/`Left`/`Centre`/`Scale`/`Scanner*` — those had become a second
  spelling of the same values.
- `IBaseView` with `BaseView8Bit`/`BaseView16Bit` holds the chrome every
  screen shares: `DrawViewHeader`, `DrawTextPretty`, `DrawBorder` and
  `DrawHyperspaceCountdown`. The last two came off `EliteDraw`, which no
  longer draws any chrome; `EliteMain` calls them through the base view.
- `Scanner`, `LaserDraw`, `GradientSun` and `StripedPlanet` each split into
  a base holding the logic and per-tier subclasses holding the colours and
  positions. `Scanner` is the substantial one: ~20 offsets hardcoded to the
  512x129 scanner became named members, so each tier's art sets its own HUD
  layout.
- `ShortRangeChartView` splits into `ShortRangeChartViewBase` plus per-tier
  views. Its cross-hair clamps, previously the literals 510 and 339, are a
  per-tier `CrossBounds`.
- `IEliteDraw.Tier` lets `PlanetFactory` and `SunFactory` pick a renderer;
  the rest resolve per-tier through DI. `IsEightBit` is one shared helper
  rather than three copies.

### Fixed (16-bit HUD after the 640x512 widening, 2026-07-30)

- The 16-bit scanner art is 640x129. Comparing it against the 512-wide
  version shows it was re-laid-out rather than stretched: the left dial
  panel did not move (its edge is at x=97-100 in both), the radar's centre
  line moved half the increase (253 to 317), and everything from the
  compass rightwards moved the full 128 — the right panel's edge 411 to
  539, and the compass ring's centre 386.5 to 514.5.
- `Scanner16Bit`'s right-hand cluster is +128 accordingly, which puts the
  energy banks back inside their numbered boxes and the speed, roll and
  climb indicators back in the SP/RL/DC panel instead of over the radar.
  `ScannerCentre` needed no change: it derives from `ViewLayout.Centre`.

### Added (A view per tier for every screen, 2026-07-30)

- Every screen now has both an 8-bit and a 16-bit view, in a folder and
  under a name that says which tier it is: the 18 existing views moved to
  `Views/SixteenBit/<Screen>View16Bit.cs` (namespace
  `EliteSharpLib.Views.SixteenBit`), matching the `Views/EightBit/`
  convention that the first 8-bit views established.
- Nine new 8-bit views complete the set: `GalacticChartView8Bit`,
  `QuitView8Bit`, `Intro1View8Bit`, `Intro2View8Bit`,
  `EscapeCapsuleView8Bit`, `GameOverView8Bit`, `OptionsView8Bit`,
  `SettingsListView8Bit` and `PilotView8Bit`. The earlier survey had
  called several of these "no second class needed" because their layout
  is already `Centre`/`ScannerTop`-relative; a view per tier per screen
  is now the rule regardless, so each tier's spacing constants can be
  authored independently instead of one set serving both.
- All 18 `IView<TModel>` registrations branch on
  `IAssetLocator.Tier` via the existing `IsEightBit(sp)` helper.
- `SettingsListView8Bit` keeps the two-column grid rather than dropping
  to one, even though a name and its value do not fit side by side at
  40 characters: the shared `SettingsListController`'s cursor moves in
  steps of two, so a single visual column would make Up/Down look like
  it skipped a row. Each cell stacks its value under its name instead.
- `OptionsView8Bit` word-wraps the credits itself. `IEliteDraw.DrawTextPretty`
  was tried first and is unusable here - it breaks text that already fits
  (its `i += maxlen` then clamp-to-`Length - 1` always finds a break point)
  and draws left-aligned where these lines are centred. Backlogged.
- The DI registrations split again, into a second static class
  (`EliteSplitAnimatedScreensServiceCollectionExtensions`, holding the
  sequence and flight screens): nine more view types took
  `EliteSplitScreensServiceCollectionExtensions` to 102 coupled types
  against CA1506's class limit of 96. Splitting a method could not fix
  it this time, since that metric is per class.
- The durable rules from this and the preceding controller/view work now
  live in [architecture-principles.md](docs/architecture-principles.md)
  under "Screens: controllers, models and per-tier views", and the
  backlog's tier-presentation section is down to the three items still
  open. Verified by build, the full test suite, and screenshotting the
  new 8-bit views live (title screen, ship parade, galactic chart,
  options, game settings, engine settings, quit, front view);
  `GameOverView8Bit` and `EscapeCapsuleView8Bit` are single centred
  lines and were not driven to on screen.

### Added (8-bit views for every absolute-layout screen, 2026-07-30)

- `InventoryView8Bit`, `MarketView8Bit`, `EquipmentView8Bit`,
  `PlanetDataView8Bit`, `ConstrictorMissionView8Bit`,
  `ThargoidMissionView8Bit`, `LoadCommanderView8Bit` and
  `SaveCommanderView8Bit` (`Views/EightBit/`), each wired through the
  same `IsEightBit(sp) ? ... : ...` DI pattern `CommanderStatusView8Bit`
  established. These are first-draft layouts (build, render, no
  obvious overflow, checked once per screen) rather than
  worst-case-verified ones - the maintainer is taking the spacing
  judgement over from here.
- `PilotView`'s one absolute coordinate needed no second class: its
  hardcoded `y=358` was always just the 16-bit `ScannerTop - 25`
  (383-25=358), so it now reads `_draw.ScannerTop - 25` and serves both
  tiers unchanged.
- `MarketView8Bit` drops the 16-bit view's separate "Unit" column
  (folded into the quantity text, e.g. `"20t"`) since five columns
  does not fit 320px/8px-per-character.
- `AddSplitConsoleScreens` split again, into `AddSplitStatusScreens`
  (`CommanderStatus`/`Inventory`/`PlanetData`), after the second and
  third 8-bit view tripped CA1506's per-method limit a second time.

### Added (First 8-bit view authored: CommanderStatus, 2026-07-30)

- `CommanderStatusView8Bit` (`Views/EightBit/`), a fresh 320x256 layout
  for the commander status screen rather than a scaled-down version of
  the 16-bit one, since the two tiers' fonts aren't proportional (8-bit
  is a fixed 8x8 grid; 16-bit is proportional at up to 32px). The
  `IView<CommanderStatusModel>` DI registration now branches on
  `IAssetLocator.Tier` via a new `IsEightBit(IServiceProvider)` helper,
  which exists specifically to keep `IAssetLocator`/`SystemTier` off
  every such registration's own CA1506 class-coupling count - inlining
  the check tripped `AddSplitConsoleScreens`'s limit on the first
  screen.
- The 8-bit layout uses one equipment column where 16-bit uses two:
  at 320px/8px-per-character the widest realistic string ("Front
  Military Laser", from a maxed-out laser loadout) already needs more
  than half the screen's width, and the column wrap point is
  positional rather than content-aware, so two columns wide enough for
  that string cannot fit side-by-side. Row pitch is tightened to 8px
  (no gap, matching the font exactly) so all four laser mounts plus
  every other upgrade - checked as the worst case, not just whatever a
  starting commander happens to show - fits above the scanner with
  only the single most extreme case landing a few pixels tight.
- Corrected a stale backlog claim in passing: "the 8-bit tier is
  already the maintainer's configured default" was checked against the
  actual `elite.sharp` config and found false (it's `SixteenBit`);
  verifying an 8-bit layout requires temporarily editing that file.

### Changed (Tier presentation follow-on decisions, 2026-07-30)

Documentation only - no code changes. Resolves the two items left after
the controller/view split, both of which were decisions blocking the
next implementation work rather than code to write.

- Dropped the "retire the pass-through controllers" step outright.
  `DockingView`/`HyperspaceView`/`LaunchView` are the only three left
  of the six it named, and all three draw through a shared
  `BreakPattern` that takes no model and is already resolution-
  independent - there is nothing to extract into a controller/model/
  view triple, so they stay plain `IScreenController` implementations.
- Surveyed every converted screen's `Draw` method and bucketed each
  one: absolute-layout screens that need a fresh 8-bit view (ten of
  them, including `PilotView` for one hardcoded line even though its
  view-name text is otherwise resolution-independent), derived-layout
  screens where a tier-specific spacing constant (or nothing at all)
  will do, and `GalacticChartView` as the strongest candidate for
  needing no second class at all, since its `ToScreen` conversion is
  entirely `Scale`/`Offset`-driven already. Full breakdown in the
  backlog.

### Changed (Controller/view split complete, the design-questions group, 2026-07-30)

- `Intro2`, the four cockpit windows and `PlanetData` each become a
  controller, a model record and a drawing-only view. Every
  behaviour-bearing screen is now split except `ShortRangeChart`, which
  stays deliberately out of scope.
- The `PilotView` family was never inheritance despite how the backlog
  described it - `PilotFrontView`/`PilotRearView`/`PilotLeftView`/
  `PilotRightView` were four near-identical classes composing their own
  instance of the same `PilotView`, differing only in a view-name
  string, which laser mount to draw and which starfield direction to
  scroll. Collapsed into one `PilotController` parameterised by a new
  `PilotDirection` enum, sharing one `IView<PilotModel>` registration
  across all four screens - the largest structural change so far,
  deleting four files rather than splitting one. `PopulateScreens`
  constructs the four `PilotController` instances directly
  (`CreatePilotController`), since a single `AddSingleton<PilotController>()`
  registration can only ever serve one screen.
- `Intro2` and `PlanetData` had none of the layout/content entanglement
  the backlog worried about: `Intro2View.Draw()` never touched the
  parade ship itself (drawn by the universe, like `Intro1`'s rolling
  Cobra), and `PlanetDataView`'s fixed y-coordinates never depended on
  which content was selected, unlike `ShortRangeChartView`'s row
  packing. Both converted the same mechanical way as every screen
  before them.
- `LaserDraw` (the crosshair/beam renderer, now constructed by
  `PilotView` instead of the old shared `PilotView` behaviour class)
  takes `RNG` for cosmetic beam jitter only, so it stayed on the view
  side rather than the controller, unlike this codebase's usual pattern
  of keeping `RNG` on the controller (e.g. `GameOverController`'s cargo
  scatter).
- New controller tests: `Intro2ControllerTests`, `PilotControllerTests`,
  `PlanetDataControllerTests`.

### Changed (Controller/view split, the text-entry group, 2026-07-30)

- `LoadCommander` and `SaveCommander` each become a controller, a model
  record and a drawing-only view. Sixteen screens are now split, with
  three items left on the backlog's list.
- Kept as two independent view classes rather than one shared view like
  `SettingsListView`: `SettingsListView`'s sharing came from code that
  was already one class before conversion, but Load and Save were
  always separate, and `SaveCommanderView` draws its name box with
  `DrawRectangle` at an explicit position where `LoadCommanderView`
  uses `DrawRectangleCentre` - each view keeps its own original call
  rather than assuming the two are pixel-identical.
- The "Press SPACE to continue." footer is hardcoded in both views
  rather than carried in the model, matching the existing precedent in
  `ThargoidMissionView`/`ConstrictorMissionView` for this exact
  recurring phrase. The screen-specific headline text ("Error Loading
  Commander!", "Commander Saved.", "Error Saving Commander!") is
  content and does go through the model.
- New controller tests type through the same `IKeyboard` fake the real
  input goes through, which caught that typed input can only ever be
  upper-case (`HandleInput` only recognises `ConsoleKey.A`-`Z`, and
  there is no lower-case equivalent) - the first draft of the tests
  used mixed-case names and silently failed to type the lower-case
  letters.

### Changed (Controller/view split, the selection-cursor group, 2026-07-30)

- `Options`, `Market`, `Equipment`, `Settings` and `EngineSettings` each
  become a controller, a model record and a drawing-only view.
  Fourteen screens are now split, with five items left on the backlog's
  list.
- `SettingsListController`/`SettingsListView` replace the old abstract
  `SettingsListView`: the abstract base keeps the shared cursor and
  navigation, and one `SettingsListView` (now drawing-only) is shared by
  both `SettingsController` and `EngineSettingsController` via a single
  `IView<SettingsListModel>` registration - the first split screen where
  a view is shared across controllers rather than paired 1:1. The two
  concrete files are renamed to match (`SettingsController.cs`,
  `EngineSettingsController.cs`); their existing behaviour tests moved
  with them (`SettingsControllerTests`, `EngineSettingsControllerTests`,
  `SettingsControllerFixture`), otherwise unchanged.
- `MarketController`'s cursor bug is fixed: `StockType` starts at 1
  (`Food`), but the cursor was a plain `int` clamped to `[0, Count-1]`,
  so reset left nothing highlighted (position 0 is `StockType.None`)
  and the last row (`AlienItems`) could never be reached. The clamp is
  now `[1, Count]` and `Reset` starts on `StockType.Food`;
  `MarketControllerTests` covers the first and last row.
- Padding/alignment format specifiers stayed in the views even where the
  underlying number is content, e.g. `MarketRow.ForSaleQuantity` and
  `MarketModel.Cash` are raw numbers because the original right-aligned
  them to a fixed column width - a layout concern. `EquipmentModel.Cash`
  by contrast is a fully-formatted string, matching
  `CommanderStatusModel.Cash`'s precedent, because the original drew it
  as one indivisible line with no alignment trick.
- DI registrations for split screens moved to their own class,
  `EliteSplitScreensServiceCollectionExtensions`, and gained a third
  method, `AddSplitMenuScreens`: adding the five screens first tripped
  the per-method CA1506 limit again, and then - once corrected - tripped
  the *class*-level CA1506 limit on `EliteServiceCollectionExtensions`
  itself (96 coupled types), which the new class resolves since the
  metric is computed per class.
- New controller tests: `OptionsControllerTests`,
  `MarketControllerTests`, `EquipmentControllerTests`.

### Changed (Controller/view split, the mechanical three, 2026-07-30)

- `GameOver`, `EscapeCapsule` and `ConstrictorMission` each become a
  controller, a model record and a drawing-only view. Nine of the
  eighteen behaviour-bearing screens are now split — half way.
- The animation counters the backlog named are now controller state,
  and renamed `_i` to `_tick` on the way: a controller that no longer
  draws has no loop for `_i` to belong to. `EscapeCapsule` publishes
  `IsAlertVisible` rather than the count, so neither tier's view owns a
  timer and both stop showing the alert on the same tick.
- `ConstrictorMission` mirrors `ThargoidMission` exactly, down to
  keying the view's layout off the model's stage. Which of the two
  second paragraphs the galaxy selects is now the controller's
  decision, so it is testable without a renderer.
- `AddSplitScreens` split into `AddSplitConsoleScreens` and
  `AddSplitSequenceScreens`. Adding the three tripped CA1506 at 49
  coupled types, which is what the method's own comment had predicted;
  the halves leave headroom for the nine screens still to convert.
- `ConstrictorMissionControllerTests` covers the stage selection, both
  galaxy variants and the debrief bounty; `EscapeCapsuleControllerTests`
  pins the alert lapsing on tick 90 rather than 89.

### Changed (Controller/view split, five more screens, 2026-07-29)

- `Quit`, `ThargoidMission`, `Intro1`, `CommanderStatus` and
  `Inventory` each become a controller, a model record and a
  drawing-only view, joining the galactic chart below. Six of the
  eighteen behaviour-bearing screens are now split.
- `CommanderStatus` is the archetype: its ratings table, condition,
  legal-status banding and equipment list move to the controller,
  leaving the view with the x=16/x=200 label columns that the 8-bit
  layout will re-author. `Inventory` and `CommanderStatus` have no
  input of their own but plenty of formatting, which is why they were
  converted here rather than left as drawing-only screens — otherwise
  each tier's view would re-derive the same strings.
- Every screen draws through `IView<TModel>`. A parameterless `IView`
  was added for screens with input but no data and removed the same
  day: two screens needed it at once and a single registration can only
  serve one, so the second would have silently displaced the first. A
  screen's fixed wording is content rather than layout, so it belongs
  in a model like everything else.
- `ShortRangeChart` is deliberately not converted. Its `Reset`
  entangles text-row packing with which planets are drawn at all, and
  blob size depends on `CarryFlag`, a side effect of the name
  generation that only runs when a name wins a free row. See the
  backlog for the decision and the shape to revisit it with.
- `CommanderStatusControllerTests` covers the rating bands, condition,
  legal-status bands, the witchspace blank and the equipment list -
  all without a renderer.

### Changed (Controller/view seam, galactic chart, 2026-07-29)

- First step of the tier presentation architecture: a screen's
  behaviour and its layout are now separable, so a per-tier view can
  vary the layout without duplicating the behaviour.
- `IView` (`Reset`/`Update`/`Draw`/`HandleInput`) is renamed
  `IScreenController`, and `IView<TModel>` is the new drawing-only
  contract. Because the old interface was already exactly the
  controller shape, the 27 unconverted screens satisfy
  `IScreenController` unchanged and needed no transitional wrappers.
- `GalacticChartView` is split into `GalacticChartController` +
  `GalacticChartModel` + a drawing-only `GalacticChartView`. The
  controller works entirely in galaxy space - the raw (D, B) of a
  `GalaxySeed`, 0-255 per axis - and the view applies `Scale`/`Offset`,
  so nothing tier-specific remains on the behaviour side.
- `GameState.Cross` is gone. Chart cursor state was only ever read by
  the two chart screens, and storing it in screen pixels forced the
  round-trip through `Scale` that kept the behaviour tier-coupled. Each
  chart now owns its own cursor; the galactic chart's is in galaxy
  space, and `ShortRangeChartView` keeps a screen-space copy until it
  is converted.
- Cursor bounds move with it: the old 512-space clamps (x 1-510,
  y 37-293) become galaxy-space constants that reproduce them exactly
  at the 16-bit tier. At the 8-bit tier they are a deliberate change -
  the old screen-space clamps mapped to roughly twice the galaxy's
  extent there, letting the cursor leave the galaxy, and the step size
  was likewise doubled. Both now match the 16-bit tier.
- `GalacticChartControllerTests` covers the cursor stepping and
  clamping, the find-by-name prompt and the caption/detail formatting -
  none of it needing a renderer, which is the point of the model.

### Added (Config-driven logging, 2026-07-29)

- `engine.logging` gains `minimumLevel` and `retainedFileCount`,
  replacing the two values previously hardcoded in
  `GameApp.CreateSeriLogger`. The `ELITE_LOG_LEVEL`/`SCR_LOG_LEVEL`
  environment variables still override the level - the escape hatch
  for when the config file itself is what needs debugging.
- Solves the ordering problem note left on this item: reading the
  config needs a logger, but the real logger needs the config's
  retained-file-count to be built correctly. `GameApp.Run` now reads
  the engine settings through a throwaway console-only bootstrap
  logger (whose level is already fully known from the environment
  variable alone) before building the one real logger the rest of the
  app uses, rather than building the real logger first and only
  patching its level afterwards.
- `GameApp.Run` takes a `readEngineSettings` delegate alongside
  `buildServices`, and `buildServices` now receives the already-read
  `EngineConfigSettings` instead of reading them again itself - both
  games' `SDLProgram.BuildServices` shrink accordingly.

### Added (Window scale, 2026-07-29)

- `engine.windowScale` magnifies the window without changing what is
  drawn: an integer, defaulting to 1, that both games multiply their
  render resolution by when creating the window. At scale 2 Elite's
  512x512 fills a 1024x1024 window with each pixel doubled, not with
  more of the scene. It is deliberately independent of `tier` — the
  tier chooses which machine's art and resolution the game reproduces,
  the scale only how large that appears on a modern display.
- The framebuffer stays native at any scale. Both backends already
  composed each frame into a fixed-size texture and blitted it to the
  window on every present, so the magnification is
  `SDL_SetRenderLogicalPresentation` in integer-scale mode on the
  renderer plus nearest-neighbour sampling on the two presented
  textures. Every drawing coordinate above that is logical-space and
  unchanged, so no game code knows the window size — and because
  logical presentation applies only when drawing to the window,
  `SDLGraphics`' render target and `SaveScreen` still work at the
  native resolution.
- Out-of-range scales are repaired like every other engine setting:
  zero, negative, or past 4 (larger than any display the game could be
  shown on) goes back to 1 rather than failing at startup.

### Added (Useful.App: one shared composition root, 2026-07-29)

- The two `SDLProgram.cs` files were about half duplicate: `Main` and
  `CreateSeriLogger` were identical between the games apart from four
  strings. They move to `GameApp.Run` in a new `Useful.App` assembly,
  which each `Main` now calls with its title, log file name, log-level
  environment variable and its own `BuildServices`. Everything left in
  the two files is genuinely game-specific.
- `Useful.App` is the app layer as an assembly rather than as a pair of
  entry points: it is the only place Serilog is referenced now (the two
  app projects drop their four Serilog package references and their
  duplicate `LogMessages`), and no game or engine library references it.
  `docs/architecture-principles.md` is updated to say so.
- `BuildServices` collapses too: the shared preamble - logger factory,
  the backend the config selects, the graphics/sound/keyboard it
  exposes, and the tier's asset locator - becomes
  `AddGameEngine(engine, width, height, title, loggerFactory)` in
  `Useful.App`. Each game's `BuildServices` is now its resolution, that
  one call, and its own three or four registrations. Stunt Car Racer
  gains `AddScrMain()` so its game registrations live in its own library
  as Elite's already did, rather than being spelled out in the app.
- Games gain `IGameApp` alongside `IGame` - `Run` for the composition
  root, `Update`/`Draw` for `GameHost`, kept as separate interfaces
  because they are separate roles.
- `Main` returns an exit code instead of the host calling
  `Environment.Exit`, so the container disposes its singletons on the
  way out of a failed start rather than the process vanishing mid-stack.
  This is what the principles already asked for; it only became
  enforceable once the code moved somewhere the analyser checks it.

### Changed (Engine settings read once, shared, 2026-07-29)

- Each game had its own `ReadBackend`, `ReadSystemTier` and
  `ReadWindowScale` for the composition root to call before the DI
  container exists, six methods with the same body between them - and
  each one opened, bound, repaired and possibly rewrote the whole config
  file for a single value. They collapse to one `ReadEngineSettings` per
  game returning the whole `EngineConfigSettings`, over a shared
  `EngineConfigReader.Read<TConfig>` in `Useful.Abstraction`. Startup now
  reads the file once instead of three times, and a new engine setting no
  longer means a new method in both games.
- `ConfigSettings<TGameSettings>` gains a non-generic `ConfigSettings`
  base holding the version, the engine settings and `Repair`. It is what
  lets the shared reader name a config type without knowing the game's
  own settings type; games still derive from the generic one.

### Changed (Stunt Car Racer selects its asset tier, 2026-07-29)

- Stunt Car Racer reads `engine.tier` from its config and builds its
  `AssetLocator` from it, as Elite already did. The tier was shared
  config the game simply ignored: every asset came from the default
  16-bit set whatever the file said.
- `ScrPalette` no longer creates an `AssetLocator` of its own. It takes
  an `IAssetLocator`, which the composition root supplies, so the
  palette follows the configured tier and the manifest is read once per
  run rather than once per construction. `StuntCarRacerMain` receives
  the locator alongside the abstraction to hand it on.
- The software backend now gets the same locator as the SDL one. It was
  falling through to the overload that makes its own, so under the
  default backend the tier would not have reached the assets even once
  it was read.
- Stunt Car Racer still ships only the 16-bit set, so selecting the
  8-bit tier fails at startup on the manifest's `Tiers` list rather
  than loading 16-bit art under an 8-bit label. The 8-bit assets
  themselves remain outstanding in the backlog.

### Changed (Config hardened before release, 2026-07-29)

- The file carries a `version` now. Nothing migrates yet - the point is
  that a later rename or restructure *can*, rather than being
  indistinguishable from a corrupt file. A file with no version reads
  as the current one, which is what every file written before this was.
- One value that cannot be honoured no longer costs the user the other
  twenty. `ConfigFile<T>`'s all-or-nothing `isValid` predicate is
  replaced by a `repair` hook that puts individual unusable values back
  to their defaults in place and reports whether it had to, and the
  settings either side of it survive. The limit is a value the JSON
  binder cannot parse at all (a misspelt enum name, a string where a
  number belongs) - that fails the whole bind, so the defaults still
  stand.
- A config file that is about to be rewritten as something other than
  what it held is first copied to `<name>.bad`, so a hand-edit that
  didn't parse is recoverable instead of being silently overwritten by
  the next settings change.
- Engine validation is shared. `EngineConfigSettings.Repair` checks the
  backend, tier, frame rate, graphic style and depth sort once for both
  games - Stunt Car Racer previously passed no validator at all, so a
  bad value there sat in the file until it broke something at startup.
- `polygonRenderMode` is renamed `depthSort` (as is the enum behind it,
  which never covered wireframe and so was never a "polygon render
  mode"), and `sound.musicOn` / `sound.effectsOn` become `sound.music`
  / `sound.effects` - inside a `sound` group the suffix said nothing.

### Added (A separate Engine Settings screen, 2026-07-29)

- The settings screen splits in two, along the same line as the config
  file. Game Settings keeps Elite's own (planet style, sun style,
  planet descriptions, instant dock); the new Engine Settings screen
  takes the shared ones (graphic style, depth sort, music, effects).
  Both are reached from the Options screen; the F-keys are left as the
  original had them.
- The backend and the tier are on it too, marked `*` against a
  "Applies when the game is restarted" footer: both are read before the
  container is built, so the screen saves the choice and the next
  launch acts on it rather than pretending it took effect.
- Music and effects had no way to change at runtime -
  `AudioController` held them in readonly fields - so they are settable
  properties now, and switching music off stops what is already
  playing. `StopMusic` no longer no-ops when music is already off,
  which is what made that possible.
- Both screens are one `SettingsListView` base: the two-column layout,
  the Back row, the navigation and the save-on-change behaviour were
  going to be duplicated otherwise.

### Changed (Engine settings grouped into graphics and sound, 2026-07-29)

- The `engine` element now groups what it holds: `engine.graphics`
  (`fps`, `graphicStyle`, `polygonRenderMode`) and `engine.sound`
  (`musicOn`, `effectsOn`), backed by new `GraphicsConfigSettings` and
  `SoundConfigSettings` types.
- `graphicsBackend` and `tier` stay at the top of `engine`, because
  neither is graphics-only: the backend picks the mixer as well as the
  rasteriser, and the tier picks an asset set that includes the music
  and effects. `graphicsBackend` is renamed to `backend` to stop the
  name claiming otherwise - the `GraphicsBackend` enum becomes
  `Backend`, and both games' `ReadGraphicsBackend` becomes
  `ReadBackend`.
- The shared `engine` block is documented in the main
  [readme](README.md#configuration); each game's readme now documents
  only its own `game` block and links there for the rest.

### Changed (One graphic style for the whole world, 2026-07-29)

- `shipWireframe` and `laserWireframe` are replaced by a single engine
  setting, `graphicStyle` (`Wireframe` or `Solid`). The picture can no
  longer end up half outlines and half filled faces, and it is one
  choice rather than one per object type. `shipRenderMode` moves to the
  engine as `polygonRenderMode`, since it is a property of the
  rasteriser rather than of Elite.
- The per-object styles are now what they always meant: `planetStyle`
  is `Solid | Striped | Fractal` and `sunStyle` is `Solid | Gradient`,
  and both only apply when `graphicStyle` is `Solid`. `PlanetType` loses
  its `Wireframe` member - a wireframe world picks `WireframePlanet` for
  every planet, whatever the style says.
- New `WireframeSun`, since there wasn't one: a plain white filled disc,
  the same size as the filled suns but with none of the flare they
  scatter round the edge. Filled rather than outlined, because an
  outlined circle reads as a planet.
- The Settings screen follows: "Ship Style" and "Laser Style" collapse
  into one "Graphic Style" row, and changing it rebuilds both the planet
  and the sun so the switch shows on the next frame.

### Changed (Config tidy-up: new location, name and shape, 2026-07-29)

- The shared user-data folder is now `The Sharp Kind` rather than
  `TheSharpKind` (`%AppData%\The Sharp Kind` on Windows,
  `~/.config/The Sharp Kind` elsewhere), and the config files are
  `elite.sharp` and `stuntcarracer.sharp` instead of `elitesharp.cfg`
  and `stuntcarracersharp.cfg`. Nothing migrates the old files, so an
  existing install starts from defaults; saves and logs move with the
  folder.
- The config file is now two elements deep: the shared settings live
  under `engine` and each game's own under `game`, so the two can't
  collide as more games arrive. `BaseConfigSettings` is no longer a base
  class - it is `EngineConfigSettings`, a plain type held by the new
  `ConfigSettings<TGameSettings>` root, from which `EliteConfig` and
  `ScrConfig` derive. `GameState.Config` is that root, so the game's own
  settings are reached through `Config.Game.X` and the shared ones
  through `Config.Engine.X`.
- `Fps` moves from `EliteConfigSettings` to the engine settings,
  alongside `GraphicsBackend` and `Tier` - it's a render-loop setting,
  not an Elite one.
- `IsViewFullFrame` is gone. It was get-only with no setter and nothing
  ever assigned it, so it read `false` forever - which made
  `EliteDraw.Bottom`'s `ScreenHeight - BorderWidth` branch dead code,
  while the property itself still turned up in the written file.
- Property names are written in camelCase (`"fps"`, `"instantDock"`),
  matching normal JSON style. Reads are case-insensitive, so a
  hand-edited file in either casing still binds.

### Fixed (3D projection zoom split from the view centre, 2026-07-29)

- Elite's world-to-screen projection was written as
  `((x * 256 / z) + (Centre.X / 2)) * Scale`, which only recovers the
  true view centre when `Scale` is exactly 2. At the 8-bit tier
  (`Scale` 1) everything 3D was centred a quarter of the way across the
  screen instead of the middle — the reason 8-bit ships, planets and
  suns sat up and to the left. It is now `Centre + (x * Focus / z)`,
  where `IEliteDraw.Focus` is the projection's focal length in pixels,
  derived from the tier's screen width (× 1.0). At 16-bit that is
  512 × 1.0, which reduces to the old maths exactly, so the 16-bit
  render is unchanged — verified by screenshot before and after.
- The five sites that inlined the old form are converted:
  `ShipBase.ProjectPoint`, `EliteDraw.ProjectExplosionPoints`,
  `PlanetRenderer.GetPlanetPosition`, `SolidSun.Draw` and
  `GradientSun.Draw`. The planet/sun radii and
  `WireframePlanet`'s detail threshold are in the same 256-wide space,
  so they follow `Focus / 256` rather than `Scale` — which keeps the
  field of view identical at every tier instead of tying it to a
  whole-number magnification.
- `Stars` had it too, which confined the whole 8-bit starfield to the
  top-left quadrant of the viewport. Star coordinates are held in the
  same 256-wide space, so they map to the screen through `Focus / 256`
  as well, and the star-space bounds that recycle a star off the edge
  are now the screen's half-extents divided back through that factor —
  so the starfield fills exactly the view at any tier. The four inlined
  copies of the mapping collapse into one `ToScreen` helper.
- `LaserDraw` carried the same mistaken construction for the crosshair
  centre, the beam target and the two laser mount points; all four now
  come off `Centre` / `ScannerLeft` / `ScannerRight` directly, so the
  8-bit crosshair is centred and the beams converge on it.
- `Scale` is left meaning coordinate/window magnification only, freeing
  it for the `WindowScale` work.

### Added (Elite runs at the 8-bit tier, 2026-07-28)

- `Tier` is a config setting (on `BaseConfigSettings`, so both games
  inherit it), read before the container exists the way `GraphicsBackend`
  already was. Elite's render resolution is derived from it rather than
  being its own setting — 320x256 for 8-bit, 512x512 for 16-bit — so the
  asset set and the resolution cannot disagree. Retires the "Get these
  from config" comment and the dead QHD block.
- `EliteDraw` no longer hardcodes the 16-bit HUD: the scanner's width and
  height come from the scanner bitmap, so each tier's art sets its own
  HUD size (8-bit 320x56 against 16-bit 512x129).
- Bitmap fonts support two sheet shapes. The 16-bit sheets are
  proportional — glyphs run 4 to 17 pixels wide, terminated by magenta
  markers — and the 8-bit BBC Micro sheet is monospaced 8x8 in a
  12-column grid, as the hardware it imitates was. `BitmapFont` carries
  its sheet's geometry instead of a static cell size, and the manifest
  carries it per font.
- A tier's manifest differences live in `AssetManifest.<Tier>.json`
  beside the base manifest, overlaid entry by entry, rather than a
  `TierOverrides` block inside it.
- `Palette/EightBit/palette.json` maps the 29 colour names game code uses
  onto sixteen colours, and the palette now counts against the tier's
  colour cap — colours the game draws with have to be colours the tier
  can show, whether they arrive as pixels or as a name.
- `SoftwareGraphics.DrawImage` clipped nothing: `DrawPixel` only tests
  bounds while a clip region is set, so an image landing partly
  off-screen wrote outside the framebuffer and threw. Bounds are now
  clamped once per image rather than per pixel. 16-bit never hit this
  because nothing there drew off-screen.
- Elite boots, renders and plays at 320x256 with the 8-bit asset set.

### Added (First 8-bit Elite bitmaps, 2026-07-28)

- `Assets/Images/EightBit/` gains `scanner.bmp` and the four laser
  bitmaps. `scanner.bmp` is a 4bpp indexed BMP with a 10-entry palette
  and a 94-byte data offset — a file the old `BitmapReader` would have
  rejected outright on both counts, and the first real asset to exercise
  the broadened decoder.
- The set is 11 distinct opaque colours against the 8-bit cap of 16: the
  lasers reuse the scanner's palette apart from the mining laser's
  purple.
- The tier is **not** declared in the manifest yet. Nine images, a bitmap
  font pair and a palette are still missing, and `AssetLocator`'s
  fallback now points at the empty flat `Images/` folder, so declaring
  `"EightBit"` would produce a tier that cannot load. See the backlog for
  what remains.

### Changed (Posterised the over-budget 16-bit assets, 2026-07-28)

- `font2.bmp` (both games' copies, byte-identical) and SCR's `atlas.bmp`
  are quantised to the 12-bit RGB space real 16-bit hardware used —
  each channel keeps its high nibble and replicates it, so 0xFF stays
  0xFF and 0x00 stays 0x00. `atlas.bmp`'s alpha is snapped to 0 or 255
  at a threshold of 128, clearing all 2765 of its partial-alpha pixels.
  Both were anti-aliased modern-quality art sitting in the 16-bit slot;
  no other asset needed touching.
- Elite's set drops from 2481 distinct opaque colours to 145, SCR's from
  5095 to 349, both against the 4096 cap. Neither game looks different:
  the quantisation is invisible at these palette sizes, confirmed by
  smoke-testing the Elite intro, status screen and front view, and SCR's
  menu, track preview and in-race cockpit.
- With both games compliant, `AssetSet`'s validation is no longer
  warn-only: an over-cap set, or any pixel whose alpha is neither 0 nor
  255, now fails at startup with the per-asset breakdown logged first so
  the message names the files to look at.

### Added (Eager asset loading with colour-budget validation, 2026-07-28)

- `AssetSet` decodes every image and bitmap font for the active tier once
  up front and hands both graphics backends the same `FastBitmap`
  instances. `SoftwareGraphics` decoded via `BitmapReader` while
  `SDLGraphics` decoded via `SDL_LoadBMP`, so the two backends could
  disagree about a file and the SDL one bypassed the managed decoder
  entirely — including any validation attached to it. `SDLGraphics` now
  uploads textures from the decoded bitmaps, setting the blend mode
  explicitly since `SDL_CreateTexture` (unlike
  `SDL_CreateTextureFromSurface`) does not infer it from the alpha
  channel.
- `AssetColourBudget` counts distinct opaque colours across the union of
  a game's whole tier set and checks it against the tier cap (16 for
  8-bit, 4096 for 16-bit). Fully transparent pixels are excluded; pixels
  whose alpha is neither 0 nor 255 are reported separately, since the
  renderer treats transparency as all-or-nothing. Bitmap fonts count too,
  even for the SDL backend that never draws with them — they are part of
  the tier's set.
- Warn-only for now, logged through each app's existing Serilog sinks
  with a per-asset breakdown at Information, until the over-budget assets
  are posterised. Running SCR confirms it end to end: 5095 colours
  against the 4096 cap, `Atlas` 2676 and `Large` 2431, plus 2765
  partial-alpha pixels. Elite's 2481 passes silently.
- `IAssetLocator` gains `Tier` so the validator knows which cap applies.
  `AssetSet` lives in `Useful.Graphics` rather than `Useful.Assets` — it
  holds `FastBitmap`s, and putting it in `Useful.Assets` would invert
  that project reference.

### Added (Per-tier asset resolution, 2026-07-28)

- `SystemTier` (`EightBit`, `SixteenBit`) and tier resolution in
  `AssetLocator`: the tier-varying categories resolve to
  `<Category>/<Tier>/<file>` and fall back to `<Category>/<file>`, which
  is what keeps the tier-neutral categories — audio, models, TrueType
  fonts, tracks — from needing a copy per tier. The tier is fixed at
  construction, so `IAssetLocator`'s members and every consumer of them
  are unchanged.
- `AssetManifest` gains `Tiers`, so asking for a tier a game ships no
  assets for fails at construction rather than silently falling back at
  first draw, and `TierOverrides` for the cases where a tier's set uses a
  different filename for the same logical name.
- Both games' images, bitmap fonts and palette moved into `SixteenBit/`
  subfolders, and both manifests declare that tier. No behaviour change:
  the full suite passes and both games render identically.
- `ScrPalette` and `FakeAssetLocator` each built the palette path by
  hand, bypassing `AssetLocator` entirely — the move broke both.
  `ScrPalette` now goes through `AssetLocator` so tier resolution stays
  in one place; `FakeAssetLocator` still builds a plain string, since its
  consumers include tests with no asset manifest to read. `ScrPalette`
  constructing its own locator remains a composition-root violation and
  is now recorded in the backlog.

### Added (BMP and PNG image decoding, 2026-07-28)

- `ImageReader.Read` is the new single entry point for loading image
  assets, picking the decoder from the file's magic bytes rather than its
  extension. `SoftwareGraphics` now loads images and bitmap fonts through
  it.
- `BitmapReader` was a reader of one specific 32bpp export — it rejected
  every other bit depth and assumed pixel data began at a fixed offset of
  150 bytes. It is now a real BMP decoder: 1/4/8bpp palettised, 24bpp BGR
  and 32bpp BGRA, the header's declared data offset, rows padded to a
  4-byte boundary, and top-down images with a negative height. Compressed
  and BITMAPCOREHEADER files are rejected with a specific message rather
  than decoded wrongly. 32bpp is still read as BGRA rather than honouring
  BI_BITFIELDS masks, which is what every committed asset and
  `BitmapWriter` use.
- `PngReader` decodes non-interlaced PNGs of every colour type and bit
  depth, on `System.IO.Compression.ZLibStream` rather than a third-party
  imaging library, so the Software backend and the headless tests keep
  working. 16-bit samples truncate to their high byte; `tRNS`
  transparency is honoured for palettised, greyscale and truecolour
  images; interlaced files are rejected outright. Chunk CRCs are not
  verified.
- Both decoders are covered by tests building files in memory — bit
  depths, row padding, row order, palette transparency, all five PNG
  scanline filters, and the rejection paths.

### Changed (Glob the asset lists in both game projects, 2026-07-28)

- `EliteSharpLib.csproj` listed all 118 of its assets as individual
  `<None Update>` entries, and `StuntCarRacerSharpLib.csproj` another 32.
  Both are now a single `Assets\**\*` glob, so adding the per-tier asset
  folders from [asset-structure.md](docs/asset-structure.md) needs no
  project-file churn.
- Elite keeps a second, narrower `Assets\SFX\*.wav;Assets\Music\*.ogg`
  entry marking the uncompressed sources `Never` — they stay in source
  control but out of the build output, as before. SCR copies its whole
  asset tree, so it needs no exclusion (its `.wav` files are the ones it
  actually plays).
- One behaviour change: `Assets\SFX\pulse.wav` was the only Elite `.wav`
  marked `PreserveNewest` while its 15 siblings were `Never`. Nothing
  reads it — the manifest maps `Pulse` to `pulse.ogg` — so the glob
  normalises it to `Never` and the file is no longer copied. Asset output
  is otherwise byte-identical for both games.

### Changed (Instant, auto-saved game settings, 2026-07-28)

- Every setting in `SettingsView` now takes effect on the next frame and
  is written to the config file as it's changed, so the "Save Settings"
  row is gone — the last row is now "Back", which just returns to the
  options screen.
- Ship Style previously needed a restart: `IPolygonRenderer` was chosen
  once at DI-registration time. The new `ConfigPolygonRenderer` holds
  the wireframe/painter/z-buffer strategies and picks one per frame from
  the live config.
- Planet Style and Sun Style previously only applied to objects created
  after the change. `Space.RefreshPlanetStyle`/`RefreshSunStyle` rebuild
  the current planet/sun in place, preserving position and orientation.
- `SpaceTests` covers the two refresh methods; new `SettingsViewTests`
  covers the auto-save and the Back row.

### Added (Laser style setting, 2026-07-28)

- The firing laser beams were drawn outlined or filled according to the
  ship-style setting. `EliteConfigSettings.LaserWireframe` now controls
  them independently, exposed as "Laser Style: Solid/Wireframe" in
  `SettingsView` alongside the ship style and persisted with the rest of
  the config.
- `LaserDrawTests` covers the two styles; `sdl-drive`'s key table gained
  the comma/period/slash keys, since the arrow keys are extended-key
  codes that `PostMessage` doesn't deliver to SDL and the settings
  screen's left/right needed a working alternative.

### Added (Per-laser crosshairs and beam colours, 2026-07-28)

- Every laser type drew the same white cross (issue #15). Each type now
  has its own crosshair sprite — `laser-pulse.bmp`, `laser-beam.bmp`,
  `laser-military.bmp`, `laser-mining.bmp` in `Assets/Images`, replacing
  the `laser-crosshairs.png` reference art — drawn centred on the view
  by `LaserDraw.DrawLaserSights`, and the firing beams are coloured per
  type instead of always red-orange: pale yellow for beam and bright
  purple for mining, matching their crosshair sprites, with pulse and
  military staying red-orange.
- `IGraphics.ImageSize` returns a loaded image's width and height, so a
  caller can centre an image on a point rather than only across the
  screen as `DrawImageCentre` does.
- `PilotView` now draws the firing beams alongside the crosshair rather
  than in `Draw()`, which doesn't know which laser is mounted.

### Fixed (Pilot views showed the front laser's crosshair, 2026-07-28)

- `PilotRearView`, `PilotLeftView` and `PilotRightView` all passed
  `_ship.LaserFront.Type` to `DrawLaserSights`, so every view drew the
  front laser's sights; `Combat.FireLaser` already picked the right
  laser per view, so only the visuals were wrong. Each view now passes
  its own laser, which the per-type crosshairs make visible.
- Added `LaserDrawTests` covering the crosshair drawn for each laser
  type and none drawn without a laser.

### Added (Wireframe planet equator, meridian and crater, 2026-07-28)

- `WireframePlanet` drew a bare circle. It now also draws the surface
  detail the original does (`PL9`, `PLS2`/`PLS22` in the BBC Micro
  source): either an equator and a meridian — two half ellipses sharing
  the planet's nose vector, drawn against its roof and side vectors,
  each starting at the `PLS4` angle `arctan(-nosev_z / other_z)` — or a
  crater, a full ellipse of half the planet's radius offset 222/256 of
  the radius along the roof vector and hidden when that vector points
  away. As in the original (`SOS1`), the choice comes from bit 1 of the
  system's tech level, so `PlanetFactory.Create` now takes the tech
  level. No detail is drawn below a radius of 6.
- `WireframePlanet` defaults its pitch and roll to 127 as the original
  sets them, so it turns without damping and the surface detail sweeps
  round. Only this style spins: `SolidPlanet`, `StripedPlanet` and
  `FractalPlanet` map their surface from `Rotmat`'s rows through
  `PlanetRenderer`, which assumes `(M21, M22)` stays unit-length, so a
  turning matrix makes their landscape swim and pulse.
- Added `WireframePlanetTests` cases for the equator-and-meridian, the
  crater, and the crater hidden on the planet's far side.

### Changed (TrueType font sizes moved into the asset manifest, 2026-07-28)

- `SDLGraphics.LoadFont` switched on the game-specific font names
  "Small"/"Large" to pick point sizes 12/18, baking game data into the
  shared library and throwing for any other font name. The point size
  now travels with the asset: `FontsTrueType` manifest entries are
  objects (`{ "File": ..., "PointSize": ... }`), `IAssetLocator`
  exposes `FontTrueTypes` as `TrueTypeFontAsset` (path + point size)
  in place of `FontTrueTypePaths`, and `LoadFont` just opens the file
  at the given size. Both games' manifests keep their existing 12/18
  sizes, so rendering is unchanged.

### Fixed (Universe.RemoveShip left Planet/StationOrSun dangling, 2026-07-28)

- `Universe.RemoveShip` only removed from `_objects`, so passing the
  planet or the station/sun left `Planet`/`StationOrSun` pointing at a
  removed object. Removal now mirrors `AddNewShip`'s routing and clears
  the matching reference. Station removal previously worked only by
  accident, because `Combat.RemoveShip` adds a sun that overwrites
  `StationOrSun` first; that path is unchanged.
- Added `UniverseTests` cases covering planet and station removal.

### Changed (ShipBase draw buffer reuse, 2026-07-28)

- `ShipBase.Draw` allocated a `new Vector4[100]` for every ship on every
  tick; the transformed-point buffer is now a per-instance field grown
  once to the model's point count.
- Deleted the discarded `_ = VectorMaths.UnitVector(...)` call in
  `ShipBase.Draw` — `UnitVector` is pure, so the call had no effect.
- `EliteDraw._pointList`'s magic `100` is now a named `MaxModelPoints`
  constant (it bounds model points, unrelated to the renderers'
  `MAXPOLYS`).

### Fixed (lone-wolf ship models missing from the manifest, 2026-07-27)

- `CobraMk3Lone` and `PythonLone` have no `Models` entry in
  `AssetManifest.json` — they are subclasses of `CobraMk3`/`Python`
  that only change flags, bounty and loot, and share the parent's mesh,
  so there is no model file of their own to list. `ShipFactory` built
  its prototypes straight from the manifest, so neither existed and
  `CreateLoneWolf` threw an unhandled `EliteException` on 2 of its 5
  rolls — `Combat.CreateLoneWolf` has no catch, so a lone-wolf
  encounter could take the game down.
- `ShipFactory.Create` now iterates its own ship table rather than the
  manifest, resolving each ship's model through an `s_modelNames`
  variant→parent map, and skipping ships the manifest supplies no model
  for. Manifest keys that name no known ship still throw.
- Added `ShipFactoryTests.CreateLoneWolfPicksShipByRoll`, which builds
  the factory from the real `AssetLocator` so a missing manifest entry
  for any lone wolf fails the build.

### Changed (ShipFactory: explicit factory table instead of reflection, 2026-07-27)

- `ShipFactory.CreateShipFromName` resolved ship classes from the
  `AssetManifest.json` model names via `Type.GetType` plus a non-public
  `Activator.CreateInstance` (with an `S3011` suppression and two
  TODOs). It now looks the name up in a static
  `Dictionary<string, Func<IEliteDraw, RNG, IShip>>` of explicit
  constructor lambdas covering all 33 ship types, so a bad manifest
  entry is a plain lookup miss rather than arbitrary type activation,
  and the ship constructors stay `internal`.
- Added `ShipFactoryTests.CreateUnknownModelNameThrowsEliteException`
  for the miss path. Full solution builds, full test suite green
  (437 tests), Elite smoke-tested live: intro ship parade and front
  view render as before.

### Changed (streaming texture for the depth-layer composite, 2026-07-27)

- `SDLGraphics.FlushDepthLayer` created an `SDL_Surface` and an
  `SDL_Texture` from the CPU depth layer and destroyed both on every
  flush — a GPU allocation and a synchronous upload for every frame
  drawing depth-tested geometry, which is every frame in SCR and every
  ZBuffer frame in Elite. It now uses one persistent
  `SDL_TEXTUREACCESS_STREAMING` texture, created alongside the layer in
  `ClearDepth` and re-uploaded with `SDL_UpdateTexture`.
- The new texture sets `SDL_BLENDMODE_BLEND` explicitly:
  `SDL_CreateTextureFromSurface` inferred alpha blending from the
  surface's pixel format, but `SDL_CreateTexture` does not, and the
  depth layer is transparent everywhere nothing was rasterised — left
  at the default it would have painted an opaque rectangle over
  everything drawn before the flush.
- Sibling of the framebuffer-blit change below; same fix, same file
  pair. Full test suite green, both apps smoke-tested live on the
  Hardware backend (Elite's depth-tested intro ship; SCR's track
  preview, where the sky and scenery drawn before the depth pass stay
  visible around the track — the check that blending survived).

### Changed (streaming texture for the software framebuffer blit, 2026-07-27)

- `SoftwareAbstraction.SoftwareScreenUpdate` created an `SDL_Surface`
  and an `SDL_Texture` from the CPU framebuffer on every presented
  frame, then destroyed both — a GPU allocation and a synchronous
  upload per frame. It now creates one
  `SDL_TEXTUREACCESS_STREAMING` texture in the constructor and
  re-uploads the pixels with `SDL_UpdateTexture`; the texture is
  destroyed in `Dispose` before the renderer.
- The sibling half of the backlog item (`SDLGraphics.DrawImage`/
  `DrawImagePart` creating a texture per call) was already fixed —
  `_imageTextures` is built once in `SDLGraphics.Create`.
- No behaviour change: full test suite green, both apps smoke-tested
  live on the Software backend (Elite's intro, ship parade and front
  view; SCR's track menu and track preview).

### Changed (`Scale` moved out of `IGraphics`, 2026-07-27)

- `IGraphics.Scale` (hardcoded `2` in both backends) was Elite's
  coordinate scale leaking into the shared graphics library — no other
  consumer read it. It is now `IEliteDraw.Scale`, and Elite's ~50 call
  sites go through `_draw.Scale` instead of `_draw.Graphics.Scale`.
- `DrawRectangleCentre` centred with `(ScreenWidth - width) / Scale` in
  both `SoftwareGraphics` and `SDLGraphics`, which was correct only
  because `Scale` happened to equal `2`; it now divides by `2`
  explicitly.
- `SDLGraphics` divided positions by `(2 / Scale)` in ten places — an
  exact no-op at `Scale == 2` — removed.
- `Scale` dropped from `FakeGraphics`, SCR's `RecordingGraphics` (which
  returned `1` and was never read), the `SoftwareGraphics` benchmark and
  the two test assertions covering it. `FakeEliteDraw` gained it.
- No behaviour change: full test suite green, both apps smoke-tested
  live (Elite's intro, charts and front view; SCR's track menu, preview
  and race HUD).

### Closed (issue #5, "Measure/improve code complexity", 2026-07-27)

- The whole solution was audited with each candidate rule raised to
  `warning` under `TreatWarningsAsErrors=false`, counts deduplicated by
  file/line (multi-targeting reports each site twice):

  | Rule | Violations | Outcome |
  | --- | --- | --- |
  | `CA1502` cyclomatic complexity | — | Enabled at `warning` |
  | `CA1506` class coupling | — | Enabled at `warning` |
  | `S1451` license headers | 333 → 0 | Fixed and enabled |
  | `S2234` argument order | 4 → 0 | Fixed and enabled |
  | `S2583` unreachable code | 0 | Enabled |
  | `S1541` method complexity | 61 (all prod) | Left `none` |
  | `S3776` cognitive complexity | 41 (40 prod) | Split methods, then enabled |
  | `S107` parameter count | 20 (19 prod) | Left `none` |
  | `S109` magic numbers | 4087 (4085 prod) | Left `none` |

- The rules left at `severity = none` are recorded under Won't in the
  [backlog](docs/backlog-roadmap.md): their remaining sites are
  concentrated in ported 6502/Amiga reference methods whose length and
  hardcoded constants are inherent to the source algorithms, and
  `CA1502`/`CA1506` already enforce a complexity ceiling.

### Added (`CA5394` enabled, 2026-07-27)

- Enabled `CA5394` (do not use insecure randomness), scoped off in the
  two assemblies that must reach `System.Random` directly rather than
  repo-wide:
  - `Useful` (its own `.editorconfig`) — `RandomSource` is the single
    wrapper every game and test goes through, deliberately the fast,
    seedable kind of randomness rather than a cryptographic one.
  - `StuntCarRacerSharpLib.Tests` (its own `.editorconfig`) —
    `OpponentPhysicsTests` drives a seeded `Random` directly to assert
    the opponent's speed logic consumes the RNG stream exactly once per
    piece; comparing the raw stream *is* the assertion, so it cannot go
    through `IRandomSource`.
- The rule's stale "Disabled repo-wide" comment in the root
  [.editorconfig](.editorconfig) was corrected to match.

### Added (`CA1515` enabled, 2026-07-27)

- Enabled `CA1515` (consider making public types internal). Seven types
  could not be narrowed, each scoped off where it lives rather than
  repo-wide:
  - The BenchmarkDotNet classes in `src/elite/perf` and
    `src/useful/perf` (a new `.editorconfig` per `perf` folder) —
    BenchmarkDotNet compiles a generated runner into a separate assembly
    that references them, so they must stay public.
  - `RecordingGraphics` and `FakeAbstraction` in
    `StuntCarRacerSharpLib.Fakes` (its own `.editorconfig`) — a shared
    fakes library consumed by `StuntCarRacerSharpLib.Tests`. `CA1515`
    fires there only because `Microsoft.NET.Test.Sdk` generates an entry
    point, which makes the assembly look like an application; the Elite
    and Useful fakes projects do not reference it and stayed silent.
  - `SoftwareSoundTests.AudioAssetFixture`, folded into the `CA1034`
    `#pragma` already covering it — xUnit1000 requires a public test
    class and CS0051 then forces the fixture public too.

### Added (`S1451` license headers enforced, 2026-07-27)

- Enabled `S1451` (missing copyright/license header). Sonar rule
  parameters cannot be set from `.editorconfig` — only from a
  `SonarLint.xml` supplied as an `AdditionalFiles` item — so
  [SonarLint.xml](SonarLint.xml) is new at the repo root and wired into
  every project from [Directory.Build.props](Directory.Build.props). It
  matches the header by shape rather than by three literal strings
  (`// '[^']+' - Andy Hawkins \d{4}(-\d{4})?\.`), so it stays valid as
  years advance and covers all three header variants.
- Copyright years brought up to date in the `file_header_template`
  values and across all files: Elite `2023` → `2023-2026`, Useful
  `2025` → `2023-2026`. SCR keeps `2026`, and gained the
  `src/scr/.editorconfig` `file_header_template` it never had — until
  now `IDE0073` was not checking SCR's headers at all.
- Upstream attribution lines (C.J. Pinder / Bell & Braben for Elite,
  the remake and Crammond/MicroStyle/MicroProse lines for SCR) are
  unchanged; the multi-line templates still carry them.

### Added (Analyser audit; `S2234`/`S2583` enabled, 2026-07-27)

- Completed the code-quality-gate audit from issue #5: built the whole
  solution with `S107`, `S109`, `S1451`, `S1541`, `S2234`, `S2583` and
  `S3776` each at `warning` and `TreatWarningsAsErrors=false`, and
  recorded deduplicated per-rule counts in
  [backlog-roadmap.md](docs/backlog-roadmap.md) (multi-targeting reports
  every site twice, so raw warning totals are double the real figure).
  Headline: `S109` 4087, `S1451` 333, `S1541` 61, `S3776` 41, `S107` 20,
  `S2234` 4, `S2583` 0.
- Enabled the two correctness-flavoured rules outright, joining `CA1502`
  and `CA1506`: `S2583` (unreachable conditional code) was already clean,
  and `S2234`'s four sites were all false positives from deliberate
  coordinate swaps, cleared by renaming the parameters rather than
  suppressing — `PlanetRenderer.RenderPlanetLine`'s `x`/`y` become
  `offsetX`/`offsetY` (its four callers mirror the octants of Doros'
  circle algorithm on purpose), and `TrackRendererTests.Cross`'s
  `a`/`b`/`c` become `origin`/`first`/`second`. No behaviour change; the
  visual-dump tests cover the planet renderer.
- The remaining rules stay `none` with follow-up backlog items:
  complexity/parameter-count wants a tracked, ratcheted report rather
  than a hard gate, `S1451` is a mechanical 333-file decision, and
  `S109` is recommended against enabling at all.

### Fixed (Planets painting over the view border, 2026-07-27)

- The previous entry below claimed `WireframePlanet`/`FractalPlanet`/
  `PlanetRenderer` had "no reference to the view bounds at all" — that
  was wrong; `GetPlanetPosition`'s bounding-box cull and
  `RenderPlanetLine`'s per-pixel clip against `IEliteDraw.Left`/`Right`/
  `Top`/`Bottom` have existed since 2025-03-19 (`git blame`), well before
  issue #8. Live-testing found no bleed into the scanner console, but the
  user pointed at the actual bug: the planet's fill paints directly over
  the 1px-wide border line itself. Root cause: `EliteDraw.DrawBorder`
  ([EliteDraw.cs](src/elite/libs/EliteSharpLib/Graphics/EliteDraw.cs))
  draws its rectangle via `Graphics.DrawRectangle`, whose last-inclusive-
  pixel convention (`start + size - 1`) puts the border's own row/column
  one pixel inside of what `Right`/`Bottom` claim, while `Left`/`Top`
  (position-based, no such convention) already line up correctly with
  the border's near edge. Pixel-sampled a screenshot to confirm: the
  border's right column and bottom row both sit at `Right - 1`/
  `Bottom - 1`, one pixel short of where every consumer assumed. Fixed
  both places that derive a content boundary from `Right`/`Bottom`:
  `PlanetRenderer.RenderPlanetLine`'s manual clip (`s.Y >= Bottom`/
  `s.X < Right`, mirroring the already-correct `Top`/`Left` exclusions)
  covers `FractalPlanet`/`StripedPlanet`, and `EliteDraw.Width`/`Height`
  (feeding `SetViewClipRegion`'s graphics-level clip, `-1` each) covers
  `WireframePlanet`/`SolidPlanet`, which draw via `DrawCircle`/
  `DrawCircleFilled` with no manual bounds check of their own — and this
  same clip region is shared by ships and lasers, so it closes the same
  1px gap there too. `Right`/`Bottom`/`Bottom`-as-height stayed
  unchanged, since `DrawBorder` still needs them to draw the border in
  its current (correct) visual position; only the content-boundary
  consumers were tightened. Verified by pixel-sampling a live screenshot
  before and after: the border row was the planet's fill colour
  (`16,80,144`) before the fix and pure white (`255,255,255`, matching
  an unobstructed reference column) after. Full solution build and full
  test suite (436 tests) pass. Removed the backlog item this was split
  from, since its stated cause didn't hold up.

### Fixed (Enemy laser fire follows the ship's real firing direction, 2026-07-27)

- `ShipBase.DrawLasers` ([ShipBase.cs](src/elite/libs/EliteSharpLib/Ships/ShipBase.cs))
  drew each NPC's laser bolt from its mount point to `(Location.X > 0 ? 0
  : 511, _rng.Random(256) * 2)` — a screen-edge X picked by which side the
  ship was on, paired with a Y uniformly random over the whole view,
  ignoring the ship's actual firing angle, and hardcoded to a 512-tall
  view regardless of `ScannerWidth`/height. Replaced with a real
  direction-to-boundary projection: extend the ship's local vector from
  its origin through the laser mount out to a very large distance
  (`FarAimDistance`), project it through the same perspective transform as
  every other model point (factored into a new shared `ProjectPoint`
  helper) to get the on-screen vanishing point of the ship's real firing
  direction, add a small random spread (`LaserAimSpread`) so repeated
  shots still vary, then clip the ray to wherever it actually leaves the
  view rectangle (`IEliteDraw.Left`/`Right`/`Top`/`Bottom`, derived from
  screen size) via a new `ProjectToViewBoundary` helper. Investigated the
  reporter's second concern (planets not clipped to the scanner/viewport
  boundary either) — confirmed `WireframePlanet`/`FractalPlanet`/
  `PlanetRenderer` still have no reference to the view bounds, but that's
  a separate, unfixed concern (different draw path, not covered by this
  change) and remains open. Added
  `ShipBaseTests.DrawLasersProjectsAlongFiringDirectionAndClipsToViewBoundary`,
  which replicates the projection/clip math independently and asserts the
  drawn endpoint lands where the real trajectory exits the view rather
  than at a fixed screen edge. Verified with a full solution build, the
  full test suite (436 passing), and by launching `EliteSharp` through
  several undocked flight/combat sessions (firing the player's own laser,
  which shares the same `DrawPolygonFilled` draw path) with no crashes or
  rendering regressions; was unable to reproduce a live NPC-fired shot in
  the time available (ship encounters are probabilistic) to visually
  confirm this specific path beyond the unit test.

### Changed (Extract shared star-plot helper in Stars, 2026-07-24)

- `Stars.FrontStarfield`/`RearStarfield`/`SideStarfield`
  ([Stars.cs](src/elite/libs/EliteSharpLib/Stars.cs)) each repeated the
  same ~30-line block that plots a star at its current screen location
  (bounds-check, draw the 1-4 pixels sized by depth) before moving it;
  extracted into a private `PlotStar(int i)` helper returning the
  computed screen position, cutting ~60 duplicated lines. Behaviour is
  unchanged — same computation, same draw calls, same order. Verified
  with a full solution build, the full test suite, and by launching
  `EliteSharp` (ran cleanly through several seconds of frames, which
  exercise all three starfield draw paths, before being stopped).

### Removed (Delete stale build-output-only directories, 2026-07-24)

- Deleted `src/elite/apps/EliteSharp.SDL/`, `src/elite/libs/EliteSharp/`,
  `src/elite/test/EliteSharp.Tests/`, and
  `src/elite/perf/EliteSharp.Benchmarks/` — leftovers from earlier project
  renames, each containing only `bin`/`obj` output, already gitignored
  (never tracked), and referenced by no project or solution entry.

### Changed (Clean stale EliteSharpLib.csproj items, 2026-07-24)

- `EliteSharpLib.csproj` carried a `<Compile Remove="Controls\**" />` /
  `<EmbeddedResource Remove="Controls\**" />` / `<None Remove="Controls\**" />`
  block and fourteen `None Update="sfx\*.wav"` items pointing at a
  `Controls/` and a `sfx/` folder that no longer exist under
  `src/elite/libs/EliteSharpLib/`; removed both (confirmed neither path
  exists). Left the ~280-line hand-maintained `Assets\*` item list as-is —
  converting it to glob items is a separate, purely stylistic change with
  its own risk of altering per-extension `CopyToOutputDirectory` overrides
  (e.g. `.wav`/`.ogg` pairs under `Assets\SFX`/`Assets\Music` set
  `Never`/`PreserveNewest` differently), not needed to fix the stale
  references.

### Changed (Remove Vector4.Cloner(), 2026-07-24)

- `Vector4.Cloner()`
  ([MathsExtensions.cs](src/useful/libs/Useful/Maths/MathsExtensions.cs))
  new'd up a copy of a `Vector4`, but `Vector4` is a struct, so plain
  assignment already copies; replaced its four call sites in
  [Extensions.cs](src/elite/libs/EliteSharpLib/Extensions.cs) and
  [Space.cs](src/elite/libs/EliteSharpLib/Space.cs) with direct
  assignment and deleted the method. Dropped the now-unused
  `using Useful.Maths;` from `Extensions.cs`.

### Fixed (Remove debug log spam from DrawCircleFilled, 2026-07-24)

- `SoftwareGraphics.DrawCircleFilled`
  ([SoftwareGraphics.cs](src/useful/libs/Useful.Graphics/SoftwareGraphics.cs))
  left a `Debug.WriteLine($"{x},{y}")` in its scanline loop, logging every
  scanline of every filled circle (planets, suns) every debug-build frame;
  removed.

### Removed (Close stale SDLSound loop-pitch threading item, 2026-07-24)

- The backlog's "document and harden the `SDLSound` loop-pitch threading
  contract" item described the old SDL2_mixer `Mix_RegisterEffect`
  resampler, where a managed callback running on the audio thread read a
  pitch field the game thread wrote. The 2026-07-23 SDL2→SDL3 migration
  (see below) replaced that whole mechanism: `SDLSound.PlayLoop`
  ([SDLSound.cs](src/useful/libs/Useful.SDL/SDLSound.cs)) now calls
  `MIX_SetTrackFrequencyRatio` directly, a native SDL3_mixer API with no
  managed callback and no field shared across threads — confirmed no
  `Mix_RegisterEffect` usage remains anywhere in `src/`. Nothing left to
  document or harden, so the item is closed rather than actioned.

### Changed (Message the bare ShipFactory exceptions, 2026-07-24)

- The `throw new EliteException()` calls in `ShipFactory`'s `CreateLoneWolf`,
  `CreatePackHunter`, `CreatePirate`, and `CreateTrader` switch-expression
  defaults ([ShipFactory.cs](src/elite/libs/EliteSharpLib/Ships/ShipFactory.cs))
  carried no message, so a hit would be undiagnosable in a log; each now
  reports the offending roll (and, for the lone-wolf case, the computed
  index alongside the underlying `rnd`).

### Added (Track benchmark history in CI, 2026-07-24)

- Resolved the "how to record and monitor historical benchmark numbers"
  decision from the previous cleanup: added
  [.github/workflows/benchmarks.yml](.github/workflows/benchmarks.yml), a
  `workflow_dispatch`-only job (manual trigger — shared runners are too
  noisy to gate every push/PR on) that runs all three `*.Benchmarks`
  projects and records each benchmark class's results to the `gh-pages`
  branch via `benchmark-action/github-action-benchmark`, giving each class
  its own trend chart. Added `[JsonExporterAttribute.FullCompressed]` to
  `PlanetBenchmarks`, `SunBenchmarks`, `KeyboardBenchmarks`, and
  `SoftwareGraphicsBenchmarks` so BenchmarkDotNet emits the JSON report the
  action reads. Also fixed `EliteSharpLib.Benchmarks/Program.cs`, which ran
  `PlanetBenchmarks` twice instead of running `SunBenchmarks` too — found
  while wiring this up; `SunBenchmarks` had never actually executed.
  One-time manual step still needed once this merges: enable GitHub Pages
  (Settings → Pages → Source → `gh-pages` branch) after the workflow's
  first run creates the branch, to get a browsable dashboard. Built the
  full solution (all green); did not execute the new workflow itself,
  since it depends on the `gh-pages` branch/Pages state.

### Changed (Fix EliteSharp namespace rename leftover and stop committing benchmark reports, 2026-07-24)

- `LogMessages.cs` was still in namespace `EliteSharp.SDL` from before the
  `EliteSharp.SDL` project was renamed to `EliteSharp` (`SDLProgram.cs` had
  already been fixed); moved it to `EliteSharp` and dropped the now-unused
  `using EliteSharp.SDL;` from `SDLProgram.cs`.
- Removed the committed BenchmarkDotNet report files under
  `src/elite/perf/EliteSharpLib.Benchmarks/reports/`,
  `src/useful/perf/Useful.Controls.Benchmarks/reports/`, and
  `src/useful/perf/Useful.Graphics.Benchmarks/reports/` — generated
  artifacts per the architecture doc's solution-hygiene rule — and added
  `src/*/perf/*/reports/` to `.gitignore` so they don't get recommitted.
  How to record and monitor historical benchmark numbers over time (e.g. a
  CI job that posts results to a PR comment) remains an open decision, not
  addressed here. Built the full solution and ran the complete test suite
  (all green).

### Changed (Pin FastBitmap pixel arrays lazily, 2026-07-24)

- `FastBitmap` pinned its pixel array with a `GCHandle` in the constructor
  unconditionally, even though `BitmapHandle` (the only reader of the pin) is
  never touched by short-lived bitmaps such as the LRU text-glyph cache
  ([SoftwareGraphics.cs](src/useful/libs/Useful.Graphics/SoftwareGraphics.cs))
  or the intermediates `FastBitmap.Resize` creates — only the screen
  framebuffer and SDL's depth-layer bitmap ever cross into native code.
  `BitmapHandle` now allocates the pinned `GCHandle` on first access instead
  of in the constructor, and `Dispose` only frees it if it was ever pinned,
  so bitmaps that never leave managed code no longer permanently pin (and
  fragment the GC heap with) their backing array. Built the full solution
  and ran the complete test suite (all green).

### Changed (ScreenManager.Current no longer nullable, 2026-07-24)

- `ScreenManager<TId, TScreen>.Current` was `TScreen?`, forcing every
  post-setup call site (`EliteMain.Update`, `StuntCarRacerMain.Update`/`Draw`,
  `GameState.CurrentView`) to use the null-forgiving operator. `Current` is
  now `TScreen` and throws `InvalidOperationException` if read before `Set`
  has been called at least once — both games already call `Set` during
  construction/init, before their update loops run, so this only changes an
  unreachable silent-null path into a diagnosable exception.
  `ScreenManager.CurrentId` is unchanged (`default!` on an unset enum ID is
  harmless — it's only ever compared, never dereferenced).

### Added (Test coverage badge on README, 2026-07-24)

- The CI coverage step turned out to be silently broken: `Directory.Build.props`
  sets `DebugType=none`/`DebugSymbols=false` for Release builds, so the
  `dotnet-coverage collect` + `coverage.runsettings` pipeline had no PDBs to
  map hits to source lines and had been uploading an empty/all-zero
  `coverage.cobertura.xml` artifact (its module-include filter also never
  covered `StuntCarRacerSharpLib`). Replaced it in
  [build-and-package.yml](.github/workflows/build-and-package.yml) with
  `dotnet test --collect:"XPlat Code Coverage"` (coverlet, already referenced
  by every test project) run with `-p:DebugType=portable -p:DebugSymbols=true`
  for that build+test pass only, merged across all test projects and rendered
  to an SVG with ReportGenerator (`-assemblyfilters` drops `*.Tests`/`*.Fakes`/
  `*.Benchmarks` so the number reflects shipped code, both games included).
  The CI job now commits the refreshed badge to
  [docs/images/coverage-badge.svg](docs/images/coverage-badge.svg) on pushes
  to `master` (`[skip ci]`, needs `permissions: contents: write`), and
  README.md displays it. Removed the now-unused `coverage.runsettings`.
  Real numbers as of this change: 70.8% line coverage.

### Changed (Adopt central package management, 2026-07-24)

- All 26 `.csproj` files pinned `PackageReference` versions independently.
  Added `Directory.Packages.props` at the repo root with
  `ManagePackageVersionsCentrally` and one `PackageVersion` per package
  (29 total), and dropped the `Version` attribute from every
  `PackageReference` in the tree. The five analyzer packages
  (`Roslynator.Analyzers`, `Roslynator.CodeAnalysis.Analyzers`,
  `Roslynator.Formatting.Analyzers`, `SonarAnalyzer.CSharp`,
  `StyleCop.Analyzers`) were copy-pasted with identical
  `PrivateAssets`/`IncludeAssets` settings into every project, so those
  moved into [Directory.Build.props](Directory.Build.props) instead,
  alongside the existing `MinVer` reference (now also version-less).
  No package versions changed. Verified with a full solution build, the
  full test suite, and a smoke launch of both `EliteSharp` and
  `StuntCarRacerSharp`.

### Fixed (FractalPlanet landscape generation wasn't deterministic per system, 2026-07-24)

- `FractalPlanet.GenerateLandscape`
  ([FractalPlanet.cs](src/elite/libs/EliteSharpLib/Planets/FractalPlanet.cs))
  seeded a local `Random(seed)` for the corner grid but drew
  `CalcMidpointColor`'s midpoint-displacement jitter from the shared
  game-wide `RNG`, so a planet's fine detail differed between visits to
  the same system depending on how much other game activity had advanced
  the shared RNG in between — unlike the reference
  `generate_fractal_landscape(rnd_seed)` (`fesh0r/newkind`'s `threed.c`),
  which reseeds one stream for the whole landscape so revisiting a system
  always renders a byte-identical planet. Both the corner grid and the
  jitter now draw from the one seeded `RandomSource` local to each
  `FractalPlanet`, which also made the `RNG` dependency unused — dropped
  it from `FractalPlanet`'s constructor, `PlanetFactory.Create`, and the
  two call sites in `Space.cs`. Added
  `FractalPlanetTests.GenerateLandscapeIsDeterministicPerSeed`, which
  failed under the old shared-RNG behaviour and passes now.

### Changed (Remove analyzer suppressions in favour of real fixes or scoped disables, 2026-07-24)

- Worked through the backlog's suppression list: CA2227 collection
  properties ([ThreeDModel.cs](src/useful/libs/Useful.Assets/Models/ThreeDModel.cs),
  `Point.cs`, `Face.cs`, `AssetManifest.cs`,
  [SaveState.cs](src/elite/libs/EliteSharpLib/Save/SaveState.cs)) became
  init-only, since `System.Text.Json` supports that natively. CA1308
  ([Extensions.cs](src/elite/libs/EliteSharpLib/Extensions.cs)) and
  RCS1231 were resolved with equivalent code the rules don't flag.
  SA1401's exposed field
  ([PlanetRenderer.cs](src/elite/libs/EliteSharpLib/Planets/PlanetRenderer.cs))
  became a property. CA5394
  ([RandomSource.cs](src/useful/libs/Useful/RandomSource.cs),
  `FractalPlanet.cs`) and S6640 (assembly-scoped in `Useful.SDL`, unsafe
  interop only) are verified false positives for this codebase, so
  they're now disabled via `.editorconfig` instead of scattered local
  suppressions — a more honest opt-out. CA1034
  (`SoftwareSoundTests.cs`) is confirmed unfixable (xUnit1000 requires a
  public test class, which then forces its constructor parameter type
  public too via CS0051) and stays as a justified local suppression.
  Documented the resulting triage policy in
  [architecture-principles.md](docs/architecture-principles.md). One suppression remains,
  `ShipFactory.cs` (S3011, reflection into a non-public constructor) —
  left for the `ShipFactory.CreateShipFromName` dictionary-replacement
  item, which resolves it as a side effect.

### Changed (Replace custom Guard.ArgumentNull with ArgumentNullException.ThrowIfNull, 2026-07-24)

- `Guard.ArgumentNull` ([Guard.cs](src/useful/libs/Useful/Guard.cs)) was a
  hand-rolled null-check helper predating .NET's
  `ArgumentNullException.ThrowIfNull`, which does the same job as a framework
  intrinsic (per the architecture doc's "prefer dotnet framework intrinsics"
  rule). Replaced all call sites across `Useful.*`, `EliteSharpLib`, and
  `StuntCarRacerSharpLib` with `ArgumentNullException.ThrowIfNull`, then
  deleted `Guard.cs`, `ValidatedNotNullAttribute.cs`, and `GuardTests.cs`.
  Dropped the now-unused `using Useful;` this left behind in nine files.

### Fixed (Config file never written to %AppData% until a setting was changed, 2026-07-24)

- `ConfigFile<T>.ReadConfig()`
  ([ConfigFile.cs](src/useful/libs/Useful/Config/ConfigFile.cs)) only ever
  read from disk, falling back to `new T()` in memory when no file existed;
  it never wrote that file back out. Both games only call `WriteConfig`
  from their Settings/Options screen, so on a fresh install
  `%AppData%\TheSharpKind` stayed empty until the user opened Settings —
  nothing else confirmed the config path was writable or let a player find
  the file to hand-edit. `ReadConfig` now checks whether the file existed
  before reading, and if not, writes the resolved defaults immediately so a
  config file always appears at startup for both Elite
  (`elitesharp.cfg`) and SCR (`stuntcarracersharp.cfg`). Corrupt/invalid
  existing files are untouched, since the write only happens when the file
  was missing beforehand. Verified via the existing `ConfigFileTests` in
  both `Useful.Tests` and `EliteSharpLib.Tests`.

### Changed (Extract a shared BaseConfigSettings to deduplicate the two games' config types, 2026-07-23)

- `ConfigSettings` (Elite) and `ScrConfigSettings` (SCR) had drifted into
  duplicating the same three properties — `EffectsOn`, `GraphicsBackend`,
  `MusicOn` — with identical defaults and comments. Added
  `BaseConfigSettings`
  ([BaseConfigSettings.cs](src/useful/libs/Useful.Abstraction/Config/BaseConfigSettings.cs))
  to `Useful.Abstraction.Config` — the only assembly already referenced by
  both game libraries that can also see the `GraphicsBackend` enum — and had
  both settings types inherit from it instead. `ScrConfigSettings` had
  nothing game-specific left, so it collapsed to
  `internal sealed class ScrConfigSettings : BaseConfigSettings;`
  ([ScrConfigSettings.cs](src/scr/libs/StuntCarRacerSharpLib/Config/ScrConfigSettings.cs)).
  Also renamed Elite's settings type from `ConfigSettings` to
  `EliteConfigSettings`
  ([EliteConfigSettings.cs](src/elite/libs/EliteSharpLib/Config/EliteConfigSettings.cs))
  to match the SCR naming and avoid confusion with the generic
  `Useful.Config.ConfigFile<T>` machinery it's used with. `ConfigFile<T>`
  itself needed no changes — it was already generic. Verified both games'
  libs, both test projects, and the config round-trip tests still pass.

### Fixed (SCR crash on Hardware GraphicsBackend: missing FontsTrueType manifest entry, 2026-07-23)

- Selecting `GraphicsBackend: Hardware` for StuntCarRacerSharp crashed at
  startup with `KeyNotFoundException: The given key 'Small' was not present
  in the dictionary` from `SDLGraphics.DrawTextLeft`
  ([SDLGraphics.cs:375](src/useful/libs/Useful.SDL/SDLGraphics.cs)), reached
  via `TrackMenuScreen.Draw()`. `SDLGraphics` loads its `_fonts` dictionary
  from `IAssetLocator.FontTrueTypePaths`, which is populated from the
  manifest's `FontsTrueType` section — but SCR's
  [AssetManifest.json](src/scr/libs/StuntCarRacerSharpLib/Assets/AssetManifest.json)
  only ever had a `FontsBitmap` section (used by the `Software` backend), so
  under `Hardware` the font dictionary came back empty. Elite's manifest
  already had both sections. Added a `FontsTrueType` section to SCR's
  manifest (`Small`/`Large` both mapping to `OpenSans-Regular.ttf`, matching
  Elite's mapping), copied that font file into
  `src/scr/libs/StuntCarRacerSharpLib/Assets/FontsTrueType/`, and added the
  matching `CopyToOutputDirectory` entry in
  [StuntCarRacerSharpLib.csproj](src/scr/libs/StuntCarRacerSharpLib/StuntCarRacerSharpLib.csproj).
  Verified with a full solution build and confirmed the TTF now lands in the
  app's `bin/Assets/FontsTrueType` output.

### Added (GraphicsBackend config switch between Software and SDL3_mixer-backed Hardware audio, 2026-07-23)

- The in-progress SDL2→SDL3 migration had staged `SDLAbstraction.cs`/`SDLSound.cs`
  ([SDLAbstraction.cs](src/useful/libs/Useful.SDL/SDLAbstraction.cs),
  [SDLSound.cs](src/useful/libs/Useful.SDL/SDLSound.cs)) for deletion: `SDLSound`
  was built entirely on the old `SDL2.SDL_mixer` API, which no longer compiles
  now that `Useful.SDL` has moved to `ppy.SDL3-CS`/`ppy.SDL3_mixer-CS`, and
  `SDLAbstraction` only failed to compile because of that. Both games were left
  hardcoded to `SoftwareAbstraction`, with no way to opt into hardware-accelerated
  rendering at all. Restored `SDLAbstraction` unchanged and rewrote `SDLSound`
  against SDL3_mixer's new `MIX_Track` API — an improvement over the old one, not
  just a port: SDL3_mixer exposes per-track pitch (`MIX_SetTrackFrequencyRatio`)
  and panning (`MIX_SetTrackStereo`) natively, so the old resample-via-effect-callback
  trick (and its single dedicated "pitched one-shot" channel) is gone; all 16
  voices in the one-shot pool can now carry their own pitch, matching
  `SoftwareSound`'s capabilities instead of a subset of them.

  Added a `GraphicsBackend` enum (`Useful.Abstraction`,
  [GraphicsBackend.cs](src/useful/libs/Useful.Abstraction/GraphicsBackend.cs):
  `Software` or `Hardware`) and a matching `GraphicsBackend` property (default
  `Software`) on both games' settings —
  [EliteConfigSettings.cs](src/elite/libs/EliteSharpLib/Config/EliteConfigSettings.cs) and
  [ScrConfigSettings.cs](src/scr/libs/StuntCarRacerSharpLib/Config/ScrConfigSettings.cs).
  Since those settings types are internal (so `Program.Main` can't reference them
  directly — the same reason `AddEliteConfig`/`AddScrConfig` exist) and
  `Useful.SDL` is deliberately not a dependency of either game-logic library,
  each library instead exposes a `ReadGraphicsBackend(userDataPath, loggerFactory)`
  helper
  ([EliteServiceCollectionExtensions.cs](src/elite/libs/EliteSharpLib/EliteServiceCollectionExtensions.cs),
  [StuntCarRacerServiceCollectionExtensions.cs](src/scr/libs/StuntCarRacerSharpLib/StuntCarRacerServiceCollectionExtensions.cs))
  that reads just the public enum. Both `SDLProgram.cs`s
  ([Elite](src/elite/apps/EliteSharp/SDLProgram.cs),
  [SCR](src/scr/apps/StuntCarRacerSharp/SDLProgram.cs)) now read that setting
  before building the DI container and construct `SDLAbstraction` or
  `SoftwareAbstraction` accordingly, instead of hardcoding `SoftwareAbstraction`.

  Live-testing the `Hardware` backend against a real `elitesharp.cfg` surfaced a
  second issue: SDL3_mixer's bundled MIDI decoder is Timidity-derived and expects
  a GUS patch set on disk via `timidity.cfg`, which this project doesn't ship, so
  loading `theme.mid`/`danube.mid` failed with "Audio data is in
  unknown/unsupported/corrupt format" and crashed the app at DI-composition time.
  `SDLSound` now predecodes `.mid` music with the same MeltySynth + bundled
  `TimGM6mb.sf2` SoundFont path `SoftwareSound` already uses successfully, and
  hands the result to the mixer as raw PCM via `MIX_LoadRawAudio`; `.ogg` SFX
  still load through SDL3_mixer's native decoder unchanged. Docs
  (`elite-readme.md`, `scr-readme.md`) Configuration sections updated with the
  new setting. Built the full solution (`Useful.SDL` and both apps/libs/test
  projects) and ran `EliteSharp` against the real `%AppData%\TheSharpKind\elitesharp.cfg`
  (`GraphicsBackend: Hardware`) that had previously crashed it, confirming it now
  starts cleanly.

### Changed (Renamed StuntCarRacer to StuntCarRacerSharp; SCR added to CI, 2026-07-22)

- `build-and-package.yml` only built/published EliteSharp, so master/PR
  builds never caught SCR publish regressions. Added matching
  `StuntCarRacerSharp` Windows/Linux publish + artifact-upload steps and
  renamed the workflow from "Build and Package EliteSharp" to "Build and
  Package" since it now covers both games.
- SCR's app/library/test project names (`StuntCarRacer`,
  `StuntCarRacerLib`, `StuntCarRacerLib.Tests`, `StuntCarRacerLib.Fakes`)
  didn't end in "Sharp" the way Elite's do (`EliteSharp`,
  `EliteSharpLib`, ...), an inconsistency across the two games in this
  shared repo. Renamed throughout: project/folder/csproj names to
  `StuntCarRacerSharp`/`StuntCarRacerSharpLib`/`StuntCarRacerSharpLib.Tests`/
  `StuntCarRacerSharpLib.Fakes`, their C# `namespace`s and `using`s, the
  `.slnx` entries, `.gitignore`'s asset-tracking rule, `README.md`'s
  `dotnet run` path, and every reference across `docs/backlog-roadmap.md`
  and this file. Left the internal class names as-is (`StuntCarRacerMain`,
  `StuntCarRacerServiceCollectionExtensions`, ...), matching how Elite
  keeps `EliteMain`/`EliteServiceCollectionExtensions` unprefixed inside
  `EliteSharpLib` — only the project/assembly identifier gains the
  suffix. Also left ptitSeb's reference-source mentions
  (`StuntCarRacer.cpp`) untouched since that's an external C++ file, not
  our code. Built the full solution, ran the complete test suite (all
  green: 84 EliteSharpLib.Tests + 188 StuntCarRacerSharpLib.Tests), and
  smoke-tested both `EliteSharp` and `StuntCarRacerSharp` via the
  `GAME_KEY_SCRIPT`/`GAME_FRAME_DUMP_DIR` harness, confirming both still
  launch and render their opening screens correctly under the renamed
  assemblies.

### Added (Tag-triggered release CI, 2026-07-22)

- No CI path published Release artifacts or cut a GitHub Release; tagging
  `v1.0.0` would have done nothing. Added
  [release.yml](.github/workflows/release.yml), triggered on `v*` tags,
  which self-contained-publishes both EliteSharp and StuntCarRacerSharp for
  win-x64/linux-x64/linux-arm64, zips each app/RID combination, and creates
  a GitHub Release via `softprops/action-gh-release` with
  `generate_release_notes: true` and a body noting the StuntCarRacerSharp builds
  are preview given its open defects list. StuntCarRacerSharp had no publish
  profiles at all, so added `PublishProfileWindows`/`PublishProfileLinux`/
  `PublishProfileLinuxArm64` under
  [StuntCarRacerSharp/Properties/PublishProfiles](src/scr/apps/StuntCarRacerSharp/Properties/PublishProfiles)
  mirroring Elite's, plus the missing `PublishProfileLinuxArm64` for Elite.
  Both apps' existing profiles published into the same repo-root
  `publish/<rid>/` folder, which would have let StuntCarRacerSharp's publish
  overwrite EliteSharp's (or vice versa) once both ran in one job — renamed
  every `PublishDir` to `publish/<AppName>/<rid>/` and updated
  [build-and-package.yml](.github/workflows/build-and-package.yml)'s
  artifact-upload paths to match. Built the full solution, ran the
  complete test suite (all green), and locally ran all six
  app/RID `dotnet publish` combinations to confirm each lands in its own
  directory with no errors.

### Changed (Tag-driven semantic versioning via MinVer, 2026-07-22)

- CI stamped `Version`/`FileVersion` from `0.<yy><day-of-year>.<run-number>.0`,
  so version numbers reset meaning every build and carried no relation to
  releases. Added `MinVer` (via [Directory.Build.props](Directory.Build.props),
  `MinVerTagPrefix` `v`, matching the already-decided `v1.0.0` first-tag
  scheme) so every project's version is now derived from the nearest `v*`
  git tag plus commit height — tagging a commit *is* the release-versioning
  step, with untagged builds getting a `0.0.0-alpha.0.<height>+<sha>`
  pre-release version. Updated
  [build-and-package.yml](.github/workflows/build-and-package.yml) to fetch
  full history (MinVer needs the tags), removed the manual
  `/p:Version=.../p:FileVersion=...` stamping from every build/test/publish
  step, and now reads the computed version for artifact naming via the
  `minver-cli` global tool instead. Built the full solution and ran the
  complete test suite (all green); verified the built `EliteSharp.dll`'s
  `ProductVersion` reads `0.0.0-alpha.0.<height>+<sha>` as expected pre-tag.

### Added (Scripted input + frame dump in the real SDL apps, 2026-07-22)

- The headless harnesses cover the common case, but the rare check that
  must exercise the true SDL window/present path still meant OS-level
  focus stealing and key injection to verify live. `GameHost.Run`
  (shared by both games) now supports two environment-variable-gated
  debug facilities, so default behaviour with neither var set is
  unchanged: `GAME_KEY_SCRIPT` (a file path, or the script text itself)
  is parsed by the new `Useful.Controls.KeyScriptParser` and replayed
  into the real keyboard sink tick-by-tick by the new
  `KeyScriptPlayer`, and `GAME_FRAME_DUMP_DIR` enables both an F12
  debug key and the script's `SaveFrame` command, both of which dump
  the current native-resolution framebuffer there as a BMP via a new
  `IGraphics.SaveScreen(path)` (implemented on `SoftwareGraphics` by
  writing its back buffer directly, and on `SDLGraphics` via
  `SDL_RenderReadPixels`, plus no-ops/recording on the two `IGraphics`
  test fakes). `KeyScriptEvent`/`KeyScriptAction` moved from
  `Useful.Fakes.Harness` (test-only) to `Useful.Controls` (production)
  so both the headless harnesses and this real-app player share one
  definition instead of two; `KeyScriptAction` gained a `SaveFrame`
  member. Added `KeyScriptParserTests`, `KeyScriptPlayerTests`, and a
  `SoftwareGraphics.SaveScreen` round-trip test. Verified live: ran
  both apps with a scripted key file and `GAME_FRAME_DUMP_DIR` set, no
  OS focus/injection involved, and confirmed the dumped BMPs
  (640x400/512x512) show the correct rendered frame (SCR's track menu,
  Elite's intro/parade screen) with clean logs. Built the full solution
  and ran the complete test suite (all green).

### Added (Elite headless game harness + shared harness base, 2026-07-22)

- Added the Elite equivalent of SCR's `HeadlessGameHarness`, building on
  `EliteMainTests`' DI-composed construction/smoke test: drives the same DI
  graph `SDLProgram.Main` does (`AddEliteConfig` + `AddEliteMain`) with
  `FakeAbstraction` wrapping a real `SoftwareGraphics` (no SDL window) and
  calls `EliteMain.Update`/`Draw` directly per tick — `EliteMain.Run` is
  unusable headlessly as-is, since it hands off to `GameHost.Run`'s
  real-time, wall-clock-waiting loop. Added an internal `EliteMain.State`
  property exposing `GameState` (which stays internal) for the harness to
  read, mirroring `StuntCarRacerMain.Screens`/`Race`.
- Writing it alongside SCR's existing harness surfaced straight
  copy-pasted code (`KeyScriptEvent`, `KeyScriptAction`, and the
  tap/hold/release scripting loop in `Step`/`Run`/`SaveFrame`) that would
  only have grown more duplicated as more games got one. Extracted the
  shared parts into `Useful.Fakes`
  ([Harness/HeadlessGameHarnessBase.cs](src/useful/test/Useful.Fakes/Harness/HeadlessGameHarnessBase.cs),
  `KeyScriptEvent.cs`, `KeyScriptAction.cs`, new `Useful.Fakes.Harness`
  namespace; added a `Useful.Graphics` project reference to `Useful.Fakes`
  for `SoftwareGraphics`/`BitmapWriter`): a public generic
  `HeadlessGameHarnessBase<TState>` owns the `SoftwareGraphics`, the
  scripted-input `Step`/`Run` loop, and `SaveFrame`, via two abstract hooks
  (`UpdateGame`/`DrawGame`) and an abstract `State` property. Both games'
  `HeadlessGameHarness` classes now just build their own object graph
  (Elite's DI container vs SCR's direct constructor) and supply their own
  `GameStateSummary` shape — genuinely different per game, so those stay
  local — while `KeyScriptEvent`/`KeyScriptAction` and the harness
  machinery are defined exactly once. Verified with a throwaway test that
  the Elite harness still renders a real frame (cockpit/HUD/scanner) before
  deleting it. Built the full solution and ran the complete test suite
  (all green, same pass counts as before the refactor); no production
  behaviour changed beyond the new `EliteMain.State` accessor, so no live
  smoke test was needed.

### Added (BitmapWriter + BitmapReader rename in Useful.Graphics, 2026-07-22)

- `BitmapFile.Read` was the only half of a load/save pair, and the
  BMP-dumping code duplicated itself across both games' `VisualDumpTests`.
  Renamed `BitmapFile` to `BitmapReader`
  ([BitmapReader.cs](src/useful/libs/Useful.Graphics/BitmapReader.cs),
  updating its `SoftwareGraphics.Create` call sites and test file to
  match) and added `BitmapWriter.Write`
  ([BitmapWriter.cs](src/useful/libs/Useful.Graphics/BitmapWriter.cs)): a
  standard 32bpp `BITMAPV5HEADER` BGRA bottom-up BMP, padded to
  `BitmapReader`'s fixed pixel-data offset so a written file reads back
  correctly, and valid enough to open in an ordinary image viewer too.
  Both games' `VisualDumpTests` and SCR's new `HeadlessGameHarness` (see
  below) now call the shared `BitmapWriter.Write` instead of a
  hand-rolled 24bpp `SaveBmp` each maintained separately. Added
  `BitmapWriterTests.cs` (signature/length check, a round trip through
  `BitmapReader` with distinct per-corner colours incl. alpha, and a
  non-square bitmap) alongside the renamed `BitmapReaderTests.cs`. Also
  tidied `BitmapReader.Read` while it was open: it allocated a fresh
  byte array per field, including a `new byte[4]` for *every pixel* in
  the double loop; switched to `BinaryPrimitives.ReadInt32/16LittleEndian`
  reading spans of the original buffer directly (zero allocation, and it
  now applies the correct endian handling to width/height/bit-depth too,
  not just pixel colour as before). Replaced the `Debug.Assert` on the
  "BM" signature — compiled out in Release, so a non-BMP file would
  silently misparse instead of failing — with a thrown `UsefulException`,
  matching the existing bit-depth check. Built the full solution and ran
  the complete test suite (all green).

### Added (Reusable headless SCR game harness, 2026-07-22)

- Verifying a change against the running game meant driving the real SDL
  window with OS-level focus stealing and key injection — slow and flaky
  for agent-driven sessions, and `VisualDumpTests` only exercises
  hand-composed scenes, not the whole game (screens, HUD, menu flow).
  Added `HeadlessGameHarness` to `StuntCarRacerSharpLib.Tests`
  ([HeadlessGameHarness.cs](src/scr/test/StuntCarRacerSharpLib.Tests/HeadlessGameHarness.cs)):
  runs the real `StuntCarRacerMain` against a real `SoftwareGraphics` (no
  SDL window), generalising `StuntCarRacerMainTests.StartRace`'s manual
  key-press sequence into a scripted `KeyScriptEvent` timeline (tap/hold/
  release at a given tick), and exposes a `GameStateSummary` (current
  screen, race started, player/opponent track piece, distance to
  opponent) so most checks never need a rendered frame — `SaveFrame` is
  there for the rare case that does (calling the shared `BitmapWriter`
  above). Added `HeadlessGameHarnessTests.cs` covering the initial menu
  state, driving menu→race via a scripted timeline, and a frame dump
  smoke test. Built the full solution and ran the complete test suite
  (all green); no production code changed, so no live smoke test was
  needed.

### Added (EliteMain construction/smoke test using fakes, 2026-07-22)

- Elite had no test that even constructed its composition, unlike SCR's
  `StuntCarRacerMainTests`. Added `EliteMainTests.cs`
  ([EliteMainTests.cs](src/elite/test/EliteSharpLib.Tests/EliteMainTests.cs)):
  builds the same DI graph `SDLProgram.Main` does
  (`AddEliteConfig`+`AddEliteMain`) with a new `FakeAbstraction` swapped in
  for `IAbstraction` (no SDL window/sound device/keyboard needed) and the
  real `AssetLocator`/shipped assets otherwise, then resolves `EliteMain`
  and runs one `Update()` tick, asserting it doesn't throw and
  `IsRunning` stays true. Added `FakeAbstraction` to `EliteSharpLib.Fakes`
  (mirroring SCR's), and gave the shared `FakeGraphics`
  ([FakeGraphics.cs](src/useful/test/Useful.Graphics.Fakes/FakeGraphics.cs))
  an optional width/height constructor (default 0, unchanged for existing
  callers) — `Stars.CreateNewStar` derives its random ranges from
  `EliteDraw.Centre`, which comes from `IGraphics.ScreenWidth/Height`, and a
  0x0 fake screen produced a negative range that threw; `FakeAbstraction`
  now sizes it 512x512 to match Elite's real resolution. Built the full
  solution, ran the complete test suite (all green, 396 tests across the
  solution including the new test), and smoke-tested the live app.

### Added (SpaceTests covering Space's flight/hyperspace/docking logic, 2026-07-22)

- `Space` ([Space.cs](src/elite/libs/EliteSharpLib/Space.cs)) was the last
  class named by the "Elite's core game logic is largely untested" backlog
  item — notably `LaunchPlayer`'s `LegalStatus |= _trade.IsCarryingContraband()`
  line, where "the contraband bug lived here for years." Added
  `SpaceTests.cs` (26 tests) covering `LaunchPlayer` (contraband carried over
  into `LegalStatus`, clean without contraband, flight state + undocking),
  `DockPlayer` (docks, resets ship speed and weapons), `JumpWarp` (mass-locked
  by a non-exempt object present, mass-locked with no planet, clamps the jump
  to 1024 and moves every object), `StartHyperspace`/`StartGalacticHyperspace`
  (fuel/equipment/already-ready guards), `CountdownHyperspace` (decrements,
  and completes a galactic jump at zero), `EngageDockingComputer` (docking
  view only with a station present) and `UpdateAltitude`/`UpdateCabinTemp`
  (safe defaults in witchspace/without a planet or sun, the near-body
  distance formula, the fuel-scoop temperature band, and the too-close/too-hot
  game-over paths). Added `FakeView` to `EliteSharpLib.Fakes` (a no-op `IView`)
  so tests can register the `Docking`/`GameOver`/`Hyperspace` screens that
  `GameState.SetView` requires. `UpdateUniverse` (the per-frame object
  update/render loop) is not covered here — it's exercised indirectly by the
  existing visual-dump/smoke tests rather than unit-level assertions. Built
  the full solution, ran the complete test suite (all green, 80 in
  `EliteSharpLib.Tests` including the 26 new tests).

### Added (PlanetControllerTests covering the pure-logic galaxy/planet generation code, 2026-07-22)

- `PlanetController` ([PlanetController.cs](src/elite/libs/EliteSharpLib/PlanetController.cs))
  had no test coverage — part of the "Elite's core game logic is largely
  untested" backlog item, which also named `SaveFile` round-trip coverage as
  a starting point; that round trip already exists in `SaveFileTests`
  (`SaveCommanderThenLoadCommanderRoundTrips`), as does coverage for `Combat`,
  `ConfigFile` and `RNG.GenerateRandomNumber` (`CombatTests`,
  `ConfigFileTests`, `RNGTests`), so only `PlanetController` was still
  missing. Added `PlanetControllerTests.cs` covering
  `CalculateDistanceToPlanet` (zero for the same planet, symmetric),
  `GeneratePlanetData` (deterministic from seed), `WaggleGalaxy`
  (deterministic, fields stay byte-ranged over many iterations),
  `NamePlanet` (deterministic, doesn't mutate its input seed),
  `FindPlanetNumber` (matches a galaxy waggled that many times, -1 when not
  found), `FindPlanetByName` (finds/fails by name) and `DescribeInhabitants`
  (Human Colonials below seed byte E 128, alien description above). `Space`
  remains untested and stays on the backlog. Built the full solution, ran
  the complete test suite (all green, 54 in `EliteSharpLib.Tests` including
  the 14 new tests).

### Fixed (AssetLocator.Create no longer exclusively locks AssetManifest.json, 2026-07-21)

- `AssetLocator.Create()` ([AssetLocator.cs](src/useful/libs/Useful.Assets/AssetLocator.cs))
  opened `AssetManifest.json` via `File.Open(path, FileMode.Open)`, which
  defaults to `FileAccess.ReadWrite`/`FileShare.None` — an exclusive lock for
  a read-only operation. Two concurrent callers (e.g. two test classes each
  constructing their own `AssetLocator`/`SoftwareGraphics` in parallel xUnit
  collections) could race and throw `IOException` on the file, wrapped as
  `UsefulException`. Found while adding `EliteDrawTests.cs` alongside the
  existing `VisualDumpTests.cs`, both of which call `AssetLocator.Create()`.
  Now opens with `FileAccess.Read, FileShare.Read`. Verified with an ad hoc
  8-way concurrent `AssetLocator.Create()` probe run 5 times alongside
  `VisualDumpTests` (no failures pre-fix reproduced it, post-fix all green);
  probe was throwaway and not committed. Built the full solution, ran the
  complete test suite (all green).

### Fixed (EliteDraw.DrawTextPretty no longer underflows on a word longer than the line width, 2026-07-21)

- `DrawTextPretty` ([EliteDraw.cs](src/elite/libs/EliteSharpLib/Graphics/EliteDraw.cs)) scanned
  backward from the line-width breakpoint for a space/comma/period with no
  lower bound; a word longer than the available width (no break character
  anywhere in range) walked the index past 0 and threw
  `IndexOutOfRangeException`. The backward scan now stops at `previous`, and
  when no break character is found in range the word is hard-broken at the
  line-width boundary instead. Added `EliteDrawTests.cs` covering a 200-character
  unbreakable word. Built the full solution, ran the complete test suite (all
  green, 41 in `EliteSharpLib.Tests` including the new test).

### Fixed (Sanitize SaveFile.SaveCommander's filename and log its save failures, 2026-07-21)

- `SaveFile.SaveCommander` ([SaveFile.cs](src/elite/libs/EliteSharpLib/Save/SaveFile.cs))
  built the save path as `save.CommanderName + ".cmdr"` from raw user input —
  invalid filename characters threw and path separators could escape the save
  directory. `PathFor` (shared by both `LoadCommander` and `SaveCommander`) now
  replaces any character in `Path.GetInvalidFileNameChars()` with `_` before
  combining it into a path. Also converted `SaveCommander`'s two
  `Debug.WriteLine`/`Debug.Fail` catch blocks to the `ILogger<SaveFile>`
  `LoadCommander` started using in the previous entry, adding a
  `FailedToSaveCommander` `LogMessages` entry following the same
  `GameOverView`/`Combat` exemplar. Added `SaveFileTests.cs` coverage for a
  commander name containing `../` path separators, asserting the save stays
  inside the save directory as a single file. Built the full solution, ran the
  complete test suite (all green, 40 in `EliteSharpLib.Tests` including the
  new test).

### Fixed (SaveFile.LoadCommander no longer throws on a corrupt or hand-edited save, 2026-07-21)

- `SaveFile.LoadCommander` ([SaveFile.cs](src/elite/libs/EliteSharpLib/Save/SaveFile.cs))
  caught its own failures, reset `_lastSaved` to Jameson, then `throw;`ed —
  contradicting its `bool`-return contract and `LoadCommanderView`'s
  "Error Loading Commander!" path, which never ran. Worse,
  `SaveStateToGameState` indexes `GalaxySeed[0..5]`, `CurrentCargo[i]`,
  `StationStock[i]` and `Lasers[0..3]` and `Enum.Parse`s `EnergyUnit`/
  `LaserType` with no validation, so a truncated or hand-edited `.cmdr`
  file threw `IndexOutOfRangeException`/`FormatException` there instead.
  Added `IsValidSave`, checking the array lengths `SaveStateToGameState`
  assumes and that both enum strings parse, following `ConfigFile<T>.ReadConfig`'s
  read-validate-fallback shape ([ConfigFile.cs](src/useful/libs/Useful/Config/ConfigFile.cs));
  `LoadCommander` now validates before calling `SaveStateToGameState`,
  returns `false` on any read/parse/validation failure, and resets to
  Jameson without rethrowing. `SaveFile` takes an optional
  `ILogger<SaveFile>` (defaulting to `NullLogger<SaveFile>.Instance`,
  the `GameOverView`/`Combat` exemplar), and the removed `Debug.WriteLine`
  is now a Warning via two new `LogMessages` entries
  (`FailedToLoadCommander`, `CommanderValidationFailed`).
  `EliteServiceCollectionExtensions.AddEliteMain` passes
  `ILoggerFactory.CreateLogger<SaveFile>()` through.
  `SaveCommander`'s two `Debug.WriteLine`/`Debug.Fail` catch blocks are
  unchanged — tracked as a separate backlog item since they're a save,
  not a load, and the fix there is a different shape (sanitizing the
  filename). Added `SaveFileTests.cs`
  ([SaveFileTests.cs](src/elite/test/EliteSharpLib.Tests/Save/SaveFileTests.cs))
  covering a missing save file, corrupt JSON, a truncated `galaxySeed`
  array, and a save/load round trip. Built the full solution, ran the
  complete test suite (all green, 39 in `EliteSharpLib.Tests` including
  the 4 new ones), and smoke-tested the built Elite app — it starts and
  constructs its full DI graph (including the changed `SaveFile`
  constructor) without error.

### Fixed (Stop swallowing input/console errors in EliteMain.Update, 2026-07-21)

- `EliteMain.Update` wrapped `_scanner.UpdateConsole()` and
  `_gameState.CurrentView!.HandleInput()` in a `catch (Exception) { Debug.WriteLine(...) }`
  ([EliteMain.cs](src/elite/libs/EliteSharpLib/EliteMain.cs)), so in Release
  builds (where `Debug.WriteLine` compiles away) every error from either call
  was silently swallowed each tick, violating the architecture doc's "never
  catch-all on the frame path" rule. Removed the catch-all: `SDLProgram.Main`
  ([SDLProgram.cs](src/elite/apps/EliteSharp/SDLProgram.cs)) already wraps
  `elite.Run()` in a try/catch that logs Critical via Serilog and rethrows,
  and the whole game loop runs synchronously on that same thread
  (`GameLoop.Run` → `GameHost.Run` → `EliteMain.Run`, no background thread),
  so any exception now surfaces there with a full stack trace instead of
  vanishing.

### Changed (Let SoftwareGraphicsBenchmarks take CLI filter/job arguments, 2026-07-21)

- `Useful.Graphics.Benchmarks`'s `Program.Main`
  ([Program.cs](src/useful/perf/Useful.Graphics.Benchmarks/Program.cs)) called
  `BenchmarkRunner.Run<T>()` directly, so `dotnet run -c Release` always ran
  all 23 benchmarks under the full `DefaultConfig` job (~10 minutes) with no
  way to narrow it down — the friction that made the clip-region regression
  above expensive to chase down. Switched to
  `BenchmarkSwitcher.FromAssembly(...).Run(args, config)`, which accepts
  `--filter`/`--job`/etc.; verified that plain `dotnet run -c Release` (no
  arguments) still runs every benchmark under the same `DefaultConfig` job
  as before — `BenchmarkSwitcher` prompts interactively when given no
  arguments even with a single benchmark class in the assembly, so `Main`
  now substitutes `--filter *` in that case to keep today's default
  behaviour. Local iteration on a change can now run in seconds instead of
  minutes, e.g. `dotnet run -c Release -- --filter *DrawPixel* --job dry`.
  Also switched the job's toolchain to `InProcessNoEmitToolchain`: the
  default out-of-process toolchain generates and builds a throwaway project
  per run, and BenchmarkDotNet resolves it by scanning the whole repo tree
  for a matching project *name* — which throws `NotSupportedException` the
  moment more than one copy exists on disk, as happens when a Claude Code
  isolated-worktree agent's checkout sits nested under
  `.claude/worktrees/*` inside the same repo. Running in-process sidesteps
  the lookup entirely (and removes the per-run build step, on top of the
  `--filter`/`--job` speedup above), at the cost of slightly less isolation
  between iterations than a separate process gives — an acceptable trade
  for a suite whose numbers are read as relative comparisons rather than
  absolute ones. `Useful.Controls.Benchmarks` and `EliteSharpLib.Benchmarks`
  have the same hardcoded-`Main` pattern and would benefit from the same
  change; not done here since neither was needed for this investigation.

### Fixed (Implement SoftwareGraphics.SetClipRegion, 2026-07-21)

- `SoftwareGraphics.SetClipRegion`
  ([SoftwareGraphics.cs](src/useful/libs/Useful.Graphics/SoftwareGraphics.cs))
  was an empty no-op, while Elite actively relies on clip regions
  (`EliteDraw.SetViewClipRegion`) to keep view drawing inside the border and
  off the scanner area — in the software renderer this protection silently
  did nothing. Added a clip rectangle (defaulting to the full screen) that
  both `DrawPixel` overloads test against, and routed
  `DrawRectangleFilledInt`/`DrawRectangleInt` (which wrote straight to the
  backing bitmap) through the clipped `DrawPixel` too, so every pixel-writing
  path is now covered. A same-toolchain A/B benchmark of the first pass
  showed this cost real per-pixel throughput — the private `DrawPixel(int,
  int, uint)` had previously been a bare `_screen.SetPixel` passthrough with
  no checks at all, so `DrawLine`/`DrawCircleFilled`/`DrawRectangle*` (all of
  which funnel through it per pixel) roughly doubled. Added a
  `_clipIsFullScreen` flag, true whenever the clip covers the whole screen
  (the default, and where most frames spend most of their time — e.g. Elite
  only narrows the clip for the 3D view); both `DrawPixel` overloads check it
  first and skip straight to the cheap unclipped path when it's set, only
  paying the four-field clip comparison while a narrower clip is actually
  active. Re-running the same A/B afterwards showed the regression cut from
  ~100% down to ~10-25% (a few nanoseconds) on the affected benchmarks — the
  remaining cost being the one unavoidable guard branch. Added
  `SetClipRegionRestrictsPixelWritesToRegion`,
  `SetClipRegionRestrictsRectangleFilledToRegion` and
  `SetClipRegionBackToFullScreenRestoresUnclippedDrawing` in
  `SoftwareGraphicsTests`. Verified live against the real `EliteDraw` render
  path (mirroring `EliteMain.Update`'s clip/border/clip-to-view sequence): a
  full-screen rectangle drawn under the view clip stayed off both the border
  and the scanner area. Built the full solution and ran the complete test
  suite (all green).

### Fixed (Bound the text bitmap cache with LRU eviction, 2026-07-21)

- `SoftwareGraphics._textCache`
  ([SoftwareGraphics.cs](src/useful/libs/Useful.Graphics/SoftwareGraphics.cs))
  cached one bitmap per distinct (font, colour, text) forever, and Elite
  renders ever-changing strings (bounty amounts, countdowns), so long
  sessions leaked memory steadily. The cache is now a bounded
  (`TextCacheCapacity` = 256) LRU: a `LinkedList` tracks recency, hits move
  their node to the front, and once full the least-recently-used bitmap is
  disposed and evicted before a new one is added. Added
  `GenerateTextBitmapCachesAndEvictsLeastRecentlyUsed` in
  `SoftwareGraphicsTests`, exercising a real font asset. Built the full
  solution and ran the complete test suite (all green).

### Fixed (Stop disposing cached text bitmaps on every draw, 2026-07-21)

- `DrawTextCentre`/`DrawTextLeft`/`DrawTextRight`
  ([SoftwareGraphics.cs](src/useful/libs/Useful.Graphics/SoftwareGraphics.cs))
  wrapped the bitmap returned by `GenerateTextBitmap` in `using`, but that
  same bitmap is stored in `_textCache` and handed back again on the next
  call with the same (font, colour, text) — so every cached bitmap was
  disposed (its `GCHandle` freed) right after its first use, working today
  only because text drawing never reads `FastBitmap.BitmapHandle`. Removed
  the `using`s; cached bitmaps are now disposed only when
  `SoftwareGraphics.Dispose()` runs, alongside the screen bitmap. Built the
  full solution and ran the complete test suite (all green).

### Changed (Share injectable randomness between Elite and SCR, 2026-07-21)

- Elite's `RNG` and SCR's `CarPhysics`/`OpponentPhysics` each had their own
  ad-hoc take on injectable randomness (see the entry below for Elite's), and
  SCR's `CarPhysics` had none at all (`private readonly Random _random =
  new(0);`, no constructor seam). The generic part is now a shared
  `Useful.IRandomSource`/`Useful.RandomSource` (`NextInt`/`Random`/
  `TrueOrFalse`/`GaussianRandom`, wrapping an injected `System.Random`), with
  a `Useful.Fakes.FakeRandomSource` that returns fixed, test-set values so a
  test can force an exact branch (e.g. "the 1-in-256 roll succeeds")
  without hunting for a seed. `EliteSharpLib.RNG` keeps only what's
  genuinely Elite-specific — `Seed`, `GenerateRandomNumber` (6502),
  `GenMSXRandomNumber` (MSX) — composing a `RandomSource` for everything
  else; its `RNG(Random random)` constructor (and every existing DI
  registration/consumer/test) is unchanged, plus a new `RNG(IRandomSource
  randomSource)` overload lets tests inject a `FakeRandomSource` directly.
  `CarPhysics`/`OpponentPhysics` (and the `Race`/`StuntCarRacerMain` chain
  that builds them) now take `IRandomSource` via constructor injection, with
  a convenience overload defaulting to an unseeded `RandomSource` so the
  ~30 existing `new(track)`-style test call sites didn't need to change.
  `StuntCarRacerServiceCollectionExtensions.AddScrRandom` registers the
  `Random`/`IRandomSource` singletons (mirroring
  `EliteServiceCollectionExtensions`); `SDLProgram.cs` switched
  `StuntCarRacerMain`'s registration to an explicit factory, since an
  implicit-constructor-selecting `AddSingleton<StuntCarRacerMain>()` would
  now tie between two equally-resolvable two-argument constructors.
  Added `EliteSharpLib.Tests/RNGTests.cs`, `CombatTests.cs` (Combat had zero
  tests before this), and `FakeRandomSource`-driven branch tests in
  `ShipFactoryTests.cs`; added the same for `CarPhysicsTests.cs`
  (`EngineFluctuation`/`OffRoadPitch`, previously either untested or only
  range-asserted) and `OpponentPhysicsTests.cs` (opponent-selection roll).
  Built the full solution, ran the complete test suite (all green, 35
  EliteSharpLib.Tests + 184 StuntCarRacerSharpLib.Tests), and smoke-tested both
  `EliteSharp` and `StuntCarRacerSharp` launching and running cleanly.

### Changed (Replace the static crypto RNG with an injected, seedable service, 2026-07-21)

- `RNG` ([RNG.cs](src/elite/libs/EliteSharpLib/RNG.cs)) was a static class
  whose `Random(...)` delegated every call to
  `RandomNumberGenerator.GetInt32` — cryptographically secure but far
  slower than needed in Elite's hot per-tick paths — and whose `Seed`
  state was static mutable, making the 6502/MSX planet-description
  generators untestable and coupling every caller to global state. `RNG`
  is now an instance class constructed from an injected `System.Random`,
  registered as a singleton in `EliteServiceCollectionExtensions` (a
  fresh `Random` in production, seedable in tests) alongside the
  `Random` singleton itself, per the architecture doc's "randomness is
  always an injected, seedable source, never a static mutable RNG"
  rule. Every consumer (`Combat`, `Space`, `Stars`, `Universe`,
  `Pilot`, `EliteDraw`, `LaserDraw`, `ShipFactory`/`ShipBase` and all
  30 ship subclasses, `PlanetFactory`/`FractalPlanet`,
  `SunFactory`/`GradientSun`/`SolidSun`, the four `Pilot*View`s,
  `GameOverView`, `PlanetDataView`, `EscapeCapsuleView`) now takes
  `RNG` by constructor injection instead of calling the static class.
  Built the full solution, ran the complete test suite (all green),
  and smoke-tested `EliteSharp` launching and running cleanly.

### Changed (Remove two-phase construction from SDLSound/SoftwareGraphics, 2026-07-21)

- `SDLSound` required a separate `Initialize(assetLocator)` call after
  construction to load music/sfx — `SDLAbstraction`
  ([SDLAbstraction.cs](src/useful/libs/Useful.SDL/SDLAbstraction.cs)) had
  silently skipped that call, so any code path through it would have thrown
  `KeyNotFoundException` the first time it tried to play a sound.
  `SDLSound`'s constructor now takes `IAssetLocator` directly and loads
  music/sfx itself ([SDLSound.cs](src/useful/libs/Useful.SDL/SDLSound.cs));
  both `SDLAbstraction` and `SoftwareAbstraction`
  ([SoftwareAbstraction.cs](src/useful/libs/Useful.SDL/SoftwareAbstraction.cs))
  updated to pass the asset locator in. `SoftwareGraphics.Fonts`/`Images`
  ([SoftwareGraphics.cs](src/useful/libs/Useful.Graphics/SoftwareGraphics.cs))
  were `internal`-settable properties populated after construction via an
  object initializer in `Create`, leaving them mutable by any other code in
  the assembly afterwards (the benchmark project did exactly that); they are
  now get-only, assigned once in the constructor, with `Create` computing
  the dictionaries before calling it. `ShipFactory.Create`
  ([ShipFactory.cs](src/elite/libs/EliteSharpLib/Ships/ShipFactory.cs)) set
  its `_ships` field the same way via an object initializer on a
  parameterless-constructed instance — private, so not the same
  half-built-instance risk, but the same pattern; it now takes the built
  dictionary through a private constructor instead. Built the full
  solution, ran the complete test suite (all green), and smoke-tested both
  `EliteSharp` and `StuntCarRacerSharp` launching cleanly.

### Changed (IKeyboard split into producer/consumer interfaces, 2026-07-21)

- `IKeyboard` ([IKeyboard.cs](src/useful/libs/Useful.Controls/IKeyboard.cs)) mixed
  the producer API (`KeyDown`/`KeyUp`/settable `Close`, written by `SDLInput`
  as raw key events arrive) with the consumer API (`IsPressed`/`IsHeld`/
  `LastPressed`/`ClearPressed`/`Poll`/readable `Close`, polled by games) —
  game code holding an `IKeyboard` had no way to be stopped from also
  injecting key events. Split the producer side into a new
  `IKeyboardSink` ([IKeyboardSink.cs](src/useful/libs/Useful.Controls/IKeyboardSink.cs));
  `IKeyboard` keeps the consumer API with `Close` now read-only.
  `SoftwareKeyboard` and `FakeKeyboard` implement both interfaces
  unchanged; `IInput.Register` (and its `SDLInput`/`FakeInput`/benchmark
  implementations) now take `IKeyboardSink` instead of `IKeyboard`.
  `IAbstraction.Keyboard` stays `IKeyboard`, so no game/view code changed.
  Built the full solution and ran the complete test suite (all green).

### Changed (SCR screens given their real dependencies, 2026-07-21)

- Every SCR screen (`RaceScreen`, `TrackMenuScreen`, `TrackPreviewScreen`,
  `GameOverScreen`) took the whole `StuntCarRacerMain` and reached through
  it service-locator style. Extracted a new `Race`
  ([Race.cs](src/scr/libs/StuntCarRacerSharpLib/Race.cs)) that owns the
  track/car/opponent/bridge state, its renderers, and the
  `LoadTrack`/`PhysicsDue`/`DrawWorld`/`DrawHud`/`UpdateSounds`/
  `UpdateEngineSound` behaviour that operates on them; `StuntCarRacerMain`
  now keeps only the run loop, screen wiring and audio setup. Each screen's
  constructor now takes `Race` plus the specific stable dependencies it
  actually uses (`IKeyboard`, `ScreenManager<GameMode, IGameScreen>`,
  `IGraphics`, `ScrPalette`, `ISound`) instead of the whole game object, so
  dependencies are visible in the constructor signature. Built the full
  solution, ran the complete test suite (all green, including
  `StuntCarRacerSharpLib.Tests`' 180 tests), and smoke-tested the built SCR app
  (starts and constructs its full DI graph, including `Race` and all four
  screens, without error).

### Added (Remaining EliteSharpLib Debug calls converted to logging, 2026-07-21)

- The last scattered `Debug.Fail`/`Debug.WriteLine` calls outside
  `Combat` are converted: `Space`'s three "Failed to create Planet/Sun"
  `WriteLine`s ([Space.cs](src/elite/libs/EliteSharpLib/Space.cs)) and one
  each in `ConstrictorMissionView`, `EscapeCapsuleView`, `Intro1View`,
  `Intro2View` — all legitimate runtime conditions (universe full) — now
  log a Warning via the existing `LogMessages.FailedToCreateShip`, same
  as the `GameOverView`/`Combat` exemplars. `Intro2View`'s parade-ship
  failure now logs the specific ship's name instead of a fixed
  "first Parade ship" string. `Space` and the four views all take an
  optional `ILogger<T>` (defaulting to `NullLogger<T>.Instance`) through
  their constructors, and `EliteServiceCollectionExtensions` resolves
  `ILoggerFactory` to pass each one through. This closes out the
  EliteSharpLib logging backlog item — every operational `Debug.*` call
  in the library is now converted. Built the full solution, ran the
  complete test suite (all green), and smoke-tested the built Elite app
  (starts, logs "Starting Elite - The Sharp Kind", and constructs its
  full DI graph — including every changed class — without error).

### Added (Combat's Debug.Fail calls converted to logging/exceptions, 2026-07-21)

- `Combat` was the heaviest concentration of `Debug.Fail` calls in
  `EliteSharpLib` (17, [Combat.cs](src/elite/libs/EliteSharpLib/Conflict/Combat.cs)),
  none of which survive a Release build. `Combat` now takes an optional
  `ILogger<Combat>` (defaulting to `NullLogger<Combat>.Instance`,
  following the `GameOverView` exemplar), and the 16 "Failed to create
  &lt;ship&gt;" cases — all legitimate runtime conditions (universe full)
  — log a Warning via the existing `LogMessages.FailedToCreateShip`.
  "Incorrect loot type" was a programming-error case (an unreachable
  `ShipType` branch), so it now throws `EliteException` instead —
  `LaunchLoot`'s if/else-if chain became a switch expression to keep the
  analyzer happy with the added throw arm.
  `EliteServiceCollectionExtensions.AddEliteMain` now resolves
  `ILoggerFactory` and passes `CreateLogger<Combat>()` through. Built the
  full solution, ran the complete test suite (all green), and
  smoke-tested the built Elite app (starts and constructs its full DI
  graph, including `Combat`, without error).

### Added (EliteSharpLib logging infrastructure + exemplar, 2026-07-21)

- `EliteSharpLib` had no way to log at all — every operational failure in
  the library went to `Debug.WriteLine`/`Debug.Fail`, which vanish in
  Release builds. Added a library-internal `LogMessages`
  `[LoggerMessage]` partial (`EliteSharpLib/LogMessages.cs`), following
  the same pattern as `Useful.Config.LogMessages` and the apps'
  `LogMessages.cs`. `Microsoft.Extensions.Logging.Abstractions` was
  already referenced by `EliteSharpLib.csproj`, and `ILoggerFactory` was
  already registered in the DI container (`SDLProgram`/
  `EliteServiceCollectionExtensions`), so only the per-consumer wiring
  was new.

  Proved the pattern end-to-end on `GameOverView` as the exemplar (a
  small, low-risk view with two `Debug.WriteLine("Failed to create
  ...")` calls): it now takes an optional `ILogger<GameOverView>`
  constructor parameter, defaulting to `NullLogger<GameOverView>.Instance`
  for tests/fakes, and logs both "failed to create ship" cases as a
  Warning (legitimate runtime condition — universe full — not a
  programming error) via `LogMessages.FailedToCreateShip`.
  `EliteServiceCollectionExtensions.AddEliteMain` now resolves
  `ILoggerFactory` and passes `CreateLogger<GameOverView>()` through.

  The remaining Debug.WriteLine/Debug.Fail call sites across `Combat`,
  `Space`, and the other four views convert in follow-up backlog items
  using this same infrastructure. Built the full solution, ran the
  complete test suite (all green), and smoke-tested the built Elite app
  (starts and constructs its full DI graph, including `GameOverView`,
  without error).

### Added (ConfigFile logs failures through the app's logger, 2026-07-21)

- `ConfigFile<T>`'s read/write failures went to `Debug.WriteLine` only,
  so nothing appeared in the Serilog file/console sinks the apps
  actually use — a purposefully-broken config (e.g. an invalid enum
  string) produced no trace in the log file at all, only in an attached
  debugger's Output window. `ConfigFile<T>` now takes an optional
  `ILogger<ConfigFile<T>>` (constructor overload, defaulting to
  `NullLogger<ConfigFile<T>>.Instance` — same pattern the backlog's
  library-logging item describes for `ILogger<T>` adoption) and a new
  library-internal `LogMessages` `[LoggerMessage]` partial
  (`Useful/Config/LogMessages.cs`) logs:
  - a Warning "Failed to read config file '{path}'; using defaults." —
    always visible at the apps' default Information level, no exception
    attached (so no stack-trace noise by default);
  - a separate Debug-level message carrying the actual exception, so the
    full stack trace only appears once `ELITE_LOG_LEVEL`/`SCR_LOG_LEVEL`
    is raised to `Debug`;
  - a Warning when the config fails validation (no exception, nothing
    was thrown);
  - an Error (with exception) on `WriteConfig` failure, alongside the
    existing `Debug.Fail` dev-time assertion (left as-is — orthogonal to
    logging).

  `AddEliteConfig`/`AddScrConfig` now resolve `ILoggerFactory` from the
  container (already registered by both `SDLProgram`s) and pass a
  `CreateLogger<ConfigFile<T>>()` logger through; `Useful`,
  `EliteSharpLib` and `StuntCarRacerSharpLib` each gained a
  `Microsoft.Extensions.Logging.Abstractions` package reference for
  this. `IsValidConfig` stayed `internal` (was already made so for the
  earlier DRY-unification tests) so `EliteServiceCollectionExtensions`
  can pass it as the validation predicate unchanged.

  Reproduced the exact reported scenario (`{"PlanetStyle": "QWERTY"}`)
  against the built app: at the default level the log file now shows
  `[WRN] Failed to read config file '...'; using defaults.` with no
  stack trace; with `ELITE_LOG_LEVEL=Debug` the full exception chain
  (down to `ConfigurationBinder.Bind`) appears too. Added
  `Useful.Tests` coverage (`RecordingLogger<T>` fake — Moq's generic
  `ILogger.Log<TState>` verification is awkward, a fake is simpler)
  asserting the Warning+Debug split on read failure and the
  no-exception Warning on validation failure.

### Changed (Logs moved under the shared TheSharpKind user-data folder, 2026-07-21)

- Both apps' Serilog file sink used a `logs` path relative to the
  process's current working directory, so where log files actually
  landed depended on how the app was launched (repo root via `dotnet
  run`, or next to the exe when run directly) — inconsistent with the
  config files, which are already rooted at a fixed
  `%AppData%\TheSharpKind`. `SDLProgram.Main` in both apps now computes
  `userDataPath` first and points the file sink at
  `Path.Combine(userDataPath, "logs", "elite-.log")` /
  `"scr-.log"`, so logs always land at
  `%AppData%\TheSharpKind\logs\` (`~/.config/TheSharpKind/logs/` on
  Linux/macOS) regardless of launch method. Serilog's file sink creates
  the `logs` subdirectory itself, same as it always has.

  Smoke-tested both built apps: confirmed `elite-<date>.log`/
  `scr-<date>.log` now appear under the shared user-data folder. Docs
  (`elite-readme.md`, `scr-readme.md`) updated to mention log location
  alongside the existing config-file description.

### Changed (Config file handling unified into a generic Useful type, 2026-07-21)

- Elite's `ConfigFile`/`IConfigWriter` and SCR's `ScrConfigFile` were
  near-identical (JSON read/write via `Microsoft.Extensions.Configuration`,
  same defaults-on-failure behaviour, same catch clauses) except for their
  settings type and filename — a DRY violation now that both games have
  one. Replaced both with a single generic `Useful.Config.ConfigFile<T>`
  (`where T : new()`), implementing a new generic `IConfigWriter<T>`, with
  the filename passed to the constructor and an optional
  `Func<T, bool> isValid` predicate for game-specific validation (Elite's
  `Fps > 0`/enum-range checks; SCR has none). `EliteSharpLib`'s
  `ConfigSettings` and `StuntCarRacerSharpLib`'s `ScrConfigSettings` stay put
  (they're genuinely game-specific), now bound as `ConfigFile<ConfigSettings>`
  / `ConfigFile<ScrConfigSettings>` respectively — `AddEliteConfig`/
  `AddScrConfig` and `SettingsView` (now `IConfigWriter<ConfigSettings>`)
  updated accordingly. `Microsoft.Extensions.Configuration`/`.Binder`/
  `.Json` package references moved from `EliteSharpLib.csproj`/
  `StuntCarRacerSharpLib.csproj` to `Useful.csproj`, the only project that now
  uses them directly.

  Adding those packages to `Useful` exposed a latent naming collision:
  `Useful.Maths.Extensions` triggered CA1724 (type name conflicts with
  the newly-referenced `Microsoft.Extensions` namespace) under this
  repo's warnings-as-errors build. Renamed it to `MathsExtensions` (pure
  rename — it's an extension-method container, so no call site needed
  updating) to unblock the build; unrelated to the config unification
  itself but a direct consequence of it.

  Added generic coverage in `Useful.Tests/Config/ConfigFileTests.cs`
  (defaults-when-missing, round-trip, mistyped-value fallback, failing
  validation) covering the shared logic once; `EliteSharpLib.Tests`/
  `StuntCarRacerSharpLib.Tests` keep their own `ConfigFileTests` but only for
  wiring specific to their real settings type/filename (Elite's also
  exercises the actual `IsValidConfig` predicate, now `internal` for
  testability). Full solution build and test suite pass; smoke-tested
  both built apps to confirm the DI wiring still resolves and runs.

### Fixed (Config parse failures crashed the game at startup, 2026-07-21)

- `ConfigFile.ReadConfig`/`ScrConfigFile.ReadConfig` only caught
  `IOException`/`UnauthorizedAccessException`/`FormatException` around
  `IConfiguration.Bind`, but `Microsoft.Extensions.Configuration.Binder`
  wraps type-conversion failures (e.g. a hand-edited `elitesharp.cfg`
  with a non-boolean string for `ShipWireframe`) in
  `InvalidOperationException`, not `FormatException` — so a corrupt or
  hand-edited config file crashed the whole game with an unhandled
  exception instead of falling back to defaults as intended. Both catch
  clauses now also match `InvalidOperationException`. Also tightened
  both `SDLProgram.Main`s: `GetRequiredService<EliteMain>()`/
  `<StuntCarRacerMain>()` (which triggers `ReadConfig` as part of DI
  composition) previously ran *before* the surrounding try/catch, so any
  composition-time exception bypassed the apps' own
  `LogMessages.CriticalAppTerminated` logging and crashed silently
  instead; moved the call inside the try block for both apps.

  Reproduced the exact reported crash (`elitesharp.cfg` containing
  `{"ShipWireframe": "hello!"}`) against the built app before and after
  the fix; added `ConfigFileTests`/`ScrConfigFileTests` regression cases
  covering the mistyped-value scenario, plus default-when-missing and
  write/read round-trip coverage that didn't exist for either config
  file before.

### Changed (Shared TheSharpKind user-data folder, 2026-07-21)

- Both apps' `userDataPath` (`SDLProgram.cs`) moved from their own
  per-game folder (`%AppData%\EliteSharp`, `%AppData%\StuntCarRacerSharp`) to
  a shared `%AppData%\TheSharpKind`, since they're both part of the same
  project. Elite's config filename (`elitesharp.cfg`) and commander
  saves (`.cmdr`) already avoided any collision by name, so it needed no
  change; SCR's config filename gained a `sharp` suffix
  (`stuntcarracer.cfg` → `stuntcarracersharp.cfg`) to stay unambiguous
  now it sits next to Elite's files in the same folder. Docs
  (`elite-readme.md`, `scr-readme.md`) updated to match and cross-link
  each other's Configuration section, now that the folder is shared.

### Added (SCR persisted settings, 2026-07-21)

- Gave SCR its own settings file, mirroring Elite's `ConfigFile`/
  `ConfigSettings` pattern: a new internal `ScrConfigSettings`/
  `ScrConfigFile` (`StuntCarRacerSharpLib/Config`) reads/writes
  `MusicOn`/`EffectsOn` to a JSON file (`stuntcarracersharp.cfg`) rooted
  at a user-data path, and a public
  `StuntCarRacerServiceCollectionExtensions.AddScrConfig(userDataPath)`
  (mirroring `AddEliteConfig`) registers it in DI and exposes the result
  as `Useful.Audio.AudioOptions` — the type `StuntCarRacerMain` already
  took at its `AudioController` construction site. `StuntCarRacerMain`
  gained a new public constructor overload accepting `AudioOptions` (the
  existing no-option constructors now default to `new()`, unchanged
  behaviour); `SDLProgram` wires `AddScrConfig` in ahead of the
  `StuntCarRacerMain` registration, same as Elite's `SDLProgram`.

  Skipped a settings-screen UI to write the file, following the same
  precedent as Elite's `ShipRenderMode` setting (2026-07-20): SCR has no
  settings screen at all today, so the config file remains the only way
  to change these values (still satisfies "toggle without code
  changes"). `FrameGap` and league selection stay out of scope, per the
  backlog item, as future candidates once a settings screen exists.

  Smoke-tested the built app: it starts, creates the shared user-data
  directory, and reads a hand-written config file
  (`MusicOn`/`EffectsOn` both `false`) without error. Added
  `ScrConfigFileTests` (default-when-missing, write/read round-trip) and
  two `StuntCarRacerMainTests` cases for the new constructor overload.

### Changed (Colour handling unified on FastColor, 2026-07-20)

- Phase 3 of unifying colour handling across Elite and SCR: `uint` →
  `FastColor` across the shared colour contract — `IGraphics` (and both
  implementations, `SoftwareGraphics`/`SDLGraphics`), `IPolygonRenderer`
  and its three strategies (`ZBufferRenderer`/`PainterRenderer`/
  `WireframeRenderer`), `IPaletteCollection`/`Palette`/`PaletteReader`
  (`Useful.Assets.Palettes`), `Face.Color` (`Useful.Assets.Models`), and
  SCR's `WorldPolygon.Colour`/`CarPalette`. Only the declared surfaces
  changed — internal call sites across Elite's ~110 `uint`-typed colour
  locals/fields (Views, Planets, Suns, Ships) and SCR's `ScrPalette`/
  `TrackRenderer` kept compiling unchanged via the implicit `uint`↔
  `FastColor` conversions added earlier the same day (see below), keeping
  the diff to 27 files. The exceptions needing edits were Moq test
  matchers (`It.IsAny<uint>()` → `It.IsAny<FastColor>()`), since Moq
  checks the literal parameter type rather than tolerating implicit
  conversions.

  Also resolves the open `[Useful.SDL] ToSDLColor decodes the colour as
  RGBA` defect: `ToSDLColor` now decodes via `FastColor.R`/`G`/`B`/`A`
  instead of hand-rolled bit-shifts, matching `SetRenderDrawColor` and
  every other colour in the codebase (ARGB).

### Changed (ScrPalette loaded from palette.json, 2026-07-20)

- Phase 2 of the colour-handling unification: `ScrPalette`'s hardcoded
  42-entry `uint[]` replaced with a JSON asset
  (`StuntCarRacerSharpLib/Assets/Palette/palette.json`) loaded through
  `Useful.Assets.Palettes.PaletteReader`/`IPaletteCollection` — the same
  mechanism `EliteDraw` already used for its named palette. `Colour(int)`
  stays static with a lazily-loaded backing store and keeps addressing
  colours positionally (`Track.ScrBaseColour + offset`, matching the
  original's `SCR_BASE_COLOUR`-relative scheme) rather than converting to
  an injected instance: `RoadTextures.Textures` resolves colours in a
  static field initializer at type-load time, and ~20 call sites across 8
  files address colours by numeric offset, so DI injection here would
  have meant a much larger, riskier change for the same goal.

### Changed (FastColor moved into the base Useful library, 2026-07-20)

- Phase 1 of the colour-handling unification: `FastColor`/`BaseColors`
  relocated from `Useful.Graphics` into the base `Useful` library, so
  `Useful.Assets` (which sits below `Useful.Graphics` in the dependency
  graph and owns `IPaletteCollection`/`Face.Color`) can reference it in a
  later phase without a circular project reference. No call-site changes
  needed elsewhere — `Useful.Graphics`/`Useful.SDL` already see it via
  C#'s enclosing-namespace lookup. Added implicit `uint`↔`FastColor`
  conversions so existing ARGB literals and `uint`-typed fields kept
  compiling as call sites migrated to `FastColor` over the subsequent
  phases above.

### Changed (CarMesh converted to a car.obj asset, 2026-07-20)

- SCR's `CarMesh` (the opponent's wedge-body/wheel-quad geometry,
  previously hardcoded vertex/quad arrays) now loads from a Wavefront OBJ
  asset (`StuntCarRacerSharpLib/Assets/Models/car.obj`), mirroring how Elite
  loads its ship models via `Useful.Assets.Models.ModelReader`. `CarMesh`
  changed from a static class to an instance constructed once (and
  injected into `OpponentRenderer`) instead of re-parsed on every track
  load; a new `CarPalette` resolves car.obj's five materials to
  track-palette colours since `Track.ScrBaseColour` is presently a fixed
  offset. Also fixed `.gitignore`'s blanket `*.obj` rule, which was
  silently excluding the new `car.obj` — only Elite's ship-model path was
  carved out as a genuine-OBJ exception; added SCR's models path
  alongside it.

### Changed (Polygon renderers moved to Useful.Graphics, 2026-07-20)

- `IShipRenderer`, `ShipRenderMode`, `PolygonData`, `WireframeRenderer`,
  `PainterRenderer` and `ZBufferRenderer` moved from
  `EliteSharpLib.Graphics` to `Useful.Graphics.Rendering`
  (`src/useful/libs/Useful.Graphics/Rendering/`) — after today's earlier
  work stripped their last Elite dependencies (`GameState`, the
  `ShipWireframe` check), none of the six referenced anything
  Elite-specific anymore; they only depend on `IGraphics`,
  `IAssetLocator` and `PaletteReader`, all already public in `Useful.*`.
  Renamed the two type names that said "ship": `IShipRenderer` →
  `IPolygonRenderer`, `ShipRenderMode` → `PolygonRenderMode`
  (`SubmitFace`/`faceColor` → `Submit`/`color` too, and
  `PolygonData.FaceColor` → `Color`); the three renderer classes
  (`WireframeRenderer`/`PainterRenderer`/`ZBufferRenderer`) didn't
  mention ships in their names already, so those stayed as-is. Elite's
  `ConfigSettings.ShipRenderMode` *property* keeps its name — that's
  Elite's own config key, just typed by the now-shared enum.
  `IShipRenderer`/the three renderer classes were `internal`; became
  `public` since Elite now consumes them from a different assembly
  (`PolygonData` stayed `internal`, used only inside the two chain-based
  renderers). Picked up two analyzer fixes moving to a fresh project
  (`CA1062` null-checks on the newly-public constructor/`Submit`
  parameters, `IDE0290` primary constructors) that hadn't fired while
  the types were `internal` to `EliteSharpLib`.

  Checked `StuntCarRacerSharpLib` for anything that duplicates this
  chain/depth-sort logic and could reuse it: no — `TrackRenderer`/
  `OpponentRenderer` submit `WorldPolygon`s straight to
  `Graphics.DrawPolygonFilledDepth` with no buffering step, so there's
  nothing to swap over, though `ZBufferRenderer` is the closer relative
  if SCR ever wants the explicit start/submit/end lifecycle. Also
  checked the rest of this session's changes (`AudioOptions`,
  `ConfigSettings`/`ConfigFile`) for the same "no longer game-specific"
  smell — `AudioOptions` was already correctly in `Useful.Audio`;
  `ConfigSettings`/`ConfigFile` hold genuinely Elite-only fields
  (`PlanetStyle`, `SunStyle`, the `elitesharp.cfg` filename) and stay put.

### Added (Selectable ship render mode, 2026-07-20)

- Last of the ship-rendering-strategy items: `ZBufferRenderer` and
  `PainterRenderer` are now purely filled renderers — the `ShipWireframe`
  branch (and the `GameState`/`IAssetLocator` dependencies it needed for
  the white outline colour) is gone from both, so `ZBufferRenderer`'s
  constructor is just `(IGraphics)` now and `PainterRenderer`'s
  unchanged. A new `WireframeRenderer` (`Graphics/WireframeRenderer.cs`)
  handles outline mode instead: since line order doesn't affect the
  result, it draws each submitted face immediately in `SubmitFace`
  rather than buffering a depth-sort chain at all. A new
  `ConfigSettings.ShipRenderMode` (`ShipRenderMode.Painter`/`ZBuffer`,
  defaulting `ZBuffer` — the current behaviour) selects between the two
  filled strategies; `EliteServiceCollectionExtensions`'s `IShipRenderer`
  registration now picks `WireframeRenderer`, `PainterRenderer`, or
  `ZBufferRenderer` based on `ShipWireframe`/`ShipRenderMode` at
  composition time, same as `Enum.IsDefined` validation as the other mode
  enums in `ConfigFile.IsValid`.

  Skipped the literal `FilledRenderer` wrapper the backlog item named:
  `ZBufferRenderer`/`PainterRenderer`, once stripped of their wireframe
  branch, already fully implement `IShipRenderer` as pure filled
  renderers, so a forwarding decorator would have added a class with no
  behaviour of its own. Also skipped a `SettingsView` toggle for the new
  setting — `ShipWireframe` itself has no in-game UI today either, so the
  config file remains the only way to switch (still satisfies "toggle
  without code changes").

  Added `VisualDumpTests.PainterAndZBufferRenderIdenticallyForNonDecalGeometry`,
  rendering a decal-free `Asteroid` model through both filled strategies
  and asserting pixel-for-pixel equality (passes exactly, no tolerance
  needed) — confirms the split didn't change either algorithm's actual
  behaviour. Also visually spot-checked `WireframeRenderer`'s output
  (a clean white ship outline, as expected).

### Changed (Z-buffer ship renderer renamed, 2026-07-20)

- Third of the ship-rendering-strategy items: `ShipRenderer` (the
  combined depth-sort/fill/wireframe behaviour extracted from `EliteDraw`
  earlier today) renamed to `ZBufferRenderer`, matching the
  `PainterRenderer` sibling added alongside it — no behaviour change,
  same registration in `EliteServiceCollectionExtensions.AddEliteMain`
  and the same direct construction in `PlanetBenchmarks`,
  `SunBenchmarks` and `VisualDumpTests`. The backlog item also asked to
  carry `ShipBase`'s face-root decal-inheritance logic
  (`FindFaceRoots`/`FaceMeanZ`) into this class; that logic stayed in
  `ShipBase` instead, deliberately deviating from the item's literal
  text — it computes the `z` value every `IShipRenderer.SubmitFace` call
  receives regardless of which strategy is active (`PainterRenderer`
  depends on the exact same computation), so moving it into
  `ZBufferRenderer` specifically would make `ShipBase` need to know which
  renderer is currently selected to compute a face's depth key, which
  defeats the point of the `IShipRenderer` abstraction landed earlier
  today. The open decal-seam defect this logic has (some decals lose to
  their base face at certain angles, from the 2026-07-14 z-buffer spike)
  is unchanged and untouched by this rename.

### Added (Painter's-algorithm ship renderer, 2026-07-20)

- Second of the ship-rendering-strategy items: a new
  `PainterRenderer : IShipRenderer` (`Graphics/PainterRenderer.cs`)
  restores the pre-2026-07-14-spike behaviour — a plain
  (non-depth-tested) `Graphics.DrawPolygonFilled` fill in back-to-front
  `_polyChain` order, instead of `ShipRenderer`'s per-pixel z-buffer test
  (`Graphics.DrawPolygonFilledDepth`) — as its own selectable
  implementation, without touching `ShipBase`'s face-transform code.
  Deliberately duplicates `ShipRenderer`'s chain-management code rather
  than factoring out a shared base class now, since the z-buffer item
  (still open) may reshape `ShipRenderer` further; `PainterRenderer` also
  skips the per-vertex `Depths` array entirely since the painter's fill
  never reads it. Not yet registered in the DI container — `ShipRenderer`
  stays the sole active implementation until the wireframe/filled item
  wires up config-driven selection between them. Verified with a
  throwaway test rendering the same interpenetrating-hulls scene as
  `VisualDumpTests`; both single-ship and intersecting-hull frames looked
  correct on visual inspection (not a pixel diff against the z-buffer
  path — that comparison is the wireframe/filled item's job).

### Added (Ship-rendering strategy abstraction, 2026-07-20)

- First of the ship-rendering-strategy items (backlog): a new internal
  `IShipRenderer` (`SubmitFace`/`StartFrame`/`EndFrame`,
  `Graphics/IShipRenderer.cs`) isolates the depth-sort/fill algorithm
  from `EliteDraw`, which previously hardcoded the back-to-front
  `_polyChain` chain and the `ShipWireframe` render-mode branch directly
  in `DrawPolygonFilled`/`RenderStart`/`RenderEnd` — the same fields the
  2026-07-14 z-buffer spike edited in place rather than toggled. Today's
  exact combined behaviour (chain, z-buffer fill, wireframe branch) moved
  unmodified into a new `ShipRenderer : IShipRenderer`
  (`Graphics/ShipRenderer.cs`); `PolygonData` moved into the same
  `Graphics` namespace/folder since it's now `ShipRenderer`-only.
  `EliteDraw`'s constructor takes an injected `IShipRenderer` and its
  three methods just delegate to it now. Registered as a singleton in
  `EliteServiceCollectionExtensions.AddEliteMain`, resolved before
  `EliteDraw`. This is pure extraction, not a split — the actual
  painter's/z-buffer/wireframe/filled separation is the three remaining
  backlog items. `PlanetBenchmarks`, `SunBenchmarks` and
  `VisualDumpTests` construct `EliteDraw` directly (not via DI) and
  needed a `ShipRenderer` passed in too; visually spot-checked its
  `frame_interpenetrate.bmp` dump (the two-hulls-intersecting per-pixel
  depth test) and it renders correctly, though this wasn't a pixel diff
  against a pre-change baseline.

### Added (Injectable AudioController options, 2026-07-20)

- `AudioController`'s `_musicOn`/`_effectsOn` were hardcoded `true` behind
  a pointless `#if DEBUG`/`#else` with identical branches; the constructor
  now takes a new `Useful.Audio.AudioOptions` (`MusicOn`/`EffectsOn`, both
  defaulting `true`) instead. Elite's `ConfigSettings` gained matching
  `MusicOn`/`EffectsOn` properties (default `true`, no behaviour change),
  and `EliteServiceCollectionExtensions`'s `AudioController` registration
  now builds the options from the already-resolved `GameState.Config`
  rather than a second `ConfigFile.ReadConfig()` call. SCR
  (`StuntCarRacerMain`) has no settings infrastructure at all yet, so it
  passes a plain default `AudioOptions` at its construction site — same
  always-on behaviour as before, just explicit instead of hardcoded
  inside `AudioController`. Added `PlayEffectDoesNothingWhenEffectsAreOff`
  and `PlayMusicAndStopMusicDoNothingWhenMusicIsOff` to
  `AudioControllerTests`, which needed `FakeSound.Play(string, bool)`/
  `StopMusic()` to actually count calls (previously no-ops).

### Added (Elite view registrations in the container, 2026-07-20)

- The last composition-root item: `EliteMain` no longer constructs its ~25
  `IView` screens itself. Each view (`Intro1View` through `GameOverView`)
  is now registered as a singleton in a new private
  `EliteServiceCollectionExtensions.AddEliteViews`, resolved by factory
  delegate for the same internal-accessibility reason as the domain
  services. `AddEliteMain`'s `EliteMain` factory now populates the
  `ScreenManager<Screen, IView>` singleton (`views.Add(Screen.X, ...)` for
  all 25 screens) by resolving each view from the container before
  constructing `EliteMain` — this has to happen there rather than inside
  `AddEliteViews` itself, since IServiceCollection registration is lazy
  and population needs actual built instances. `EliteMain`'s constructor
  shrinks accordingly: `Trade`, `PlanetController`, `IShipFactory`,
  `ConfigFile` and `ScreenManager<Screen, IView>` are all gone from its
  parameter list (they were only ever used to build views), leaving just
  the collaborators its `Update`/`Draw`/`Run` logic actually touches.

### Added (Elite domain services in the container, 2026-07-20)

- `EliteMain`'s constructor no longer builds its own domain graph: `GameState`,
  `PlayerShip`, `Trade`, `PlanetController`, `EliteDraw` (as `IEliteDraw`),
  `IShipFactory` (via `ShipFactory.Create`), `Universe`, `Stars`, `Pilot`,
  `Combat`, `SaveFile`, `Space`, `Scanner`, `AudioController` and the
  `ScreenManager<Screen, IView>` backing `GameState` are all now registered
  as singletons in `EliteServiceCollectionExtensions.AddEliteMain` (which
  also forwards `AssetLocator` as `IAssetLocator`), and `EliteMain`'s
  constructor just receives them. Since every one of these types is
  `internal` to `EliteSharpLib`, the container can't auto-wire them by
  reflection (that only sees public constructors), so each is registered via
  an explicit factory delegate rather than a bare `AddSingleton<T>()` — this
  is deliberately verbose over hiding it behind a bigger, riskier
  internal-to-public accessibility change. The `AudioController` `SfxSample`
  cooldown table (`// TODO: improve this`) moved into a private
  `BuildEliteSfx` helper alongside the other registrations, unchanged.
  `EliteDraw`'s `_colorText = _draw.Palette["White"]` read and `SaveFile`'s
  `ConfigFile.BaseDirectory` path both stay exactly where they were,
  now sourced from the injected `IEliteDraw`/`ConfigFile`.
  `EliteMain` still builds the ~25 views itself — that's the next backlog
  item.

### Added (Elite composition root, 2026-07-20)

- Mirrors the SCR composition root: `EliteSharp.SDLProgram.Main` builds a
  `ServiceCollection` instead of `new`-ing `SoftwareAbstraction` and
  `EliteMain` directly, registering `IAbstraction` and its forwarded
  `IGraphics`/`ISound`/`IKeyboard`, the Serilog `ILoggerFactory`, and
  `AssetLocator`. `ConfigFile` (and `EliteMain` itself, whose constructor
  now takes it) are `internal` to `EliteSharpLib` with no
  `InternalsVisibleTo` for the `EliteSharp` app, so `Program.Main` can't
  register or construct them directly; new `EliteServiceCollectionExtensions`
  in `EliteSharpLib` adds `AddEliteConfig(userDataPath)` and `AddEliteMain()`
  extension methods that do this from inside the assembly that can see
  those types, and registers `EliteMain` as `IGame` too. `EliteMain`'s
  constructor now takes `AssetLocator`/`ConfigFile` as parameters instead
  of creating them (the user-data path moved out to `Program.Main`, with
  `ConfigFile` gaining an internal `BaseDirectory` property so `EliteMain`
  can still build `SaveFile`'s path from it), and is `internal` rather
  than `public` since only the in-assembly registrar factory calls it
  now. `EliteMain`'s constructor otherwise keeps building the rest of the
  domain graph as before — that's the separate "move Elite's domain
  services into the container" item.

### Added (SCR composition root, 2026-07-20)

- `StuntCarRacerSharp.SDLProgram.Main` now builds a `ServiceCollection`
  (`Microsoft.Extensions.DependencyInjection`, newly referenced by the
  `StuntCarRacerSharp` project) instead of `new`-ing `SoftwareAbstraction` and
  `StuntCarRacerMain` directly: `SoftwareAbstraction` is registered as
  `IAbstraction` via a factory (container-owned and disposed with the
  provider), `IGraphics`/`ISound`/`IKeyboard` are forwarded from it, the
  existing Serilog-backed `ILoggerFactory` is registered as an instance,
  and `StuntCarRacerMain` is registered as itself and as `IGame`. `Main`
  resolves the concrete `StuntCarRacerMain` (not `IGame`, which has no
  `Run`) and calls `.Run()`. This is the first of the composition-root
  items — Elite's `Program.Main` mirrors the pattern next.

### Added (SCR per-effect sound volume/pitch/pan, 2026-07-20)

- Sound effects now vary per play instead of always sounding identical,
  matching the Amiga original's `DSSetMode`/`DrawDustClouds`/`DrawSparks`/
  `UpdateDamage` behaviour: Creak and Grounded volume scale with impact
  damage (`CarPhysics.CalculateDamageVolume`, shared by both, matching
  the original's identical formula in both places), and the off-road/
  edge-scrape sounds are pitched (randomly for off-road, by speed for
  the edge scrape) instead of playing at a flat pitch. Each effect also
  gets the original's fixed stereo pan (engine and Smash left,
  everything else right) and HitCar's fixed quieter volume.
  `Useful.Audio` grew the plumbing for this: `ISound.Play` and
  `AudioController.PlayEffect` take volume/pan/pitch, `SfxSample` carries
  a per-effect static volume/pan profile, and `SDLSound` implements
  pitch-shifted one-shots with the same resample-on-a-reserved-channel
  technique the engine loop already used (a new second reserved channel,
  stopping after one pass instead of looping). Elite's 9 `PlayEffect`
  call sites are unaffected (same simple no-args overload).
  Deliberate deviations: skips the original's `AmigaVolumeToDirectX` dB
  round-trip since SDL_mixer's volume is already linear (see
  `CalculateDamageVolume`'s comment for the maths); the two pitched
  effects have no "recorded rate" reference in the original either (both
  always override the frequency), so pitch=1.0 is anchored at each
  formula's own range midpoint (464 for off-road, 360 for the edge
  scrape) rather than a verified original value. Covered by new
  `AudioControllerTests` (volume/pan/pitch pass-through) and
  `CarPhysicsTests` (damage-scaled volume in range, off-road/wreck pitch
  in the derived ranges); actual audio output was not manually verified
  by ear.

### Added (SCR lap times, 2026-07-20)

- The dashboard now shows a current-lap clock and best-lap time
  (`T0:00.00`/`B0:00.00`, `M:SS.CC`), mirroring the Amiga original's
  `print.lap.time`/`show.lap.time` read-outs. `CarPhysics` tracks
  `CurrentLapTicks` (advanced once per 50Hz `ApplyEngineRevs` tick,
  the same hook the wheel-spin-rate fix uses) and `BestLapTicks`
  (updated and the current lap reset at each lap boundary in
  `UpdateLapData`); `HudRenderer` formats ticks as `M:SS.CC` and draws
  them beside the existing lap/boost/distance read-outs. Deliberate
  deviation: this is a straightforward wall-clock timer, not a port of
  the original's 3-byte BCD stopwatch, whose exact increment/wrap
  semantics were not fully reverse-engineered from the raw disassembly
  (ptitSeb never implements this feature, so there is no clean C++
  reference to check against). The follow-up "BCD fidelity" backlog
  item was closed won't-fix 2026-07-20: without a working emulator to
  verify the exact wrap semantics against, chasing byte-for-byte
  fidelity risks silently reproducing a misread of the disassembly,
  and the current wall-clock-accurate timer is arguably the more
  useful behaviour anyway. Dashboard placement is a
  reasonable slot (confirmed to fit an existing empty panel via
  `VisualDumpTests`), not verified against the original's exact
  layout. Covered by new `CarPhysicsTests` (tick advance only via
  `ApplyEngineRevs`, reset on new race, full-lap best-time recording)
  and a `HudRendererTests` case for the current/best text.

### Fixed (SCR cockpit wheel-spin rate, 2026-07-20)

- Cockpit front wheel sprites were spinning at a quarter of their
  correct rate: the wheel-angle advance lived in `CarMovement`, which
  only runs every `FrameGap`-th tick (12.5Hz), instead of the original
  `FramesWheelsEngine`'s full 50Hz rate. Split `SetWheelRotationSpeed`
  into a physics-rate speed calculation (unchanged) and a new
  `AdvanceWheelAngles`, now called from `CarPhysics.ApplyEngineRevs` —
  already the 50Hz hook, called every tick from `RaceScreen.Update` —
  keeping the original's right-wheel-reads-left-angle quirk. Covered by
  a new `CarPhysicsTests.WheelAnglesAdvanceOnEveryEngineRevsTickNotJustPhysicsFrames`
  test that drives `ApplyEngineRevs` alone (no physics frames) and
  checks the wheel frame moves.

### Changed (SCR full damage wrecks the car, 2026-07-20)

- `CarPhysics.Wrecked` now goes true when `NewDamage` reaches 240,
  matching the Amiga original's `damage.line`/`car.is.wrecked` (the HUD
  crack previously just capped out with no effect). The C# port already
  carried the rest of the Amiga's (and ptitSeb's dormant, never-triggered)
  wreck plumbing — `_wreckWheelHeightReduction`, the wheel-height
  subtraction, the `!Wrecked`-gated engine power/boost cuts, and the
  scrape-sound gate — so the only change is setting that field once the
  damage threshold is crossed; those existing paths do the rest. The race
  flow is otherwise unchanged (the opponent still finishes and the race is
  lost). Covered by new `CarPhysicsTests` (`FullDamageWrecksTheCar`,
  `PartialDamageDoesNotWreckTheCar`, `WreckedResetsOnNewRace`).

### Changed (SCR opponent speed values, 2026-07-19)

- The opponent's per-piece required speeds are now computed by a port of
  ptitSeb's `Opponent_Speed_Value()` (itself derived from the Amiga's
  `opponents.speed.values` creation assembly) instead of the old
  fluffyfreak per-track random tables: a per-track random mask and base
  from the full 64-byte `opp_track_speed_values` table (now carried
  verbatim in `OpponentData.TrackSpeedValues`, super-league rows
  included for the future Super League item), ten faster on sections
  the car can be put on, memoized so the value only re-rolls when the
  opponent changes piece. Two deliberate choices beyond the reference:
  the can-be-put-on test uses bit 7 as the Amiga's `bpl` does (ptitSeb
  tests `b < 0` on an unsigned value, which never fires), and the draw
  bridge's `SetSpeedValue` writes still take precedence via a per-piece
  override (the Amiga modified its precomputed table; ptitSeb's own
  writes go unread since nothing reads the table any more — a
  regression not copied). Covered by new `SpeedValue` unit tests
  (deterministic standard-league values, RNG-stream stability while on
  a piece, draw-bridge override precedence) and smoke-tested live.

### Audited (SCR ptitSeb parity, 2026-07-19)

- Ran a full feature-by-feature comparison of the C# port against
  `ptitSeb/stuntcarremake` (see the backlog's "Resolved (2026-07-19) —
  ptitSeb parity audit" note for the findings). One backlog item was
  found already complete and removed: **opponent name announcement** —
  `StuntCarRacerMain.DrawHud` has shown "Opponent: <name>" for the
  first four seconds of a race since the atlas HUD work (commit
  2a476b4); the remaining gaps were re-verified and tightened into
  discrete backlog items (wreck-at-full-damage plumbing references,
  wheel-spin tick rate, sound frequency/volume/pan table, mid-race 'M'
  side effects, and the unplugged racewin/racelost/wrecked/heads art
  screens).

### Fixed (Elite ship rendering, 2026-07-14)

- Elite's ships now render through the shared software z-buffer
  (`DrawPolygonFilledDepth`, the path Stunt Car Racer already used)
  instead of pure painter overdraw, fixing the long-standing "bits of
  hidden surfaces show through" artefact — with the max-Z face sort
  (checked against The New Kind's `threed.c`: `zavg` = MAX is
  authentic, not a porting bug), far-side decals and detail lines beat
  near hull faces that wrap toward the tail. Each face now rasterizes
  with one flat per-pixel depth: the mean Z of its "root" face's
  transformed points. `ShipBase` computes face roots once per instance
  from the model geometry — decal faces (cockpit windows, engine
  plates) and 2-point detail lines lie exactly (distance 0.000) in the
  plane of an earlier larger face and inherit that face's key, so they
  tie exactly and the chain's later-submission tie order draws them on
  top, the convention the models were built for. Flat rather than
  interpolated per-vertex depth is deliberate: measurement showed the
  rasterizer's clamped edge interpolation deviates from a coplanar
  face's plane by far more than any safe bias, punching seam-shaped
  holes through decals (Transporter panel, Cobra engine plates).
  Wireframe mode is unchanged apart from the sort key. This supersedes
  the backlog defect about the `zavg` max-vs-mean sort. Verified with a
  new `VisualDumpTests` that renders lone-ship, decal-heavy
  (Transporter spin, rear Cobra) and overlap/interpenetration scenes
  through the real rasterizer to BMPs for visual inspection.

### Changed (user data location, 2026-07-12)

- `ConfigFile` and `SaveFile` (`.cmdr` commander saves) now resolve their
  files against an injected base directory instead of the current working
  directory. `EliteMain` computes the default once
  (`%AppData%\EliteSharp` on Windows, `~/.config/EliteSharp` on
  Linux/macOS via `Environment.SpecialFolder.ApplicationData`) and passes
  it to both, fixing the "launched from a shortcut" breakage where the CWD
  wasn't the app's install directory, and making both classes testable
  against a temp directory. The shipped default `elitesharp.cfg` next to
  the executable is no longer read and was removed.

### Changed (config, 2026-07-12)

- `ConfigFile.ReadConfig` now reads `elitesharp.cfg` (renamed from
  `sharpkind.cfg`) through
  `Microsoft.Extensions.Configuration`'s JSON provider (bound onto
  `ConfigSettings`) instead of `System.Text.Json.Deserialize`, with startup
  validation (`Fps > 0` and each enum value in range) falling back to
  defaults on failure. Writing now goes behind a new `IConfigWriter`
  interface, which `SettingsView` depends on instead of the concrete
  `ConfigFile`.

### Fixed (logging, 2026-07-12)

- Both apps' logs were invisible without a debugger (Serilog's only sink was
  `Debug`, minimum level Verbose). Switched to a console sink plus a rolling
  daily file (`logs/elite-.log` / `logs/scr-.log`, 7 days retained), minimum
  level Information, overridable via the `ELITE_LOG_LEVEL` /
  `SCR_LOG_LEVEL` environment variables.

### Fixed (SCR track rendering, 2026-07-12)

- Track visibility artifacts (white triangles on corners, triangular bites
  in the track edges up close, spurious triangles on the side walls, torn
  bottom edge): the painter's sort (one averaged depth per segment) was
  replaced with a software z-buffer in `SoftwareGraphics`
  (`ClearDepth`/`DrawPolygonFilledDepth`/`DrawPolygonTexturedDepth`, 1/z
  depth test, perspective-correct textured fill), matching the original
  remake's Direct3D z-buffered `DrawTrack`; `Scene3D` now clips in float at
  the remake's 0.5-unit near plane instead of the Amiga fixed-point
  engine's integer `Z_CLIP_BOUNDARY = 128`. Also closes the backlog's
  "near-road sliver artifact" defect (no longer reproduces in
  `VisualDumpTests` frames) and obsoletes its `TrackRenderer`
  double-transform cleanup item (that code path was removed).
- View dipping under the track surface on bumpy landings: ported the
  remake's `LimitViewpointY` (road "tearing" prevention) into `CarPhysics`
  and wired it into `SceneCamera.FollowCar`, with unit tests.

### Removed

- Dead NAudio-backed audio stack: `SoftwareSound`, `SoundSampleProvider`
  and `PitchedLoopSampleProvider` had zero production references since
  `SoftwareAbstraction` switched to `SDLSound`, and `AudioController`'s
  `GenerateWaveFromMidi`/`WriteStereoWav` helpers were never called; deleted
  along with the `NAudio`/`NAudio.Vorbis`/`MeltySynth` package references in
  `Useful.Audio.csproj` and `EliteSharpLib.csproj`.

### Fixed (2026-07-11 architecture review — all Must items, plus one Should)

- Cross-platform audio: `SoftwareAbstraction` now uses the SDL_mixer-backed
  `SDLSound` instead of NAudio's Windows-only `WaveOutEvent`, fixing startup
  on Linux/ARM64. Follow-ups fixed in the same pass: mixer channel count
  raised 2 → 16 with channel 0 reserved for the engine loop (overlapping
  effects no longer crash with "No free channels available"), a dropped
  one-shot effect is no longer treated as fatal, and pitch-shifted looping
  was implemented in `SDLSound` via a `Mix_RegisterEffect` resampler
  (mirroring `PitchedLoopSampleProvider`'s algorithm) so SCR's engine sound
  works on all platforms.
- `SDLInput` no longer force-quits on Escape — quit policy belongs to each
  game, and Elite's ESC = launch escape capsule works again. Window close
  (`SDL_QUIT`) still exits.
- Elite contraband calculation counted Slaves twice and Narcotics never;
  now `(slaves + narcotics) * 2 + firearms` per the original, with unit
  tests.
- A missing or malformed `sharpkind.cfg` no longer crashes Elite at
  startup; it falls back to default settings.
- `SDLSound.Dispose` freed music/sfx handles with the wrong SDL_mixer APIs
  and double-freed music; now `Mix_FreeMusic` for music, `Mix_FreeChunk`
  for effects, once each.
- `SoftwareGraphics` rectangle drawing clamped Y against the screen *width*
  and overshot by one pixel — out-of-bounds crash / wrong-row artifacts on
  non-square screens (SCR is 640x400). Regression tests added.
- `FastBitmap.Resize` read one row/column past the source bitmap when
  growing. Tests added.
- `Space.JumpWarp` crashed with a NullReferenceException in witchspace
  (no planet/sun) once all Thargoids were destroyed.

### Added

- Root `README.md` landing page linking both games, docs and changelog.
- Consolidated all planning docs (`issues.md`, `release-plan.md`,
  `scr-conversion-plan.md`) into a single TODO list at
  `docs/review-findings.md`; folded `config.md` into the Elite readme;
  renamed `docs/readme.md` → `docs/elite-readme.md` and
  `docs/images/screenshot.png` → `elite-screenshot.png`.
- Business-application practices section in `docs/architecture-principles.md`
  (composition root/DI, logging, configuration, error handling, lifetimes,
  testability seams).

## Stunt Car Racer conversion — progress to date

Summarised from the retired `scr-conversion-plan.md` "Done" list. Remaining
conversion work now lives in the [backlog](docs/backlog-roadmap.md).

- Project skeleton (`src/scr/*`) wired into the solution; builds clean.
- Track geometry/data model from the original Amiga track data, including
  the draw bridge animation.
- Car physics (fixed-point, ported from the original 68000 algorithms):
  sound triggers, car-to-car collision/slipstream, wheel rotation/bounce,
  smash-hole tracking; `DisplaySpeed` matches ptitSeb's revised formula.
- Opponent AI: scripted speeds, wheel-spring dynamics, steering,
  obstruct/push/move-aside interaction, lap counting and win calculation.
- 3D projection/camera pipeline (`Scene3D`, `SceneCamera`, `ScrPalette`)
  with flat-shaded and textured polygons via `Useful.Graphics`.
- Backdrop/horizon/scenery rendering, five scenery types, N cycles them.
- Car mesh (wheels + wedge body) used to draw the opponent.
- HUD: bitmap-font text overlays plus the graphical cockpit dashboard
  ported from ptitSeb's `DrawCockpit`, sprited from one converted
  `atlas.bmp` via the new `IGraphics.DrawImagePart`.
- Track menu background drawing ptitSeb's `menu.png` over the 3D world.
- Sound via `Useful.Audio`: variable-pitch engine loop, effect triggers,
  samples converted to WAV assets.
- Game-mode/screen flow (TrackMenu, TrackPreview, Race, GameOver) with
  camera orbit/preview logic and track selection; full game loop wired up,
  all eight tracks drivable end-to-end, keyboard-controlled.
- Fixed-timestep game loop, screen state machine and sound-effect
  throttling shared with Elite (`Useful.Timing.GameLoop`,
  `ScreenManager<TId, TScreen>`, `AudioController`/`SfxSample`).
- Floating-track and spurious-triangle rendering bugs fixed with
  regression coverage; road-line textures regenerated from the palette and
  textured onto the road ±11 segments around the player.
- Control scheme rewritten to match ptitSeb (independent
  accelerate/brake/boost), fixing two pre-existing shared keyboard bugs on
  the way (wrong SDL Right-Arrow mapping; one-shot `IsPressed` breaking
  continuous controls — added non-consuming `IKeyboard.IsHeld`).
