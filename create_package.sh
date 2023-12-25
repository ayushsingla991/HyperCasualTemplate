#!/bin/bash

# Get the directory path of the script file
script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Ask for version number
read -p "Enter version number (e.g., 1.0.0): " version

# Set the Unity project folder path
unity_project_folder="$script_dir"

# Set the package/ folder path relative to the script folder
package_data_folder="$script_dir/TemplateFinal/package/"

# Set the package/ProjectData~ folder path relative to the script folder
project_data_folder="$script_dir/TemplateFinal/package/ProjectData~"

# Create the package/ProjectData~ folder
mkdir -p "$project_data_folder"

# Copy Assets, Packages, and Project Settings to ProjectData~
cp -r "$unity_project_folder/Assets" "$project_data_folder"
cp -r "$unity_project_folder/Packages" "$project_data_folder"
cp -r "$unity_project_folder/ProjectSettings" "$project_data_folder"

# Delete ProjectVersion.txt from ProjectSettings folder
rm "$project_data_folder/ProjectSettings/ProjectVersion.txt"

# Create package.json in the package folder
echo "{ \"name\": \"com.unity.template.hypercasualui\", \"displayName\": \"Hypercasual\", \"version\": \"$version\", \"type\": \"template\", \"unity\": \"2019.4\", \"description\": \"Use this template to create Hypercasual Base.\", \"dependencies\": {} }" > "$package_data_folder/package.json"

# Change to the TemplateFinal directory
cd "$script_dir/TemplateFinal" || exit

# Create a tar file with version number in the filename
tar czf "com.unity.template.hypercasualui-$version.tgz" package

# Remove the original package folder after creating the tar archive
rm -r package