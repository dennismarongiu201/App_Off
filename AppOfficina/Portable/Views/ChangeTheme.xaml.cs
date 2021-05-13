using AppOfficina.Portable.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppOfficina.Portable.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChangeTheme : ContentPage
    {
        public ChangeTheme()
        {
            InitializeComponent();
        }

        void OnPickerSelectionChanged(object sender, EventArgs e)
        {
            Picker picker = sender as Picker;
            Theme theme = (Theme)picker.SelectedItem;

            ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            if (mergedDictionaries != null)
            {
                mergedDictionaries.Clear();

                switch (theme)
                {
                    case Theme.Dark:
                        mergedDictionaries.Add(new CustomTheme());
                        break;
                 
                }
                statusLabel.Text = $"{theme.ToString()} theme loaded. Close this page.";
            }
        }

        public async Task Dismiss()
        {
            await Navigation.PopModalAsync();
        }
    }
}