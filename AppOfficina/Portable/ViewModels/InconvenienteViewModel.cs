using Android.Util;
using AppOfficina.Constants;
using AppOfficina.Portable.DTO;
using AppOfficina.Portable.Services;
using AppOfficina.Portable.Views;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Xamarin.Forms;


namespace AppOfficina.Portable.ViewModels
{
    public class InconvenienteViewModel : BaseViewModel
    {


        private readonly ApiServices _apiService;
        private string startTimeInconveniente;
        private readonly SettingsApp _SettingsApp;
        List<InconvenienteDTO> lstInconvenientiForCommessa;

        string note;

        private LavorazioneInconvenienteDTO lavorazioneInconvenienteDTO;
        private System.Timers.Timer tmr = new System.Timers.Timer();


        private string _countPhoto;
        public string countPhoto
        {
            get
            {
                return _countPhoto;
            }
            set
            {
                _countPhoto = value;
                OnPropertyChanged("countPhoto");
            }
        }

        private string _countDocument;
        public string countDocument
        {
            get
            {
                return _countDocument;
            }
            set
            {
                _countDocument = value;
                OnPropertyChanged("countDocument");
            }
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

        private InconvenienteDTO _selectedInconveniente;
        public InconvenienteDTO selectedInconveniente
        {
            get
            {
                return _selectedInconveniente;
            }
            set
            {
                _selectedInconveniente = value;
                OnPropertyChanged("selectedInconveniente");
            }
        }

        private GalleryImage _currentPhoto;
        public GalleryImage currentPhoto
        {
            get
            {
                return _currentPhoto;
            }
            set
            {
                _currentPhoto = value;
                OnPropertyChanged("currentPhoto");
            }
        }

        private CommessaDTO _selectedCommessa;
        public CommessaDTO selectedCommessa
        {
            get
            {
                return _selectedCommessa;
            }
            set
            {
                _selectedCommessa = value;
                OnPropertyChanged("selectedCommessa");
            }
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


        #region Command ComponentView
        public ICommand StartCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand PhotoCommand { get; private set; }
        public ICommand ExtraCommand { get; private set; }
        public ICommand LogOffCommand { get; private set; }
        public ICommand UpdateNote { get; private set; }
        public ICommand EndWorkCommand { get; private set; }
        public ICommand CloseInconvenienteCommand { get; private set; }
        public ICommand selectionPhoto { get; private set; }
        public ICommand AttesaRicambiCommand { get; private set; }
        public ICommand BackToCommessa { get; private set; }
        public ICommand refreshListView { get; private set; }
        //public ICommand OpenInconvenienteCommand { get; private set; }

        //caricamento foto
        public ICommand UploadPhotoCommand { get; private set; }
        
        public ICommand GestioneNoteCommand { get; private set; }
        #endregion

        #region Parametri
        public string LoggedUserName
        {
            get
            {
                return App.LoggedUser;
            }
        }


        private bool _isBusy = false;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; OnPropertyChanged("IsBusy"); }
        }


        private bool _isEnable = true;
        public bool isEnable
        {
            get { return _isEnable; }
            set { _isEnable = value; OnPropertyChanged("isEnable"); }
        }


        private bool _IsAttesaEnabled = false;
        public bool IsAttesaEnabled
        {
            get { return _IsAttesaEnabled; }
            set { _IsAttesaEnabled = value; OnPropertyChanged("IsAttesaEnabled"); }
        }

        private bool _IsBackToCommessaEnabled= true;
        public bool IsBackToCommessaEnabled
        {
            get { return _IsBackToCommessaEnabled; }
            set { _IsBackToCommessaEnabled = value; OnPropertyChanged("IsBackToCommessaEnabled"); }
        }

        private bool _IsFineLavoroEnabled = false;
        public bool IsFineLavoroEnabled
        {
            get { return _IsFineLavoroEnabled; }
            set { _IsFineLavoroEnabled = value; OnPropertyChanged("IsFineLavoroEnabled"); }
        }

        public void GoToCommessa()
        {
            string msg = "Attenzione! il tempo di lavoro sull'inconveniente è in esecuzione. Sicuri di voler terminare il lavoro e di tornare indietro?";
            CheckActivityIfEndWorking(msg);
        }

        private string _defCommessa;
        public string defCommessa
        {
            get
            {
                return _defCommessa;
            }
            set
            {
                _defCommessa = value;
                OnPropertyChanged("defCommessa");
            }
        }

        private string _IDCommessa;
        public string IDCommessa
        {
            get
            {
                return _IDCommessa;
            }
            set
            {
                _IDCommessa = value;
                OnPropertyChanged("IDCommessa");
            }
        }


        private string _targaVeicolo;
        public string targaVeicolo
        {
            get
            {
                return _targaVeicolo;
            }
            set
            {
                _targaVeicolo = value;
                OnPropertyChanged("targaVeicolo");
            }
        }



        private ObservableCollection<GalleryImage> _images;
        public ObservableCollection<GalleryImage> images
        {
            get
            {
                if (_images == null)
                {
                    _images = new ObservableCollection<GalleryImage>();
                }
                return _images;
            }
            set
            {
                _images = value;
                OnPropertyChanged("images");
            }
        }


        #region Abilitazione tasti

