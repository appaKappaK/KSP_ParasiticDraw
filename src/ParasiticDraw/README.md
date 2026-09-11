# ParasiticDraw Technical Notes

This directory contains the KSP plugin source for `ParasiticDraw.dll`.

## Runtime Scope

`ParasiticDrawAddon` is a flight-scene addon. It applies EC drain only to
`FlightGlobals.ActiveVessel` when that vessel is loaded and has a root part.
Unloaded/background vessels are intentionally outside the current runtime scope.
The addon adds demand through the active vessel's ElectricCharge resource
network; it does not modify generators, solar panels, batteries, or other
power-production behavior.

High non-physics time-warp catch-up is also intentionally not simulated. Normal
flight and physics-warp drain use `TimeWarp.deltaTime` in `FixedUpdate()`, with
large one-tick deltas clamped by `PassiveDrawCalculator.ClampDeltaTime()`.

## Settings Flow

`SettingsLoader.LoadConfigSettings()` reads
`GameData/ParasiticDraw/PluginData/ParasiticDraw/Settings.cfg`.

`SettingsLoader.Load()` then overlays save-specific KSP difficulty parameters
when `HighLogic.CurrentGame.Parameters` is available. This means:

- New saves initialize the difficulty values from `Settings.cfg`.
- Existing saves use their stored difficulty values for enabled state, preset,
  minimum part count, and global multiplier.
- `debugLogging` remains config-file only.

The addon reloads settings when KSP fires `GameEvents.OnGameSettingsApplied`.
That reload is quiet unless `debugLogging` is enabled.

## Draw Calculation

`PassiveDrawCalculator.CalculateEcPerSecond()` applies the minimum part-count
gate before calculating draw:

```text
if partCount < minimumPartCount:
  EC/s = 0
```

The active-vessel draw formula is:

```text
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
- `Strong`: `3.0x`

## Difficulty UI

KSP's stock difficulty options use `GameParameters.CustomParameterNode`.
The available numeric controls are limited:

- `CustomIntParameterUI` supports title, min/max, step size, and display format.
- `CustomFloatParameterUI` supports title, range, step count, display format,
  percentage mode, and an optional text field.

The preset control is integer-backed so it can snap exactly to the supported
presets:

- `0`: `Light` / `0.5x`
- `1`: `Standard` / `1.0x`
- `2`: `Harsh` / `2.0x`
- `3`: `Strong` / `3.0x`

The stock difficulty UI cannot show custom tick labels under the slider.

The minimum part-count difficulty slider is capped at `100` because larger
ranges make KSP's compact slider skip values in practice. Higher values can
still be set in `Settings.cfg` for new saves or config-only use.

## DBS Reporting

`ModuleParasiticDraw` is a hidden reporting module for Dynamic Battery Storage.
The ModuleManager patch adds it to command-capable parts when DBS is installed.
The addon distributes the calculated active-vessel draw across reporter modules
so DBS can include the draw in its power-consumption totals.

If ParasiticDraw is disabled, reporter rates are set to zero. DBS may still show
the `Parasitic Draw` category while the handler exists; that is DBS UI behavior,
not an active drain.

## Compatibility Notes

`VesselDrawSnapshot` is a reference type rather than a struct. This avoids a
KSP/Mono `InvalidProgramException` observed when the calculator was called from
flight with a value-type snapshot using auto-property getter calls.

## Troubleshooting

After entering flight, confirm that `KSP.log` contains:

```text
[ParasiticDraw] Loaded vX.X.X.
```

If EC drains but Dynamic Battery Storage does not show `Parasitic Draw`, confirm
that Module Manager and Dynamic Battery Storage are installed and restart KSP so
the compatibility patch can apply.

For high-visibility testing, temporarily set:

```text
debugLogging = true
globalDrawMultiplier = 20.0
```

Then load a vessel with batteries. Restore `globalDrawMultiplier = 1.0` after
testing.

When debug logging is enabled, EC shortfalls appear as:

```text
[ParasiticDraw] Shortfall: N.NNN EC on Vessel Name
```

On Linux/Proton installations, an `IOException` stating that the source and
destination are not on the same device when saving KSP difficulty settings is
usually caused by KSP Community Fixes' `ConfigNodeTempCopy` patch rather than
ParasiticDraw. If the stack trace names that patch, disable it with a
`GameData/KSPCF_UserSettings.cfg` file containing:

```cfg
@KSP_COMMUNITY_FIXES:AFTER[KSPCommunityFixes]
{
    @ConfigNodeTempCopy = false
}
```

Please include `KSP.log` when reporting a problem.
