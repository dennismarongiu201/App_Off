using AppOfficina.Constants;
using AppOfficina.Portable.DTO;
using AppOfficina.Portable.ViewModels;
using CommonServiceLocator;
using Plugin.DeviceOrientation;
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
    public partial class ExtraView : ContentPage
    {
        InconvenienteExtraViewModel vm;
        public ExtraView(InconvenienteDTO inconveniente,string commessa,string idCommessa,string targa,CommessaDTO commessaDTO)
        {
            InitializeComponent();
            vm = (InconvenienteExtraViewModel)(ServiceLocator.Current.GetInstance(typeof(InconvenienteExtraViewModel)));
            vm.Navigation = this.Navigation;
            vm.defCommessaInExtra = commessa;
            vm.idCommessa = idCommessa;
            vm.targa = targa;
            vm.numeroTelaio = commessaDTO.Telaio;
            vm.nomeVeicolo = commessaDTO.DescrizioneVeicolo;

            vm.codiceCliente = commessaDTO.codice;
            vm.ragioneSociale = commessaDTO.ragioneSociale;
            vm.telefono = commessaDTO.telefono;
            vm.mail = commessaDTO.email;
            
            vm.commessaDTO = commessaDTO;
            BindingContext = vm;
            ((InconvenienteExtraViewModel)BindingContext).Initialize(this);


        }
        private void UnsubscribeEvents()
        {
            MessagingCenter.Unsubscribe<string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError);
        }


        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            UnsubscribeEvents();
        }

     

        private void txtTitle_Focused(object sender, FocusEventArgs e)
        {
            //string currentOrientation = CrossDeviceOrientation.Current.CurrentOrientation.ToString();
            //if (!string.IsNullOrEmpty(currentOrientation))
            //{
            //    if (currentOrientation.ToUpper().Equals("LANDSCAPE"))
            //    {
            //        ScrollView.ScrollToAsync(0, MainStack.Height - 230, true);
            //    }
            //}
        }

        private void txtTitle_Unfocused(object sender, FocusEventArgs e)
        {
            txtDescriptionInconveniente.Focus();
            //string currentOrientation = CrossDeviceOrientation.Current.CurrentOrientation.ToString();
            //if (!string.IsNullOrEmpty(currentOrientation))
            //{
            //    if (currentOrientation.ToUpper().Equals("LANDSCAPE"))
            //    {
            //        ScrollView.ScrollToAsync(0, MainStack.Height, true);
            //    }
            //}


        }

        private void lstPhotoInExtra_SelectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

        }
    }
}