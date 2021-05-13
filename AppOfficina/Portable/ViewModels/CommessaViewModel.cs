using Android.Util;
using AppOfficina.Constants;
using AppOfficina.Portable.DTO;
using AppOfficina.Portable.Services;
using AppOfficina.Portable.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Input;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace AppOfficina.Portable.ViewModels
{
    public class CommessaViewModel : BaseViewModel
    {
        private readonly ApiServices _apiService ;
        List<CommessaDTO> lstCommessa = new List<CommessaDTO>();
        private string _commessa = string.Empty;



        public string commessa
        {
            get { return _commessa; }
            set { _commessa = value; OnPropertyChanged("commessa"); }
        }

        private string _targa = string.Empty;
        public string targa
        {
            get { return _targa; }
            set { _targa = value; OnPropertyChanged("targa"); }
        }


        private string _filterCommesse = "Tutti";
        public string filterCommesse
        {
            get { return _filterCommesse; }
            set { _filterCommesse = value; OnPropertyChanged("filterCommesse"); }
        }




        private ObservableCollection<CommessaDTO> _lstCommessa;
        public ObservableCollection<CommessaDTO> listItemSource
        {
            get
            {
                return _lstCommessa;
            }
            set
            {
                _lstCommessa = value;
                OnPropertyChanged("listItemSource");
            }
        }
        public string LoggedUserName
        {
            get
            {
                return App.LoggedUser;
            }
        }

        private bool _isBusy = false;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; OnPropertyChanged("IsBusy"); }
        }

        public ICommand LogOffCommand { get; private set; }
        public ICommand searchCommessa { get; private set; }
        public ICommand searchingCommessa{ get; private set; }
        public ICommand searchTarga { get; private set; }
        public ICommand SettingsApp { get; private set; }


        private CommessaDTO _selectedCommessa;
        public CommessaDTO selectedCommessa
        {
            get
            {
                return _selectedCommessa;
            }
            set
            {
                _selectedCommessa = value;
                OnPropertyChanged("selectedCommessa");
            }
        }



        private Stopwatch stopwatch;

        public CommessaViewModel(ApiServices _apiServices)
        {
            _apiService = _apiServices;
            stopwatch = new Stopwatch();
            LogOffCommand = new Command(async () => await logOffUser());
            SettingsApp = new Command(async () => await OpenActivitySettings());
            searchCommessa = new Command(async () => await searchForCommessaOrTarga());
            searchingCommessa = new Command(async () => await searchForCommessaOrTarga());
            searchTarga = new Command(async () => await searchForCommessaOrTarga());
        }

        async Task OpenActivitySettings()
        {
            await Navigation.PushAsync(new SettingsPage());
        }

        private void SetColorStatesInconvenienti(List<CommessaDTO> lstCommessa)
        {
            foreach (var commesse in lstCommessa)
            {
                string state = commesse.stato.Trim();
                if (!string.IsNullOrEmpty(state))
                {
                    switch (state)
                    {
                        case "In lavorazione":
                            commesse.ColorStates = Color.YellowGreen;
                            break;
                        case "In attesa di lavorazione":
                            commesse.ColorStates = Color.White; 
                            break;
                        case "Terminata":
                            commesse.ColorStates = Color.Green;
                            break;
                    }
                }

            }
        }

        public async Task InitializeCommessa()
        {
            try
            {
                IsBusy = true;
                if (HasInternetConnection)
                {
                    if (!string.IsNullOrEmpty(commessa) || !string.IsNullOrEmpty(targa))
                    {
                       await searchForCommessaOrTarga();
                    }
                }
                else
                {
                    MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.NoInternetConnection);
                }
                IsBusy = false;
            }
            catch (Exception)
            {
                IsBusy = false;
                MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.Commessa.UnknownApiCommessaCheck);
                return;
            }
        }

        protected override void CurrentPageOnAppearing(object sender, EventArgs eventArgs)
        {
            Task.Run(async () => await InitializeCommessa());
        }


        public async Task searchForCommessaOrTarga()
        {
            stopwatch.Reset();
            try
            {
                IsBusy = true;
                if (HasInternetConnection){
                    if (string.IsNullOrEmpty(commessa) && string.IsNullOrEmpty(targa))
                    {
                        MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.Commessa.InvalidFieldsCommessaTarga);
                        IsBusy = false;
                        return;
                    }
                    if (!string.IsNullOrEmpty(commessa) || !string.IsNullOrEmpty(targa)){
                        stopwatch.Start();
                        lstCommessa = await _apiService.CheckCommessaAsync(App.UserID,commessa, targa, filterCommesse);
                        stopwatch.Stop();

                        if (lstCommessa != null){
                            if (lstCommessa.Count > 0){
                                SetColorStatesInconvenienti(lstCommessa);
                                listItemSource = new ObservableCollection<CommessaDTO>(lstCommessa);
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
                Log.Error("AppOfficina",ecc.Message);
                IsBusy = false;
                MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.Commessa.UnknownApiCommessaCheck);
                return;
            }
        }


        public void LogOutApplication()
        {
            Task.Run(async () => await logOffUser());
        }

        public async Task logOffUser()
        {
            IsBusy = true;
            try{
                if (HasInternetConnection)
                {
                    await _apiService.LogoutUser();
                }
                else
                {
                    MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.NoInternetConnection);
                }

            }
            catch (Exception ex){
                Console.WriteLine(ex.Message);
            }
            IsBusy = false;
        }
      
    }
}