        private bool _IsStopEnabled = false;
        public bool IsStopEnabled
        {
            get
            {
                return _IsStopEnabled;
            }
            set
            {
                _IsStopEnabled = value;
                OnPropertyChanged("IsStopEnabled");
            }
        }

        private bool _IsCloseInconvenienteEnabled = false;
        public bool IsCloseInconvenienteEnabled
        {
            get
            {
                return _IsCloseInconvenienteEnabled;
            }
            set
            {
                _IsCloseInconvenienteEnabled = value;
                OnPropertyChanged("IsCloseInconvenienteEnabled");
            }
        }
        
        private bool _IsTakePhotoEnabled = false;
        public bool IsTakePhotoEnabled
        {
            get
            {
                return _IsTakePhotoEnabled;
            }
            set
            {
                _IsTakePhotoEnabled = value;
                OnPropertyChanged("IsTakePhotoEnabled");
            }
        }


        private bool _IsPhotoEnabled = false;
        public bool IsPhotoEnabled
        {
            get
            {
                return _IsPhotoEnabled;
            }
            set
            {
                _IsPhotoEnabled = value;
                OnPropertyChanged("IsPhotoEnabled");
            }
        }

        private bool _IsExtraEnabled = false;
        public bool IsExtraEnabled
        {
            get
            {
                return _IsExtraEnabled;
            }
            set
            {
                _IsExtraEnabled = value;
                OnPropertyChanged("IsExtraEnabled");
            }
        }


        private bool _IsStartEnabled = false;
        public bool IsStartEnabled
        {
            get
            {
                return _IsStartEnabled;
            }
            set
            {
                _IsStartEnabled = value;
                OnPropertyChanged("IsStartEnabled");
            }
        }


        private ObservableCollection<InconvenienteDTO> _lstInconvenienti;
        public ObservableCollection<InconvenienteDTO> lstInconvenienti
        {
            get
            {
                return _lstInconvenienti;
            }
            set
            {
                _lstInconvenienti = value;
                OnPropertyChanged("lstInconvenienti");
            }
        }



        private Color _ColorState;
        public Color ColorState
        {
            get
            {
                return _ColorState;
            }
            set
            {
                _ColorState = value;
                OnPropertyChanged("ColorState");
            }
        }


        #endregion
        #endregion

        Command<SelectedItemChangedEventArgs> _listSelectedInconvenienti;


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

        private int _indxCarousel;




        private string _noteInconveniente;
        public string noteInconveniente
        {
            get
            {
                return _noteInconveniente;
            }
            set
            {
                _noteInconveniente = value;
                OnPropertyChanged("noteInconveniente");
            }
        }

        private DateTime _dtsleepDate;

        public DateTime dtsleepDate
        {
            get
            {
                return _dtsleepDate;
            }
            set
            {
                _dtsleepDate = value;
            }
        }

        public int indxCarousel
        {
            get
            {
                return _indxCarousel;
            }
            set
            {
                _indxCarousel = value;
                OnPropertyChanged("indxCarousel");
            }
        }


        public Command<SelectedItemChangedEventArgs> listSelectedInconvenienti => _listSelectedInconvenienti ??
        (_listSelectedInconvenienti = new Command<SelectedItemChangedEventArgs>(selectionInconveniente));

        public InconvenienteViewModel(ApiServices _apiServices, SettingsApp _settings)
        {
            _apiService = _apiServices;
            _SettingsApp = _settings;
            StopCommand = new Command(async () => await stopCommand());
            StartCommand = new Command(async () => await startCommand());
            //PhotoCommand = new Command(async () => await activityPhoto());
            ExtraCommand = new Command(async () =>  await goToInconvenienteExtraActivity());

            LogOffCommand = new Command(async () => await logOffUser());
            UpdateNote = new Command(async () => await updateNote());
            EndWorkCommand = new Command(async () => await EndCommessa());

            CloseInconvenienteCommand = new Command(async () => await closeInconveniente());
            //OpenInconvenienteCommand = new Command(async () => await OpenInconveniente());

            AttesaRicambiCommand = new Command(async () => await AttesaRicambiInconveniente());
            selectionPhoto = new Command(async () => await selectionPhotoFromGallery());
            BackToCommessa = new Command(endWorking);
            refreshListView = new Command(async () => await UpdateDataSourceInconveniente());


            //UploadPhotoCommand = new Command(photoActivityPage);
            GestioneNoteCommand = new Command<InconvenienteDTO>((InconvenienteDTO param) =>  openGestioneNote());

            tmr.Interval = 1000;
            tmr.Elapsed -= OnTimedEvent;
            tmr.Elapsed += OnTimedEvent;

        }

        //private void photoActivityPage()
        //{
        //    Navigation.PushAsync(new PhotoUploadView(selectedCommessa, selectedInconveniente));
        //}

        private void  openGestioneNote()
        {
             Navigation.PushAsync(new HistoryNotesPage(selectedInconveniente,selectedCommessa));
        }

        public async Task UpdateDataSourceInconveniente()
        {
            IsBusy = true;
            if (selectedInconveniente == null)
            {
                await InitializeAsync(IDCommessa, null, null);
            }
            else
            {
                await InitializeAsync(IDCommessa, selectedInconveniente.DataInizio, selectedInconveniente.DataFine);
            }

            IsBusy = false;
        }

