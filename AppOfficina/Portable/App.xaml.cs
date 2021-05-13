using Android;
using Android.OS;
using Android.Util;
using Android.Widget;
using AppOfficina.Portable.DTO;
using AppOfficina.Portable.Services;
using AppOfficina.Portable.ViewModels;
using AppOfficina.Portable.Views;
using DLToolkit.Forms.Controls;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Distribute;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace AppOfficina.Portable
{
    public partial class App : Xamarin.Forms.Application
    {
        public static SettingsApp settings;
        public static Stopwatch timer = new Stopwatch();


        public static string ApiUrl; /*_settingsService.SetUrlApi;*/
        public static string ApiAppName;/* _settingsService.SetApiName;*/

        public static int ScreenHeight { get; set; }
        public static int ScreenWidth { get; set; }
        private static TokenInfoDTO _token = null;
        public static TokenInfoDTO Token
        {
            get
            {
                if (_token == null && App.Current.Properties.ContainsKey("jwtToken") && App.Current.Properties["jwtToken"] != null)
                {
                    _token = JsonConvert.DeserializeObject<TokenInfoDTO>(App.Current.Properties["jwtToken"].ToString());
                }
                return _token;
            }
            set
            {
                App.Current.Properties["jwtToken"] = JsonConvert.SerializeObject(value);
                _token = value;
            }
        }

        public static int UserID
        {
            get
            {
                if (App.Current.Properties.ContainsKey("UserID"))
                {
                    return Int32.Parse(App.Current.Properties["UserID"].ToString());
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                App.Current.Properties["UserID"] = value;
            }
        }


        public static bool IsLogged
        {
            get
            {
                if (App.Current.Properties.ContainsKey("IsLogged"))
                {
                    return App.Current.Properties["IsLogged"].ToString() == Boolean.TrueString ? true : false;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                App.Current.Properties["IsLogged"] = value ? Boolean.TrueString : Boolean.FalseString;
            }
        }
        public static string LoggedUser { get; set; }

        public App()
        {
            InitializeComponent();
            FlowListView.Init();
            XF.Material.Forms.Material.Init(this);
            Xamarin.Forms.DataGrid.DataGridComponent.Init();
            settings = new SettingsApp();
         
            ApiUrl = "https://ns3130987.ip-51-75-52.eu/";
            ApiAppName = "testoff"; //wsof
            Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
            if (App.IsLogged && !string.IsNullOrEmpty(LoggedUser))
            {
                MainPage = new NavigationPage(new Commessa());
            }
            else
            {
                MainPage = new MainPage();
            }
        }

        bool OnReleaseAvailable(ReleaseDetails releaseDetails)
        {
            // Look at releaseDetails public properties to get version information, release notes text or release notes URL
            string versionName = releaseDetails.ShortVersion;
            string versionCodeOrBuildNumber = releaseDetails.Version;
            string releaseNotes = releaseDetails.ReleaseNotes;
            Uri releaseNotesUrl = releaseDetails.ReleaseNotesUrl;

            // custom dialog
            var title = "Version " + versionName + " available!";
            Task answer;

            // On mandatory update, user cannot postpone
            if (releaseDetails.MandatoryUpdate)
            {
                answer = Current.MainPage.DisplayAlert(title, releaseNotes, "Download and Install");
            }
            else
            {
                answer = Current.MainPage.DisplayAlert(title, releaseNotes, "Download and Install", "Maybe tomorrow...");
            }
            answer.ContinueWith((task) =>
            {
                // If mandatory or if answer was positive
                if (releaseDetails.MandatoryUpdate || (task as Task<bool>).Result)
                {
                    // Notify SDK that user selected update
                    Distribute.NotifyUpdateAction(UpdateAction.Update);
                }
                else
                {
                    // Notify SDK that user selected postpone (for 1 day)
                    // Note that this method call is ignored by the SDK if the update is mandatory
                    Distribute.NotifyUpdateAction(UpdateAction.Postpone);
                }
            });
            // Return true if you are using your own dialog, false otherwise
            return true;
        }

        protected override void OnStart()
        {
            Distribute.ReleaseAvailable = OnReleaseAvailable;
            //AppCenter.Start("android=6a78412c-8af7-488a-93f9-493e3a1832f0;", typeof(Distribute));
            AppCenter.Start("android=969f143f-3ed8-4208-8295-651bdc082202", typeof(Distribute));
            //AppCenter.Start("969f143f-3ed8-4208-8295-651bdc082202", typeof(Distribute));
            Distribute.SetEnabledAsync(true);
        }


        protected override void OnSleep() 
        {
            // Handle when your app sleeps
            App.Current.Properties["SleepMode"] = DateTime.Now.ToString("O");
            App.Current.Properties["TimerSlepp"] = timer.Elapsed;

            if (App.IsLogged && !string.IsNullOrEmpty(LoggedUser))
            {
                var curPage = ((NavigationPage)Current.MainPage).CurrentPage;
                if (curPage != null)
                {
                    if (curPage is ViewInconveniente)
                    {
                        try
                        {
                            ViewInconveniente inconveniente = (ViewInconveniente)curPage;
                            if (inconveniente != null)
                            {
                                var vm = inconveniente.BindingContext;
                                if (vm != null)
                                {
                                    if (vm is InconvenienteViewModel)
                                    {
                                        InconvenienteViewModel vwModel = (InconvenienteViewModel)vm;
                                        //MessagingCenter.Send<string>("OK","myService");
                                        //timer.Stop();
                                    }
                                }
                            }

                        }
                        catch (Exception ecc)
                        {
                            Log.Error("AppOfficina", ecc.Message);
                        }
                    }
                }
            }
          

        }

        protected override void OnResume()
        {
           
            if (App.IsLogged && !string.IsNullOrEmpty(LoggedUser))
            {
                var curPage = ((NavigationPage)Current.MainPage).CurrentPage;
                if (curPage != null)
                {

                    if (curPage is ViewInconveniente)
                    {
                        try
                        {
                            //Gestione standBy Inconveniente -- 
                            // controllo la pagina inconveniente.
                            ViewInconveniente inconveniente = (ViewInconveniente)curPage;
                            if (inconveniente != null)
                            {
                                var vm = inconveniente.BindingContext;
                                if (vm != null)
                                {
                                    if (vm is InconvenienteViewModel)
                                    {
                                        var value = (string)App.Current.Properties["SleepMode"];
                                        InconvenienteViewModel vwModel = (InconvenienteViewModel)vm;
                                        DateTime sleepDate;
                                        if(DateTime.TryParse(value, out sleepDate))
                                        {
                                            vwModel.dtsleepDate = sleepDate;
                                            //vwModel.sleepMode();
                                            //vwModel.InitializeAsync(vwModel.IDCommessa);
                                        }
                                        

                                    }
                                }
                            }
                        }
                        catch (Exception ecc)
                        {
                            Log.Error("AppOfficina", ecc.Message);
                        }

                    }
                }
            }
        }
    }
}
