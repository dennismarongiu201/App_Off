using Android.Animation;
using AppOfficina.Android;
using AppOfficina.Constants;
using AppOfficina.Portable.DTO;
using AppOfficina.Portable.ViewModels;
using CommonServiceLocator;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.XamarinForms.Input.Calendar;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace AppOfficina.Portable.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewInconveniente : ContentPage 
    {
        private string _commessa;
        private string _targa;
        InconvenienteViewModel _vm;
        ViewCell lastCell;
        InconvenienteDTO lastInconveniente;

        public ViewInconveniente(CommessaDTO commessaDTO = null,InconvenienteDTO inconveniente = null)
        {
            InitializeComponent();

            this._commessa = commessaDTO.NumeroCommessa;
            this._targa = commessaDTO.TargaCommessa;
            _vm = (InconvenienteViewModel)(ServiceLocator.Current.GetInstance(typeof(InconvenienteViewModel)));

            _vm.Navigation = this.Navigation;
            _vm.selectedCommessa = commessaDTO;
            _vm.IDCommessa = _commessa;
            _vm.targaVeicolo = _targa;
            _vm.codiceCliente = commessaDTO.codice;
            _vm.ragioneSociale = commessaDTO.ragioneSociale;
            _vm.telefono = commessaDTO.telefono;
            _vm.mail = commessaDTO.email;
            _vm.numeroTelaio = commessaDTO.Telaio;
            _vm.nomeVeicolo = commessaDTO.DescrizioneVeicolo;
            _vm.selectedInconveniente = inconveniente;
            _vm.defCommessa = "Commessa N :" + _commessa + ",Targa Veicolo : " + _targa + " , Telaio : " + commessaDTO.Telaio + " , Veicolo : " + commessaDTO.DescrizioneVeicolo;
            BindingContext = _vm;
            ((InconvenienteViewModel)BindingContext).Initialize(this);


            //MessagingCenter.Subscribe<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, async (sender, msg) =>
            //{
            //    await DisplayAlert("Informazioni", msg, "OK");
            //});

            MessagingCenter.Subscribe<Application, DisplayAlertMessage>(this, "Attenzione!", async (sender, message) =>
            {

                const string title = "Informazioni";
                const string cancel = "No";

                var result = true;
                if (!string.IsNullOrEmpty(message.Accept) && !string.IsNullOrEmpty(message.Message))
                    result = await DisplayAlert(title, message.Message, message.Accept, cancel);
               
                if (message.OnCompleted != null)
                    message.OnCompleted(result);

            }, Application.Current);

            SetBackgroundColorButton();

        }

        private void SetBackgroundColorButton(){

            btnStart.BackgroundColor = Color.LightGreen;
            btnStop.BackgroundColor = Color.IndianRed;
            btnCloseInconveniente.BackgroundColor = Color.LightCyan;
            btnCloseInconveniente.TextColor = Color.DarkGray;
            
        }

        public void UnsubscribeEvents()
        {
            MessagingCenter.Unsubscribe<string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError);
            MessagingCenter.Unsubscribe<DisplayAlertMessage>(MessagingCenterEvents.Subscriber, "Attenzione!");
        }

        protected override bool OnBackButtonPressed(){
            _vm.GoToCommessa();           
            return true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            UnsubscribeEvents();
        }




        private void listView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var itemInconveniente = (InconvenienteDTO)e.SelectedItem;
            if (itemInconveniente == null)
            {
                _vm.IsStartEnabled = false;
                _vm.IsStopEnabled = false;
                _vm.IsCloseInconvenienteEnabled = false;
                _vm.IsExtraEnabled = true;
                _vm.IsPhotoEnabled = false;
                _vm.IsTakePhotoEnabled = false;
                btnStart.BackgroundColor = Color.LightGreen;
                btnStop.BackgroundColor = Color.IndianRed;
                btnCloseInconveniente.BackgroundColor = Color.LightCyan;
                btnCloseInconveniente.TextColor = Color.DarkGray;
            }

            if (lastInconveniente != null)
            {
                if (App.timer.IsRunning)
                {
                    listView.SelectedItem = lastInconveniente;
                    return;
                }
                else
                {
                    lastInconveniente.selection = false;
                }
            }

            if (itemInconveniente != null)
            {
                btnStart.BackgroundColor = Color.DarkGreen;
                btnStop.BackgroundColor = Color.IndianRed;
                btnCloseInconveniente.BackgroundColor = Color.FromHex("#3F51B5");
                btnCloseInconveniente.TextColor = Color.White;
                itemInconveniente.selection = true;
                if (itemInconveniente.Stato.Equals("Da autorizzare"))
                {
                    btnStart.BackgroundColor = Color.LightGreen;
                    btnStop.BackgroundColor = Color.IndianRed;
                    btnCloseInconveniente.BackgroundColor = Color.LightCyan;
                }
                else
                {
                    _vm.IsCloseInconvenienteEnabled = true;
                    _vm.IsExtraEnabled = true;
                    _vm.IsPhotoEnabled = true;
                    _vm.IsTakePhotoEnabled = true;
                    _vm.selectedInconveniente = itemInconveniente;
                }

                lastInconveniente = itemInconveniente;
            }
            _vm.listSelectedInconvenienti?.Execute(e);
        }


        private void ViewCell_Tapped(object sender, EventArgs e)
        {
            
            var viewCell = (ViewCell)sender;
            viewCell.View.BackgroundColor = Color.Transparent;
           
            if (lastCell != null){
                lastCell.View.BackgroundColor = Color.Transparent;
            }


            if (viewCell.View != null)
            {
                lastCell = viewCell;
            }
        }

        private  void listView_SelectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _vm.IsBusy = true;
            int index = -1 ;
            
            if(e.OldItems != null)
            {
                if (e.OldItems.Count > 0)
                {
                    foreach (var image in e.OldItems)
                    {
                        _vm.currentPhoto = image as GalleryImage;
                        index = _vm.images.IndexOf(_vm.currentPhoto);

                        if (index >= 0)
                        {
                            _vm.indxCarousel = index;
                        }

                    }
                    return;
                }
            }
            if(e.NewItems != null)
            {
                if (e.NewItems.Count > 0)
                {
                    foreach (var image in e.NewItems)
                    {
                        _vm.currentPhoto = image as GalleryImage;
                        index = _vm.images.IndexOf(_vm.currentPhoto);

                        if (index >= 0)
                        {
                            _vm.indxCarousel = index;
                        }
                            
                    }
                }
            }
            
            //await _vm.selectionPhotoFromGallery();
            _vm.IsBusy = false;
        }

        private async void txtNote_Unfocused(object sender, FocusEventArgs e)
        {
            Editor notes = sender as Editor;
            if(notes != null)
            {
                string strInconveniente = notes.Text;
                _vm.noteInconveniente = strInconveniente;
            }
           
            _vm.IsBusy = true;
            await _vm.updateNote();
            _vm.IsBusy = false;
        }
        

        private void listView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            //if (e.Item == null) return;

            //if (sender is ListView lv) lv.SelectedItem = null;
            //if (_vm._timer.IsRunning)
            //{
            //    listView.SelectionMode = ListViewSelectionMode.None;
            //}
            //else
            //{
            //    listView.SelectionMode = ListViewSelectionMode.Single;
            //}
        }


        private void btnUploadPhoto_Clicked(object sender, EventArgs e)
        {
            _vm.IsPhotoEnabled = false;
            Navigation.PushAsync(new PhotoUploadView(_vm.selectedCommessa, _vm.selectedInconveniente));
            _vm.IsPhotoEnabled = true;
        }

        private void btnScattaFoto_Clicked(object sender, EventArgs e)
        {
            _vm.IsTakePhotoEnabled = false;
            Navigation.PushAsync(new PhotoView(_vm.selectedCommessa, _vm.selectedInconveniente));
            _vm.IsTakePhotoEnabled = true;
        }
    }
}