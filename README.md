# ParasiticDraw

[![License: GPL v3](https://img.shields.io/badge/License-GPL%20v3-blue.svg)](LICENSE) [![KSP Version](https://img.shields.io/badge/KSP-1.12.x-green.svg)](https://www.kerbalspaceprogram.com/) [![GitHub release](https://img.shields.io/github/v/release/appakappak/KSP_ParasiticDraw?include_prereleases&sort=semver)](https://github.com/appaKappaK/KSP_ParasiticDraw/releases/latest)

> **ParasiticDraw** is a small Kerbal Space Program 1 mod that adds configurable
> passive ElectricCharge drain to the active loaded vessel.

It is intended for stations, bases, and large ships where power production can
otherwise become irrelevant once enough solar panels and battery storage are
installed. ParasiticDraw adds a steady active-vessel load based on vessel size,
mass, crew capacity, crew aboard, and command modules.

ParasiticDraw is intentionally a lightweight difficulty mechanic: it increases
ElectricCharge demand on the craft you are currently flying. It does not alter
power generation and does not simulate unloaded vessels or an entire save in
the background.

## Features

- **Adds passive ElectricCharge draw** to the active loaded vessel.
- **Scales with vessel characteristics**, including part count, mass, crew
  capacity, crew present, and command modules.
- **Can ignore small vessels** with a configurable minimum part-count threshold.
- **Uses configurable presets** for Light, Standard, Harsh, and Strong balance.
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
[ParasiticDraw] Loaded vX.X.X.
```

## Configuration

Most players can tune ParasiticDraw from KSP's difficulty options. New saves
start from the values in
`GameData/ParasiticDraw/PluginData/ParasiticDraw/Settings.cfg`; after that, the
save's difficulty settings control enabled state, preset, minimum part count,
and global multiplier.

The included defaults are:

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

Presets control the overall strength:

- `Light`: `0.5x`
- `Standard`: `1.0x`
- `Harsh`: `2.0x`
- `Strong`: `3.0x`

In KSP's difficulty menu, `Preset Multiplier` uses a compact numeric slider:

- `0`: `Light` / `0.5x`
- `1`: `Standard` / `1.0x`
- `2`: `Harsh` / `2.0x`
- `3`: `Strong` / `3.0x`

`Minimum Part Count` lets you ignore small craft. The difficulty-menu slider is
capped at `100` so it remains usable in one-part increments. Higher startup
defaults can still be placed in `Settings.cfg`.

`debugLogging` remains config-file only.

## Compatibility

ParasiticDraw works without Dynamic Battery Storage. In that case, the EC drain
still happens, but stock KSP has no dedicated UI line showing the added load.

When Dynamic Battery Storage and Module Manager are installed, ParasiticDraw adds
a hidden reporting module to command-capable parts. DBS then shows a
`Parasitic Draw` consumer category and includes the drain in its total
`Power Consumed` value.

## Troubleshooting

On Linux/Proton installations, clicking Save in KSP's difficulty settings may
occasionally show `IOException: Source and destination are not on the same
device`. If the log points to KSP Community Fixes' `ConfigNodeTempCopy` patch,
create `GameData/KSPCF_UserSettings.cfg` with:

```cfg
@KSP_COMMUNITY_FIXES:AFTER[KSPCommunityFixes]
{
    @ConfigNodeTempCopy = false
}
```

Restart KSP after adding the file. This disables only KSPCF's temporary-copy
save protection, which can conflict with Proton's filesystem mapping; it is not
required for most installations.

## Current Limitations

- Only the active, loaded vessel is affected. Unloaded vessels and background
  catch-up drain are not simulated yet.
- ParasiticDraw adds ElectricCharge demand; it does not change solar panels,
  generators, batteries, or other power-production behavior.
- High non-physics time warp is not catch-up simulated in this release.
- The calculation is based on the active vessel's current part count, mass,
  crew, and command modules; it is not a background simulation of vessel power
  usage across the save.

## Project Information

- See [CHANGELOG.md](CHANGELOG.md) for release notes.
- See [src/ParasiticDraw/README.md](src/ParasiticDraw/README.md) for technical
  notes and troubleshooting.

## License

This project is licensed under the **GNU General Public License v3.0**. See the
[LICENSE](LICENSE) file for details.
