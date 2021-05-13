using AppOfficina.Constants;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace AppOfficina.Portable.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public INavigation Navigation { get; set; }
        protected Page CurrentPage { get; private set; }

        private IConnectivity connectivity = CrossConnectivity.Current;
        public bool HasInternetConnection
        {
            get
            {
                return CrossConnectivity.Current.IsConnected;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public BaseViewModel()
        {
            
        }

      
        public void Initialize(Page page)
        {
            CurrentPage = page;
            CurrentPage.Appearing += CurrentPageOnAppearing;
            CurrentPage.Disappearing += CurrentPageOnDisappearing;
        }

        protected virtual void CurrentPageOnAppearing(object sender, EventArgs eventArgs) { }

        protected virtual void CurrentPageOnDisappearing(object sender, EventArgs eventArgs) { }

    }
}
