
.DEFAULT_GOAL := all
EMBED=False
PLF_win=win7-x64
PLF_lin=linux-x64

export DOCKER_BUILDKIT=1

USER_WORKSPACE_DIR="${PWD}"
JAVASTICS_ZIP_FILE_v9_0=JavaSticsInstall.zip
JAVASTICS_ZIP_FILE_v10_0=JavaSTICS-1.5.2-STICS-10.2.0.zip 
DSSAT_DIR=dssat_csm_develop
DSSAT_DIR_latest=dssat-csm-os
APSIM=ApsimX
START_AT=0
END_AT=-1



export DOCKER_IMG=datamill:0.1
export BUILDKIT_PROGRESS=plain
export CACHE_BUST=$(date +%s)
DOCKER_CONTAINER=datamill
DOCKER_RUN=docker run -t -v /etc/timezone:/etc/timezone:ro \
											-v /etc/localtime:/etc/localtime:ro \
											-v $(USER_WORKSPACE_DIR):/work:rw \
											--name $(DOCKER_CONTAINER) \
											$(DOCKER_IMG)

#############
#  DOWNLOAD MODELS
##

.PHONY: download_models
download_models:
	@echo "Downloading required crop simulation models..."
	@bash scripts/download_models.sh

#############
#  DATAMILL
##


clean:
	rm -Rf ./DonneesFA
	rm -Rf ./bin/datamill/*
	dotnet clean src/datamill/datamill.vbproj

build: clean
	dotnet publish \
	  -c Debug -r ${PLF_lin} \
	  -p:PublishSingleFile=False \
	  --self-contained ${EMBED} \
	  -v n \
	  src/datamill/datamill.vbproj
	mkdir -p bin/datamill/ && cp -Rf src/datamill/bin/Debug/net5.0/linux-x64/publish/* bin/datamill/



#############
#  CELSIUS
##
clean_celsius:
	rm -Rf bin/celsius/*
	dotnet clean src/celsius/celsius.vbproj

build_celsius: clean_celsius
	dotnet publish \
		-c Debug \
		-r ${PLF_lin} \
		-p:PublishSingleFile=False \
		--self-contained ${EMBED} \
		-v n \
		src/celsius/celsius.vbproj
	mkdir -p bin/celsius/ && cp -Rf src/celsius/bin/Debug/net5.0/linux-x64/publish/* bin/celsius/


#############
# APSIM
##

clean_apsim:
	rm -Rf bin/apsim/*
	rm -rf  ApsimX/Models/obj
	dotnet clean ApsimX/Models/Models.csproj

build_apsim: clean_apsim
	dotnet publish \
		--nologo -c Release -f net8.0 \
		-r ${PLF_lin} \
		--self-contained ${EMBED} \
		-v n \
		ApsimX/Models/Models.csproj
	mkdir -p bin/apsim/ && cp -Rf ApsimX/bin/Release/net8.0/linux-x64/publish/* bin/apsim/


#############
# DSSAT
##


clean_dssat:
	rm -Rf bin/dssat/*
	rm -Rf dssat_csm_develop/build
	mkdir -p bin/dssat
	rm -Rf bin/dssatv48/*
	rm -Rf dssat-csm-os/build
	mkdir -p bin/dssatv48

build_dssat: clean_dssat
	mkdir dssat_csm_develop/build
	cd dssat_csm_develop/build && cmake -DCMAKE_INSTALL_PREFIX=~/tmp/dssat .. && make install
	cp -Rf ~/tmp/dssat/* bin/dssat/ && rm -Rf ~/tmp/dssat
	mkdir dssat-csm-os/build
	cd dssat-csm-os/build && cmake -DCMAKE_INSTALL_PREFIX=~/tmp/dssat .. && make install
	mkdir -p bin/dssatv48
	cp -Rf ~/tmp/dssat/* bin/dssatv48/ && rm -Rf ~/tmp/dssat

#############
# DOCKER
##

dbuild: #build_dssat
	docker build \
	--build-arg JAVASTICS_ZIP_FILE=$(JAVASTICS_ZIP_FILE_v9_0) \
	--build-arg JAVASTICS_ZIP_FILE10=$(JAVASTICS_ZIP_FILE_v10_0) \
	--build-arg DSSAT_DIR=$(DSSAT_DIR) \
	--build-arg DSSAT_DIR_latest=$(DSSAT_DIR_latest) \
	--build-arg APSIM=$(ApsimX) \
    --build-arg CACHE_BUST=$(CACHE_BUST) \
	--progress=plain \
	-t $(DOCKER_IMG) .

#############
# SINGULARITY
##
sbuild:
	rm -f ${DOCKER_CONTAINER}.sif 2> /dev/null
	rm -f docker_${DOCKER_CONTAINER}.tar 2> /dev/null
	docker save $(shell docker images -q ${DOCKER_IMG}) -o docker_${DOCKER_CONTAINER}.tar
	singularity build -F ${DOCKER_CONTAINER}.sif docker-archive://docker_${DOCKER_CONTAINER}.tar


################
# PACKAGE
##
pack:
	rm -f datamill.tar.gz 2> /dev/null
	tar cvzf datamill.tar.gz ./bin/celsius ./bin/datamill  datamill.sif 

#./${DB_PATH}/MasterInput.db ./${DB_PATH}/ModelsDictionaryArise.db ./${DB_PATH}/CelsiusV3nov17_dataArise.db ./data ./scripts


all: build_apsim build build_celsius dbuild sbuild pack