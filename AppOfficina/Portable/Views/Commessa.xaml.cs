using AppOfficina.Android;
using AppOfficina.Constants;
using AppOfficina.Portable.DTO;
using AppOfficina.Portable.ViewModels;
using AppOfficina.Portable.Views;
using CommonServiceLocator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.XamarinForms.DataControls.ListView;
using Telerik.XamarinForms.Input.AutoComplete;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;
using ZXing.OneD;
using Android.Util;
using Log = Android.Util.Log;
using System.Reflection;

namespace AppOfficina.Portable
{

    public partial class Commessa : ContentPage
    {
      
        CommessaViewModel vm;
        public Commessa()
        {
            //registro il view model e lo inserisco nel binding context dell'activity view
            InitializeComponent();
            vm = (CommessaViewModel)(ServiceLocator.Current.GetInstance(typeof(CommessaViewModel)));
            vm.Navigation = this.Navigation;
            vm.filterCommesse = "Tutti";
            BindingContext = vm;
            ((CommessaViewModel)BindingContext).Initialize(this);
            try
            {

                MessagingCenter.Subscribe<string,string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, async (sender, msg) =>
                {
                    if (!string.IsNullOrEmpty(msg))
                    {
                        await DisplayAlert("Informazione", msg, "OK");
                    }

                });
            }
            catch (TargetInvocationException ecx)
            {
                Log.Error("AppOfficina", ecx.StackTrace);
            }
            catch (Exception ecc)
            {
                Log.Error("AppOfficina", ecc.Message);
            }

            
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            UnsubscribeEvents();
        }

        private void UnsubscribeEvents()
        {
            MessagingCenter.Unsubscribe<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError);
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                bool answer = await DisplayAlert("Logout", "Desideri uscire dall'applicazione?", "Si", "No");
                if (answer)
                {
                    App.IsLogged = false;
                    App.LoggedUser = null;
                    App.Token = null;
                    await App.Current.SavePropertiesAsync();
                    App.Current.MainPage = new MainPage();
                    //await vm.logOffUser();
                    base.OnBackButtonPressed();
                }
            });

            // Cancel the back button press as we are now handling it ourselves above (Return true as handled)
            return true;
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new ChangeTheme()));
        }

        private async void listView_ItemTapped_1(object sender, ItemTappedEventArgs e)
        {
            var item = (CommessaDTO)e.Item;
            if (item != null)
            {
                await Navigation.PushAsync(new ViewInconveniente(item));
            }
        }

        private async void ToolbarItem_Clicked_1(object sender, EventArgs e)
        {
            var opts = new ZXing.Mobile.MobileBarcodeScanningOptions
            {
                PossibleFormats = new List<ZXing.BarcodeFormat> { ZXing.BarcodeFormat.QR_CODE, ZXing.BarcodeFormat.ITF,
                ZXing.BarcodeFormat.All_1D, ZXing.BarcodeFormat.EAN_13, ZXing.BarcodeFormat.CODE_39, ZXing.BarcodeFormat.CODE_128,
                ZXing.BarcodeFormat.PDF_417, ZXing.BarcodeFormat.UPC_EAN_EXTENSION
                }
            };

            opts.TryHarder = false;
            opts.UseNativeScanning = true;
            var barcodeReader = opts.BuildBarcodeReader();
            barcodeReader.Options.AllowedLengths = new[] { 44 };
            barcodeReader.AutoRotate = false;
            

            ZXingScannerPage scanPage = new ZXingScannerPage(opts);
            scanPage.HasTorch = true;
            scanPage.IsScanning = false;
            
            scanPage.AutoFocus();
         
            scanPage.OnScanResult += (result) =>
            {

                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Navigation.PopAsync();
                    vm.commessa = result.Text.Trim();
                    await vm.searchForCommessaOrTarga();
                });
            };

            await Navigation.PushAsync(scanPage);
        }

        private void gridCommesse_LoadOnDemand(object sender, Telerik.XamarinForms.DataGrid.LoadOnDemandEventArgs e)
        {
            //gridCommesse.LoadOnDemandMode = Telerik.XamarinForms.Common.LoadOnDemandMode.Automatic;
            ////vm.listItemSource.Clear();
            ////await vm.searchForCommessaOrTarga();
            //e.IsDataLoaded = true;
            //gridCommesse.LoadOnDemandMode = Telerik.XamarinForms.Common.LoadOnDemandMode.Manual;
            //gridCommesse.IsBusy = false;
        }

        private async void gridCommesse_SelectionChanged(object sender, Telerik.XamarinForms.DataGrid.DataGridSelectionChangedEventArgs e)
        {
            var item = e.AddedItems.ToList().FirstOrDefault() as CommessaDTO;
            if (item != null)
            {
                await Navigation.PushAsync(new ViewInconveniente(item));
            }
        }
    }
}