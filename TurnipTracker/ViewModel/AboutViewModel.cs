using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
using MvvmHelpers.Commands;
using Newtonsoft.Json;
using TurnipTracker.Model;
using TurnipTracker.Services;
using Xamarin.Essentials;

// On April 17th 2020
// MattLeibow gifted 10 subscriptions to everyone!
// h0usebesuch subscribed for 11th month!
// LuciousAdorno subscribed for the first month at Tier 1!!
// mikeholloway24 cheered 100 bits!
// AdenEarnshaw cheered 200 bits
// AncientCoder Gifted a tier 1 to MattLeibow
// sparkie108 cheered 100 bits!
// h0usebesuch cheered 100 bits!
// reillydom subscribed with Twitch Prime for the first time

namespace TurnipTracker.ViewModel
{
    public class AboutViewModel : ViewModelBase
    {
        public AsyncCommand SendEmailCommand { get; }
        public AsyncCommand SubscribeCommand { get; }
        public AsyncCommand<string> OpenBrowserCommand { get; }

        public List<AttributionItem> Attributions { get; }

        public string AppInfo => $"{Xamarin.Essentials.AppInfo.VersionString} ({Xamarin.Essentials.AppInfo.BuildString})";


        List<(string Title, string Key, string Platforms)> podcasts;
        public AboutViewModel()
        {
            SubscribeCommand = new AsyncCommand(Subscribe);
            SendEmailCommand = new AsyncCommand(SendEmail);
            OpenBrowserCommand = new AsyncCommand<string>(OpenBrowser);

            Attributions = new List<AttributionItem>
            {
                new AttributionItem { Tag = "ac-nh-turnip-prices", Text = "Mike Bryant - Turnip predictor algorithm"},
                new AttributionItem { Tag = "prophet", Text = "Mike Bryant - Turnip Prophet"},
                new AttributionItem { Tag = "ninji", Text = "Ninji - original ACNH code port"},
                new AttributionItem { Tag = "sparkie108", Text="Steve aka sparkie108 for porting code"},
                new AttributionItem { Tag = "ardonpixels", Text="Icon and Artwork by Ardon Pixels"},
                new AttributionItem { Tag = "omaimakhan", Text = "Custom Font by Omaima Khan"},
                new AttributionItem { Tag = "fontawesome", Text = "Icon fonts by Font Awesome"},
                new AttributionItem { Tag = "mvvm-helpers", Text ="Mvvm Helpers"},
                new AttributionItem { Tag = "monkey-cache", Text = "Monkey Cache"},
                new AttributionItem { Tag = "pancake", Text="PancakeView"},
                new AttributionItem { Tag = "resizetizernt", Text = "Resizetizer.NT"},
                new AttributionItem { Tag = "sharpnado", Text = "Sharpnado"},
                new AttributionItem { Tag = "syncfusion", Text = "Syncfusion for Xamarin"},
                new AttributionItem { Tag = "essentials", Text = "Xamarin.Essentials"},
                new AttributionItem { Tag = "forms", Text = "Xamarin.Forms"}
            };
            podcasts = new List<(string Title, string Key, string Platforms)>
            {
                ("Apple Podcasts", "dispatch_apple", DevicePlatform.iOS.ToString()),
                ("Castbox", "dispatch_castbox", DevicePlatform.Android + "_" + DevicePlatform.iOS),
                ("Castro", "dispatch_castro", DevicePlatform.iOS.ToString()),
                ("Google Podcasts", "dispatch_google", DevicePlatform.Android + "_" + DevicePlatform.iOS),
                ("Overcast", "dispatch_ovdrcast", DevicePlatform.iOS.ToString()),
                ("Pocket Casts", "dispatch_pocketcasts", DevicePlatform.Android + "_" + DevicePlatform.iOS),
                ("Spotify", "dispatch_spotify", DevicePlatform.Android + "_" + DevicePlatform.iOS),
                ("Stitcher", "dispatch_stitcher", DevicePlatform.Android + "_" + DevicePlatform.iOS),
                ("TuneIn", "dispatch_tunein", DevicePlatform.Android + "_" + DevicePlatform.iOS),
                ("RSS", "dispatch_rss", DevicePlatform.Android + "_" + DevicePlatform.iOS),
            };
        }

        bool attachDetails = true;
        public bool AttachDetails
        {
            get => attachDetails;
            set => SetProperty(ref attachDetails, value);
        }

        async Task Subscribe()
        {
            var services = podcasts
                .Where(s => s.Platforms.Contains(DeviceInfo.Platform.ToString()))
                .Select(s => s.Title);

            var result = await App.Current.MainPage.DisplayActionSheet("Subscribe on:", "Cancel", null, services.ToArray());

            var service = podcasts.FirstOrDefault(s => s.Title == result);
            if (service.Title == null)
                return;

            Analytics.TrackEvent("PodcastSubscribe", new Dictionary<string, string>
            {
                ["service"] = service.Key
            });

            await OpenBrowser(service.Key);
        }

