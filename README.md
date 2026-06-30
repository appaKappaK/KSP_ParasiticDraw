# ParasiticDraw

[![License: GPL v3](https://img.shields.io/badge/License-GPL%20v3-blue.svg)](LICENSE) [![KSP Version](https://img.shields.io/badge/KSP-1.12.x-green.svg)](https://www.kerbalspaceprogram.com/) [![GitHub release](https://img.shields.io/github/v/release/appaKappaK/KSP_ParasiticDraw?include_prereleases&sort=semver)](https://github.com/appaKappaK/KSP_ParasiticDraw/releases/latest)

> **ParasiticDraw** is a small Kerbal Space Program 1 mod that adds configurable
> passive ElectricCharge drain to the active loaded vessel.

It is intended for stations, bases, and large ships where power production can
otherwise become irrelevant once enough solar panels and battery storage are
installed. ParasiticDraw adds a steady background load based on vessel size,
mass, crew capacity, crew aboard, and command modules.

## Features

- **Adds passive ElectricCharge draw** to the active loaded vessel.
- **Scales with vessel characteristics**, including part count, mass, crew
  capacity, crew present, and command modules.
- **Can ignore small vessels** with a configurable minimum part-count threshold.
- **Uses configurable presets** for Light, Standard, Harsh, and Custom balance.
- **Avoids save mutation**: it drains EC but does not damage vessels, disable
  parts, kill crew, or write vessel state.
- **Includes Dynamic Battery Storage compatibility** so the added drain appears
  in DBS power monitoring when DBS is installed.
- **Includes optional debug logging** for troubleshooting.

## Requirements

- Kerbal Space Program 1.12.x

## Recommended

- [Dynamic Battery Storage](https://github.com/KSPModStewards/DynamicBatteryStorage)
  for power-flow monitoring.
- [Module Manager](https://forum.kerbalspaceprogram.com/topic/50533-18x-112x-module-manager-423-july-03th-2023-fireworks-season/)
  if you want ParasiticDraw's Dynamic Battery Storage reporting patch to apply.

## Installation

Copy the packaged `ParasiticDraw` folder into your KSP `GameData` directory.

```text
GameData/
  ParasiticDraw/
    ParasiticDraw.version
    Patches/
      DynamicBatteryStorage.cfg
    PluginData/
      ParasiticDraw/
        Settings.cfg
    Plugins/
      ParasiticDraw.dll
```

After entering flight, your `KSP.log` should contain:

```text
[ParasiticDraw] Loaded v1.1.0.
```

## Configuration

`GameData/ParasiticDraw/PluginData/ParasiticDraw/Settings.cfg` contains:

```text
PARASITIC_DRAW_SETTINGS
{
  enabled = true
  debugLogging = false

  passiveDrawEnabled = true
  passiveDrawPreset = Standard

  minimumPartCount = 0

  baseVesselDraw = 0.05
  perPartDraw = 0.01
  perMassTonDraw = 0.02
  perCrewCapacityDraw = 0.15
  perCrewPresentDraw = 0.05
  perCommandModuleDraw = 0.15

  globalDrawMultiplier = 1.0
}
```

The draw formula is:

```text
if partCount < minimumPartCount:
  EC/s = 0

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
- `Custom`: `1.0x`

> **Tip:** Settings are loaded when the flight addon starts. Restart KSP after
> editing `Settings.cfg` in the current version.

## Compatibility

ParasiticDraw works without Dynamic Battery Storage. In that case, the EC drain
still happens, but stock KSP has no dedicated UI line showing the added load.

When Dynamic Battery Storage and Module Manager are installed, ParasiticDraw adds
a hidden reporting module to command-capable parts. DBS then shows a
`Parasitic Draw` consumer category and includes the drain in its total
`Power Consumed` value.

ParasiticDraw does not require Kerbalism, Near Future Electrical, SystemHeat, or
other power-system mods.

## Current Limitations

- Only the active loaded vessel is affected.
- Unloaded vessels and background catch-up drain are not simulated yet.
- High non-physics time warp is not catch-up simulated in this release.
- Settings do not hot-reload while KSP is running.
- There is no in-game settings window yet.

## Troubleshooting

After entering flight, confirm that `KSP.log` contains:

```text
[ParasiticDraw] Loaded v1.1.0.
```

If EC drains but Dynamic Battery Storage does not show `Parasitic Draw`, confirm
that Module Manager and Dynamic Battery Storage are installed and restart KSP so
the compatibility patch can apply.

For high-visibility testing, temporarily set:

```text
debugLogging = true
globalDrawMultiplier = 20.0
```

Then restart KSP and load a vessel with batteries. Restore
`globalDrawMultiplier = 1.0` after testing.

When debug logging is enabled, EC shortfalls appear as:

```text
[ParasiticDraw] Shortfall: N.NNN EC on Vessel Name
```

**Please include `KSP.log` when reporting a problem.**

## Project Information

- See [CHANGELOG.md](CHANGELOG.md) for release notes.

## License

This project is licensed under the **GNU General Public License v3.0**. See the
[LICENSE](LICENSE) file for details.
