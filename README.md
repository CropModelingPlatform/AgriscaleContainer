# AgriscaleContainer

**A containerized environment for multi-model crop simulation and agricultural systems analysis**

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Build Status](https://github.com/YOUR_USERNAME/AgriscaleContainer/workflows/Build%20and%20Test/badge.svg)](https://github.com/YOUR_USERNAME/AgriscaleContainer/actions)
[![Release](https://img.shields.io/github/v/release/YOUR_USERNAME/AgriscaleContainer)](https://github.com/YOUR_USERNAME/AgriscaleContainer/releases)
[![DOI](https://zenodo.org/badge/DOI/10.5281/zenodo.XXXXXXX.svg)](https://doi.org/10.5281/zenodo.XXXXXXX)

[![Models](https://img.shields.io/badge/Models-APSIM%20%7C%20DSSAT%20%7C%20STICS%20%7C%20CELSIUS-green)]()
[![Platform](https://img.shields.io/badge/Platform-Singularity-blue)]()


---

## 📋 Overview

AgriscaleContainer provides a reproducible, containerized environment for running multiple crop simulation models in a unified framework. It includes multiple soi-corop models (APSIM, DSSAT, STIC, CELSIUS) alongside custom tools for large-scale agricultural analysis.

### Included Models

- **APSIM** (Agricultural Production Systems sIMulator) - Process-based model for simulating agricultural systems
- **DSSAT** (Decision Support System for Agrotechnology Transfer) - Crop modeling and decision support
- **STICS** (via JavaStics) - Soil-crop simulation model with multi-crop capabilities
- **CELSIUS:** - CELSIUS model

---

## 🚀 Quick Start

### Prerequisites

- Docker or Singularity
- Linux environment (recommended) or WSL2 on Windows
- .NET 5.0+ SDK (for building from source)
- Make
- Git

### Downloading Crop Models

**⚠️ Important:** The crop simulation models are **not included** in this repository. You must download them separately before building.

```bash
# Clone this repository
git clone https://github.com/CropModelingPlatform/AgriscaleContainer.git
cd AgriscaleContainer

# Download required crop models
make download_models
# Or directly:
bash scripts/download_models.sh
```

**Model Versions Used by AgriscaleContainer:**

| Model | Version | Source | License |
|-------|---------|--------|---------|
| **APSIM** | 2024.12.7510.0 | https://github.com/APSIMInitiative/ApsimX | Academic |
| **DSSAT v4.7** | v4.7.5.6 | https://github.com/DSSAT/dssat-csm-os | BSD-3-Clause |
| **DSSAT v4.8** | v4.8.5.0 | https://github.com/DSSAT/dssat-csm-os | BSD-3-Clause |

*See `scripts/download_models.sh` for exact versions and download details.*

### Building the Container

```bash
# After downloading models, build all components
make all

# Or build specific models:
make build_apsim
make build_dssat
make build_celsius
```

---

## 📁 Project Structure

```
AgriscaleContainer/
├── scripts/
│   └── download_models.sh  # Script to download crop models
├── ApsimX/                 # APSIM model source (downloaded)
├── dssat-csm-os/           # DSSAT v4.8 source (downloaded)
├── dssat_csm_develop/      # DSSAT v4.7 source (downloaded)
├── src/
│   ├── datamill/           # Custom data processing tool
│   └── celsius/            # CELSIUS Model source
├── bin/                    # Compiled binaries (generated)
├── Makefile                # Build automation
├── Dockerfile              # Docker configuration
├── LICENSE                 # MIT License for container infrastructure
├── THIRD-PARTY-NOTICES.md  # Third-party model licenses
└── README.md               # This file
```

---

## ⚖️ Licensing

### Container Infrastructure: MIT License

The build system, scripts, and custom tools (Makefile, Dockerfile, datamill, celsius) are licensed under the **MIT License**. See [LICENSE](LICENSE) for details.

### Crop Models: Individual Licenses **⚠️ IMPORTANT**

Each crop modeling system has its own license with **different terms and restrictions**:

| Model        | License                  | Commercial Use    | Key Restrictions                                      |
|--------------|--------------------------|-------------------|-------------------------------------------------------|
| **APSIM**    | Academic License         | ❌ No*            | Non-transferable; improvements assigned to APSIM      |
| **DSSAT**    | BSD-3-Clause             | ✅ Yes            | Attribution required; permissive                      |
| **STICS**    | CeCILL (GPL-compatible)  | ⚠️ Generally Yes  | Must share modifications if distributed               |
| **CELSIUS**  | MIT                      | ✅ Yes            | Permissive                                            |

\* *Commercial use of APSIM requires a separate license agreement*

**👉 You MUST comply with the license of each model you use.**

**📄 See [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md) for complete license information.**

### Usage Recommendations

- **Academic/Research:** ✅ All models available, cite appropriately
- **Teaching:** ✅ All models available
- **Commercial Applications:**
  - DSSAT: ✅ Permitted under BSD-3
  - CELSIUS: ✅ Permitted under MIT
  - STICS: ⚠️ Review CeCILL terms
  - APSIM: ❌ Contact APSIM Initiative for commercial license

---

## 🔬 Use Cases

- **Multi-model ensemble simulations** for uncertainty quantification
- **Large-scale spatial simulations** across regions
- **Climate change impact assessments** with multiple crop models
- **Model intercomparison studies** (e.g., AgMIP, GGCMI)
- **Decision support** for agricultural management
- **Educational purposes** in crop modeling courses



---

## 📦 Dependencies

### Runtime Dependencies
- **.NET Runtime** (5.0+ for datamill/celsius, 8.0 for APSIM)
- **gfortran** (for DSSAT Fortran code)
- **Java Runtime** (for JavaStics)
- Standard Linux utilities

### Build Dependencies
- **.NET SDK** (5.0+ and 8.0)
- **CMake** (for DSSAT)
- **Docker** or **Singularity** (for containerization)
- **Make**

---

## 🤝 Contributing

Contributions to the **container infrastructure** are welcome! Please:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

**Note:** Modifications to crop models (APSIM, DSSAT, STICS) should be contributed to their respective upstream repositories.

---

## 📚 Citation

If you use AgriscaleContainer in your research, please cite this repository:

```bibtex
@software{agriscalecontainer2026,
  author       = {Midingoyi, Cyrille Ahmed},
  title        = {AgriscaleContainer: Multi-Model Crop Simulation Environment},
  year         = {2026},
  url          = {https://github.com/CropModelingPlatform/AgriscaleContainer}
}
```

**Additionally, cite each crop model you use:**

- **APSIM:** Holzworth et al. (2014). APSIM – Evolution towards a new generation of agricultural systems simulation. *Environmental Modelling & Software*, 62, 327-350.
- **DSSAT:** Jones et al. (2003). The DSSAT cropping system model. *European Journal of Agronomy*, 18(3-4), 235-265.
- **STICS:** Brisson et al. (2009). STICS: a generic model for simulating crops and their water and nitrogen balances. *Agronomie*, 18(5-6), 311-346.

---

## 📞 Support

- **Container Issues:** Open an issue on this GitHub repository
- **APSIM Questions:** https://www.apsim.info or apsim@csiro.au
- **DSSAT Questions:** https://dssat.net
- **STICS Questions:** https://www6.paca.inrae.fr/stics_eng/

---

## 📜 License Summary

```
Container Infrastructure  → MIT License (free for all uses)
APSIM                    → Academic License (non-commercial)
DSSAT                    → BSD-3-Clause (free for all uses)
STICS                    → CeCILL (GPL-compatible)
```

**For full license details, see [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md)**

---

## ⚠️ Disclaimer

This software is provided "as is" without warranty of any kind. The crop models are research tools and should not be used as the sole basis for management decisions without proper validation and expert consultation.

---

**Maintainer:** Cyrille Ahmed MIDINGOYI  
**Last Updated:** March 2026  
**Repository:** https://github.com/CropModelingPlatform/AgriscaleContainer
