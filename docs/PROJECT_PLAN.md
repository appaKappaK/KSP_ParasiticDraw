# ParasiticDraw Project Plan

## Summary

Build `ParasiticDraw` as a KSP 1.12.x mod that adds configurable vessel-level ElectricCharge drain so large ships, stations, and bases keep meaningful power requirements after solar and battery capacity become abundant.

The first playable version will be conservative: loaded active-vessel passive draw only, normal flight/physics-warp drain only, config-file settings only, no in-game UI, no ModuleManager consumption patches, and no README changes.

## Key Changes

- Add a Mono/xbuild-compatible C# project under `src/ParasiticDraw`, targeting local KSP/Unity assemblies in ignored `ksp-buildrefs/all`.
- Reference at minimum `Assembly-CSharp.dll`, `UnityEngine.dll`, `UnityEngine.CoreModule.dll`, `System.dll`, and `System.Core.dll` with `Private=false`.
- Build `ParasiticDraw.dll` into `GameData/ParasiticDraw/Plugins/`.
- Add `GameData/ParasiticDraw/ParasiticDraw.version` with KSP-AVC-style `VERSION` and `KSP_VERSION` fields, starting at mod `1.0.0` for KSP `1.12.5`.
- Add default settings at `GameData/ParasiticDraw/PluginData/ParasiticDraw/Settings.cfg`.

## Settings And Behavior

Use tempered config-only defaults for v1:

```cfg
PARASITIC_DRAW_SETTINGS
{
  enabled = true
  debugLogging = false

  passiveDrawEnabled = true
  passiveDrawPreset = Standard

  baseVesselDraw = 0.05
  perPartDraw = 0.01
  perMassTonDraw = 0.02
  perCrewCapacityDraw = 0.15
  perCrewPresentDraw = 0.05
  perCommandModuleDraw = 0.15

  globalDrawMultiplier = 1.0
}
```

Passive draw formula:

```txt
EC/s =
(
  baseVesselDraw
  + partCount * perPartDraw
  + vesselMassTons * perMassTonDraw
  + crewCapacity * perCrewCapacityDraw
  + currentCrew * perCrewPresentDraw
  + commandModuleCount * perCommandModuleDraw
)
* presetMultiplier
* globalDrawMultiplier
```

Preset multipliers:

- `Light`: `0.5x`
- `Standard`: `1.0x`
- `Harsh`: `2.0x`
- `Custom`: no preset scaling beyond `globalDrawMultiplier`

Runtime behavior:

- Apply draw only in flight scene to the active loaded vessel.
- Drain during normal flight and physics warp using `TimeWarp.deltaTime` in `FixedUpdate()`.
- Do not attempt high non-physics warp catch-up in v1; drain resumes normally when physics updates resume.
- Clamp or ignore suspicious one-tick delta spikes after scene changes, vessel switches, or warp transitions.
- Consume EC through vessel resource flow, using the active vessel/root part.
- If EC is insufficient, consume what is available and optionally log shortfall amount when `debugLogging = true`.
- Do not damage vessels, disable parts, kill crew, or mutate saves in v1.

## Implementation Notes

- Implement a small settings loader using KSP `ConfigNode`; missing or invalid values fall back to defaults.
- Keep the passive draw calculator as pure logic so it can be tested outside KSP runtime.
- Detect command modules by checking for `ModuleCommand` on vessel parts.
- Treat command-module draw stacking with crew capacity and crew-present draw as deliberate: avionics/control hardware has its own load.
- Add comments in the default settings explaining crew capacity as standby habitation load and crew present as active crew load.
- Defer unloaded/background vessel drain, focused-vessel high-warp catch-up drain, UI, ModuleManager multipliers, per-vessel overrides, and compatibility-specific integrations.

## Test Plan

- Build locally with Mono/xbuild and confirm `GameData/ParasiticDraw/Plugins/ParasiticDraw.dll` is produced.
- Unit-test settings defaults, invalid config fallback, `Light`/`Standard`/`Harsh`/`Custom` preset behavior, command-module counting, delta spike handling, and passive draw calculation.
- Run a quick balance calculation pass before manual KSP testing:
  - small probe target: low nonzero draw
  - crewed capsule target: roughly `1-3 EC/s`
  - large 200-part, 500-ton station target with defaults: about `14.85 EC/s` on Standard and `29.7 EC/s` on Harsh
- Manual KSP scenarios:
  - disabled settings apply no drain
  - small uncrewed probe has low nonzero draw
  - crewed capsule draws more than probe
  - large station draws significantly more based on parts, mass, crew capacity, and command modules
  - depleted EC produces no exception and logs shortfall only in debug mode
  - physics warp drains reasonably
  - high non-physics warp pauses/resumes without a huge catch-up drain
  - scene change and vessel switch do not produce a one-tick EC spike
- Confirm local-only ignored files stay untracked: `KSP.log` and `ksp-buildrefs/`.

## Assumptions

- Target game is KSP 1.12.5.
- First playable version is passive draw only.
- First version affects loaded active vessels only.
- First version does not simulate high non-physics warp drain; that is later work.
- First version uses config files only.
- ModuleManager is not required for v1.
- `CHANGELOG.md` and `ParasiticDraw.version` must agree on released version numbers.
- `README.md` remains untouched until the core implementation direction is proven.
