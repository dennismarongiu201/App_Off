using AppOfficina.Constants;
using AppOfficina.Portable.ViewModels;
using CommonServiceLocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing;
using ZXing.Net.Mobile.Forms;

namespace AppOfficina.Portable.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BarCodePage : ZXingScannerPage
    {
        public BarCodePage()
        {
            var vm = (BarcodeViewModel)(ServiceLocator.Current.GetInstance(typeof(BarcodeViewModel)));
            vm.Navigation = this.Navigation;
            BindingContext = vm;

            //MessagingCenter.Subscribe<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, async (sender, msg) =>
            //{
            //    await DisplayAlert("Informazione", msg, "OK");
            //});
        }

     
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            UnsubscribeEvents();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private void UnsubscribeEvents()
        {
            MessagingCenter.Unsubscribe<string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError);
            MessagingCenter.Unsubscribe<string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertInfo);
        }

     

     
    }
}