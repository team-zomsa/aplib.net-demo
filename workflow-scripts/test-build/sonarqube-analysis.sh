#!/bin/sh
set -e

# This still requires comments lol

sudo rm -rf .\.sonar\scanner
pwd; ls -alh
xvfb-run --auto-servernum ${{ env.UNITY_PATH }} -batchmode -nographics -quit -logFile "-" -customBuildName aplib.net-demo -projectPath aplib.net-demo/ -executeMethod Packages.Rider.Editor.RiderScriptEditor.SyncSolution
pwd; ls -alh
sed -i 's/<ReferenceOutputAssembly>false<\/ReferenceOutputAssembly>/<ReferenceOutputAssembly>true<\/ReferenceOutputAssembly>/g' *.csproj
sed -i 's/\([A-Za-z0-9.-]\+csproj\)/Card-Game-Simulator\/&/g' Card-Game-Simulator.sln
mv aplib.net-demo.sln ..
cd ..
dotnet tool install --global dotnet-sonarscanner
dotnet sonarscanner begin \
.\.sonar\scanner\dotnet-sonarscanner begin \
/k:"team-zomsa_aplib.net-demo" /o:"team-zomsa" \
/d:sonar.token="${{ secrets.SONAR_TOKEN }}" \
/d:sonar.host.url="https://sonarcloud.io" \
/d:sonar.cs.vscoveragexml.reportsPaths=coverage.xml \
dotnet build aplib.net-demo.sln
dotnet sonarscanner end /d:sonar.login="$SONAR_TOKEN"
cd aplib.net-demo