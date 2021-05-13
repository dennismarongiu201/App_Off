using Android.Util;
using AppOfficina.Constants;
using AppOfficina.Portable.DTO;
using AppOfficina.Portable.Services;
using AppOfficina.Portable.Views;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AppOfficina.Portable.ViewModels
{
    public class HistoryNotesPageViewModel : BaseViewModel
    {
        private readonly ApiServices _apiService;
        List<NoteDTO> lstNotesForInconveniente;

        public ICommand LogOffCommand { get; private set; }
        public ICommand AddNoteCommand{ get; private set; }

    public string LoggedUserName
        {
            get
            {
                return App.LoggedUser;
            }
        }

        private ObservableCollection<NoteDTO> _lstNote;
        public ObservableCollection<NoteDTO> lstNote
        {
            get
            {
                return _lstNote;
            }
            set
            {
                _lstNote = value;
                OnPropertyChanged("lstNote");
            }
        }



        private InconvenienteDTO _inconvenienteDTO;

        public  InconvenienteDTO inconvenienteDTO
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


        private bool _isBusy = false;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; OnPropertyChanged("IsBusy"); }
        }

        private bool _IsAddNoteEnable = true;
        public bool IsAddNoteEnable
        {
            get { return _IsAddNoteEnable; }
            set { _IsAddNoteEnable = value; 
                ((Command)AddNoteCommand).ChangeCanExecute(); 
                OnPropertyChanged("IsAddNoteEnable"); 
            }
        }

        private string _codiceCliente;
        public string codiceCliente
        {
            get
            {
                return _codiceCliente;
            }
            set
            {
                _codiceCliente = value;
                OnPropertyChanged("codiceCliente");
            }
        }


        private string _ragioneSociale;
        public string ragioneSociale
        {
            get
            {
                return _ragioneSociale;
            }
            set
            {
                _ragioneSociale = value;
                OnPropertyChanged("ragioneSociale");
            }
        }

        private string _telefono;
        public string telefono
        {
            get
            {
                return _telefono;
            }
            set
            {
                _telefono = value;
                OnPropertyChanged("telefono");
            }
        }



        private string _mail;
        public string mail
        {
            get
            {
                return _mail;
            }
            set
            {
                _mail = value;
                OnPropertyChanged("mail");
            }
        }

        private string _nomeVeicolo;
        public string nomeVeicolo
        {
            get
            {
                return _nomeVeicolo;
            }
            set
            {
                _nomeVeicolo = value;
                OnPropertyChanged("nomeVeicolo");
            }
        }



        private string _numeroTelaio;

        public string numeroTelaio
        {
            get
            {
                return _numeroTelaio;
            }
            set
            {
                _numeroTelaio = value;
                OnPropertyChanged("numeroTelaio");
            }
        }

        private string _defCommessa;
        public string defCommessa
        {
            get
            {
                return _defCommessa;
            }
            set
            {
                _defCommessa = value;
                OnPropertyChanged("defCommessa");
            }
        }

        private string _IDCommessa;
        public string IDCommessa
        {
            get
            {
                return _IDCommessa;
            }
            set
            {
                _IDCommessa = value;
                OnPropertyChanged("IDCommessa");
            }
        }


        private string _targaVeicolo;
        public string targaVeicolo
        {
            get
            {
                return _targaVeicolo;
            }
            set
            {
                _targaVeicolo = value;
                OnPropertyChanged("targaVeicolo");
            }
        }

        public HistoryNotesPageViewModel(ApiServices _apiServices)
        {
            _apiService = _apiServices;
            LogOffCommand = new Command(async () => await logOffUser());
            AddNoteCommand = new Command(async () =>
                                                    {
                                                        IsAddNoteEnable = false;
                                                        await OpenAddNote();
                                                        IsAddNoteEnable = true;
                                                    },
                                      () => {
                                          return IsAddNoteEnable;
                                      });

        }

        
        public async Task OpenAddNote()
        {   
            await PopupNavigation.Instance.PushAsync(new GestionePopupNoteView(new NoteDTO(),inconvenienteDTO));
        }

        public async Task GetNote()
        {
            try
            {
                IsBusy = true;
                if (HasInternetConnection)
                {
                    if (_inconvenienteDTO != null)
                    {
                        lstNotesForInconveniente = await _apiService.GetNoteService(_inconvenienteDTO.NumeroInconveniente);
                        if (lstNotesForInconveniente != null && lstNotesForInconveniente.Count >= 1)
                        {
                            lstNote = new ObservableCollection<NoteDTO>(lstNotesForInconveniente.OrderBy(x=>x.DataScrittura));
                        }

                    }
                }
                else
                {
                    IsBusy = false;
                    MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.NoInternetConnection);
                    return;
                }

            }
            catch (Exception exc)
            {
                Log.Error("AppOfficina", exc.Message);
                IsBusy = false;
            }
            IsBusy = false;
        }

        protected override void CurrentPageOnAppearing(object sender, EventArgs eventArgs)
        {
            Task.Run(async () => await GetNote());

        }
        public async Task logOffUser()
        {
            IsBusy = true;
            try
            {
                if (HasInternetConnection)
                {
                    await _apiService.LogoutUser();
                    IsBusy = false;
                }
                else
                {
                    IsBusy = false;
                    MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.NoInternetConnection);
                    return;
                }

            }
            catch (Exception)
            {
                IsBusy = false;
                MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.UnhandledExceptionError);
                return;
            }
        }


    }
}
