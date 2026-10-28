#!/usr/bin/env bash
set -euo pipefail
export DOTNET_ROOT=/workspace/.dotnet
export DOTNET_CLI_HOME=/workspace/.dotnet-home
export NUGET_PACKAGES=/workspace/.nuget/packages
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export PATH="$DOTNET_ROOT:$PATH"
if [ ! -x "$DOTNET_ROOT/dotnet" ]; then
  installer=$(mktemp /tmp/festivos-dotnet.XXXXXX)
  trap 'rm -f "$installer"' EXIT
  curl -fsSL https://dot.net/v1/dotnet-install.sh -o "$installer"
  bash "$installer" --version 10.0.403 --install-dir "$DOTNET_ROOT"
fi
cd /workspace/Festivos
dotnet run --project tests/Tests.csproj -c Release
dotnet publish src/App/App.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o artifacts/publish
dotnet build tests/WindowsSmoke -c Release
