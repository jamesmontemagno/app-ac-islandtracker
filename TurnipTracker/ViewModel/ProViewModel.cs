using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using MvvmHelpers.Commands;
using Plugin.InAppBilling;
using TurnipTracker.Services;

namespace TurnipTracker.ViewModel
{
    public class ProViewModel : ViewModelBase
    {
        public ProViewModel()
        {

        }

        string busyTitle = "Verifying Pro...";
        public string BusyTitle
        {
            get => busyTitle;
            set => SetProperty(ref busyTitle, value);
        }

        public string ProPrice => SettingsService.ProPrice;

        public AsyncCommand GetPriceCommand =>
            new AsyncCommand(GetPrice);

        async Task GetPrice()
        {
            if (IsBusy)
                return;

            if (IsPro)
                return;


            if(string.IsNullOrWhiteSpace(SettingsService.ProPrice) || SettingsService.ProPriceDate.AddDays(2) < DateTime.UtcNow)
            {

            }
            else
            {
                return;
            }



            BusyTitle = "Syncing prices...";

            IsBusy = true;

            try
            {

#if DEBUG
                SettingsService.ProPrice = "$2.99";
                OnPropertyChanged(nameof(ProPrice));
                return;
#endif



                //Check Offline

                var connected = await CrossInAppBilling.Current.ConnectAsync();

                if (!connected)
                {
                    return;
                }

                var items = await CrossInAppBilling.Current.GetProductInfoAsync(ItemType.InAppPurchase, productId);

                var item = items.FirstOrDefault(i => i.ProductId == productId);
                if(item != null)
                {
                    SettingsService.ProPrice = item.LocalizedPrice;
                    OnPropertyChanged(nameof(ProPrice));

                    SettingsService.ProPriceDate = DateTime.UtcNow;
                }
            }
            catch(Exception ex)
            {
                //it is alright that we couldn't get the price
            }
            finally
            {
                await CrossInAppBilling.Current.DisconnectAsync();
                IsBusy = false;
            }
        }

        public bool NeedsProSync => IsPro && SettingsService.NeedsProSync;

        public AsyncCommand PurchaseProCommand =>
            new AsyncCommand(PurchasePro);
        async Task PurchasePro()
        {
            if (IsBusy)
                return;

            if (!await CheckConnectivity("Offline", "You seem to be offline, check your internet connectivity and try again."))
                return;

            BusyTitle = "";
            Analytics.TrackEvent("ProPurchase-Start");
            IsBusy = true;


            try
            {

                //Check Offline

                var connected = await CrossInAppBilling.Current.ConnectAsync();

                if (!connected)
                {
                    await DisplayAlert("Unable to Connect", "Unable to connect to the app store, check your internet connectivity and try again.");
                    return;
                }

                Analytics.TrackEvent("ProPurchase-Connected");

                //check purchases
                var purchase = await CrossInAppBilling.Current.PurchaseAsync(productId, ItemType.InAppPurchase);

                if (purchase == null)
                {
                    Analytics.TrackEvent("ProPurchase-PurchaseComplete", new Dictionary<string, string>
                    {
                        ["purchase"] = "failed: null"
                    });
                    return;
                }
                else if (purchase.State == PurchaseState.Purchased)
                {
                    Analytics.TrackEvent("ProPurchase-PurchaseComplete", new Dictionary<string, string>
                    {
                        ["purchase"] = "success"
                    });

                    SettingsService.ProReceipt = purchase.PurchaseToken ?? string.Empty;

                    SettingsService.IsPro = true;

                    SettingsService.NeedsProSync = true;

                    try
                    {
                        await CrossInAppBilling.Current.AcknowledgePurchaseAsync(purchase.PurchaseToken);
                    }
                    catch(Exception ex)
                    {

                    }

                    try
                    {
                        await DataService.CreateProStatus(new Shared.ProStatus
                        {
                            IsPro = true,
                            Receipt = purchase.PurchaseToken ?? string.Empty
                        });
                        SettingsService.NeedsProSync = false;
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Pro status sync", "Purchase successful, but it looks like something went wrong syncing with server, please try again.");

                        Crashes.TrackError(ex);
                    }
                    finally
                    {
                        OnPropertyChanged(nameof(NeedsProSync));
                        OnPropertyChanged(nameof(IsPro));
                        OnPropertyChanged(nameof(IsNotPro));
                    }
                    return;
                }

                throw new InAppBillingPurchaseException(PurchaseError.GeneralError);

            }
            catch (InAppBillingPurchaseException purchaseEx)
            {
                Analytics.TrackEvent("ProPurchase-PurchaseComplete", new Dictionary<string, string>
                {
                    ["purchase"] = $"failed: {purchaseEx.PurchaseError}"
                });
                var message = string.Empty;
                switch (purchaseEx.PurchaseError)
                {
                    case PurchaseError.AppStoreUnavailable:
                        message = "Currently the app store seems to be unavailble. Try again later.";
                        break;
                    case PurchaseError.BillingUnavailable:
                        message = "Billing seems to be unavailable, please try again later.";
                        break;
                    case PurchaseError.PaymentInvalid:
                        message = "Payment seems to be invalid, please try again.";
                        break;
                    case PurchaseError.PaymentNotAllowed:
                        message = "Payment does not seem to be enabled/allowed, please try again.";
                        break;
                    default:
                        message = "Something has gone wrong, please try again.";
                        break;
                }

                if (string.IsNullOrWhiteSpace(message))
                    return;

                Console.WriteLine("Issue connecting: " + purchaseEx);
                await DisplayAlert("Uh Oh!", message);
            }
            catch (Exception ex)
            {
                Analytics.TrackEvent("ProPurchase-PurchaseComplete", new Dictionary<string, string>
                {
                    ["purchase"] = "failed: misc"
                });
                Console.WriteLine("Issue connecting: " + ex);
                await DisplayAlert("Uh Oh!", $"Looks like something has gone wrong, please try again. Code: {ex.Message}");
            }
            finally
            {
                await CrossInAppBilling.Current.DisconnectAsync();
                IsBusy = false;

                Analytics.TrackEvent("ProPurchase-Finish", new Dictionary<string, string>
                {
                    ["purchase"] = "failed: null"
                });
            }
        }

