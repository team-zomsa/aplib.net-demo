#!/bin/sh
set -e

# Dump license to a ulf file
echo "$UNITY_LICENSE" > license.ulf

echo "$(<license.ulf)"

# Activate unity license
# xvfb-run --auto-servernum "$UNITY_PATH" -batchmode -nographics -quit -logFile "-" -serial license.ulf
# chmod +x "$UNITY_PATH"
# sudo "$UNITY_PATH" -batchmode -nographics -quit -logFile "-"

# ls -R Unity/Hub/Editor/2022.3.19f1/Editor/

# chmod +x Unity/Hub/Editor/2022.3.19f1/Editor/Unity
Unity/Hub/Editor/2022.3.19f1/Editor/Unity -batchmode -nographics -quit -logFile "-" -manualLicenseFile "$UNITY_LICENSE"