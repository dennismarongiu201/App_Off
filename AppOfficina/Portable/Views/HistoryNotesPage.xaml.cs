using Android.Util;
using AppOfficina.Constants;
using AppOfficina.Portable.DTO;
using AppOfficina.Portable.ViewModels;
using CommonServiceLocator;
using Rg.Plugins.Popup.Services;
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
    public partial class HistoryNotesPage : ContentPage
    {
        private InconvenienteDTO _inconveniente;
        HistoryNotesPageViewModel _vmNotesPageViewModel;
        public HistoryNotesPage(InconvenienteDTO inconvenienteDTO, CommessaDTO commessaDTO)
        {
            InitializeComponent();

            _vmNotesPageViewModel = (HistoryNotesPageViewModel)(ServiceLocator.Current.GetInstance(typeof(HistoryNotesPageViewModel)));
            
            _vmNotesPageViewModel.IDCommessa = commessaDTO.NumeroCommessa;
            _vmNotesPageViewModel.targaVeicolo = commessaDTO.TargaCommessa;
            _vmNotesPageViewModel.codiceCliente = commessaDTO.codice;
            _vmNotesPageViewModel.ragioneSociale = commessaDTO.ragioneSociale;
            _vmNotesPageViewModel.telefono = commessaDTO.telefono;
            _vmNotesPageViewModel.mail = commessaDTO.email;
            _vmNotesPageViewModel.numeroTelaio = commessaDTO.Telaio;
            _vmNotesPageViewModel.nomeVeicolo = commessaDTO.DescrizioneVeicolo;
            _vmNotesPageViewModel.inconvenienteDTO = inconvenienteDTO;


            this._inconveniente = inconvenienteDTO;
            BindingContext = _vmNotesPageViewModel;
            ((HistoryNotesPageViewModel)BindingContext).Initialize(this);

            try
            {
                //questo "segnale" é l'unico modo per poter refreshare la lista delle note dopo un inserimento.
                MessagingCenter.Subscribe<App>((App)Application.Current, "OnNoteCreated", async (sender) => {
                    if(_vmNotesPageViewModel.lstNote != null)
                    {
                        if (_vmNotesPageViewModel.lstNote.Any())
                        {
                            _vmNotesPageViewModel.lstNote.Clear();
                            await _vmNotesPageViewModel.GetNote();
                        }
                    }
                });
            }
            catch (Exception ecc)
            {
                Log.Error("AppOfficina", ecc.Message);
            }
            
        }

        public void UnsubscribeEvents()
        {
            MessagingCenter.Unsubscribe<string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError);
            MessagingCenter.Unsubscribe<DisplayAlertMessage>(MessagingCenterEvents.Subscriber, "Attenzione!");
        }

     

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            UnsubscribeEvents();
        }

        private async void lstNote_ItemTapped(object sender, Telerik.XamarinForms.DataControls.ListView.ItemTapEventArgs e)
        {
            NoteDTO itemNote = e.Item as NoteDTO;
            if(itemNote != null)
            {
                await PopupNavigation.Instance.PushAsync(new GestionePopupNoteView(itemNote,null),false);
            }
            _vmNotesPageViewModel.IsAddNoteEnable = true;
        }

        private void BusyIndicator_ChildAdded(object sender, ElementEventArgs e)
        {

        }
    }
}