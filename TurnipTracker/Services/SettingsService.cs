using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace TurnipTracker.Services
{
    public static class SettingsService
    {
#if DEBUG
        static string publicCache = "1e179afb-8233-46f2-8a44-eecdc7f514da";
        static string privateCache = "16bd6522-2f85-41b0-8bdd-abec010ece1f";
#else
        static string publicCache = string.Empty;
        static string privateCache = string.Empty;
#endif

        const string publicKey = "user_public_key_stable";
        const string privateKey = "user_private_key_stable";

        public static async Task<string> GetPublicKey()
        {
            if (string.IsNullOrWhiteSpace(publicCache))
                publicCache = await GetKey(publicKey);

            return publicCache;
        }

        public static async Task<string> GetPrivateKey()
        {
            if (string.IsNullOrWhiteSpace(privateCache))
                privateCache = await GetKey(privateKey);

            return privateCache;
        }

        static async Task<string> GetKey(string key)
        {
            var val = await SecureStorage.GetAsync(key);

            if (string.IsNullOrWhiteSpace(val))
            {
                val = Guid.NewGuid().ToString();
                await SecureStorage.SetAsync(key, val);
            }

            return val;
        }

        public static bool HasRegistered
        {
            get => Preferences.Get(nameof(HasRegistered), false);
            set => Preferences.Set(nameof(HasRegistered), value);
        }

        public static bool FirstFriendRequest
        {
            get => Preferences.Get(nameof(FirstFriendRequest), true);
            set => Preferences.Set(nameof(FirstFriendRequest), value);
        }


        public static DateTime LastFriendsUpdate
        {
            get => Preferences.Get(nameof(LastFriendsUpdate), DateTime.MinValue);
            set => Preferences.Set(nameof(LastFriendsUpdate), value);
        }

        public static DateTime LastFriendRequestsUpdate
        {
            get => Preferences.Get(nameof(LastFriendRequestsUpdate), DateTime.MinValue);
            set => Preferences.Set(nameof(LastFriendRequestsUpdate), value);
        }

        public static string FriendRequestCount
        {
            get => Preferences.Get(nameof(FriendRequestCount), string.Empty);
            set => Preferences.Set(nameof(FriendRequestCount), value);
        }

        public static bool AskForSurvey
        {
            get => Preferences.Get(nameof(AskForSurvey), true);
            set => Preferences.Set(nameof(AskForSurvey), value);
        }
    }
}
