using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
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
            {
                try
                {
                    publicCache = await GetKey(publicKey);

                    if(Preferences.Get(publicKey, string.Empty) == string.Empty)
                        publicCache = Preferences.Get(publicKey, string.Empty);

                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>
                    {
                        ["key"] = "public"
                    });

                    publicCache = Preferences.Get(publicKey, string.Empty);
                }
            }

            return publicCache;
        }

        public static async Task<string> GetPrivateKey()
        {
            if (string.IsNullOrWhiteSpace(privateCache))
            {
                try
                {
                    privateCache = await GetKey(privateKey);

                    if (Preferences.Get(privateKey, string.Empty) == string.Empty)
                        privateCache = Preferences.Get(privateKey, string.Empty);
                }
                catch(Exception ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>
                    {
                        ["key"] = "private"
                    });

                    privateCache = Preferences.Get(privateKey, string.Empty);
                }
            }

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
            Preferences.Set(privateKey, key0);
            privateCache = key0;
            await SecureStorage.SetAsync(publicKey, key1);
            Preferences.Set(publicKey, key1);
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
                Preferences.Set(key, val);
            }

            return val;
        }

        public static bool FirstRun
        {
            get => Preferences.Get(nameof(FirstRun), true);
            set => Preferences.Set(nameof(FirstRun), value);
        }

        public static bool UpdateProfile
        {
            get => Preferences.Get(nameof(UpdateProfile), false);
            set => Preferences.Set(nameof(UpdateProfile), value);
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

        public static bool HideFirstTimeBuying
        {
            get => Preferences.Get(nameof(HideFirstTimeBuying), false);
            set => Preferences.Set(nameof(HideFirstTimeBuying), value);
        }

        public static bool AutoRefreshFriends
        {
            get => Preferences.Get(nameof(AutoRefreshFriends), true);
            set => Preferences.Set(nameof(AutoRefreshFriends), value);
        }

        public static int RefreshAfterHours
        {
            get => Preferences.Get(nameof(RefreshAfterHours), 2);
            set => Preferences.Set(nameof(RefreshAfterHours), value);
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


        public static int CalcTurnipsSlots
        {
            get => Preferences.Get(nameof(CalcTurnipsSlots), 40);
            set => Preferences.Set(nameof(CalcTurnipsSlots), value);
        }

        public static bool ForceRefreshFriends
        {
            get => Preferences.Get(nameof(ForceRefreshFriends), false);
            set => Preferences.Set(nameof(ForceRefreshFriends), value);
        }

        public static int SyncCount
        {
            get => Preferences.Get(nameof(SyncCount), 0);
            set => Preferences.Set(nameof(SyncCount), value);
        }

    }
}
