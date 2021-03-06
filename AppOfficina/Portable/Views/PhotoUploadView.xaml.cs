using Android.Content;
using Android.Provider;
using AppOfficina.Constants;
using AppOfficina.Portable.DTO;
using AppOfficina.Portable.Interface;
using AppOfficina.Portable.ViewModels;
using CommonServiceLocator;
using Plugin.FileSystem.Abstractions;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppOfficina.Portable.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PhotoUploadView : ContentPage
    {
        PhotoUploadViewModel _vm;
        public PhotoUploadView(CommessaDTO commessa, InconvenienteDTO convDTO)
        {
            InitializeComponent();
            _vm = (PhotoUploadViewModel)(ServiceLocator.Current.GetInstance(typeof(PhotoUploadViewModel)));
            _vm.SelectedCommessa = commessa;
            _vm.SelectedInconveniente = convDTO;
            _vm.idCommessa = commessa.NumeroCommessa;
            _vm.targa = commessa.TargaCommessa;
            _vm.numeroTelaio = commessa.Telaio;
            _vm.nomeVeicolo = commessa.DescrizioneVeicolo;

            _vm.codiceCliente = commessa.codice;
            _vm.ragioneSociale = commessa.ragioneSociale;
            _vm.telefono = commessa.telefono;
            _vm.mail = commessa.email;

            _vm.Navigation = this.Navigation;
            BindingContext = _vm;
            //MessagingCenter.Subscribe<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, async (sender, msg) =>
            //{
            //    await DisplayAlert("Warning", msg, "OK");
            //});
            ((PhotoUploadViewModel)BindingContext).Initialize(this);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            UnsubscribeEvents();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            NavigationPage.SetHasBackButton(this, false);
            await _vm.previewPhoto();
            NavigationPage.SetHasBackButton(this, true);
        }

        protected override bool OnBackButtonPressed()
        {
            if (_vm.IsBusy)
            {
                return true;
            }
            return false;
        }

        private void UnsubscribeEvents()
        {
            MessagingCenter.Unsubscribe<string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError);
        }

        private async void listView_SelectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                var _PopupPhoto = new PhotoCarouselPage(0, null, e.NewItems);
                await PopupNavigation.Instance.PushAsync(_PopupPhoto, false);
            }
        }

    }
}