        public AsyncCommand RestorePurchasesCommand =>
            new AsyncCommand(RestorePurchases);

        const string productId = "islandtrackerpro";

        async Task RestorePurchases()
        {
            if (IsBusy)
                return;


            if (!await CheckConnectivity("Offline", "You seem to be offline, check your internet connectivity and try again."))
                return;

            BusyTitle = "Verifying purchase...";
            IsBusy = true;

            try
            {


                //Check Offline


                var connected = await CrossInAppBilling.Current.ConnectAsync();

                if (!connected)
                {
                    await DisplayAlert("Unable to Connect","Unable to connect to the app store, check your internet connectivity and try again.");
                    return;
                }

                //check purchases

                var purchases = await CrossInAppBilling.Current.GetPurchasesAsync(ItemType.InAppPurchase);

                if (purchases?.Any(p => p.ProductId == productId) ?? false)
                {
                    Analytics.TrackEvent("ProPurchase-Restore", new Dictionary<string, string>
                    {
                        ["purchase"] = "success"
                    });
                    //Purchase restored
                    SettingsService.IsPro = true;
                    SettingsService.NeedsProSync = true;
                    OnPropertyChanged(nameof(NeedsProSync));
                    OnPropertyChanged(nameof(IsPro));
                    OnPropertyChanged(nameof(IsNotPro));

                    var purchase = purchases.FirstOrDefault(p => p.ProductId == productId);

                    if(string.IsNullOrWhiteSpace(SettingsService.ProReceipt))
                    {

                        SettingsService.ProReceipt = purchase?.PurchaseToken ?? string.Empty;
                    }

                    var ack = purchase?.IsAcknowledged ?? true;
                    if(!ack)
                    {
                        try
                        {
                            await CrossInAppBilling.Current.AcknowledgePurchaseAsync(purchase.PurchaseToken);
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                }
                else
                {
                    Analytics.TrackEvent("ProPurchase-Restore", new Dictionary<string, string>
                    {
                        ["purchase"] = "none found"
                    });
                    await DisplayAlert("Hmmmm!", $"Looks like we couldn't find your previous purchase. Tap on the purchase button to attempt to purchase or restore Island Tracker Pro.");
                }

            }
            catch (Exception ex)
            {
                Analytics.TrackEvent("ProPurchase-Restore", new Dictionary<string, string>
                {
                    ["purchase"] = "failed: error"
                });
                Debug.WriteLine("Issue connecting: " + ex);
                await DisplayAlert("Uh Oh!", $"Looks like something has gone wrong, please try again or tap on the Purchase button to attempt to restore this specific purchase.  Code: {ex.Message}");
            }
            finally
            {
                await CrossInAppBilling.Current.DisconnectAsync();
                IsBusy = false;
            }
        }


        public AsyncCommand SyncProStatusCommand =>
            new AsyncCommand(SyncProStatus);

        async Task SyncProStatus()
        {

            if (!await CheckConnectivity("Offline", "You seem to be offline, check your internet connectivity and try again."))
                return;


            if (IsBusy)
                return;


            BusyTitle = "Syncing status...";
            IsBusy = true;

            try
            {
                await DataService.CreateProStatus(new Shared.ProStatus
                {
                    IsPro = true,
                    Receipt = SettingsService.ProReceipt ?? string.Empty
                });
                SettingsService.NeedsProSync = false;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Pro status sync", "It looks like something went wrong syncing with server, please try again.");

                Crashes.TrackError(ex);
            }
            finally
            {
                IsBusy = false;
                OnPropertyChanged(nameof(NeedsProSync));
                OnPropertyChanged(nameof(IsPro));
                OnPropertyChanged(nameof(IsNotPro));
            }
        }

        public AsyncCommand RetrieveProStatusCommand =>
            new AsyncCommand(RetrieveProStatus);


        async Task RetrieveProStatus()
        {
            if (IsBusy)
                return;

            BusyTitle = "Verifying purchase...";
            IsBusy = true;
            try
            {
                Analytics.TrackEvent("RetrieveProStatus");
                var status = await DataService.GetProStatus();
                if(status?.IsPro ?? false)
                {
                    SettingsService.IsPro = true;
                    SettingsService.NeedsProSync = false;
                    OnPropertyChanged(nameof(IsPro));
                    OnPropertyChanged(nameof(IsNotPro));
                    OnPropertyChanged(nameof(NeedsProSync));
                }
                else
                {
                    await DisplayAlert("Uh Oh!", $"No past pro status found. Please purchase Island Tracker Pro or contact support.");
                }
            }
            catch (HttpResponseException hrex) when (!string.IsNullOrWhiteSpace(hrex.Message))
            {
                await DisplayAlert("Uh oh, turbulence", hrex.Message);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Uh oh, turbulence", "Looks like something went wrong. Check internet and try again.");

                Crashes.TrackError(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
