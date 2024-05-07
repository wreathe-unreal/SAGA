# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.0] - 2022-12-20
### Added 
- Added `AnimationSet` to `BinaryCharm.Samples.TextMeshProAlpha.TextAnimation`
- Added `DialogueAppear` to 
 `BinaryCharm.Samples.TextMeshProAlpha.TextAnimation.Effects`

### Changed
- Restructured directory layout
- Installation as `Package` (no cluttering of the `Assets` folder)
- Adjusted Demo UI layout (previously worked well only at 16:9 aspect ratio)
- Improved code formatting consistency
- Improved documentation (new formatting, new usage instructions and details)

### Fixed
- Fixed `SmoothIncrementalAppear` not working properly in case of strings
  containing formatting tags


## [1.0.1] - 2020-08-05
### Fixed
- Fixed package manager dependencies
- Fixed button size in demo scene
- Prevented some compiler warnings


## [1.0.0] - 2020-07-14
- Initial release of BinaryCharm.TextMeshProAlpha
