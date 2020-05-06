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
        static string publicCache = "2e179afb-8233-46f2-8a44-eecdc7f514da";
        static string privateCache = "26bd6522-2f85-41b0-8bdd-abec010ece1f";
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

        public static async Task<string> TransferOut()
        {
            var key1 = await GetPrivateKey();
            var key2 = await GetPublicKey();
            var encoding = Encoding.GetEncoding("iso-8859-1");
            var authenticationBytes = encoding.GetBytes($"{key1}|{key2}");
            return Convert.ToBase64String(authenticationBytes);
        }

        public static async Task<bool> TransferIn(string encodedKeys)
        {
            var encoding = Encoding.GetEncoding("iso-8859-1");
            var s = encoding.GetString(Convert.FromBase64String(encodedKeys));

            var keys = s.Split('|');
            if (keys.Length != 2)
                return false;

            var key0 = keys[0];
            var key1 = keys[1];

            if (!Guid.TryParse(key0, out _) || !Guid.TryParse(key1, out _))
                return false;

            await SecureStorage.SetAsync(privateKey, key0);
            privateCache = key0;
            await SecureStorage.SetAsync(publicKey, key1);
            publicCache = key1;
            return true;
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

        public static bool FirstRun
        {
            get => Preferences.Get(nameof(FirstRun), true);
            set => Preferences.Set(nameof(FirstRun), value);
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
