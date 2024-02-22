#!/bin/sh
set -e

# Dump license to a ulf file
# echo "$UNITY_LICENSE"

echo $UNITY_PATH

# Activate unity license
# xvfb-run --auto-servernum "$UNITY_PATH" -batchmode -nographics -quit -logFile "-" -serial license.ulf
chmod +x "$UNITY_PATH"
sudo "$UNITY_PATH" -batchmode -nographics -quit -logFile "-"