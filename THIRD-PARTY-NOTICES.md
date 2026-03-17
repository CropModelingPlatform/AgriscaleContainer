# Third-Party Software Notices and Licenses

AgriscaleContainer bundles several third-party crop modeling systems, each with its own license terms. **Users must comply with the license requirements of each model they choose to use.**

---

## 1. APSIM (Agricultural Production Systems sIMulator)

**Location:** `ApsimX/`  
**License:** APSIM General Use License Agreement  
**License File:** [ApsimX/LICENSE.md](ApsimX/LICENSE.md)

**Key Points:**
- ✅ **Permitted:** Free use for research, education, and publishing results
- ⚠️ **Restrictions:**
  - Non-commercial use only (commercial use requires separate agreement)
  - Non-transferable, non-sublicensable license
  - Any improvements to APSIM code become property of the APSIM Initiative
  - Cannot use APSIM logos/trademarks without written permission
- 📋 **Attribution Required:** Cite APSIM in publications
- 🔗 **More Info:** https://www.apsim.info

**Copyright Holders:**
- State of Queensland (Department of Agriculture and Fisheries)
- University of Queensland
- CSIRO (Commonwealth Scientific and Industrial Research Organisation)
- AgResearch Limited (New Zealand)
- University of Southern Queensland
- Iowa State University
- The New Zealand Institute for Plant and Food Research Limited

**Summary:** Academic/research-focused license with IP assignment clause. Suitable for academic work but restrictive for commercial applications.

---

## 2. DSSAT (Decision Support System for Agrotechnology Transfer)

**Location:** `dssat_csm_develop/`  
**License:** BSD 3-Clause License  
**License File:** [dssat_csm_develop/license.txt](dssat_csm_develop/license.txt)

**Key Points:**
- ✅ **Permitted:** Redistribution and use in source/binary forms with or without modification
- ✅ **Commercial Use:** Allowed
- 📋 **Requirements:**
  - Retain copyright notice and license in source distributions
  - Include license in binary distributions
  - Do not use DSSAT Foundation name for endorsement without permission
- 🔗 **More Info:** https://dssat.net

**Copyright:** © 2019, DSSAT Foundation

**Summary:** Permissive open-source license compatible with commercial and academic use.

---

## 3. STICS / JavaStics

**Location:** `JavaSticsInstall.zip` (bundled), potentially in `src/`  
**License:** CeCILL (likely) or CeCILL-C  
**License Info:** https://www6.paca.inrae.fr/stics_eng/About-us/Licence

**Key Points:**
- ✅ **Permitted:** Use, modification, and distribution
- 📋 **Requirements:** CeCILL is GPL-compatible, requires sharing modifications if distributed
- 🔗 **More Info:** https://www6.paca.inrae.fr/stics

**Copyright:** © INRAE (Institut National de Recherche pour l'Agriculture, l'Alimentation et l'Environnement)

**Summary:** French GPL-equivalent license for research software. Suitable for academic and some commercial uses.

**Note:** Exact license version should be verified in the JavaStics installation package.

---

## 4. Additional Dependencies

This container may include additional software dependencies installed via package managers (apt, dotnet, etc.). These components maintain their original licenses:

- **.NET Runtime:** MIT License (Microsoft)
- **System Libraries:** Various (GPL, LGPL, BSD, MIT, etc.)
- **Scientific Libraries:** NumPy, SciPy, matplotlib, etc. (mostly BSD/MIT)

---

## Compliance Summary

| Model | Commercial Use | Modifications | Redistribution | Attribution |
|-------|----------------|---------------|----------------|-------------|
| **APSIM** | ❌ No (requires agreement) | ⚠️ Yes (IP assigned to APSIM) | ⚠️ Limited | ✅ Required |
| **DSSAT** | ✅ Yes | ✅ Yes (keep license) | ✅ Yes | ✅ Required |
| **STICS** | ⚠️ Generally Yes (verify) | ✅ Yes (share if distributed) | ✅ Yes | ✅ Required |

---

## How to Comply

### For Academic/Research Use:
1. ✅ Cite each model used in your publications
2. ✅ Include attribution in any derived works
3. ✅ Respect non-commercial restrictions (especially for APSIM)

### For Commercial Use:
1. ⚠️ **APSIM:** Contact APSIM Initiative for commercial license
2. ✅ **DSSAT:** Permitted under BSD-3-Clause
3. ⚠️ **STICS:** Review CeCILL terms or contact INRAE

### For Container Redistribution:
1. ✅ Include this THIRD-PARTY-NOTICES.md file
2. ✅ Include all original license files (in respective subdirectories)
3. ✅ Do not remove copyright notices
4. ✅ Clearly state which models are included in your distribution

---

## Questions?

- **APSIM:** Email apsim@csiro.au or visit https://www.apsim.info
- **DSSAT:** Visit https://dssat.net
- **STICS:** Visit https://www6.paca.inrae.fr/stics_eng/ or contact INRAE
- **This Container:** Open an issue on the GitHub repository

---

**Last Updated:** March 2026  
**Container Maintainer:** Cyrille Ahmed MIDINGOYI
