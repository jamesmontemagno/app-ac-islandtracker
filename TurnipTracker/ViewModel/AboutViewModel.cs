using System;
using System.Collections.Generic;
using System.IO;
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
        public AsyncCommand<string> OpenBrowserCommand { get; }
        public AboutViewModel()
        {
            SendEmailCommand = new AsyncCommand(SendEmail);
            OpenBrowserCommand = new AsyncCommand<string>(OpenBrowser);
        }

        bool attachDetails = true;
        public bool AttachDetails
        {
            get => attachDetails;
            set => SetProperty(ref attachDetails, value);
        }

        async Task OpenBrowser(string type)
        {
            var url = type switch
            {
                "github" => "https://www.github.com/jamesmontemagno",
                "twitter" => "https://www.twitter.com/jamesmontemagno",
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
