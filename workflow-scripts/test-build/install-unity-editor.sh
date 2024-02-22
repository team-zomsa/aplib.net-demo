#!/bin/sh
set -e

# Download unity download keys
wget -qO - https://hub.unity3d.com/linux/keys/public | gpg --dearmor | sudo tee /usr/share/keyrings/Unity_Technologies_ApS.gpg > /dev/null

# Retrieve unity hub versions
sudo sh -c 'echo "deb [signed-by=/usr/share/keyrings/Unity_Technologies_ApS.gpg] https://hub.unity3d.com/linux/repos/deb stable main" > /etc/apt/sources.list.d/unityhub.list'

# Install unity hub and required packages
sudo apt update
sudo apt install unityhub
sudo apt install -y libgconf-2-4 libglu1 libasound2 libgtk2.0-0 libgtk-3-0 libnss3 zenity xvfb libfuse2 at-spi2-core
sudo apt update

# Set up directory for the editor installation
mkdir -p ./Unity/Hub/Editor

# Install unity editor
xvfb-run --auto-servernum unityhub --headless install-path --set ./Unity/Hub/Editor
xvfb-run --auto-servernum unityhub --headless install --version 2022.3.19f1 --changeset 244b723c30a6

# Get current path
current_path=$(pwd)

echo "HELLO I JUST INSTALLED UNITY YOU CAN FIND IT HERE:"
echo "$current_path/Unity/Hub/Editor/..."

echo ""
echo ""
echo ""
echo ""
echo ""

echo "LET ME ALSO PRINT THAT DIRECTORY FOR YOU"
ls -alh ./Unity/Hub/Editor