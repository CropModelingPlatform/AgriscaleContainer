# .NET REPO AND VERSION
ARG REPO=mcr.microsoft.com/dotnet/runtime-deps
ARG DOTNET_VERSION_5=5.0.1
ARG DOTNET_VERSION_6=6.0.0
ARG DOTNET_VERSION_8=8.0.0

# Installer image
#FROM amd64/buildpack-deps:buster-curl as installer
FROM debian:bookworm-slim AS installer
RUN apt-get update && apt-get install -y curl ca-certificates

ARG DOTNET_VERSION_5 
ARG DOTNET_VERSION_6
ARG DOTNET_VERSION_8


RUN apt-get update
ARG USER=root
USER ${USER}

# Install .NET 5.0.0
RUN curl -SL --output dotnet5.tar.gz https://dotnetcli.azureedge.net/dotnet/Runtime/$DOTNET_VERSION_5/dotnet-runtime-$DOTNET_VERSION_5-linux-x64.tar.gz \
    && dotnet_sha512='791af58eb2a4c7e7a65f0d940cfa09cda3318cb482728dbf40848543e1d04aa9ffd7e8d4fdede1b4fbc6f54250bae4e0c4a5bf208e04705f5c5f00375ac009b7' \
    && echo "$dotnet_sha512 dotnet5.tar.gz" | sha512sum -c - \
    && mkdir -p /dotnet/5.0 \
    && tar -ozxf dotnet5.tar.gz -C /dotnet/5.0 \
    && rm dotnet5.tar.gz

# Installa .NET 6.0.0
RUN curl -fSL --output dotnet6.tar.gz https://dotnetcli.azureedge.net/dotnet/Runtime/$DOTNET_VERSION_6/dotnet-runtime-$DOTNET_VERSION_6-linux-x64.tar.gz \
        && dotnet_sha512='7cc8d93f9495b516e1b33bf82af3af605f1300bcfeabdd065d448cc126bd97ab4da5ec5e95b7775ee70ab4baf899ff43671f5c6f647523fb41cda3d96f334ae5' \
        && echo "$dotnet_sha512 dotnet6.tar.gz" | sha512sum -c - \
        && mkdir -p /dotnet/6.0 \
        && tar -ozxf dotnet6.tar.gz -C /dotnet/6.0 \
        && rm dotnet6.tar.gz

RUN curl -fSL --output dotnet8.tar.gz https://dotnetcli.azureedge.net/dotnet/Runtime/$DOTNET_VERSION_8/dotnet-runtime-$DOTNET_VERSION_8-linux-x64.tar.gz \
        && dotnet_sha512='16a93af328bcf61775875f4007c23081e2cb7aa8e2fba724aea6a61bc7ecf7466cc368121b08b58ac3b72f68cb67801c68c6505591eb35f18461db856bb08b37' \
        && echo "$dotnet_sha512 dotnet8.tar.gz" | sha512sum -c - \
        && mkdir -p /dotnet/8.0 \
        && tar -ozxf dotnet8.tar.gz -C /dotnet/8.0 \
        && rm dotnet8.tar.gz
        
RUN apt-get purge -y --auto-remove curl
# COMPILE DSSAT
# Dssat builder
FROM debian:bullseye-slim AS builder
RUN apt-get update && apt-get install -y build-essential cmake gfortran
ARG DSSAT_DIR
COPY $DSSAT_DIR /dssat
RUN ls -l /dssat
RUN rm -Rf /dssat/build && mkdir /dssat/build && mkdir /usr/share/dssat
WORKDIR /dssat/build
RUN cmake -DCMAKE_INSTALL_PREFIX=/usr/share/dssat .. && make install
RUN ls -l /usr/share/dssat

#DSSAT V4.8

ARG DSSAT_DIR_latest
COPY $DSSAT_DIR_latest /dssatv48
RUN ls -l /dssatv48
RUN rm -Rf /dssatv48/build && mkdir /dssatv48/build && mkdir /usr/share/dssatv48
WORKDIR /dssatv48/build
RUN cmake -DCMAKE_INSTALL_PREFIX=/usr/share/dssatv48 .. && make install
RUN ls -l /usr/share/dssatv48

