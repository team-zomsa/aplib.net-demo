#!/bin/sh
set -e

activation_file=${UNITY_ACTIVATION_FILE:-./unity3d.alf}

if [[ -z "${UNITY_USERNAME}" ]] || [[ -z "${UNITY_PASSWORD}" ]]; then
  echo "UNITY_USERNAME or UNITY_PASSWORD environment variables are not set, please refer to instructions in the readme and add these to your secret environment variables."
  exit 1
fi

# Dump $UNITY_LICENSE to a file
# echo "$UNITY_LICENSE" > license.ulf

# Activate unity license
# xvfb-run --auto-servernum "$UNITY_PATH" -batchmode -nographics -quit -logFile "-" -serial license.ulf
# chmod +x "$UNITY_PATH"
# sudo "$UNITY_PATH" -batchmode -nographics -quit -logFile "-"

xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' \
  Unity/Hub/Editor/2022.3.19f1/Editor/Unity \
    -logFile /dev/stdout \
    -batchmode \
    -nographics \
    -username "$UNITY_USERNAME" -password "$UNITY_PASSWORD" |
      tee ./unity-output.log

# Debug cat
cat ./unity-output.log

cat ./unity-output.log |
  grep 'LICENSE SYSTEM .* Posting *' |
  sed 's/.*Posting *//' > "${activation_file}"

ls "${UNITY_ACTIVATION_FILE:-./unity3d.alf}"
exit_code=$?



# ls -R Unity/Hub/Editor/2022.3.19f1/Editor/

# chmod +x Unity/Hub/Editor/2022.3.19f1/Editor/Unity
if [[ ${exit_code} -eq 0 ]]; then
    Unity/Hub/Editor/2022.3.19f1/Editor/Unity -batchmode -nographics -quit -logFile "-" -manualLicenseFile "${activation_file}"
fi
exit $exit_code