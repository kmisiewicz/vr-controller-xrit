# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
### Added
- In-air move direction multiplier

## [1.0.0-pre.3] - 2022-01-08
### Added
- Slope gravity

### Changed
- Updated code to support XR Interaction Toolkit 2.0.0-pre.6
- Moved some assets from package to demo scene sample
- Improved custom inspectors scaling with multiple properties in one line

## [1.0.0-pre.2] - 2021-11-24
### Added
- Climbing Provider
- Gravity Provider (collider floating over ground allows to climb bumps and stairs, contains ground checker)
- Demo Scene

### Fixed
- Missing stuff from prefabs and demo scene

## [1.0.0-pre.1] - 2021-10-18
### Added
- Basic movement providers substitutes:
  - Continuous movement and turning with vignette to help with motion sickness
  - Snap turning an teleportation with blinking (fading screen to black)
- Real-space movement: body collisions with head leaning over small obstacles
