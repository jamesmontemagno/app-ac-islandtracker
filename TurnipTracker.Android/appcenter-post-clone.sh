#!/usr/bin/env bash

echo "Variables:"

# Updating manifest
sed -i '' "s/AC_ANDROID/$AC_ANDROID/g" $BUILD_REPOSITORY_LOCALPATH/TurnipTracker/App.xaml.cs

sed -i '' "s/AC_SYNC/$AC_SYNC/g" $BUILD_REPOSITORY_LOCALPATH/TurnipTracker/App.xaml.cs

cat $BUILD_REPOSITORY_LOCALPATH/TurnipTracker/App.xaml.cs

echo "Manifest updated!"