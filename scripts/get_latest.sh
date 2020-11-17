#!/bin/bash
PROJECT_NAME="Veracode.OSS.Declare"
RELEASE_URL="https://api.github.com/repos/sebcoles/veracode.oss.declare/releases/latest"
DESTINATION_DIRECTORY="declare"

echo $PROJECT_NAME latest binary downloader

# Get latest
if [[ `uname` == *"NT"* ]] || [[ `uname` == *"UWIN"* ]]; then	
    DOWNLOAD_URL=$(curl --silent $RELEASE_URL | grep -Po '"browser_download_url": "\K.*?(?=")' | grep 'windows')
elif [[ `uname` == *"Darwin"* ]]; then
    DOWNLOAD_URL=$(curl --silent $RELEASE_URL | grep -Po '"browser_download_url": "\K.*?(?=")' | grep 'macos')
else
    DOWNLOAD_URL=$(curl --silent $RELEASE_URL | grep -Po '"browser_download_url": "\K.*?(?=")' | grep 'ubuntu')
fi

curl $DOWNLOAD_URL -O -J -L

# Uncompress and cleanup
if [[ `uname` == *"NT"* ]] || [[ `uname` == *"UWIN"* ]]; then
    unzip *Veracode*.zip -d $DESTINATION_DIRECTORY
	rm *Veracode*.zip
else
	mkdir -p $DESTINATION_DIRECTORY
	tar -xzf *Veracode*.tar.gz
	mv *Veracode*/* $DESTINATION_DIRECTORY/
	rm -rf *Veracode*
fi

# Set to path
cd $DESTINATION_DIRECTORY
export PATH=\$PATH:$(pwd)

echo $PROJECT_NAME downloaded!
