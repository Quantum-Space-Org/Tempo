#!/bin/bash
set -e

# Move to the repo root (assuming this script is in ./IaaC)
cd "$(dirname "$0")/.."

NUGET_SOURCE="https://nuget.pkg.github.com/Quantum-Space-Org/index.json"

# Initialize an associative array to store the latest version of each package
declare -A latest_versions
declare -A package_paths

# Find all .nupkg files in ./build
echo "🔍 Searching for NuGet packages in ./build ..."
nupkgs=$(find ./build -name "*.nupkg")

if [ -z "$nupkgs" ]; then
  echo "❌ No .nupkg files found in ./build"
  exit 1
fi

echo "✅ Found the following .nupkg files:"
echo "$nupkgs"

# Loop through each .nupkg file and keep track of the latest version for each package
for PACKAGE in $nupkgs; do
  # Extract the file name and remove the .nupkg extension
  FILE_NAME=$(basename "$PACKAGE" .nupkg)

  # Use regex to match and extract the package name and version
  if [[ "$FILE_NAME" =~ ^(Quantum\.[A-Za-z0-9\.\-]+)\.([0-9]+\.[0-9]+\.[0-9]+.*)$ ]]; then
    PACKAGE_ID="${BASH_REMATCH[1]}"
    VERSION="${BASH_REMATCH[2]}"

    # Compare versions and keep the latest
    CURRENT_VERSION="${latest_versions["$PACKAGE_ID"]}"
    if [[ -z "$CURRENT_VERSION" || "$VERSION" > "$CURRENT_VERSION" ]]; then
      latest_versions["$PACKAGE_ID"]="$VERSION"
      package_paths["$PACKAGE_ID"]="$PACKAGE"
    fi
  else
    # If the filename doesn't match the expected pattern, print a warning
    echo "⚠️ Skipping unrecognized package filename format: $FILE_NAME"
  fi
done

# Function to increment version (patch level)
increment_version() {
  local version=$1
  local IFS='.'
  IFS='.' read -r -a version_parts <<< "$version"
  local patch="${version_parts[2]}"
  patch=$((patch + 1)) # Increment the patch version
  echo "${version_parts[0]}.${version_parts[1]}.$patch"
}

# Now publish only the latest versions for each package
echo "🚀 Starting to publish the latest versions of packages..."

for PACKAGE_ID in "${!package_paths[@]}"; do
  PACKAGE="${package_paths["$PACKAGE_ID"]}"
  VERSION="1.0.0"

  echo "🚀 Publishing $PACKAGE (Version: $VERSION) to GitHub Packages..."

  # Attempt to push the package and handle conflicts gracefully
  OUTPUT=$(dotnet nuget push "$PACKAGE" --source "$NUGET_SOURCE" --api-key "$GITHUB_TOKEN" 2>&1)

  # If a 409 Conflict occurs, bump the version and retry
  if [[ "$OUTPUT" == *"409 Conflict"* ]]; then
    echo "⚠️ Version $VERSION of package $PACKAGE_ID already exists. Incrementing version..."
    # Increment the version and publish the new version
    NEW_VERSION=$(increment_version "$VERSION")
    NEW_PACKAGE="${PACKAGE_ID}.${NEW_VERSION}.nupkg"
    echo "🚀 Publishing new version: $NEW_PACKAGE to GitHub Packages..."
    dotnet nuget push "$NEW_PACKAGE" --source "$NUGET_SOURCE" --api-key "$GITHUB_TOKEN"
  elif [[ "$OUTPUT" == *"Error"* ]]; then
    # Handle other errors
    echo "❌ Error occurred while publishing $PACKAGE. Error details: $OUTPUT"
    exit 1
  else
    # Successfully published
    echo "✅ Successfully published $PACKAGE (Version: $VERSION)"
  fi
done

echo "✅ All latest versions of the packages have been processed successfully!" 