        //public async Task OpenInconveniente()
        //{
        //    IsBusy = true;
        //    IsOpenInconvenienteEnabled = false;
        //    try
        //    {
        //        if (HasInternetConnection)
        //        {
        //            if (selectedInconveniente.Stato.Equals("Eseguito"))
        //            {
        //                LavorazioneInconvenienteDTO bOkApriInconveniente = await _apiService.StartInconveniente(selectedInconveniente.NumeroInconveniente, selectedInconveniente.Extra,selectedInconveniente.DataInizioLavorazione);
        //                if (bOkApriInconveniente != null)
        //                {
        //                    InitializeAsync(IDCommessa, null, null);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.NoInternetConnection);
        //        }

        //    }
        //    catch (Exception ecc) {
        //        Log.Error("AppOfficina", ecc.Message);
        //    }

        //    IsBusy = false;
        //    IsOpenInconvenienteEnabled = true;
        //}

        public async Task EndCommessa()
        {

            //controllo lo stato di tutti gli inconvenienti
            //se tutti  gli inconvenienti sono in stato eseguito
            //cambio lo stato della commessa
            if (lstInconvenientiForCommessa != null && lstInconvenientiForCommessa.Count > 0)
            {
                //prendo tutti gli inconvenienti
                int CountInconvenienti = lstInconvenientiForCommessa.Count;
                //contatore stato-inconveniente "Eseguito"
                int CountEseguitiInconvenienti = 0;
                foreach (var inconveniente in lstInconvenientiForCommessa)
                {
                    if (inconveniente.Stato.Equals("Eseguito"))
                    {
                        CountEseguitiInconvenienti++;
                    }
                }

                if (CountInconvenienti == CountEseguitiInconvenienti)
                {
                    bool bOk = await ChangeStatusCommessa(selectedCommessa.NumeroCommessa);
                    if (bOk)
                    {
                        await Navigation.PopAsync();
                    }
                }
            }
        }

