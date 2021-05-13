using AppOfficina.Portable.DTO;
using AppOfficina.Portable.ViewModels;
using CommonServiceLocator;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
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
    public partial class GestionePopupNoteView : PopupPage
    {
        GestioneNoteViewModel _vmNotesViewModel;
        InconvenienteDTO _inconveniente;
        NoteDTO _note;
        public GestionePopupNoteView(NoteDTO note,InconvenienteDTO inconveniente = null)
        {
            InitializeComponent();

            this._inconveniente = inconveniente;
            this._note = note;

            _vmNotesViewModel = (GestioneNoteViewModel)(ServiceLocator.Current.GetInstance(typeof(GestioneNoteViewModel)));

            _vmNotesViewModel.inconvenienteDTO = inconveniente;
            _vmNotesViewModel.noteDTO = note;

            BindingContext = _vmNotesViewModel;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            //MessagingCenter.Send<App>((App)Application.Current, "OnNoteCreated");

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if(_note != null)
            {
                if (!string.IsNullOrEmpty(_note.Testo))
                {
                    txtNote.Text = _note.Testo;
                    txtNote.IsEnabled = false;
                }
                else
                {
                    txtNote.IsEnabled = true;
                }
               
            }
          
        }

        private void txtNote_Focused(object sender, FocusEventArgs e)
        {
            _vmNotesViewModel.IsOKEnabled = true;
        }

        private async void btnEsci_Clicked(object sender, EventArgs e)
        {
            await base.Navigation.PopPopupAsync();
        }


    }
}