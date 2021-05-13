using Android.Content.PM;
using AppOfficina.Constants;
using AppOfficina.Portable.DTO;
using AppOfficina.Portable.Services;
using AppOfficina.Portable.Views;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Permission = Plugin.Permissions.Abstractions.Permission;

namespace AppOfficina.Portable.ViewModels
{
    public class InconvenienteExtraViewModel : BaseViewModel
    {
        private readonly ApiServices _apiService;
        List<GalleryImage> galleryImages = new List<GalleryImage>();

        public ICommand LogOffCommand { get; private set; }
        public ICommand save { get; private set; }
        public ICommand PhotoCommand{ get; private set; }

        private bool _isBusy = false;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; OnPropertyChanged("IsBusy"); }
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
        public string LoggedUserName
        {
            get
            {
                return App.LoggedUser;
            }
        }

       
        private string _defCommessaInExtra;
        public string defCommessaInExtra
        {
            get { return _defCommessaInExtra; }
            set { _defCommessaInExtra = value; OnPropertyChanged("defCommessaInExtra"); }
        }

        private string _titoloInconveniente;
        public string titoloInconveniente
        {
            get { return _titoloInconveniente; }
            set { _titoloInconveniente = value; OnPropertyChanged("titoloInconveniente"); }
        }

        private string _descrInconveniente;
        public string descrInconveniente
        {
            get { return _descrInconveniente; }
            set { _descrInconveniente = value; OnPropertyChanged("descrInconveniente"); }
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

        private CommessaDTO _commessaDTO;
        public CommessaDTO commessaDTO
        {
            get { return _commessaDTO; }
            set { _commessaDTO = value; OnPropertyChanged("commessaDTO"); }
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

        private ObservableCollection<GalleryImage> _imagesExtra;
        public ObservableCollection<GalleryImage> imagesExtra
        {
            get
            {
                if (_imagesExtra == null)
                {
                    _imagesExtra = new ObservableCollection<GalleryImage>(galleryImages);
                }
                return _imagesExtra;
            }
            set
            {
                _imagesExtra = value;
                OnPropertyChanged("imagesExtra");
            }
        }

        private bool _IsSaveEnabled = true;
        public bool IsSaveEnabled
        {
            get
            {
                return _IsSaveEnabled;
            }
            set
            {
                _IsSaveEnabled = value;
                ((Command)save).ChangeCanExecute();
                OnPropertyChanged("IsSaveEnabled");
            }
        }

        public InconvenienteExtraViewModel(ApiServices _service)
        {
            _apiService = _service;
            LogOffCommand = new Command(async () => await logOffUser());
         
            save = new Command(async () => {
                IsSaveEnabled = false;
                await saveInconveniente();
                IsSaveEnabled = true;
            }, () => {
                return IsSaveEnabled;
            });

            PhotoCommand = new Command(async () => await ExecuteCameraCommand());
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
                        CompressionQuality = 92,
                        PhotoSize = PhotoSize.Medium,
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
                        imagesExtra.Add(glImage);
                    }
                    file.Dispose();
                }
                IsBusy = false;
            }

        }

        public void UnsubscribeEvents()
        {
            MessagingCenter.Unsubscribe<string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError);
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

        async Task saveInconveniente()
        {
            try
            {
                IsBusy = true;
              
                if (HasInternetConnection)
                {
                    if (!string.IsNullOrEmpty(idCommessa))
                    {
                        List<string> photo = new List<string>();
                        string base64Photo = string.Empty;
                        ImageSource imageUpload = null;
                        byte[] lstBytePhoto = null;
                        if (!string.IsNullOrEmpty(titoloInconveniente) && !string.IsNullOrEmpty(descrInconveniente))
                        {
                            foreach (var image in imagesExtra)
                            {
                                imageUpload = image.Source;
                                lstBytePhoto = ImageSourceToByteArray(imageUpload);

                                base64Photo = Convert.ToBase64String(lstBytePhoto);
                                if (!string.IsNullOrEmpty(base64Photo))
                                {
                                    if (lstBytePhoto != null && lstBytePhoto.Length > 0)
                                    {
                                        photo.Add(base64Photo);
                                    }
                                }
                            }
                            bool bInsert = await _apiService.setInconvenienteExtra(idCommessa, titoloInconveniente, descrInconveniente, photo);
                            if (bInsert)
                            {
                                //await Navigation.PushAsync(new ViewInconveniente(commessaDTO));
                                await Navigation.PopAsync();

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
            catch (Exception)
            {
                IsBusy = false;
                MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.UnhandledExceptionError);
                return;
            }


        }


        async Task logOffUser()
        {
            IsBusy = true;
            try
            {
                if (HasInternetConnection)
                {
                    await _apiService.LogoutUser();
                }

            }
            catch (Exception) { }
            
            IsBusy = false;
        }
    }
}
