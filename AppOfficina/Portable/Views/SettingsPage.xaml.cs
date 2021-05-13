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

namespace AppOfficina.Portable.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            var vm = (SettingsViewModel)(ServiceLocator.Current.GetInstance(typeof(SettingsViewModel)));
            vm.Navigation = this.Navigation;
            BindingContext = vm;


            MessagingCenter.Subscribe<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, async (sender, msg) =>
            {
                await DisplayAlert("Informazione", msg, "OK");
            });
        }

        
    }
}