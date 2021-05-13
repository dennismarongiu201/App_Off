using AppOfficina.Portable.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AppOfficina.Portable.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly SettingsApp _SettingsApp;
        private readonly ApiServices apiServices;
        public string LoggedUserName
        {
            get
            {
                return App.LoggedUser;
            }
        }


        private string _serverApi;
        public string ServerApi
        {
            get
            {
                return _serverApi;
            }
            set{
                _serverApi = value;
                OnPropertyChanged("ServerApi");
            }
        }


        private string _ApiName;
        public string ApiName
        {
            get{
                return _ApiName;
            }
            set{
                _ApiName = value;
                OnPropertyChanged("ApiName");
            }
        }


        public ICommand LogOffCommand { get; private set; }

        public ICommand SaveSettings { get; private set; }

        public SettingsViewModel(SettingsApp _settingsApp,ApiServices _apiServices)
        {
            _SettingsApp = _settingsApp;
            apiServices = _apiServices;
            LogOffCommand = new Command(async () => await logOffUser());
            SaveSettings = new Command(async () => await SaveSettingsInApp());


            ServerApi = _SettingsApp.GetValueOrDefault("url_api",_SettingsApp.SetUrlApi);
            ApiName = _SettingsApp.GetValueOrDefault("name_api", _SettingsApp.SetApiName);
        }


        async Task SaveSettingsInApp(){
            string newServerApi = ServerApi.Trim();
            string newapiName = ApiName.Trim();
            if (!string.IsNullOrEmpty(newServerApi)){
                 _SettingsApp.SetUrlApi = newServerApi;
            }
            if (!string.IsNullOrEmpty(newapiName)){
                _SettingsApp.SetApiName = newapiName;
            }
            await apiServices.LogoutUser();
            App.Current.MainPage = new NavigationPage(new MainPage());
        }


        async Task logOffUser()
        {
            await apiServices.LogoutUser();
        }
    }
}
