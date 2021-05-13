using AppOfficina.Constants;
using AppOfficina.Portable.DTO;
using AppOfficina.Portable.Services;
using AppOfficina.Portable.Views;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Rg.Plugins.Popup.Services;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Telerik.XamarinForms.Input.DataForm;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace AppOfficina.Portable.ViewModels
{
    public class PhotoViewModel : BaseViewModel
    {
        private readonly ApiServices _apiService;
        List<GalleryImage> galleryImages = new List<GalleryImage>();

        public ICommand LogOffCommand { get; private set; }
        public ICommand SaveAndSend { get; private set; }

        public ICommand ItemPhotoCommand { get; private set; }


        public PhotoViewModel(ApiServices _apiServices)
        {
            _apiService = _apiServices;
            LogOffCommand = new Command(async () => await logOffUser());
            SaveAndSend = new Command(async () => await SaveAndUploadPhoto());

          
           
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

        protected override void CurrentPageOnAppearing(object sender, EventArgs eventArgs)
        {
            if(!images.Any())
                IsEnableSendPhoto = false;
        }

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

        async Task SaveAndUploadPhoto()
        {
            IsBusy = true;
            IsEnableSendPhoto = false;
            List<UploadPhotoInconvenienteDTO> lstPhoto = new List<UploadPhotoInconvenienteDTO>();
            ImageSource imageUpload = null;
            string base64Photo = string.Empty;
            byte[] lstBytePhoto = null;
            try
            {
                if (HasInternetConnection)
                {
                    if (SelectedInconveniente != null)
                    {
                        foreach (var image in images)
                        {
                            imageUpload = image.Source;
                            lstBytePhoto = ImageSourceToByteArray(imageUpload);

                            base64Photo = Convert.ToBase64String(lstBytePhoto);
                            if (!string.IsNullOrEmpty(base64Photo))
                            {
                                if (lstBytePhoto != null && lstBytePhoto.Length > 0)
                                {
                                    lstPhoto.Add(new UploadPhotoInconvenienteDTO { NumeroInconveniente = SelectedInconveniente.NumeroInconveniente, Extra = SelectedInconveniente.Extra, Base64Foto = base64Photo });
                                }
                            }
                        }
                        bool bUploadSuccess = await _apiService.UploadPhotoForInconveniente(lstPhoto);
                        if (bUploadSuccess)
                        {
                            //RadPopup popup = new RadPopup();
                            //ContentPopupConfirm content = new ContentPopupConfirm(popup);
                            //View popupContent = (View)((ControlTemplate)content.Resources["PopupTemplate"]).CreateContent();

                            //popup.Content = popupContent;
                            //popup.IsModal = true;
                            //popup.OutsideBackgroundColor = Color.FromHex("6F000000");
                            //popup.Placement = PlacementMode.Center;
                            //popup.AnimationType = PopupAnimationType.Zoom;
                            //popup.IsOpen = true;
                            await Navigation.PopAsync();
                        }
                    }
                }
                else
                {
                    MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.NoInternetConnection);
                }
            
            }
            catch (Exception ec)
            {
                IsBusy = false;
                Console.Write(ec.Message);
            }
            IsBusy = false;
            IsEnableSendPhoto = true;
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
            get {
                if(_selectedItemPhoto == null){
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



        private ICommand _cameraCommand;
        public ICommand CameraCommand
        {
            get { return _cameraCommand ?? new Command(async () => await ExecuteCameraCommand(), () => CanExecuteCameraCommand()); }
        }


        /// <summary>
        /// Determines whether this instance can execute camera command.
        /// </summary>
        /// <returns><c>true</c> if this instance can execute camera command; otherwise, <c>false</c>.</returns>
        public bool CanExecuteCameraCommand()
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Executes the camera command.
        /// </summary>
        /// <returns>The camera command.</returns>
        public async Task ExecuteCameraCommand()
        {
            await CrossMedia.Current.Initialize();
            MediaFile file = null;
            var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync<CameraPermission>();
            var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync<StoragePermission>();
            if (cameraStatus != PermissionStatus.Granted || storageStatus != PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera, Permission.Storage });
                cameraStatus = results[Permission.Camera];
                storageStatus = results[Permission.Storage];
            }
            if (cameraStatus == PermissionStatus.Granted && storageStatus == PermissionStatus.Granted)
            {
                try
                {
                    file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                    {
                        Directory = "AppOfficine",
                        SaveToAlbum = true,
                        SaveMetaData = true,
                        DefaultCamera = CameraDevice.Rear,
                        CompressionQuality = 98,
                        PhotoSize = PhotoSize.Large,
                    });
                }
                catch (Exception ecc)
                {
                    MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, ecc.Message);
                    throw;
                }


                IsBusy = true;
                if (file != null)
                {
                    byte[] imageAsBytes = null;
                    using (var memoryStream = new MemoryStream())
                    {
                        file.GetStream().CopyTo(memoryStream);
                        imageAsBytes = memoryStream.ToArray();
                    }

                    if (imageAsBytes.Length > 0)
                    {
                        var imageSource = ImageSource.FromStream(() => new MemoryStream(imageAsBytes));

                        string nameFile = System.IO.Path.GetFileName(file.Path);

                        //per ogni foto che faccio l'aggiungo alla galleria virtuale dell'app
                        GalleryImage glImage = new GalleryImage { Source = imageSource, OrgImage = imageAsBytes, isCheckedImage = false, pathImage = file.Path, nameImage = nameFile };
                        galleryImages.Add(glImage);
                        images.Add(glImage);
                        IsEnableSendPhoto = true;
                    }
                    file.Dispose();
                }
                IsBusy = false;
            }
          
        }
    }
}
