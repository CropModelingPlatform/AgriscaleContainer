# Quick Start Guide

## First Time Setup

1. **Clone the repository:**
   ```bash
   git clone https://github.com/CropModelingPlatform/AgriscaleContainer.git
   cd AgriscaleContainer
   ```

2. **Download crop models:**
   ```bash
   make download_models
   ```
   This downloads:
   - APSIM 2024.12.7510.0 (~200 MB)
   - DSSAT v4.7.5.6 (~10 MB)
   - DSSAT v4.8.5.0 (~10 MB)

3. **Build:**
   ```bash
   make build         # Build all
   make build_apsim   # Build APSIM only
   make build_dssat   # Build DSSAT only
   ```

## Model Versions

| Model | Version | License |
|-------|---------|---------|
| APSIM | 2024.12.7510.0 | Academic (non-commercial) |
| DSSAT v4.7 | v4.7.5.6 (Feb 2019) | BSD-3-Clause |
| DSSAT v4.8 | v4.8.5.0 (Dec 2024) | BSD-3-Clause |

## Notes

- Model source code is **NOT** included in this repository
- Models are downloaded from their official GitHub repositories
- This keeps the repository lightweight and respects licensing
- See `scripts/download_models.sh` for download details
