#!/bin/bash
set -e

# Version to release
VERSION="1.1.6-alpha"

# Colors
GREEN='\033[0;32m'
NC='\033[0m' # No Color

echo -e "${GREEN}Building CSX Framework v$VERSION...${NC}"

# 1. Clean previous builds
rm -rf Duality.Compiler/bin Duality.Compiler/obj Duality.Core/bin Duality.Core/obj Duality.CLI/bin Duality.CLI/obj
dotnet normalize

# 2. Build & Pack Core
echo -e "${GREEN}Packing Duality.Core...${NC}"
dotnet pack Duality.Core/Duality.Core.csproj -c Release /p:PackageVersion=$VERSION

# 3. Build & Pack Compiler
echo -e "${GREEN}Packing Duality.Compiler...${NC}"
dotnet pack Duality.Compiler/Duality.Compiler.csproj -c Release /p:PackageVersion=$VERSION

# 4. Build & Pack CLI
echo -e "${GREEN}Packing Duality.CLI...${NC}"
dotnet pack Duality.CLI/Duality.CLI.csproj -c Release /p:PackageVersion=$VERSION

echo -e "${GREEN}Build Complete! Packages are in bin/Release folders.${NC}"
echo ""
echo "To publish to NuGet.org, run:"
echo "dotnet nuget push Duality.Core/bin/Release/Duality.Core.$VERSION.nupkg -k YOUR_API_KEY -s https://api.nuget.org/v3/index.json"
echo "dotnet nuget push Duality.Compiler/bin/Release/Duality.Compiler.$VERSION.nupkg -k YOUR_API_KEY -s https://api.nuget.org/v3/index.json"
echo "dotnet nuget push Duality.CLI/nupkg/Duality.CLI.$VERSION.nupkg -k YOUR_API_KEY -s https://api.nuget.org/v3/index.json"
