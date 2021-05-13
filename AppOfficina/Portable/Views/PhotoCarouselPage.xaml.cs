using AppOfficina.Portable.ViewModels;
using CommonServiceLocator;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace AppOfficina.Portable.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PhotoCarouselPage : PopupPage
    {

        private IEnumerable<object> lstPhoto;
        private IList newItems;
        private int _position;
        PhotoCarouselViewModel _vm;
        public PhotoCarouselPage(int position, IEnumerable<object> selectedItem, IList newItems)
        {
            InitializeComponent();
            _vm = (PhotoCarouselViewModel)(ServiceLocator.Current.GetInstance(typeof(PhotoCarouselViewModel)));
            this.lstPhoto = selectedItem;
            this.newItems = newItems;
            this._position = position;
           
            if(lstPhoto != null){
                _vm.lstImages = (System.Collections.ObjectModel.ObservableCollection<GalleryImage>)lstPhoto;
            }
            
            _vm.positionItem = position;
            if(newItems != null){
                carouselPhoto.ItemsSource = newItems;
            }
            BindingContext = _vm;

        }

     

        private async void OnCloseButtonTapped(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAllAsync();
            carouselPhoto.ItemsSource = null;
        }
    }
}