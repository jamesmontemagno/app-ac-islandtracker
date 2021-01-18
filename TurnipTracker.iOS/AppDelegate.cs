using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using StoreKit;
using Syncfusion.SfNumericTextBox.XForms.iOS;
using Syncfusion.SfNumericUpDown.XForms.iOS;
using Syncfusion.XForms.iOS.ComboBox;
using Syncfusion.XForms.iOS.MaskedEdit;
using Syncfusion.XForms.iOS.Shimmer;
using Syncfusion.XForms.iOS.TextInputLayout;
using TurnipTracker.Services;
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
            SfNumericUpDownRenderer.Init();
            new Syncfusion.SfBusyIndicator.XForms.iOS.SfBusyIndicatorRenderer();
            Syncfusion.XForms.iOS.EffectsView.SfEffectsViewRenderer.Init();
            Syncfusion.SfChart.XForms.iOS.Renderers.SfChartRenderer.Init();
            Syncfusion.XForms.iOS.Expander.SfExpanderRenderer.Init();
            Sharpnado.MaterialFrame.iOS.iOSMaterialFrameRenderer.Init();
            Syncfusion.XForms.iOS.Buttons.SfSegmentedControlRenderer.Init();
            LoadApplication(new App());

            //initialize current one.
            Plugin.InAppBilling.InAppBillingImplementation.OnShouldAddStorePayment = OnShouldAddStorePayment;
            var current = Plugin.InAppBilling.CrossInAppBilling.Current;

            return base.FinishedLaunching(app, options);
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            Xamarin.Forms.Application.Current.SendOnAppLinkRequestReceived(new Uri(url.AbsoluteString));
            /*var myApp = App.Current as TurnipTracker.App;
            if (null != myApp && null != url)
            {
                myApp.OnAppLinkRequestReceived(new Uri(url.AbsoluteString));
            }*/
            return true;

        }

        bool OnShouldAddStorePayment(SKPaymentQueue queue, SKPayment payment, SKProduct product)
        {
            SettingsService.IsPro = true;
            SettingsService.NeedsProSync = true;
            return true;
        }


    }
}
