#!/bin/sh
set -e

# Get unity license
UNITY_LICENSE=$1
UNITY_PATH=$2

# Activate unity license
xvfb-run --auto-servernum "$UNITY_PATH" -batchmode -nographics -quit -logFile "-" -serial "$UNITY_LICENSE"