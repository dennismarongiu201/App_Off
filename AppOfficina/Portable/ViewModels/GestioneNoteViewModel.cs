using Android.Util;
using AppOfficina.Constants;
using AppOfficina.Portable.DTO;
using AppOfficina.Portable.Services;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AppOfficina.Portable.ViewModels
{
    public class GestioneNoteViewModel :BaseViewModel
    {
        private readonly ApiServices _apiService;


        public ICommand InsertNoteCommand { get; private set; }

        
        public GestioneNoteViewModel(ApiServices _apiServices)
        {
            _apiService = _apiServices;

            InsertNoteCommand = new Command(async () => await InsertNewNoteInconveniente());
        }

     

        private InconvenienteDTO _inconvenienteDTO;

        public InconvenienteDTO inconvenienteDTO
        {
            get
            {
                return _inconvenienteDTO;
            }
            set
            {
                _inconvenienteDTO = value;
            }
        }

        private NoteDTO _noteDTO;

        public NoteDTO noteDTO
        {
            get
            {
                return _noteDTO;
            }
            set
            {
                _noteDTO = value;
            }
        }

        private string _note;

        public string note
        {
            get
            {
                return _note;
            }
            set
            {
                _note = value;
                OnPropertyChanged("note");
            }
        }

        private bool _isBusy = false;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; OnPropertyChanged("IsBusy"); }
        }

        private bool _IsOKEnabled ;
        public bool IsOKEnabled
        {
            get
            {
                return _IsOKEnabled;
            }
            set
            {
                _IsOKEnabled = value;
                OnPropertyChanged("IsOKEnabled");
            }
        }

        public async Task InsertNewNoteInconveniente()
        {
            try
            {
                IsBusy = true;
                IsOKEnabled = false;
                if (HasInternetConnection)
                {
                    if (!string.IsNullOrEmpty(inconvenienteDTO.NumeroInconveniente))
                    {
                        if (!string.IsNullOrEmpty(note))
                        {
                            bool bInsert = await _apiService.InsertNota(inconvenienteDTO.NumeroInconveniente, note);
                            if (bInsert)
                            {
                                await base.Navigation.PopPopupAsync();
                                MessagingCenter.Send<App>((App)Application.Current, "OnNoteCreated");
                            }
                        }
                    }

                }
                else
                {
                    MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.NoInternetConnection);
                }
                IsBusy = false;
            }
            catch (Exception ecc)
            {
                IsBusy = false;
                Log.Error("AppOfficina", ecc.Message);
                return;
            }
            IsOKEnabled = true;
        }
    }
}
