using AppOfficina.Portable.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AppOfficina.Portable.ViewModels
{
    public class PhotoCarouselViewModel : BaseViewModel
    {
        private readonly ApiServices _apiservice;

        private ObservableCollection<GalleryImage> _lstImages;
        public ObservableCollection<GalleryImage> lstImages
        {
            get
            {
                return _lstImages;
            }
            set
            {
                _lstImages = value;
                OnPropertyChanged("lstImages");
            }
        }

        private int _positionItem;
        public int positionItem
        {
            get
            {
                return _positionItem;
            }
            set
            {
                _positionItem = value;
                OnPropertyChanged("positionItem");
            }
        }


        public PhotoCarouselViewModel(ApiServices _apiservices)
        {
            _apiservice = _apiservices;
        }

      
    }
}
