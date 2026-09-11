# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [Unreleased]

## [1.2.2] - 2026-09-11

### Fixed

- Fixed an error that could appear when changing and saving the global draw
  multiplier in KSP's difficulty settings.

## [1.2.1] - 2026-07-01

### Added

- KSP difficulty-options integration for the main ParasiticDraw settings.
- Source-level technical notes for implementation details, KSP UI constraints,
  and troubleshooting.

### Changed

- Difficulty options now use a single enable toggle and initialize from
  `Settings.cfg`.
- Difficulty sliders now use finer increments for exact minimum part counts and
  global multiplier values.
- Difficulty-options multiplier input no longer uses KSP's oversized text box.
- Difficulty-options minimum part-count slider now caps at 100 to allow
  practical one-part increments in KSP's compact slider control.
- Preset difficulty tooltip no longer repeats the preset index mapping.
- Global multiplier difficulty display now includes the `x` suffix.
- Startup logging now shows the global multiplier with the `x` suffix.
- Redundant `Custom` preset has been replaced by a `Strong` 3x preset.

### Fixed

- Clear Dynamic Battery Storage reporter rates when ParasiticDraw is disabled.
- ParasiticDraw reporter fields now request two-decimal EC/s formatting.
- Settings reloads from KSP's settings-applied event no longer spam `KSP.log`
  unless debug logging is enabled.
- Fixed a KSP/Mono `InvalidProgramException` in the passive draw calculator.
- Guarded difficulty-settings loading when game parameters are unavailable.

## [1.1.0] - 2026-06-30

### Added

- `minimumPartCount` setting to skip passive draw on vessels below a configured
  part-count threshold.

## [1.0.0] - 2026-06-30

### Added

- Initial repository scaffold for the ParasiticDraw KSP mod.
- Project plan, default settings, version metadata, build project, passive draw
  plugin, and calculation tests for the first playable version.
- Dynamic Battery Storage compatibility reporting for ParasiticDraw passive
  consumption.

## Versioning

**Current version:** 1.2.2

- **Major (1.#.#)**: Breaking changes or major reworks of core mechanics.

- **Minor (#.1.#)**: New features, settings, or compatibility improvements (backward compatible).

- **Patch (#.#.1)**: Bug fixes, docs, and trivial cleanups.
