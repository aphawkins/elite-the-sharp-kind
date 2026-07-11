# Release plan

How to get this repo from "actively developed" to "something anyone can
download and run." The bar is **polished, not perfect** — a hobby project
release, not a commercial ship. Two games live here (Elite, playable;
Stunt Car Racer, mid-conversion — see `scr-conversion-plan.md`) sharing the
`Useful.*` libraries, so a few decisions below apply to the repo as a
whole rather than one game.

## Decision: what actually ships in v1

Elite is feature-complete and playable end-to-end. SCR is explicitly
"in development" (`scr-readme.md`) with a long remaining-work list
(float-conversion, gamepad/joystick, Super League, chase view, several
known bugs — see `scr-conversion-plan.md`'s Remaining work). Recommendation:

- **v1.0**: Elite only. It's done, tested, and already builds/publishes in
  CI.
- **SCR ships separately** once its own remaining-work list is short
  enough to call done, either as a v1.1/v2.0 that adds it alongside Elite,
  or as its own tagged pre-release (`scr-v0.1-preview`) if it's worth
  putting in front of people before it's fully done. Don't block Elite's
  release on SCR.

This is a judgement call, not a fact — revisit if priorities differ.

## Current state (as of this writing)

- **CI** (`.github/workflows/build-and-package.yml`): builds the whole
  solution, runs all tests with coverage on every push/PR to `master`,
  publishes Elite as self-contained single-file builds for `win-x64` and
  `linux-x64`, uploads both as workflow artifacts. It does **not** publish
  SCR (no publish profile exists for `StuntCarRacer.csproj` yet), and
  nothing turns those artifacts into a GitHub Release — they just sit in
  the Actions run.
- **Versioning**: CI stamps every build `0.<yyddd>.<run number>.0` (date +
  run number), not tied to a git tag or semantic version. No git tags and
  no GitHub Releases exist in this repo yet.
- **Tests**: 273 tests across 8 test projects, all passing
  (`Useful.*.Tests` ×6, `EliteSharpLib.Tests`, `StuntCarRacerLib.Tests`).
  Coverage is collected (`dotnet-coverage`, cobertura format) and uploaded
  as a CI artifact, but nothing reads it — no badge, no threshold, no
  tracked trend.
- **Static analysis**: Roslynator, SonarAnalyzer.CSharp and
  StyleCop.Analyzers are already wired into every project with
  `TreatWarningsAsErrors=true` (`Directory.Build.props`), so most
  complexity/quality issues already fail the build rather than needing a
  separate pass. Package versions are pinned per-project rather than
  centrally (no `Directory.Packages.props`) — fine at this scale, worth
  revisiting if it drifts.
- **Shared libraries** (`Useful.*`): consumed only via in-repo
  `ProjectReference`, not packaged or versioned independently. No NuGet
  metadata (`PackageId`, `IsPackable`, etc.) anywhere.
- **Docs**: `docs/readme.md`, `docs/scr-readme.md`, `docs/config.md`,
  `docs/issues.md`, plus root `CONTRIBUTING.md`/`CODE_OF_CONDUCT.md`/
  `SECURITY.md`/`LICENSE` (MIT). **There is no root `README.md`** — GitHub
  renders whatever's at the repo root as the project homepage, and right
  now that's nothing. This is the single highest-value fix in this whole
  plan.

## Checklist

### 1. Feature completeness / bug bar

- [ ] Confirm Elite has no release-blocking bugs open (check
      `docs/issues.md` — currently a loose TODO list, not triaged by
      severity; split it into "blocks release" vs "known limitation,
      documented and fine to ship").
- [ ] If SCR ships in this release: work the release-blocking subset of
      `scr-conversion-plan.md`'s Remaining work (at minimum: whatever
      currently-broken behavior would embarrass to ship — check that list
      fresh rather than assuming it's still accurate).
- [ ] Decide what's a documented known-limitation vs a must-fix. Not
      everything in `issues.md`/the conversion plan needs fixing before
      v1 — it needs a decision, recorded somewhere, so it doesn't look
      like an oversight.

### 2. Tests & quality gates

- [ ] Full suite green on the release commit (`dotnet test EliteSharp.slnx`)
      — already true as of this writing, keep it that way.
- [ ] Decide if coverage needs a visible number (e.g. a badge from the
      existing cobertura output, or a Codecov/Coveralls upload step) or
      if "collected but not gated" is fine for v1. A number with no
      target isn't very actionable — pick one or drop the collection step.
- [ ] Nothing else needed here specifically for "complexity" — the
      analyzer stack (Roslynator/Sonar/StyleCop) already gates that at
      build time. If a specific complexity metric matters (cyclomatic
      complexity trend, duplication %), that's a SonarCloud/SonarQube
      integration, which is a bigger lift than this release needs — treat
      as a future nice-to-have, not a blocker.

### 3. Shared libraries (`Useful.*`) & NuGet

- [ ] Decide if `Useful.*` gets published to NuGet.org for this release.
      Nothing outside this repo consumes them today, so packaging them is
      about enabling *future* reuse, not something v1 needs functionally.
      Recommendation: **skip for v1**, revisit if/when something outside
      this repo wants to depend on them. Packaging six libraries
      (`Useful`, `.Abstraction`, `.Assets`, `.Audio`, `.Controls`,
      `.Graphics`) with proper `PackageId`/`Description`/`RepositoryUrl`
      metadata, a consistent version, and API-stability considerations is
      real work that doesn't move the games forward.
- [ ] If packaging them anyway: add `IsPackable`/`PackageId`/
      `PackageDescription`/`RepositoryUrl`/`PackageLicenseExpression` to
      each, a `dotnet pack` CI step, and a decision on whether package
      versions track the app version or version independently (they'll
      likely need to eventually, since libraries and apps evolve at
      different rates — just not necessarily *this* release).

### 4. Versioning

- [ ] Switch from the date+run-number scheme to semantic versioning
      (`MAJOR.MINOR.PATCH`) driven by a git tag, e.g.
      [MinVer](https://github.com/adamralph/minver) or
      [Nerdbank.GitVersioning](https://github.com/dotnet/Nerdbank.GitVersioning)
      — either derives `Version`/`FileVersion`/`InformationalVersion` from
      the latest `v*` tag automatically, so tagging a commit *is* the
      release-versioning step instead of a separate manual one.
- [ ] Decide the first tag: `v1.0.0` if Elite is considered stable/done,
      or `v0.1.0` if you'd rather signal "early access." Either is
      defensible; `v1.0.0` fits better given Elite is actually feature-
      complete, not a preview.
- [ ] Keep the CI workflow's manual version-stamping as a fallback only
      for untagged builds (or drop it once tag-driven versioning lands).

### 5. Release packaging (installer vs. zip vs. single-file)

- [ ] Elite already builds self-contained single-file executables for
      win-x64/linux-x64 (`PublishSingleFile=true`,
      `IncludeNativeLibrariesForSelfExtract=false`,
      `PublishReadyToRun=true`) — this is already the right shape for a
      hobby-project release: no installer needed, just "download, unzip
      (or chmod +x), run." **Recommendation: ship as zip/tar.gz of the
      publish output, not an installer** — an installer (MSIX, Inno Setup,
      a `.deb`) is real ongoing maintenance (code signing, update
      channels, uninstall hooks) for a project this size, and the
      self-contained exe already avoids needing the .NET runtime
      preinstalled.
- [ ] Add a publish profile + CI publish/artifact step for
      `StuntCarRacer.csproj` mirroring Elite's, once SCR is in scope for a
      release. Same `win-x64`/`linux-x64` shape.
- [ ] Add a `PublishProfile` (or `RuntimeIdentifier`) for `osx-x64`/
      `osx-arm64` if macOS support is wanted — currently untested on any
      Mac per `docs/readme.md`'s tested-platform list (Windows 10 x64,
      Ubuntu 24.04 x64, Raspberry Pi 4 ARM64). Otherwise, state clearly in
      the release notes which platforms are actually supported.
- [ ] Consider a `linux-arm64` publish target explicitly, given the
      Raspberry Pi 4 is already a stated supported platform but isn't one
      of CI's two publish targets today.

### 6. GitHub release mechanics

- [ ] Tag the release commit (`git tag v1.0.0 && git push origin v1.0.0`).
- [ ] Add a CI job (or a manual step, to start) that runs only on tag
      push, downloads/re-publishes the artifacts, and creates a GitHub
      Release via `gh release create` or the
      `softprops/action-gh-release` Action, attaching the win-x64/
      linux-x64 zips.
- [ ] Write release notes. A `CHANGELOG.md` doesn't exist yet — either
      start one (Keep a Changelog format is a reasonable default) or rely
      on the GitHub Release body plus `gh release create --generate-notes`
      (auto-generates notes from merged PRs, which works well if PR titles
      are already descriptive).
- [ ] Decide pre-release vs. full release flagging on GitHub (relevant if
      SCR ships as a preview per the scope decision above).

### 7. Documentation

- [ ] **Add a root `README.md`.** This is the single most visible gap —
      GitHub shows nothing on the repo homepage today. Content: what the
      project is, a screenshot (already have `docs/images/screenshot.png`
      for Elite), links to `docs/readme.md`/`docs/scr-readme.md` for full
      per-game detail, download/run instructions, and a licence line.
      Doesn't need to duplicate the per-game docs — a short landing page
      that links out is enough.
- [ ] Add download/install instructions once release artifacts exist
      ("download the zip for your OS from Releases, extract, run").
      `docs/readme.md` currently says "the dotnet runtime 8 will need to
      be installed until such time a self-contained exe is published" —
      that time has arrived (Elite already publishes self-contained); the
      wording needs an update either way — either to reflect that, or to
      correct the runtime version reference in the meantime (Elite/SCR
      target net10.0, not 8, per `Directory.Build.props`).
- [ ] Cross-link `docs/readme.md` ↔ `docs/scr-readme.md` ↔ the new root
      `README.md` so a visitor can actually find both games from wherever
      they land.
- [ ] `docs/issues.md` reads as an internal scratch TODO list — fine to
      keep as-is for maintainers, but don't link it from the public-facing
      README; link (or fold into) GitHub Issues instead if you want an
      external-facing bug tracker.

## Suggested order of operations

1. Root `README.md` (cheap, highest visibility win).
2. Versioning switch (MinVer/Nerdbank.GitVersioning) — do this before the
   first tag, not after, so the first release already has a clean version.
3. Triage `docs/issues.md` and the SCR remaining-work list against the v1
   scope decision; fix release-blockers, explicitly punt the rest.
4. Add the tag-triggered release-artifact/GitHub-Release CI job.
5. Tag `v1.0.0`, let CI produce the release, write/generate notes.
6. Everything else (NuGet packaging, macOS/arm64 targets, coverage
   badges, installers) is a fast-follow, not a blocker.

## Open questions for the maintainer

These need a decision, not just a task:

- Does v1 ship Elite only, or Elite + SCR together?
- `v1.0.0` or `v0.1.0` for the first tag?
- Is a visible coverage number worth the CI/badge setup, or is "tests are
  green" enough?
- Any interest in NuGet-packaging `Useful.*` for this release, or defer
  entirely until something outside this repo wants to consume them?
- Which platforms are actually claimed as supported in the release
  (Windows/Linux x64 only, or also arm64/macOS)?
