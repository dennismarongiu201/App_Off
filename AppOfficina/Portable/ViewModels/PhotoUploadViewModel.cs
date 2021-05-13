using Android.Util;
using AppOfficina.Constants;
using AppOfficina.Portable.DTO;
using AppOfficina.Portable.Services;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AppOfficina.Portable.ViewModels
{
    public class PhotoUploadViewModel: BaseViewModel
    {
        private readonly ApiServices _apiService;
        List<GalleryImage> galleryImages = new List<GalleryImage>();

        public ICommand LogOffCommand { get; private set; }
        public ICommand SaveAndSend { get; private set; }

        public ICommand ItemPhotoCommand { get; private set; }


        public PhotoUploadViewModel(ApiServices _apiServices)
        {
            _apiService = _apiServices;
            LogOffCommand = new Command(async () => await logOffUser());
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


        private bool _isBusy = false;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; OnPropertyChanged("IsBusy"); }
        }

        private bool _IsEnableSendPhoto = false;
        public bool IsEnableSendPhoto
        {
            get { return _IsEnableSendPhoto; }
            set { _IsEnableSendPhoto = value; OnPropertyChanged("IsEnableSendPhoto"); }
        }





        private LayoutOption selectedLayout;

        public LayoutOption SelectedLayout
        {
            get
            {
                if (selectedLayout == null)
                {
                    selectedLayout = new LayoutOption(LayoutType.Grid, string.Empty);
                }
                return this.selectedLayout;
            }
            set
            {
                if (this.selectedLayout != value)
                {
                    this.selectedLayout = value;
                    OnPropertyChanged("SelectedLayout");
                }
            }
        }

        private ObservableCollection<GalleryImage> _images;
        public ObservableCollection<GalleryImage> images
        {
            get
            {
                if (_images == null)
                {
                    _images = new ObservableCollection<GalleryImage>(galleryImages);
                }
                return _images;
            }
            set
            {
                _images = value;
                OnPropertyChanged("images");
            }
        }


        public string LoggedUserName
        {
            get
            {
                return App.LoggedUser;
            }
        }

        private InconvenienteDTO _inconveniente;
        public InconvenienteDTO SelectedInconveniente
        {
            get
            {
                return _inconveniente;
            }
            set
            {
                _inconveniente = value;
                OnPropertyChanged("SelectedInconveniente");
            }
        }

        private CommessaDTO _SelectedCommessa;
        public CommessaDTO SelectedCommessa
        {
            get
            {
                return _SelectedCommessa;
            }
            set
            {
                _SelectedCommessa = value;
                OnPropertyChanged("SelectedCommessa");
            }
        }

        private string _idCommessa;
        public string idCommessa
        {
            get { return _idCommessa; }
            set { _idCommessa = value; OnPropertyChanged("idCommessa"); }
        }


        private string _targa;
        public string targa
        {
            get { return _targa; }
            set { _targa = value; OnPropertyChanged("targa"); }
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

        //protected override void CurrentPageOnAppearing(object sender, EventArgs eventArgs)
        //{
        //    previewPhoto();
        //}

        private byte[] ImageSourceToByteArray(ImageSource source)
        {
            StreamImageSource streamImageSource = (StreamImageSource)source;
            System.Threading.CancellationToken cancellationToken = System.Threading.CancellationToken.None;
            Task<Stream> task = streamImageSource.Stream(cancellationToken);
            Stream stream = task.Result;

            byte[] byteArrayPhoto;
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                byteArrayPhoto = ms.ToArray();
            }

            return byteArrayPhoto;
        }




     

        public async Task previewPhoto()
        {
            List<PhotoInconvenienteDTO> galleryImages = null;
            IsBusy = true;
            try
            {

                if (HasInternetConnection)
                {
                    if (SelectedInconveniente != null)
                    {
                        if (!string.IsNullOrEmpty(SelectedInconveniente.NumeroInconveniente))
                        {
                            galleryImages = await _apiService.GetPhotoForInconvenienteAsync(SelectedInconveniente.NumeroInconveniente, SelectedInconveniente.Extra);
                            images.Clear();
                            if (galleryImages.Any())
                            {
                                foreach (var base64String in galleryImages)
                                {
                                    GalleryImage galleryImage = new GalleryImage();
                                    ImageSource imageForInconveniente = null;

                                    try
                                    {
                                        imageForInconveniente = ImageSource.FromStream(
                                            () => new MemoryStream(Convert.FromBase64String(base64String.base64Foto)));
                                        galleryImage.isCheckedImage = false;
                                        galleryImage.Source = imageForInconveniente;
                                        images.Add(galleryImage);
                                    }
                                    catch (Exception ec)
                                    {
                                        Console.WriteLine(ec.Message);
                                    }
                                }
                            }
                        }
                        IsBusy = false;

                    }
                }
                else
                {
                    MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.NoInternetConnection);
                }
            }
            catch (Exception ec)
            {
                Log.Error("AppOfficina", ec.Message);
                //   MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.UnhandledExceptionError);
                MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.UnhandledExceptionError);
                IsBusy = false;
                return;
            }

        }




        async Task logOffUser()
        {
            IsBusy = true;
            await _apiService.LogoutUser();
            IsBusy = false;
        }




        private GalleryImage _selectedItemPhoto;
        public GalleryImage selectedItemPhoto
        {
            get
            {
                if (_selectedItemPhoto == null)
                {
                    _selectedItemPhoto = new GalleryImage();
                }
                return this._selectedItemPhoto;
            }
            set
            {
                this._selectedItemPhoto = value;
                OnPropertyChanged("selectedItemPhoto");

            }
        }
    }
    


    
}

