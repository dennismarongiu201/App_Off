using Android.Util;
using AppOfficina.Constants;
using AppOfficina.Portable.DTO;
using AppOfficina.Portable.Interface;
using AppOfficina.Portable.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AppOfficina.Portable.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {

        private TokenInfoDTO _userToken;
        public string UserName { get; set; }
        public string Password { get; set; }
        private ApiServices _apiService ;

        private Stopwatch stopwatch;
        public ICommand LoginCommand { get; private set; }

        //private bool _appLogoAvailable;
        //public bool AppLogoAvailable
        //{
        //    get
        //    {
        //        return true /*File.Exists(AppLogoPath.);*/;
        //    }
        //}
        public ImageSource AppLogoPath
        {
            get
            {
                return Device.RuntimePlatform == Device.Android
                ? ImageSource.FromFile("logo.jpg")
                : ImageSource.FromFile("Images/logo.jpg");
            }
        }

       
        private bool _isBusy = false;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; OnPropertyChanged("IsBusy"); }
        }


        public LoginViewModel(ApiServices service)
        {
            _apiService = service;
            stopwatch = new Stopwatch();
            LoginCommand = new Command(async () => await Login());
        }

        public async Task Login()
        {
            stopwatch.Reset();
            try
            {
                IsBusy = true;
                if (HasInternetConnection){
                    if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
                    {
                        MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.Login.InvalidCredentials);
                        IsBusy = false;
                        return;
                    }
                    if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                    {
                        stopwatch.Start();
                        _userToken = await _apiService.CheckApiCredentialsAsync(UserName, Password);
                        
                        stopwatch.Stop();
                        if (_userToken != null){
                            // qua andró a stampare un popup in caso di lavorazione rimaste pendenti.
                            await FinishLoginProcess();
                        }
                        else
                        {
                            IsBusy = false;
                            return;
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
            }
        }



        public async Task FinishLoginProcess()
        {
            App.UserID = _userToken.userId;
            App.LoggedUser = _userToken.username;
            App.IsLogged = true;
            App.Token = _userToken;
            await App.Current.SavePropertiesAsync();
            App.Current.MainPage = new NavigationPage(new Commessa());
        }
    }
}
