#!/bin/sh
set -e

# Get unity license
UNITY_LICENSE=$1
UNITY_PATH=$2

# Dump license to a ulf file
echo "$UNITY_LICENSE" > license.ulf

# Activate unity license
xvfb-run --auto-servernum "$UNITY_PATH" -batchmode -nographics -quit -logFile "-" -serial license.ulf