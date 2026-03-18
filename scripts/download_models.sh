#!/bin/bash
set -e

echo "========================================="
echo "AgriscaleContainer - Model Downloader"
echo "========================================="
echo ""
echo "This script downloads the required crop simulation models."
echo "Total download size: ~300 MB (shallow clones)"
echo ""

# Color output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# APSIM - Latest stable release
APSIM_VERSION="2024.12.7510.0"
if [ ! -d "ApsimX" ]; then
    echo -e "${BLUE}Downloading APSIM ${APSIM_VERSION}...${NC}"
    git clone --depth 1 --branch ${APSIM_VERSION} \
        https://github.com/APSIMInitiative/ApsimX.git 2>/dev/null || \
    git clone --depth 1 https://github.com/APSIMInitiative/ApsimX.git
    echo -e "${GREEN}✓ APSIM downloaded${NC}"
else
    echo -e "${GREEN}✓ APSIM already present${NC}"
fi

# DSSAT v4.7.5 (develop branch - for backward compatibility)
DSSAT_V47_TAG="v4.7.5.6"  # Feb 2019 release
if [ ! -d "dssat_csm_develop" ]; then
    echo -e "${BLUE}Downloading DSSAT v4.7.5 (develop branch)...${NC}"
    # Clone the entire repo and checkout specific version
    git clone https://github.com/DSSAT/dssat-csm-os.git dssat_csm_develop
    cd dssat_csm_develop
    git checkout ${DSSAT_V47_TAG} 2>/dev/null || git checkout develop
    cd ..
    echo -e "${GREEN}✓ DSSAT v4.7.5 downloaded${NC}"
else
    echo -e "${GREEN}✓ DSSAT v4.7.5 already present${NC}"
fi

# DSSAT v4.8.5 (latest release)
DSSAT_V48_VERSION="v4.8.5.0"
if [ ! -d "dssat-csm-os" ]; then
    echo -e "${BLUE}Downloading DSSAT ${DSSAT_V48_VERSION}...${NC}"
    git clone --depth 1 --branch ${DSSAT_V48_VERSION} \
        https://github.com/DSSAT/dssat-csm-os.git
    echo -e "${GREEN}✓ DSSAT v4.8.5 downloaded${NC}"
else
    echo -e "${GREEN}✓ DSSAT v4.8.5 already present${NC}"
fi

echo ""
echo "========================================="
echo -e "${GREEN}✓ All models downloaded successfully!${NC}"
echo "========================================="
echo ""
echo "Model versions:"
echo "  - APSIM:      ${APSIM_VERSION}"
echo "  - DSSAT v4.7: ${DSSAT_V47_TAG}"
echo "  - DSSAT v4.8: ${DSSAT_V48_VERSION}"
echo ""
echo "You can now run: make build"
echo ""
