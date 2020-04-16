#!/usr/bin/env bash

##########################################################################
# This is the Cake bootstrapper script for Linux and OS X.
# This file is based on https://github.com/cake-build/resources modified for Cake.CoreCLR 
# Feel free to change this file to fit your needs.
##########################################################################

CAKE_VERSION=0.37.0

# Define directories.
SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
TOOLS_DIR=$SCRIPT_DIR/tools
CAKE_ROOT= $TOOLS_DIR/cake.coreclr/
CAKE_EXE=$TOOLS_DIR/cake.coreclr/Cake.dll

# Define default arguments.
SCRIPT="build.cake"
TARGET="Default"
CONFIGURATION="Release"
VERBOSITY="verbose"
DRYRUN=
SHOW_VERSION=false
CAKE_ARGUMENTS=()

# Parse arguments.
for i in "$@"; do
    case $1 in
        -s|--script) SCRIPT="$2"; shift ;;
        --) shift; CAKE_ARGUMENTS+=("$@"); break ;;
        *) CAKE_ARGUMENTS+=("$1") ;;
    esac
    shift
done

# Make sure that dotnet core installed.
if ! [ -x "$(command -v dotnet)" ]; then
  echo 'Error: dotnet is not installed.' >&2
  exit 1
fi

# Install cake if its not installed
if [ ! -f "$CAKE_EXE" ]; then

    # Make sure the tools folder exist.
    if [ ! -d "$TOOLS_DIR" ]; then
    mkdir "$TOOLS_DIR"
    fi

    # Make sure that tools.csproj exist.
    if [ ! -f "$TOOLS_DIR/tools.csproj" ]; then
        echo "Creating tools.csproj..."
        echo "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>netcoreapp3.1</TargetFramework></PropertyGroup></Project>" > $TOOLS_DIR/tools.csproj
        if [ $? -ne 0 ]; then
            echo "An error occurred while creating tools.csproj."
            exit 1
        fi
    fi

    # Add dependencies
    dotnet add $TOOLS_DIR/tools.csproj package Cake.CoreCLR -v $CAKE_VERSION --package-directory $TOOLS_DIR
    mv $TOOLS_DIR/cake.coreclr/$CAKE_VERSION/* $TOOLS_DIR/cake.coreclr/
    rm -rf $TOOLS_DIR/cake.coreclr/$CAKE_VERSION/
    rm -f $TOOLS_DIR/tools.csproj
fi

# Make sure that Cake has been installed.
if [ ! -f "$CAKE_EXE" ]; then
    echo "Could not find Cake.exe at '$CAKE_EXE'."
    exit 1
fi

# Start Cake
if $SHOW_VERSION; then
    dotnet $TOOLS_DIR/cake.coreclr/Cake.dll -version
else
    dotnet $TOOLS_DIR/cake.coreclr/Cake.dll $SCRIPT "${CAKE_ARGUMENTS[@]}"
fi