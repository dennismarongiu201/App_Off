using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppOfficina.Portable.DTO
{
    public class TokenInfoDTO
    {
        [JsonProperty("userId")]
        public int userId { get; set; }
        [JsonProperty("username")]
        public string username { get; set; }
        [JsonProperty("jwtToken")]
        public string jwtToken { get; set; }
        [JsonProperty("expiresIn")]
        public DateTime expiresIn { get; set; }
        public long ExpiresRefreshToken { get; set; }
        public string refreshToken { get; set; }
        public long StoredDate { get; set; }
    }

    public class TokenLogoutDTO
    {
        public string token { get; set; }
    }
}
