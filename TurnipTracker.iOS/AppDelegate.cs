using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using Sharpnado.Presentation.Forms.iOS;
using Syncfusion.SfNumericTextBox.XForms.iOS;
using Syncfusion.XForms.iOS.ComboBox;
using Syncfusion.XForms.iOS.MaskedEdit;
using Syncfusion.XForms.iOS.Shimmer;
using Syncfusion.XForms.iOS.TextInputLayout;
using UIKit;

namespace TurnipTracker.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            global::Xamarin.Forms.FormsMaterial.Init();

            SfMaskedEditRenderer.Init();
            new SfNumericTextBoxRenderer();
            new SfComboBoxRenderer();
            SfTextInputLayoutRenderer.Init();

            SfShimmerRenderer.Init();
            Syncfusion.XForms.iOS.EffectsView.SfEffectsViewRenderer.Init();
            SharpnadoInitializer.Initialize(enableInternalLogger: true);
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            //if (Xamarin.Essentials.Platform.OpenUrl(app, url, options))
            //    return true;

            
            if(url != null && url.Host == "invite" &&
                url.Scheme == "acislandtracker")
            {
                var component = new NSUrlComponents(url.AbsoluteString);
                var id = component.QueryItems.FirstOrDefault(i => i.Name == "id")?.Value;
                var name = component.QueryItems.FirstOrDefault(i => i.Name == "name")?.Value;

                if (!string.IsNullOrWhiteSpace(id))
                    App.Current.MainPage.DisplayAlert($"Add Friend", $"Would you like to add {name} as a friend?", "Yes", "No").ContinueWith(t => { }); ;
                return true;
            }

            return base.OpenUrl(app, url, options);
        }


    }
}
