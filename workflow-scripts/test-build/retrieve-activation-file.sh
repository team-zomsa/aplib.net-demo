#!/usr/bin/env bash
set -e


# Check for existance of required environment variables
if [[ -z "${UNITY_USERNAME}" ]] || [[ -z "${UNITY_PASSWORD}" ]]; then
  echo "UNITY_USERNAME or UNITY_PASSWORD environment variables are not set, please refer to instructions in the readme and add these to your secret environment variables."
  exit 1
fi

xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' \
  Unity/Hub/Editor/2022.3.19f1/Editor/Unity \
    -logFile /dev/stdout \
    -batchmode \
    -nographics \
    -createManualActivationFile \
    -username "$UNITY_USERNAME" -password "$UNITY_PASSWORD"