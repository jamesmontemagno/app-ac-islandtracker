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
        static string publicCache = "TESTPUBLIC";
        static string privateCache = "TESTPRIVATE";
#else
        static string publicCache = string.Empty;
        static string privateCache = string.Empty;
#endif

        const string publicKey = "user_public_key";
        const string privateKey = "user_private_key";

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
    }
}
