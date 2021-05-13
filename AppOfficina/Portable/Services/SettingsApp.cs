using Plugin.Settings.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppOfficina.Portable.Services
{
    public class SettingsApp : ISettingsService
    {
        #region Setting Constants


        //chiavi
        private const string UrlApi = "url_api";
        private const string ApiName = "name_api";
        private const string KeyTimeInconveniente = "tm_incov";

        //valori
        private string UrlApiService = "http://ns3050971.ip-149-202-211.eu/";
        private string NameApi = "wsof";
        private string timeInconv = string.Empty;

        #endregion

        #region Settings Properties

        public string SetUrlApi
        {
            get => GetValueOrDefault(UrlApi, UrlApiService);
            set => AddOrUpdateValue(UrlApi, value);
        }

        public string SetApiName
        {
            get => GetValueOrDefault(ApiName, NameApi);
            set => AddOrUpdateValue(ApiName, value);
        }

        //provvisorio : todo
        public string SetTimeInconveniente
        {
            get => GetValueOrDefault(KeyTimeInconveniente, timeInconv);
            set => AddOrUpdateValue(KeyTimeInconveniente, value);
        }

        #endregion
        #region Public Methods


        public Task AddOrUpdateValue(string key, bool value) => AddOrUpdateValueInternal(key, value);
        public Task AddOrUpdateValue(string key, string value) => AddOrUpdateValueInternal(key, value);
        public bool GetValueOrDefault(string key, bool defaultValue) => GetValueOrDefaultInternal(key, defaultValue);
        public string GetValueOrDefault(string key, string defaultValue) => GetValueOrDefaultInternal(key, defaultValue);

        #endregion

        #region Internal Implementation

        async Task AddOrUpdateValueInternal<T>(string key, T value)
        {
            if (value == null)
            {
                await Remove(key);
            }

            Application.Current.Properties[key] = value;
            try
            {
                await Application.Current.SavePropertiesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to save: " + key, " Message: " + ex.Message);
            }
        }

        T GetValueOrDefaultInternal<T>(string key, T defaultValue = default(T))
        {
            object value = null;
            if (Application.Current.Properties.ContainsKey(key))
            {
                value = Application.Current.Properties[key];
            }
            return null != value ? (T)value : defaultValue;
        }

        async Task Remove(string key)
        {
            try
            {
                if (Application.Current.Properties[key] != null)
                {
                    Application.Current.Properties.Remove(key);
                    await Application.Current.SavePropertiesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to remove: " + key, " Message: " + ex.Message);
            }
        }

        #endregion
    }
}