        async Task OpenBrowser(string type)
        {
            Analytics.TrackEvent("OpenBrowser", new Dictionary<string, string>
            {
                ["type"] = type
            });

            var url = type switch
            {
                "featurerequest" => "https://github.com/jamesmontemagno/islandtracker-feedback",
                "newsletter" => "https://mailchi.mp/06f065f73ce9/acislandtracker",
                "terms" => "https://refractored.ghost.io/terms/",
                "privacy" => "https://refractored.ghost.io/about/",
                "survey" => App.IsStore ? "https://forms.microsoft.com/Pages/ResponsePage.aspx?id=DQSIkWdsW0yxEjajBLZtrQAAAAAAAAAAAAMAAINl_EhUNTRGN0RKVkc4U01DOFRYODk0QkxWNVIxUi4u" : "https://forms.office.com/Pages/ResponsePage.aspx?id=DQSIkWdsW0yxEjajBLZtrQAAAAAAAAAAAAMAAINl_EhURU9ZTVRZWVE0WExFMEJXTDhTSlkxQVZRSi4u",
                "coffee" => "https://www.buymeacoffee.com/jamesmontemagno",
                "resizetizernt" => "https://raw.githubusercontent.com/jamesmontemagno/app-ac-islandtracker/master/Licenses/resizetizernt.txt",
                "fontawesome" => "https://fontawesome.com/",
                "omaimakhan" => "https://www.fiverr.com/omaimakhan",
                "ac-nh-turnip-prices" => "https://raw.githubusercontent.com/jamesmontemagno/app-ac-islandtracker/master/Licenses/ac-nh-turnip-prices.txt",
                "prophet" => "https://turnipprophet.io/",
                "ninji" => "https://twitter.com/_Ninji/status/1244818665851289602?s=20",
                "sparkie108" => "https://github.com/sparkie108",
                "ardonpixels" => "https://linktr.ee/ardonpixels",
                "mvvm-helpers" => "https://raw.githubusercontent.com/jamesmontemagno/app-ac-islandtracker/master/Licenses/mvvm-helpers.txt",
                "monkey-cache" => "https://raw.githubusercontent.com/jamesmontemagno/app-ac-islandtracker/master/Licenses/monkey-cache.txt",
                "pancake" => "https://raw.githubusercontent.com/jamesmontemagno/app-ac-islandtracker/master/Licenses/pancakeview.txt",
                "sharpnado" => "https://raw.githubusercontent.com/jamesmontemagno/app-ac-islandtracker/master/Licenses/sharpnado.txt",
                "syncfusion" => "https://www.syncfusion.com/xamarin-ui-controls",
                "essentials" => "https://raw.githubusercontent.com/jamesmontemagno/app-ac-islandtracker/master/Licenses/xamarin-essentials.txt",
                "forms" => "https://raw.githubusercontent.com/jamesmontemagno/app-ac-islandtracker/master/Licenses/xamarin-forms.txt",
                "github" => "https://www.github.com/jamesmontemagno",
                "twitter" => "https://www.twitter.com/jamesmontemagno",
                "dispatch_apple" => "https://itunes.apple.com/us/podcast/nintendo-dispatch/id1378538583?mt=2&ls=1",
                "dispatch_google" => "https://www.google.com/podcasts?feed=aHR0cHM6Ly93d3cubmludGVuZG9kaXNwYXRjaC5jb20vcnNz",
                "dispatch_castbox" => "https://castbox.fm/channel/Nintendo-Dispatch-id2120075",
                "dispatch_castro" => "https://castro.fm/podcast/4cc82fa9-fda9-4aa4-a94b-3ff554c28969",
                "dispatch_overcast" => "https://overcast.fm/itunes1378538583/nintendo-dispatch",
                "dispatch_pocketcasts" => "https://pca.st/M9aC",
                "dispatch_spotify" => "https://open.spotify.com/show/6cdlRoFm72s76AUWh1dOQH?si=Hjhq-uCdR-mHK2bxo0sgxA",
                "dispatch_stitcher" => "https://www.stitcher.com/s?fid=184564&refid=stpr",
                "dispatch_tunein" => "https://tunein.com/podcasts/Video-Games/Nintendo-Dispatch-p1123545/",
                "dispatch_rss" => "https://nintendodispatch.com/rss",
                _ => "https://www.montemagno.com"
            };

            await Browser.OpenAsync(url);
        }





        async Task SendEmail()
        {
            try
            {
                var key = await SettingsService.GetPublicKey();
                var builder = new StringBuilder();
                builder.AppendLine($"App Version: {Xamarin.Essentials.AppInfo.VersionString} | Build: {Xamarin.Essentials.AppInfo.BuildString}");
                builder.AppendLine($"OS: {DeviceInfo.Platform}");
                builder.AppendLine($"OS Version: {DeviceInfo.VersionString}");
                builder.AppendLine($"Manufacturer: {DeviceInfo.Manufacturer}");
                builder.AppendLine($"Device Model: {DeviceInfo.Model}");
                builder.AppendLine($"Public Key: {key}");
                builder.AppendLine(string.Empty);
                builder.AppendLine("Please let me know what issues you are having here:");

                Analytics.TrackEvent("SendEmail");
                var message = new EmailMessage
                {
                    Subject = $"Island Tracker Issue. {AppInfo}",
                    Body = builder.ToString(),
                    To = new List<string> { "acislandtracker@gmail.com" }
                };

                if (AttachDetails)
                {
                    var fn = "Attachment.json";
                    var days = DataService.GetCurrentWeek();
                    var json = JsonConvert.SerializeObject(days);
                    var file = Path.Combine(FileSystem.CacheDirectory, fn);
                    File.WriteAllText(file, json);

                    message.Attachments.Add(new EmailAttachment(file));
                }

                await Email.ComposeAsync(message);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Unable to send email", "Email acislandtracker@gmail.com directly.");
            }
        }

    }
}
