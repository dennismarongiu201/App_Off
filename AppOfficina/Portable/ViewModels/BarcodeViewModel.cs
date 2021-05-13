using AppOfficina.Portable.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AppOfficina.Portable.ViewModels
{
    public class BarcodeViewModel : BaseViewModel
    {
        private readonly ApiServices _apiService;

        public ICommand LogOffCommand { get; private set; }

        private bool _isBusy = false;
        public bool IsBusy{
            get { return _isBusy; }
            set { _isBusy = value; OnPropertyChanged("IsBusy"); }
        }


        public BarcodeViewModel(ApiServices _apiServices){
            _apiService = _apiServices;
            LogOffCommand = new Command(async () => await logOffUser());
        }

        async Task logOffUser(){
            IsBusy = true;
            await _apiService.LogoutUser();
            IsBusy = false;
        }
    }
}
