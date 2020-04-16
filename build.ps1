##########################################################################
# This is the Cake bootstrapper script for PowerShell.
# This file is based on https://github.com/cake-build/resources modified for Cake.CoreCLR 
# Feel free to change this file to fit your needs.
##########################################################################

<#
.SYNOPSIS
This is a Powershell script to bootstrap a Cake build.
.DESCRIPTION
This Powershell script will download NuGet if missing, restore NuGet tools (including Cake)
and execute your Cake build script with the parameters you provide.
.PARAMETER Script
The build script to execute.
.PARAMETER Target
The build script target to run.
.PARAMETER Configuration
The build configuration to use.
.PARAMETER Verbosity
Specifies the amount of information to be displayed.
.PARAMETER ShowDescription
Shows description about tasks.
.PARAMETER DryRun
Performs a dry run.
.PARAMETER Experimental
Uses the nightly builds of the Roslyn script engine.
.PARAMETER Mono
Uses the Mono Compiler rather than the Roslyn script engine.
.PARAMETER SkipToolPackageRestore
Skips restoring of packages.
.PARAMETER ScriptArgs
Remaining arguments are added here.
.LINK
https://cakebuild.net
#>

[CmdletBinding()]
Param(
    [string]$Script = "build.cake",
    [string]$Target,
    [string]$Configuration,
    [ValidateSet("Quiet", "Minimal", "Normal", "Verbose", "Diagnostic")]
    [string]$Verbosity,
    [switch]$ShowDescription,
    [Alias("WhatIf", "Noop")]
    [switch]$DryRun,
    [switch]$Experimental,
    [switch]$Mono,
    [switch]$SkipToolPackageRestore,
    [Parameter(Position = 0, Mandatory = $false, ValueFromRemainingArguments = $true)]
    [string[]]$ScriptArgs
)

Write-Host "Preparing to run build script..."

if (!$PSScriptRoot) {
    $PSScriptRoot = Split-Path $MyInvocation.MyCommand.Path -Parent
}

$CAKE_VERSION = "0.37.0"

$TOOLS_DIR = Join-Path $PSScriptRoot "tools"
$CAKE_ROOT = Join-Path $TOOLS_DIR "/cake.coreclr/"
$CAKE_EXE = Join-Path $CAKE_ROOT "/Cake.dll"

# Make sure that dotnet core installed.
try {
    dotnet --version
}
catch {
    Throw "Error: dotnet is not installed."
} 

# Install cake if its not installed
if (!(Test-Path $CAKE_EXE)) {
    # Make sure tools folder exists
    if ((Test-Path $PSScriptRoot) -and !(Test-Path $TOOLS_DIR)) {
        Write-Verbose -Message "Creating tools directory..."
        New-Item -Path $TOOLS_DIR -Type directory | out-null
    }

    # Make sure that tools.csproj exist.
    if (!(Test-Path "$TOOLS_DIR/tools.csproj")) {
        Write-Verbose -Message "Creating tools.csproj..."    
        try {        
            New-Item "$TOOLS_DIR/tools.csproj" -ItemType file
            "<Project Sdk=""Microsoft.NET.Sdk""><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>netcoreapp3.1</TargetFramework></PropertyGroup></Project>" | Out-File -FilePath "$TOOLS_DIR/tools.csproj" -Append
        }
        catch {
            Throw "Could not download packages.config."
        }
    }

    $CAKE_NFW = Join-Path $TOOLS_DIR "/cake/"

    # Add dependencies
    dotnet add $TOOLS_DIR/tools.csproj package Cake.CoreCLR -v $CAKE_VERSION --package-directory $TOOLS_DIR
    # Add Cake.exe for VsCode intellisense support
    dotnet add $TOOLS_DIR/tools.csproj package Cake -v $CAKE_VERSION --package-directory $TOOLS_DIR

    # Clean up
    Move-Item -Path $CAKE_ROOT/$CAKE_VERSION/* -Destination $CAKE_ROOT
    Remove-Item $CAKE_ROOT/$CAKE_VERSION/ -Force -Recurse
    Move-Item -Path $CAKE_NFW/$CAKE_VERSION/* -Destination $CAKE_NFW
    Remove-Item $CAKE_NFW/$CAKE_VERSION/ -Force -Recurse
    Remove-Item $TOOLS_DIR/tools.csproj
}

# Make sure that Cake has been installed.
if (!(Test-Path $CAKE_EXE)) {
    Throw "Could not find Cake.dll at $CAKE_EXE"
}

# Build Cake arguments
$cakeArguments = @("$Script");
if ($Target) { $cakeArguments += "-target=$Target" }
if ($Configuration) { $cakeArguments += "-configuration=$Configuration" }
if ($Verbosity) { $cakeArguments += "-verbosity=$Verbosity" }
if ($ShowDescription) { $cakeArguments += "-showdescription" }
if ($DryRun) { $cakeArguments += "-dryrun" }
if ($Experimental) { $cakeArguments += "-experimental" }
if ($Mono) { $cakeArguments += "-mono" }
$cakeArguments += $ScriptArgs

# Start Cake
Write-Host "Running build script..."
&dotnet $TOOLS_DIR/cake.coreclr/Cake.dll $cakeArguments
exit $LASTEXITCODE