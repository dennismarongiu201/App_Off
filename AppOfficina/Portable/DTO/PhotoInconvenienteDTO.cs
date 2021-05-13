using Android.Util;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AppOfficina.Portable.Services
{
    public class PhotoInconvenienteDTO
    {
        [JsonProperty("base64Foto")]
        public string base64Foto { get; set; }
    }


    public class UploadPhotoInconvenienteDTO
    {
        public string NumeroInconveniente { get; set; }
		public bool Extra { get; set; }
        public string Base64Foto { get; set; }
    }

}