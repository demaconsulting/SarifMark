#!/usr/bin/env bash
# Build and test SarifMark

set -e  # Exit on error

echo "🔧 Building SarifMark..."
dotnet build --configuration Release

echo "✅ Running tests..."
dotnet test --configuration Release --verbosity normal

echo "✨ Build and test completed successfully!"
