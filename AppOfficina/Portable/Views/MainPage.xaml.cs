using AppOfficina.Constants;
using AppOfficina.Portable.ViewModels;
using CommonServiceLocator;
using Plugin.DeviceOrientation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;


namespace AppOfficina.Portable
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            var vm = (LoginViewModel)(ServiceLocator.Current.GetInstance(typeof(LoginViewModel)));
            vm.Navigation = this.Navigation;
            BindingContext = vm;
            

            MessagingCenter.Subscribe<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, async (sender, msg) =>
            {
                await DisplayAlert("Attenzione", msg, "OK");
            });

            MessagingCenter.Subscribe<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertInfo, async (sender, msg) =>
            {
                await DisplayAlert("Info", msg, "OK");
            });
        }

        private void txtEmail_Unfocused(object sender, FocusEventArgs e)
        {
            Password.Focus();
            string currentOrientation = CrossDeviceOrientation.Current.CurrentOrientation.ToString();
            if (!string.IsNullOrEmpty(currentOrientation))
            {
                if (currentOrientation.ToUpper().Equals("LANDSCAPE"))
                {
                    ScrollView.ScrollToAsync(0, MainStack.Height, true);
                }
            }
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            UnsubscribeEvents();
        }

        private void UnsubscribeEvents()
        {
            MessagingCenter.Unsubscribe<string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError);
            MessagingCenter.Unsubscribe<string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertInfo);
        }

        private void txtEmail_Focused(object sender, FocusEventArgs e)
        {
            string currentOrientation = CrossDeviceOrientation.Current.CurrentOrientation.ToString();
            if (!string.IsNullOrEmpty(currentOrientation))
            {
                if (currentOrientation.ToUpper().Equals("LANDSCAPE")){
                    ScrollView.ScrollToAsync(0, MainStack.Height - 250, true);
                }
            }
            
        }
    }
}
