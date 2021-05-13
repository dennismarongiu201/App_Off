using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;

namespace AppOfficina.Android
{

    public class CustomNavigationPage : NavigationPage
    {
        public CustomNavigationPage(Page root)
            : base(root)
        {
        }
    }

}