#!/usr/bin/env bash

echo "Variables:"

# Updating manifest

sed -i '' "s/AC_IOS/$AC_IOS/g" $BUILD_REPOSITORY_LOCALPATH/TurnipTracker/App.xaml.cs

sed -i '' "s/APP_SECRET/$APP_SECRET/g" $BUILD_REPOSITORY_LOCALPATH/TurnipTracker.iOS/Info.plist

echo "Manifest updated!"