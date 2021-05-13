using AppOfficina.Portable.DTO;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace AppOfficina.Portable.Utils
{
    public static class Extensions
    {
        public static void SetBearerToken(this HttpClient client)
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", App.Token.jwtToken);
        }

        public static bool ShouldRefresh(this TokenInfoDTO token)
        {
            if (DateTime.Now.Ticks - token.StoredDate >= token.expiresIn.Ticks - token.StoredDate)
            {
                return true;
            }
            return false;
        }
    }
}
