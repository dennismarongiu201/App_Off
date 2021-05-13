using System.Threading.Tasks;

namespace AppOfficina.Portable.Services
{
    public interface ISettingsService
    {
        string SetUrlApi { get; set; }
        string SetApiName { get; set; }
        string SetTimeInconveniente { get; set; }


        bool GetValueOrDefault(string key, bool defaultValue);
        string GetValueOrDefault(string key, string defaultValue);
        Task AddOrUpdateValue(string key, bool value);
        Task AddOrUpdateValue(string key, string value);
    }
}