#!/usr/bin/env bash

echo "Variables:"

# Updating manifest
sed -i '' "s|AC_ANDROID|$AC_ANDROID|g" $BUILD_REPOSITORY_LOCALPATH/TurnipTracker/App.xaml.cs

sed -i '' "s|AC_SYNC|$AC_SYNC|g" $BUILD_REPOSITORY_LOCALPATH/TurnipTracker/App.xaml.cs

sed -i '' "s|AC_BASEURL|$AC_BASEURL|g" $BUILD_REPOSITORY_LOCALPATH/TurnipTracker/App.xaml.cs
sed -i '' "s|AC_GetFriendsKey|$AC_GetFriendsKey|g" $BUILD_REPOSITORY_LOCALPATH/TurnipTracker/App.xaml.cs
sed -i '' "s|AC_GetFriendRequestsKey|$AC_GetFriendRequestsKey|g" $BUILD_REPOSITORY_LOCALPATH/TurnipTracker/App.xaml.cs
sed -i '' "s|AC_PostApproveFriendRequestKey|$AC_PostApproveFriendRequestKey|g" $BUILD_REPOSITORY_LOCALPATH/TurnipTracker/App.xaml.cs
sed -i '' "s|AC_DeleteRemoveFriendKey|$AC_DeleteRemoveFriendKey|g" $BUILD_REPOSITORY_LOCALPATH/TurnipTracker/App.xaml.cs
sed -i '' "s|AC_PostSubmitFriendRequestKey|$AC_PostSubmitFriendRequestKey|g" $BUILD_REPOSITORY_LOCALPATH/TurnipTracker/App.xaml.cs
sed -i '' "s|AC_PutUpdateProfileKey|$AC_PutUpdateProfileKey|g" $BUILD_REPOSITORY_LOCALPATH/TurnipTracker/App.xaml.cs
sed -i '' "s|AC_PutUpdateTurnipPricesKey|$AC_PutUpdateTurnipPricesKey|g" $BUILD_REPOSITORY_LOCALPATH/TurnipTracker/App.xaml.cs
sed -i '' "s|AC_PostCreateProfileKey|$AC_PostCreateProfileKey|g" $BUILD_REPOSITORY_LOCALPATH/TurnipTracker/App.xaml.cs
sed -i '' "s|AC_PostRemoveFriendRequestKey|$AC_PostRemoveFriendRequestKey|g" $BUILD_REPOSITORY_LOCALPATH/TurnipTracker/App.xaml.cs
sed -i '' "s|AC_GetFriendRequestCountKey|$AC_GetFriendRequestCountKey|g" $BUILD_REPOSITORY_LOCALPATH/TurnipTracker/App.xaml.cs

if [ "$APPCENTER_BRANCH" == "appstore" ]; then
sed -i '' "s|AC_IsStore|$AC_IsStore|g" $BUILD_REPOSITORY_LOCALPATH/TurnipTracker/App.xaml.cs
fi

cat $BUILD_REPOSITORY_LOCALPATH/TurnipTracker/App.xaml.cs

echo "Manifest updated!"