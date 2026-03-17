# Changelog

All notable changes to AgriscaleContainer will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Planned
- GitHub Actions CI/CD pipeline
- Automated testing framework
- Docker Hub automated builds
- Singularity Hub integration
- Enhanced documentation with examples
- Performance benchmarking suite

## [1.0.0] - 2026-03-17

### Added
- Initial public release
- Multi-model container supporting APSIM, DSSAT, and STICS
- Custom utilities: datamill and celsius
- Comprehensive build system using Makefile
- Docker and Singularity support
- Complete licensing documentation
  - MIT License for container infrastructure
  - THIRD-PARTY-NOTICES.md for all bundled models
- GitHub issue templates and PR templates
- CITATION.cff for academic citation
- CONTRIBUTING.rst with contribution guidelines

### Container Infrastructure
- Makefile-based build automation
- Support for .NET 5.0 and 8.0 builds
- Fortran compilation for DSSAT
- Java integration for STICS/JavaStics
- Cross-platform build targets (Linux x64 primary)

### Included Models
- **APSIM** - Agricultural Production Systems sIMulator
  - .NET 8.0 based build
  - Full model capabilities
  - Subject to APSIM General Use License
  
- **DSSAT** - Decision Support System for Agrotechnology Transfer
  - Fortran/CMake build system
  - BSD-3-Clause licensed
  
- **STICS** - Soil-Crop Simulation Model
  - JavaStics integration
  - CeCILL licensed

### Custom Tools
- **datamill** - Data processing utility (.NET 5.0)
- **celsius** - Climate analysis tool (.NET 5.0)

### Documentation
- Comprehensive README.md with quick start guide
- Detailed license documentation
- Contributing guidelines
- Citation information
- GitHub templates for issues and PRs

### Build Targets
- `make build` - Build datamill
- `make build_celsius` - Build celsius
- `make build_apsim` - Build APSIM
- `make build_dssat` - Build DSSAT
- `make clean` - Clean all build artifacts
- Individual clean targets for each component

## [0.9.0] - Pre-release

### Added
- Initial development versions
- Prototype build system
- Basic Docker support

---

## Release Notes Guidelines

### Version Number Format
- **Major.Minor.Patch** (e.g., 1.0.0)
- **Major**: Breaking changes, major new features
- **Minor**: New features, backward compatible
- **Patch**: Bug fixes, minor improvements

### Categories
- **Added**: New features
- **Changed**: Changes in existing functionality
- **Deprecated**: Soon-to-be removed features
- **Removed**: Removed features
- **Fixed**: Bug fixes
- **Security**: Security fixes

### License Compliance Notes
Any changes to included models or their versions should be documented with:
- Model version number
- License changes (if any)
- Compatibility notes

[Unreleased]: https://github.com/YOUR_USERNAME/AgriscaleContainer/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/YOUR_USERNAME/AgriscaleContainer/releases/tag/v1.0.0
[0.9.0]: https://github.com/YOUR_USERNAME/AgriscaleContainer/releases/tag/v0.9.0
