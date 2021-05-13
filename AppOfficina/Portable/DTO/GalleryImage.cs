using System.ComponentModel;
using Telerik.Windows.Documents.Flow.Model;
using Xamarin.Forms;

namespace AppOfficina.Portable
{
    public class GalleryImage : INotifyPropertyChanged
    {
        public ImageSource Source { get; set; }
        public byte[] OrgImage { get; set; }



        private bool _isCheckedImage;
        public bool isCheckedImage
        {
            get
            {
                return _isCheckedImage;
            }
            set
            {
                _isCheckedImage = value;
                OnPropertyChanged("isCheckedImage");

            }
        }



        private string _numberPhoto;
        public string numberPhoto
        {
            get
            {
                return _numberPhoto;
            }
            set
            {
                _numberPhoto = value;
                OnPropertyChanged("numberPhoto");

            }
        }

        public string pathImage { get; set; }

        public string nameImage { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}