using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmHelpers;
using MvvmHelpers.Commands;
using Newtonsoft.Json;
using TurnipTracker.Services;
using Xamarin.Essentials;

namespace TurnipTracker.ViewModel
{
    public class AboutViewModel : BaseViewModel
    {
        public AsyncCommand SendEmailCommand { get; }
        public AsyncCommand SubscribeCommand { get; }
        public AsyncCommand<string> OpenBrowserCommand { get; }


        List<(string Title, string Key, string Platforms)> podcasts;
        public AboutViewModel()
        {
            SubscribeCommand = new AsyncCommand(Subscribe);
            SendEmailCommand = new AsyncCommand(SendEmail);
            OpenBrowserCommand = new AsyncCommand<string>(OpenBrowser);
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

            await OpenBrowser(service.Key);
        }

        async Task OpenBrowser(string type)
        {
            var url = type switch
            {
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
            var message = new EmailMessage
            {
                Subject = "Island Tracker Issue",
                Body = "Describe issue here",
               To = new List<string> { "refractoredllc@gmail.com"}
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
    }
}