USER root
RUN rm -rf /var/lib/apt/lists/*

# Application image
FROM mcr.microsoft.com/dotnet/runtime:6.0-bullseye-slim AS app

# Add .NET Core 3.1 runtime.
COPY --from=mcr.microsoft.com/dotnet/runtime:5.0 /usr/share/dotnet /usr/share/dotnet
RUN mkdir /dotnet-backup && find /usr/share/dotnet -maxdepth 1 -type f -exec cp -l {} /dotnet-backup/ \;
# Add .NET 5.0 runtime.
COPY --from=mcr.microsoft.com/dotnet/runtime:6.0 /usr/share/dotnet /usr/share/dotnet
# FROM debian:buster-slim
COPY --from=installer /dotnet/8.0 /usr/share/dotnet
RUN mv /dotnet-backup/* /usr/share/dotnet/ && rmdir /dotnet-backup

# Vérifier que les deux versions sont installées
RUN dotnet --list-runtimes

# DSSAT
COPY --from=builder ["/usr/share/dssat", "/usr/share/dssat"]
COPY --from=builder ["/usr/share/dssatv48", "/usr/share/dssatv48"]
RUN ls -l /usr/share/dssat
RUN chmod a+x /usr/share/dssat/run_dssat && chmod a+x /usr/share/dssat/dscsm047
RUN ln -s /usr/share/dssat/run_dssat /usr/bin/dssat

RUN ls -l /usr/share/dssatv48
RUN chmod a+x /usr/share/dssatv48/run_dssat && chmod a+x /usr/share/dssatv48/dscsm048
RUN ln -s /usr/share/dssatv48/run_dssat /usr/bin/dssatv48


#APSIM
# Copy build artifacts from the intermediate container to /apsim
RUN mkdir -p /usr/share/apsim
COPY ./bin/apsim/* /usr/share/apsim/
RUN ls -l /usr/share/apsim
# Add apsim to path
RUN chmod a+x /usr/share/apsim/Models
RUN ln -s /usr/share/apsim/Models /usr/bin/




# ADD USER ARISE
RUN useradd -d /home/arise -m -s /bin/bash arise  

# INSTALL CONDA AND PYTHON 3 PACKAGES
ENV PATH="/opt/conda/bin:${PATH}"
ARG PATH="/opt/conda/bin:${PATH}"
RUN apt-get update && apt-get install --no-install-recommends -y wget zip unzip parallel cdo git
# && rm -rf /var/lib/apt/lists/*
RUN wget https://github.com/conda-forge/miniforge/releases/latest/download/Miniforge3-Linux-x86_64.sh -O Miniforge3.sh \
    && bash Miniforge3.sh -b -p /opt/conda \
    && rm -f Miniforge3.sh \
    && ln -s /opt/conda/etc/profile.d/conda.sh /etc/profile.d/conda.sh \
    && echo ". /opt/conda/etc/profile.d/conda.sh" >> ~/.bashrc \
    && echo "conda activate base" >> ~/.bashrc \
    && find /opt/conda/ -follow -type f -name '*.a' -delete \
    && find /opt/conda/ -follow -type f -name '*.js.map' -delete \
    && /opt/conda/bin/conda clean -afy 
    #&& conda install --yes mamba -n base \
RUN apt-get update && apt-get install -y python3 python3-pip sqlite3 libsqlite3-dev


RUN pip install netcdf4 xarray pandas glob2 numba scipy joblib geopandas rioxarray rasterio \
    && pip install --no-cache dask==2023.1.1 fastparquet pyarrow zarr numcodecs duckdb \
    && pip install --no-cache --upgrade distributed 

RUN conda --version

#RUN conda install -c conda-forge cdo netcdf4 xarray=0.16.2 xarray-extras=0.4.2 pandas=1.1.5 glob2 joblib sqlite dask numba
# remove xarray-extras=0.4.2

# INSTALL GIS LIBS (gdal)
RUN apt-get install -y gdal-bin build-essential openmpi-bin openmpi-doc libopenmpi-dev time strace && rm -rf /var/lib/apt/lists/*

RUN pip install mpi4py memory_profiler
# Install ModFileGen from github https://github.com/CropModelingPlatform/ModFileGen.git
# Install ModFileGen
ARG CACHE_BUST

RUN echo "Cache Bust: $CACHE_BUST" && rm -rf ModFileGen && git clone --branch stics https://github.com/cyrillemidingoyi/ModFileGen.git \
    && cd ModFileGen && python setup.py install

# Test MPI Installation
RUN mpicc --version
RUN mpiexec --version
#RUN mpiexec -n 5 python -m mpi4py.bench helloworld

# INSTALL STICS V9
ARG JAVASTICS_ZIP_FILE
COPY $JAVASTICS_ZIP_FILE /usr/src
RUN ls -l /usr/src
RUN unzip -q /usr/src/$JAVASTICS_ZIP_FILE -d /opt/stics
RUN ls -l /opt/stics
RUN chmod -R a+rX /opt/stics
RUN chmod a+x /opt/stics/bin/stics_modulo
# remove $JAVASTICS_ZIP_FILE
RUN rm -rf /usr/src/$JAVASTICS_ZIP_FILE

#INSTALL STICS V10
ARG JAVASTICS_ZIP_FILE10
COPY $JAVASTICS_ZIP_FILE10 /usr/src
RUN ls -l /usr/src
RUN unzip -q /usr/src/$JAVASTICS_ZIP_FILE10 -d /opt/sticsv10
RUN mv /opt/sticsv10/JavaSTICS-1.5.2-STICS-10.2.0/* /opt/sticsv10/
RUN rm -rf /opt/sticsv10/JavaSTICS-1.5.2-STICS-10.2.0
RUN ls -l /opt/sticsv10
RUN chmod -R a+rX /opt/sticsv10
RUN chmod a+x /opt/sticsv10/bin/stics_modulo
# remove $JAVASTICS_ZIP_FILE10
RUN rm -rf /usr/src/$JAVASTICS_ZIP_FILE10

#INSTALL DATAMILL
RUN mkdir -p /usr/share/datamill
COPY ./bin/datamill/* /usr/share/datamill/
RUN chmod a+x /usr/share/datamill/datamill
RUN ln -s /usr/share/datamill/datamill /usr/bin/


#INSTALL CELSIUS
RUN echo "Cache Bust: $CACHE_BUST" mkdir -p /usr/share/celsius
COPY ./bin/celsius/* /usr/share/celsius/
RUN chmod a+x /usr/share/celsius/celsius
RUN ln -s /usr/share/celsius/celsius /usr/bin/

# CREATE AND SET WORKSPACE FOR USER ARISE
RUN mkdir /work
RUN chown -R arise /work
USER arise
WORKDIR /work