        public async Task<bool> ChangeStatusCommessa(string numeroCommessa)
        {
            bool bOK = false;
            try
            {

                if (HasInternetConnection)
                {
                    bOK = await _apiService.changeStatusCommessa(numeroCommessa);
                }
                else
                {
                    MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.NoInternetConnection);
                    return bOK;
                }

            }
            catch (Exception)
            {
                MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.UnhandledExceptionError);
                IsBusy = false;
                return bOK;
            }
            return bOK;
        }


        public async Task selectionPhotoFromGallery()
        {
            images.Clear();
            try
            {
                if (HasInternetConnection)
                {
                    List<PhotoInconvenienteDTO> galleryImages = await _apiService.GetPhotoForInconvenienteAsync(selectedInconveniente.NumeroInconveniente, selectedInconveniente.Extra);
                    if (galleryImages != null)
                    {
                        if (galleryImages.Any())
                        {
                            foreach (var base64String in galleryImages)
                            {
                                ImageSource imageForInconveniente = null;
                                GalleryImage galleryImage = new GalleryImage();
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
                                    //solo per debug
                                    Console.WriteLine(ec.Message);
                                }
                            }
                            await PopupNavigation.Instance.PushAsync(new PhotoCarouselPage(indxCarousel, images, null), false);
                        }
                    }
                }
                else
                {
                    MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.NoInternetConnection);
                }

            }
            catch (Exception)
            {
                MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.UnhandledExceptionError);
                IsBusy = false;
                return;
            }

        }

        async Task AttesaRicambiInconveniente()
        {
            IsBusy = true;
            IsAttesaEnabled = false;
            try
            {
                if (HasInternetConnection)
                {
                    if (selectedInconveniente != null)
                    {
                        if (selectedInconveniente.Stato.Equals("In corso") || selectedInconveniente.Stato.Equals("Autorizzato") || selectedInconveniente.Stato.Equals("Aperto") || selectedInconveniente.Stato.Equals("Sospeso"))
                        {
                            if (App.timer.IsRunning)
                            {
                                string msg = "Attenzione! il tempo di lavoro sull'inconveniente è in esecuzione. Sicuri di voler interrompere il lavoro?";
                                CheckActivityIfEndWorking(msg);
                            }
                            bool bAttesaRicambi = await _apiService.waitRicambiInconveniente(selectedInconveniente.NumeroInconveniente, selectedInconveniente.Extra);
                            if (bAttesaRicambi)
                            {
                                if (selectedInconveniente.DataInizioLavorazione.Equals(DateTime.MinValue))
                                {
                                    await InitializeAsync(IDCommessa, selectedInconveniente.DataInizio, selectedInconveniente.DataFine);
                                }
                                else
                                {
                                    await InitializeAsync(IDCommessa, selectedInconveniente.DataInizioLavorazione.ToString("dd/MM/yyyy HH:mm:ss"), selectedInconveniente.DataFine);
                                }

                            }
                        }
                        else
                        {
                            MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.Inconveniente.StatusInconvenienteRicambi);
                            IsBusy = false;
                        }
                    }
                }
                else
                {
                    MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.NoInternetConnection);
                }

            }
            catch (Exception ecc)
            {
                Log.Error("AppOfficina", ecc.Message);
                MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.UnhandledExceptionError);
                IsBusy = false;
                return;
            }
            IsAttesaEnabled = true;
            IsBusy = false;
        }

        #region METODI NON USATI
        async void previewCountDocument()
        {
            int numbercountDocument = 0;
            IsBusy = true;
            try
            {

                if (HasInternetConnection)
                {
                    if (selectedInconveniente != null)
                    {
                        if (!string.IsNullOrEmpty(selectedInconveniente.NumeroInconveniente))
                        {
                            numbercountDocument = await _apiService.CheckDocumentAsync(selectedInconveniente.NumeroInconveniente);
                            if (numbercountDocument >= 0)
                            {
                                countDocument = numbercountDocument.ToString();
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
            catch (Exception)
            {
                MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.UnhandledExceptionError);
                IsBusy = false;
                return;
            }

        }

        async void previewCountPhoto()
        {
            int numbercountPhoto = 0;
            IsBusy = true;
            try
            {

                if (HasInternetConnection)
                {
                    if (selectedInconveniente != null)
                    {
                        if (!string.IsNullOrEmpty(selectedInconveniente.NumeroInconveniente))
                        {
                            numbercountPhoto = await _apiService.CheckNumberPhoto(selectedInconveniente.NumeroInconveniente);
                            if (numbercountPhoto >= 0)
                            {
                                countPhoto = numbercountPhoto.ToString();
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
            catch (Exception exx)
            {
                Log.Error("AppOfficina", exx.Message);
                IsBusy = false;
                return;
            }

        }


        async void previewPhoto()
        {
            List<PhotoInconvenienteDTO> galleryImages = null;
            IsBusy = true;
            try
            {

                if (HasInternetConnection)
                {
                    if (selectedInconveniente != null)
                    {
                        if (!string.IsNullOrEmpty(selectedInconveniente.NumeroInconveniente))
                        {

                            galleryImages = await _apiService.PreviewPhotoForInconvenienteAsync(selectedInconveniente.NumeroInconveniente, selectedInconveniente.Extra);
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
            catch (Exception)
            {
                MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.UnhandledExceptionError);
                IsBusy = false;
                return;
            }

        }

        #endregion

        void selectionInconveniente(SelectedItemChangedEventArgs e)
        {
            try
            {
                images.Clear();
                IsAttesaEnabled = true;

                InconvenienteDTO inconveniente = (InconvenienteDTO)e.SelectedItem;
                ClearDisableButton(lstInconvenientiForCommessa);
                if (inconveniente != null)
                {
                    inconveniente.IsEnableNote = true;
                    inconveniente.selection = true;
                    if (inconveniente.Stato.Equals("Eseguito"))
                    {
                        inconveniente.IsEnableNote = false;
                        IsStartEnabled = true;
                        IsStopEnabled = false;
                        IsCloseInconvenienteEnabled = false;
                        IsPhotoEnabled = false;
                        IsTakePhotoEnabled = false;
                        IsAttesaEnabled = false;

                    }

                    if (inconveniente.Stato.Equals("Da autorizzare"))
                    {
                        IsStartEnabled = false;
                        IsStopEnabled = false;
                        IsCloseInconvenienteEnabled = false;
                        IsExtraEnabled = true;
                        IsPhotoEnabled = true;
                        IsTakePhotoEnabled = true;
                        IsAttesaEnabled = false;
                    }

                }

                
                if (inconveniente.Stato.ToLower() != "eseguito" && inconveniente.Stato.ToLower() != "da autorizzare")
                {
                    IsStartEnabled = true;
                }

                if (inconveniente.Extra)
                {
                    IsStartEnabled = false;
                    IsStopEnabled = false;
                }

                if (CheckIfCanCloseCommessa(lstInconvenientiForCommessa))
                {
                    IsFineLavoroEnabled = true;
                }

            }

            catch (Exception ec)
            {
                IsBusy = false;
                Log.Error("AppOfficina", ec.Message);
            }

        }

        #region  Utility
        private bool CheckIfCanCloseCommessa(List<InconvenienteDTO> lstInconvenientiForCommessa)
        {
            return lstInconvenientiForCommessa.All(x => x.Stato.ToLower() == "eseguito");
        }

        private void ClearDisableButton(List<InconvenienteDTO> lstInconvenientiForCommessa)
        {
            foreach(var inconveniente in lstInconvenientiForCommessa)
            {
                inconveniente.IsEnableNote = false;
            }
        }

        #endregion

        async Task closeInconveniente()
        {
            try
            {
                IsBusy = true;
                IsCloseInconvenienteEnabled = false;
                if (HasInternetConnection)
                {
                    if (selectedInconveniente != null)
                    {
                        if (!string.IsNullOrEmpty(selectedInconveniente.NumeroInconveniente))
                        {
                            if (selectedInconveniente.Stato != "Da autorizzare" &&
                                selectedInconveniente.Stato != "Eseguito" &&
                                selectedInconveniente.Stato != "In attesa di ricambi")
                            {
                                bool bCloseInconveniente = await _apiService.closeInconveniente(selectedInconveniente.NumeroInconveniente, selectedInconveniente.Extra);
                                if (bCloseInconveniente)
                                {
                                    await InitializeAsync(IDCommessa, null, null);
                                }
                            }
                            else
                            {
                                MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, "Attenzione per chiudere l'inconveniente deve essere in stato Aperto o in corso.");
                                IsBusy = false;
                                return;
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
                MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.UnhandledExceptionError);
                IsBusy = false;
                return;
            }
        }

        public async Task updateNote()
        {
            try
            {

                IsBusy = true;
                if (HasInternetConnection)
                {
                    if (selectedInconveniente != null)
                    {
                        note = noteInconveniente.Trim();
                        if (!string.IsNullOrEmpty(selectedInconveniente.NumeroInconveniente))
                        {
                            bool bInsert = await _apiService.UpdateNoteInconveniente(selectedInconveniente.NumeroInconveniente, selectedInconveniente.Extra, note);
                            if (bInsert)
                            {
                                //in fase di recovery 
                                //dataInizio lavorazione presa da mfiles.
                                if (selectedInconveniente.DataInizioLavorazione.Equals(DateTime.MinValue))
                                {
                                    await InitializeAsync(IDCommessa, selectedInconveniente.DataInizio, selectedInconveniente.DataFine);
                                }
                                else
                                {
                                    await InitializeAsync(IDCommessa, selectedInconveniente.DataInizioLavorazione.ToString("dd/MM/yyyy HH:mm:ss"), selectedInconveniente.DataFine);
                                }
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
            catch (Exception ecc)
            {
                Log.Error("AppOfficina", ecc.Message);
                IsBusy = false;
            }
        }




        protected override void CurrentPageOnAppearing(object sender, EventArgs eventArgs)
        {
            if (selectedInconveniente == null)
            {
                Task.Run(async () => await InitializeAsync(IDCommessa, string.Empty, string.Empty));
            }
            else
            {
                Task.Run(async () => await InitializeAsync(IDCommessa, selectedInconveniente.DataInizio, selectedInconveniente.DataFine));
            }

        }

        private void SetColorStatesInconvenienti(List<InconvenienteDTO> lstInconvenientiForCommessa)
        {
            foreach (var inconvenienti in lstInconvenientiForCommessa)
            {
                string state = inconvenienti.Stato;
                if (!string.IsNullOrEmpty(state))
                {
                    switch (state)
                    {
                        case "Aperto":
                            inconvenienti.ColorState = Color.Blue;
                            break;
                        case "Eseguito":
                            inconvenienti.ColorState = Color.Green;
                            break;
                        case "Rifiutato":
                            inconvenienti.ColorState = Color.Red;
                            break;
                        case "In attesa di ricambi":
                            inconvenienti.ColorState = Color.Orange;
                            break;
                        case "Da autorizzare":
                            inconvenienti.ColorState = Color.Yellow;
                            break;
                        case "In corso":
                            inconvenienti.ColorState = Color.LightGreen;
                            break;
                        case "Autorizzato":
                            inconvenienti.ColorState = Color.LightSeaGreen;
                            break;
                        case "Sospeso":
                            inconvenienti.ColorState = Color.LightGray;
                            break;
                    }
                }
            }
        }

        private void SetStatesButton()
        {
            IsAttesaEnabled = false;
            IsStartEnabled = false;
            IsCloseInconvenienteEnabled = false;
            IsExtraEnabled = true;
            IsPhotoEnabled = false;
            IsTakePhotoEnabled = false;
            IsStopEnabled = false;
            isEnable = true;

        }
       
  
        public async Task InitializeAsync(string idCommessa, string dataInizio, string dataFine)
        {
            IsBusy = true;
            try
            {
                if (HasInternetConnection)
                {
                    if (!string.IsNullOrEmpty(idCommessa))
                    {
                        SetStatesButton();
                        lstInconvenientiForCommessa = await _apiService.CheckInconvenienteForCommessa(idCommessa);
                        
                        if (lstInconvenientiForCommessa != null && lstInconvenientiForCommessa.Any())
                        {
                            //set semaforo stato inconveniente
                            SetColorStatesInconvenienti(lstInconvenientiForCommessa);
                            //format tempo
                            FormatTimeInconveniente(lstInconvenientiForCommessa, dataInizio, dataFine);
                            //stati bottone in fase di inizializzazione
                            
                            lstInconvenienti = new ObservableCollection<InconvenienteDTO>(lstInconvenientiForCommessa);
                            if (selectedInconveniente != null)
                            {
                                var selectedInconv = lstInconvenientiForCommessa.Where(c => c.NomeInconveniente == selectedInconveniente.NomeInconveniente).FirstOrDefault();
                                if (selectedInconv != null)
                                {
                                    selectedInconveniente = selectedInconv;
                                    if (!string.IsNullOrEmpty(dataInizio) && string.IsNullOrEmpty(dataFine))
                                    {
                                        selectedInconv.ColorState = Color.LightGreen;
                                        selectedInconv.Stato = "in lavorazione";
                                    }
                                    if (App.timer.IsRunning)
                                    {
                                        //selectedInconv.selection = true;
                                        //if (App.Current.Properties.ContainsKey("SleepMode"))
                                        //{
                                        //    DateTime resume = DateTime.Now;
                                          
                                        //    TimeSpan timeSpan = dtsleepDate - resume;
                                        //    var prova = +timeSpan;
                                        //    var timeElapsed = App.timer.Elapsed + (-timeSpan);
                                        //    var timerprecedente = App.Current.Properties["TimerSlepp"];
                                        //    App.timer.Elapsed.Add(timeElapsed);                                     
                                        //}

                                        if (!string.IsNullOrEmpty(dataInizio))
                                        {
                                            Device.BeginInvokeOnMainThread(() =>
                                            {
                                                selectedInconv.IsEnableNote = true;
                                                selectedInconv.DataInizio = dataInizio;
                                                selectedInconv.DataFine = dataFine;
                                            });
                                        }
                                        IsFineLavoroEnabled = false;
                                        IsCloseInconvenienteEnabled = false;
                                        IsExtraEnabled = false;
                                        IsPhotoEnabled = true;
                                        IsTakePhotoEnabled = true;
                                        isEnable = false;
                                        selectedInconveniente.IsEnableNote = true;
                                        IsAttesaEnabled = false;
                                        selectedInconv.selection = true;
                                        IsStartEnabled = false;
                                        //IsStopEnabled = true;

                                    }
                                    else
                                    {
                                        selectedInconveniente.DataInizio = dataInizio;
                                        selectedInconveniente.DataFine = dataFine;
                                        IsFineLavoroEnabled = false;
                                        selectedInconv.selection = true;
                                        IsStartEnabled = true;
                                        IsCloseInconvenienteEnabled = true;
                                        IsExtraEnabled = true;
                                        IsPhotoEnabled = true;
                                        IsTakePhotoEnabled = true;
                                        selectedInconveniente.IsEnableNote = true;
                                        selectedInconveniente.IsLoadTimer = false;
                                        IsAttesaEnabled = true;
                                    }


                                    if (selectedInconv.Stato.Equals("Da autorizzare"))
                                    {
                                        IsStartEnabled = false;
                                        IsStopEnabled = false;
                                        IsCloseInconvenienteEnabled = false;
                                        IsAttesaEnabled = false;
                                        IsPhotoEnabled = true;
                                        IsTakePhotoEnabled = true;
                                    }

                                    if (selectedInconv.Stato.Equals("Eseguito"))
                                    {
                                        IsStartEnabled = true;
                                        IsStopEnabled = false;
                                        IsCloseInconvenienteEnabled = false;
                                        IsAttesaEnabled = false;
                                        IsPhotoEnabled = false;
                                        IsTakePhotoEnabled = false;
                                    }

                                    if (selectedInconv.Extra)
                                    {
                                        IsStartEnabled = false;
                                        IsStopEnabled = false;
                                    }

                                    //se tutti i componenti sono in stato eseguito riabilito il tasto fine lavoro
                                    if (CheckIfCanCloseCommessa(lstInconvenientiForCommessa)){
                                        IsFineLavoroEnabled = true;
                                    }

                                }
                            }
                        }
                    }
                }
                else
                {
                    IsBusy = false;
                    MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.NoInternetConnection);
                    return;
                }
            }
            catch (Exception ecc)
            {
                IsBusy = false;
                Log.Error("AppOfficina", ecc.Message);
                MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.UnhandledExceptionError);
            }
            IsBusy = false;

        }



        private void FormatTimeInconveniente(List<InconvenienteDTO> lstInconvenientiForCommessa, string dataInizio, string dataFine)
        {
            foreach (var inconveniente in lstInconvenientiForCommessa)
            {
                TimeSpan timeInconveniente = TimeSpan.FromSeconds(inconveniente.SecondiLavorati);
                if (inconveniente.SecondiLavorati > 0)
                {
                    // n/3600
                    inconveniente.OreLavorate = (int)timeInconveniente.Hours;
                    // n/60
                    inconveniente.MinutiLavorati = (int)timeInconveniente.Minutes;
                }

                //controllo se ho la datalavorazione
                //se la datalavorazione equivale al valore minimo , non mi serve vederla
                if (!inconveniente.DataInizioLavorazione.Equals(DateTime.MinValue))
                {
                    inconveniente.visibleDateInizioLavorazione = true;
                    inconveniente.selection = true;
                    inconveniente.Stato = "In lavorazione";
                    inconveniente.ColorState = Color.LightGreen;
                    selectedInconveniente = inconveniente;
                    App.timer.Restart();
                    tmr.Start();
                    IsStartEnabled = false;
                }

            }

            if (selectedInconveniente != null)
            {
                if (!string.IsNullOrEmpty(dataInizio))
                {
                    selectedInconveniente.DataInizio = dataInizio;
                }
                else
                {
                    if(!selectedInconveniente.DataInizioLavorazione.Equals(DateTime.MinValue))
                    {
                        selectedInconveniente.DataInizio = selectedInconveniente.DataInizioLavorazione.ToString("dd/MM/yyyy HH:mm:ss");
                    }
                    else
                    {
                        selectedInconveniente.DataInizio = string.Empty;
                    }
                }


                if (!string.IsNullOrEmpty(dataFine))
                {
                    selectedInconveniente.DataFine = dataFine;
                }
                else
                {
                    selectedInconveniente.DataFine = string.Empty;
                }
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
                    IsBusy = false;
                }
                else
                {
                    IsBusy = false;
                    MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.NoInternetConnection);
                    return;
                }

            }
            catch (Exception)
            {
                IsBusy = false;
                MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.UnhandledExceptionError);
                return;
            }

        }

        private void CheckActivityIfEndWorking(string msg)
        {
            var message = new DisplayAlertMessage();
            if (App.timer.IsRunning)
            {
                message.Message = msg;
            }
            message.Accept = "Si";
            message.OnCompleted += async (accept) =>
            {
                if (accept)
                {
                    bool ret = false;
                    if (App.timer.IsRunning)
                    {                    
                        string strDtStopInconveniente = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        string dataInizioLavorazione = startTimeInconveniente;
                        if (!string.IsNullOrEmpty(startTimeInconveniente) || !string.IsNullOrEmpty(strDtStopInconveniente))
                        {
                            IsBusy = true;
                            if (HasInternetConnection)
                            {
                                if (string.IsNullOrEmpty(dataInizioLavorazione))
                                {
                                    dataInizioLavorazione = selectedInconveniente.DataInizioLavorazione.ToString("dd/MM/yyyy HH:mm:ss");
                                }

                                if (!string.IsNullOrEmpty(dataInizioLavorazione))
                                {
                                    selectedInconveniente.IsLoadTimer = false;
                                    //se ho l'oggetto che mi arriva dallo start l'id corretto lo trovo in quel DTO altrimenti c'è lo già nel DTO dell'inconveniente
                                    if (lavorazioneInconvenienteDTO != null)
                                    {
                                        ret = await _apiService.SendTimeWorking(selectedInconveniente.NumeroInconveniente, selectedInconveniente.Extra, Convert.ToDateTime(dataInizioLavorazione), Convert.ToDateTime(strDtStopInconveniente), selectedInconveniente.Note, lavorazioneInconvenienteDTO.IdMarcatempo);
                                    }
                                    else
                                    {
                                        ret = await _apiService.SendTimeWorking(selectedInconveniente.NumeroInconveniente, selectedInconveniente.Extra, Convert.ToDateTime(dataInizioLavorazione), Convert.ToDateTime(strDtStopInconveniente), selectedInconveniente.Note, selectedInconveniente.IdMarcatempo);
                                    }
                                    if (ret)
                                    {
                                        App.timer.Stop();
                                        tmr.Stop();
                                        await InitializeAsync(IDCommessa, dataInizioLavorazione, strDtStopInconveniente);
                                        IsStopEnabled = false;
                                        IsStartEnabled = true;
                                        IsExtraEnabled = true;
                                        isEnable = true;
                                        IsAttesaEnabled = true;
                                        IsCloseInconvenienteEnabled = true;
                                        await Navigation.PopAsync();
                                    }
                                    else
                                    {
                                        Device.BeginInvokeOnMainThread(() =>
                                        {
                                            selectedInconveniente.IsEnableNote = true;
                                            selectedInconveniente.DataFine = string.Empty;
                                        });
                                        IsStartEnabled = false;
                                        IsStopEnabled = true;
                                    }
                                    
                                }
                            }
                            IsBusy = false;
                        }
                    }
                    else
                    {
                        await Navigation.PopAsync();
                    }
                   
                }
            };
            MessagingCenter.Send<Application, DisplayAlertMessage>(Application.Current, "Attenzione!", message);
        }

        void endWorking()
        {
            string msg = "Attenzione! il tempo di lavoro sull'inconveniente è in esecuzione. Sicuri di voler interrompere il lavoro e di tornare indietro?";
            CheckActivityIfEndWorking(msg);
        }

        async Task goToInconvenienteExtraActivity()
        {
            if (selectedCommessa != null)
            {
                await Navigation.PushAsync(new ExtraView(selectedInconveniente, defCommessa, selectedCommessa.NumeroCommessa, selectedCommessa.TargaCommessa, selectedCommessa));
            }
        }

        async Task activityPhoto()
        {
            await Navigation.PushAsync(new PhotoView(selectedCommessa, selectedInconveniente));
        }

       
        

        async Task startCommand()
        {
            IsStartEnabled = false;
            

            startTimeInconveniente = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            var selectedInconv = lstInconvenientiForCommessa.Where(c => c.NomeInconveniente == selectedInconveniente.NomeInconveniente).FirstOrDefault();
            if (selectedInconv != null)
            {
                
                selectedInconv.selection = true;
                Device.BeginInvokeOnMainThread(() =>
                {
                    selectedInconv.IsEnableNote = true;
                    selectedInconv.DataInizio = startTimeInconveniente;
                    selectedInconv.DataFine = string.Empty;
                });

                //per fare start l'inconveniente non deve essere da autorizzare
                if (selectedInconv.Stato.ToLower() != "da autorizzare")
                {
                    IsBusy = true;
                    try
                    {
                        if (HasInternetConnection)
                        {
                            lavorazioneInconvenienteDTO = await _apiService.StartInconveniente(selectedInconv.NumeroInconveniente, selectedInconv.Extra, Convert.ToDateTime(startTimeInconveniente));
                            if (lavorazioneInconvenienteDTO != null && selectedInconv.Stato.ToLower() != "in corso")
                            {
                                //timer generico per lavorazioni
                                //utilizzato per la logica di inizio-fine
                                App.timer.Reset();
                                App.timer.Start();
                                //timer di supporto per prevenire errore concerto.
                                
                                tmr.Start();
                                if (!string.IsNullOrEmpty(startTimeInconveniente))
                                {
                                    selectedInconv.ColorState = Color.LightGreen;
                                    selectedInconv.Stato = "in lavorazione";
                                }

                                await InitializeAsync(IDCommessa, selectedInconv.DataInizio, null);
                                IsBackToCommessaEnabled = false;
                                IsStartEnabled = false;
                                IsStopEnabled = false;
                                IsExtraEnabled = false;
                                isEnable = false;
                                IsAttesaEnabled = false;
                                IsCloseInconvenienteEnabled = false;
                            }
                            else
                            {
                                IsStartEnabled = true;
                                IsStopEnabled = false;
                                //se errore resetto la data di inizio
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    selectedInconv.IsEnableNote = true;
                                    selectedInconv.DataInizio = string.Empty;
                                    selectedInconv.DataFine = string.Empty;
                                });
                            }
                        }
                        else
                        {
                            MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.NoInternetConnection);
                        }
                    }
                    catch (Exception ecc)
                    {
                        IsStartEnabled = true;
                        IsBusy = false;
                        Log.Error("AppOfficina", ecc.Message);
                        MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.UnhandledExceptionError);
                    }
                    IsBusy = false;
                }
            }
        }

        //ogni secondo del mio stopwatch fittizzio vado a controllare se il timer di abilitazione tasti arriva ai 30 sec.
        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            // qua devo abilitare il tasto dopo x secondi
            Device.BeginInvokeOnMainThread(() =>
            {
                if (selectedInconveniente != null)
                {
                    TimeSpan dtDifference = DateTime.Now - Convert.ToDateTime(selectedInconveniente.DataInizio);
                    if(dtDifference.TotalSeconds >= 30)
                    {
                        if (IsBusy)
                        {
                            IsStopEnabled = false;
                            IsBackToCommessaEnabled = false;
                        }
                        else
                        {
                            IsStopEnabled = true;
                            IsBackToCommessaEnabled = true;
                        }
                        
                    }
                }
                
                if (App.timer.Elapsed.Seconds >= 30)
                {
                    if (IsBusy)
                    {
                        IsStopEnabled = false;
                    }
                    else
                    {
                        IsStopEnabled = true;
                    }
                    IsBackToCommessaEnabled = true;
                }
              
            });
        }

        async Task stopCommand()
        {
            //TimeSpan addTimeSpan = new TimeSpan(App.timer.Elapsed.Hours, App.timer.Elapsed.Minutes, App.timer.Elapsed.Seconds);
            string strDtStopInconveniente = string.Empty;
            string dataInizioLavorazione = startTimeInconveniente;
            IsStopEnabled = false;
            tmr.Stop();
            try
            {
                bool ret = false;
                if (HasInternetConnection)
                {
                    strDtStopInconveniente = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    IsBusy = true;
                    if (string.IsNullOrEmpty(dataInizioLavorazione))
                    {
                        dataInizioLavorazione = selectedInconveniente.DataInizioLavorazione.ToString("dd/MM/yyyy HH:mm:ss");
                    }
                    if (!string.IsNullOrEmpty(dataInizioLavorazione))
                    {
                        selectedInconveniente.IsLoadTimer = false;
                        //se ho l'oggetto che mi arriva dallo start l'id corretto lo trovo in quel DTO altrimenti c'è lo già nel DTO dell'inconveniente
                        if (lavorazioneInconvenienteDTO != null){
                            ret = await _apiService.SendTimeWorking(selectedInconveniente.NumeroInconveniente, selectedInconveniente.Extra, Convert.ToDateTime(dataInizioLavorazione), Convert.ToDateTime(strDtStopInconveniente), selectedInconveniente.Note, lavorazioneInconvenienteDTO.IdMarcatempo);
                        }
                        else{
                            ret = await _apiService.SendTimeWorking(selectedInconveniente.NumeroInconveniente, selectedInconveniente.Extra, Convert.ToDateTime(dataInizioLavorazione), Convert.ToDateTime(strDtStopInconveniente), selectedInconveniente.Note, selectedInconveniente.IdMarcatempo);
                        }
                        
                        //se la chiamata mi restituisce OK
                        if (ret)
                        {
                            App.timer.Stop();
                         
                            await InitializeAsync(IDCommessa, dataInizioLavorazione, strDtStopInconveniente);
                            selectedInconveniente.DataFine = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                            IsStopEnabled = false;
                            IsStartEnabled = true;
                            IsExtraEnabled = true;
                            isEnable = true;
                            IsAttesaEnabled = true;
                            IsCloseInconvenienteEnabled = true;
                        }
                        else
                        {
                            //se errore resetto la data di inizio
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                selectedInconveniente.IsEnableNote = true;
                                selectedInconveniente.DataFine = string.Empty;
                            });
                            IsStartEnabled = false;
                            IsStopEnabled = true;
                        }    
                    }
                }
                else
                {
                    IsStopEnabled = true;
                    IsBusy = false;
                    MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.NoInternetConnection);
                    return;
                }
            }
            catch (Exception ex)
            {
                IsStopEnabled = true;
                IsBusy = false;
                Log.Error("AppOfficina", ex.Message);
                MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.UnhandledExceptionError);
            }       
            IsBusy = false;
        }


    }
}
