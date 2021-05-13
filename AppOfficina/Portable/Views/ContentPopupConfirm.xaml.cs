using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppOfficina.Portable.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContentPopupConfirm : ContentView
    {
        private RadPopup _popup;
        public ContentPopupConfirm(Telerik.XamarinForms.Primitives.RadPopup popup)
        {
            InitializeComponent();
            this._popup = popup;
        }

        private void RadButton_Clicked(object sender, EventArgs e)
        {
            if (_popup != null)
                _popup.IsOpen = false;
        }
    }
}