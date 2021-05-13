using AppOfficina.Constants;
using AppOfficina.Portable.DTO;
using AppOfficina.Portable.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Telerik.Windows.Documents.Core.TextMeasurer;
using Xamarin.Android.Net;
using Xamarin.Forms;

namespace AppOfficina.Portable.Services
{
    public class ApiServices
    {
        private readonly HttpClient _client;
        private readonly CookieContainer _cookies;
        private readonly HttpClientHandler _handler;


        public ApiServices()
        {
            _handler = new HttpClientHandler();
            _cookies = new CookieContainer();
            _handler.CookieContainer = _cookies;
            _handler.UseCookies = true;
            

            _client = new HttpClient(_handler)
            {
                Timeout = new TimeSpan(0, 3, 20),
                BaseAddress = new Uri(App.ApiUrl),
                          
            };
        }


        #region LOGIN - REFRESH TOKEN
        private void LogOutIfUnauthorized()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                App.IsLogged = false;
                await LogoutUser();
                //App.IsLogged = false;
                //App.LoggedUser = null;
                //App.Token = null;
                //await App.Current.SavePropertiesAsync();
                //App.Current.MainPage = new MainPage();
            });
        }


        public async Task<bool> LogoutUser()
        {

            await SetAuthToken(0);

            Uri uri = new Uri(string.Format(App.ApiUrl + App.ApiAppName + "/Users/revoke-token", string.Empty));
            string json = JsonConvert.SerializeObject(new TokenLogoutDTO { token = App.Token.refreshToken });
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, uri);
            httpRequest.Content = content;
            var response = await _client.SendAsync(httpRequest);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                App.IsLogged = false;
                App.LoggedUser = null;
                App.Token = null;
                await App.Current.SavePropertiesAsync();
                App.Current.MainPage = new MainPage();
                return true;
            }

            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return false;
            }

            else
            {
                return false;
            }
        }

        public async Task<int> CheckNumberPhoto(string numeroInconveniente)
        {
            int numberOfPhoto = 0;
            try
            {
                // metodo che controlla la scadenza del token
                await SetAuthToken(0);
                if (!string.IsNullOrEmpty(numeroInconveniente))
                {
                    Uri uri = new Uri(string.Format(App.ApiUrl + App.ApiAppName + "/Users/numero-foto", string.Empty));
                    string json = JsonConvert.SerializeObject(new NumeroFotoRequest { NumeroInconveniente = numeroInconveniente });
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, uri);
                    httpRequest.Content = content;

                    var response = await _client.SendAsync(httpRequest);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = response.Content.ReadAsStringAsync().Result;
                        if (!string.IsNullOrEmpty(result))
                        {
                            numberOfPhoto = Convert.ToInt32(result);
                        }
                        //if (!string.IsNullOrEmpty(userInfoDTO.jwtToken))
                        //{
                        //    //questa scadenza vale solo per il token di refresh

                        //}
                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.Login.ApiUserNotFound);
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        throw new UnAuthorizedException();
                    }
                    else
                    {
                        return numberOfPhoto;
                    }
                }

            }
            catch (UnAuthorizedException)
            {
                LogOutIfUnauthorized();
            }

            return numberOfPhoto;
        }





        public async Task<TokenInfoDTO> CheckApiCredentialsAsync(string user, string pass)
        {
            Uri uri = new Uri(string.Format(App.ApiUrl + App.ApiAppName + "/Users/authenticate", string.Empty));
            string json = JsonConvert.SerializeObject(new UserDTO { username = user, password = pass });
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, uri);
            httpRequest.Content = content;
            IEnumerable<string> cookieStrings = null;
           
            var response = await _client.SendAsync(httpRequest);
       
            response.Headers.TryGetValues("refreshToken", out cookieStrings);
            IEnumerable<Cookie> responseCookies = _cookies.GetCookies(uri).Cast<Cookie>();
            foreach (Cookie cookie in responseCookies)
            {
                _cookies.Add(uri, cookie);
            }

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                TokenInfoDTO userInfoDTO = JsonConvert.DeserializeObject<TokenInfoDTO>(result);
                if (!string.IsNullOrEmpty(userInfoDTO.jwtToken))
                {
                    //questa scadenza vale solo per il token di refresh
                    string decodeRefreshToken = HttpUtility.UrlDecode(responseCookies.Select(c => c.Value).FirstOrDefault());

                    userInfoDTO.refreshToken = decodeRefreshToken;
                    userInfoDTO.ExpiresRefreshToken = responseCookies.Select(c => c.Expires.Ticks).FirstOrDefault();
                    userInfoDTO.StoredDate = DateTime.Now.Ticks;
                    App.Token = userInfoDTO;
                    return userInfoDTO;
                }
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.Login.ApiUserNotFound);
            }
            else
            {
                MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.Login.UnknownApiCredentialsCheck);
            }
            return null;
        }

        private async Task SetAuthToken(int userId)
        {

            if (App.Token.ShouldRefresh())
            {
                await RefreshTokenAsync(userId);
            }
            _client.SetBearerToken();
        }


        public async Task RefreshTokenAsync(int userId)
        {
            Uri uri = new Uri(string.Format(App.ApiUrl + App.ApiAppName + "/Users/refresh-token", string.Empty));
            string json = string.Empty;
            try
            {
                if (userId > 0)
                    json = JsonConvert.SerializeObject(userId);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, uri);
                httpRequest.Content = content;

                _handler.CookieContainer.Add(new Cookie("refreshToken", App.Token.refreshToken, "/", uri.Host));

                
                var response = await _client.SendAsync(httpRequest);
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    IEnumerable<Cookie> responseCookies = _cookies.GetCookies(uri).Cast<Cookie>();
                    TokenInfoDTO refreshTokenDTO = JsonConvert.DeserializeObject<TokenInfoDTO>(result);
                    if (!string.IsNullOrEmpty(refreshTokenDTO.jwtToken))
                    {
                        string decodeRefreshToken = HttpUtility.UrlDecode(responseCookies.Select(c => c.Value).FirstOrDefault());

                        refreshTokenDTO.refreshToken = decodeRefreshToken;
                        refreshTokenDTO.ExpiresRefreshToken = responseCookies.Select(c => c.Expires.Ticks).FirstOrDefault();
                        //refreshTokenDTO.expiresIn = new DateTime(refreshTokenDTO.ExpiresRefreshToken);
                        refreshTokenDTO.StoredDate = DateTime.Now.Ticks;
                        App.Token.jwtToken = refreshTokenDTO.jwtToken;
                        App.Token = refreshTokenDTO;
                        await App.Current.SavePropertiesAsync();
                    }
                }
              
            }
            catch (Exception ecc)
            {
                Console.WriteLine(ecc.Message);
            }

        }

        #endregion

        #region COMMESSA WS

        public async Task<List<CommessaDTO>> CheckCommessaAsync(int userId, string commessa, string targa,string stato)
        {
            Uri uri = new Uri(string.Format(App.ApiUrl + App.ApiAppName + "/Users/ricerca-commessa", string.Empty));

            List<CommessaDTO> lstCommesse = null;
            string json = string.Empty;
            try
            {
                // metodo che controlla la scadenza del token
                await SetAuthToken(userId);


                if (!string.IsNullOrEmpty(commessa) && string.IsNullOrEmpty(targa))
                {
                    json = JsonConvert.SerializeObject(new CommessaRequestDTO { numeroCommessa = commessa, numeroTarga = string.Empty, Stato = stato });
                }
                if (!string.IsNullOrEmpty(targa) && string.IsNullOrEmpty(commessa))
                {
                    json = JsonConvert.SerializeObject(new CommessaRequestDTO { numeroCommessa = string.Empty, numeroTarga = targa, Stato = stato });
                }
                if (!string.IsNullOrEmpty(commessa) && !string.IsNullOrEmpty(targa))
                {
                    json = JsonConvert.SerializeObject(new CommessaRequestDTO { numeroCommessa = commessa, numeroTarga = targa, Stato = stato });
                }

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, uri)
                {
                    Content = content
                };

                var response = await _client.SendAsync(httpRequest);
                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        Device.BeginInvokeOnMainThread(() => MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.Commessa.NoFoundCommessa));
                        return lstCommesse;
                    }
                    else
                    {
                        var result = response.Content.ReadAsStringAsync().Result;
                        if (!string.IsNullOrEmpty(result))
                        {
                            lstCommesse = JsonConvert.DeserializeObject<List<CommessaDTO>>(result);
                            if (lstCommesse.Count > 1)
                            {
                                return lstCommesse;
                            }
                        }

                    }

                }
                
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnAuthorizedException();
                }
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    Device.BeginInvokeOnMainThread(() => MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.Commessa.UnknownApiCommessaCheck));
                    return lstCommesse;
                }
            }
            catch (UnAuthorizedException)
            {
                LogOutIfUnauthorized();
            }
            return lstCommesse;
        }

        #endregion


        #region INCONVENIENTE WS
    

        public async Task<List<NoteDTO>> GetNoteService (string numeroInconveniente)
        {
            List<NoteDTO> listNote = null;
            try
            {
                await SetAuthToken(0);
                Uri uri = new Uri(string.Format(App.ApiUrl + App.ApiAppName + "/Users/get-note", string.Empty));
                string json = string.Empty;
                if (!string.IsNullOrEmpty(numeroInconveniente))
                {
                    json = JsonConvert.SerializeObject(new NoteRequestDTO { NumeroInconveniente = numeroInconveniente });
                }
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, uri);
                httpRequest.Content = content;

                var response = await _client.SendAsync(httpRequest);
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    if (!string.IsNullOrEmpty(result))
                    {
                        listNote = JsonConvert.DeserializeObject<List<NoteDTO>>(result);
                        if (listNote.Count > 1)
                        {
                            return listNote;
                        }
                    }
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnAuthorizedException();
                }
            }
            catch (UnAuthorizedException)
            {
                LogOutIfUnauthorized();
            }
            return listNote;
        }

        public async Task<bool> InsertNota(string numeroInconveniente, string testoNota)
        {
            bool ret = false;
            try
            {
                await SetAuthToken(0);
                Uri uri = new Uri(string.Format(App.ApiUrl + App.ApiAppName + "/Users/crea-nota", string.Empty));
                string json = string.Empty;
                if (!string.IsNullOrEmpty(numeroInconveniente))
                {
                    json = JsonConvert.SerializeObject(new RequestInsertNotaDTO { NumeroInconveniente = numeroInconveniente ,TestoNota = testoNota });
                }
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, uri);
                httpRequest.Content = content;

                var response = await _client.SendAsync(httpRequest);
                if (response.IsSuccessStatusCode)
                {
                    ret = true;
                    var result = response.Content.ReadAsStringAsync().Result;
                    
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnAuthorizedException();
                }
            }
            catch (UnAuthorizedException)
            {
                LogOutIfUnauthorized();
            }

            return ret;
        }

        public async Task<int> CheckDocumentAsync(string numeroInconveniente)
        {
            int ret = 0;
            try
            {
                await SetAuthToken(0);
                Uri uri = new Uri(string.Format(App.ApiUrl + App.ApiAppName + "/Users/numero-documenti", string.Empty));
                string json = string.Empty;
                if (!string.IsNullOrEmpty(numeroInconveniente))
                {
                    json = JsonConvert.SerializeObject(new InconvenienteWSDTO { NumeroInconveniente = numeroInconveniente});
                }
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, uri);
                httpRequest.Content = content;

                var response = await _client.SendAsync(httpRequest);
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    if (!string.IsNullOrEmpty(result))
                    {
                        ret = Convert.ToInt32(result);
                    }
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnAuthorizedException();
                }
            }
            catch (UnAuthorizedException)
            {
                LogOutIfUnauthorized();
            }

            return ret;
        }


        public async Task<bool> UploadPhotoForInconveniente(List<UploadPhotoInconvenienteDTO> lstPhoto)
        {
            // metodo che controlla la scadenza del token
            await SetAuthToken(0);


            Uri uri = new Uri(string.Format(App.ApiUrl + App.ApiAppName + "/Users/carica-foto", string.Empty));
            string json = string.Empty;
            try
            {
                if (lstPhoto != null && lstPhoto.Count > 0)
                {
                    json = JsonConvert.SerializeObject(lstPhoto);
                }
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, uri);

                if (!string.IsNullOrEmpty(json))
                {
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    httpRequest.Content = content;
                    var response = await _client.SendAsync(httpRequest);
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        throw new UnAuthorizedException();
                    }
                }


            }
            catch (UnAuthorizedException)
            {
                LogOutIfUnauthorized();
            }
            return false;
        }
       
        //Gestione foto (Caricamento e presa)
        public async Task<List<PhotoInconvenienteDTO>> GetPhotoForInconvenienteAsync(string numeroInconveniente, bool extra)
        {
            List<PhotoInconvenienteDTO> lstStringBase64 = new List<PhotoInconvenienteDTO>();
            try
            {
                await SetAuthToken(0);
                Uri uri = new Uri(string.Format(App.ApiUrl + App.ApiAppName + "/Users/get-foto", string.Empty));

                string json = string.Empty;
                if (!string.IsNullOrEmpty(numeroInconveniente))
                {
                    json = JsonConvert.SerializeObject(new InconvenienteWSDTO { NumeroInconveniente = numeroInconveniente, Extra = extra });
                }
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, uri);
                httpRequest.Content = content;

                var response = await _client.SendAsync(httpRequest);
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    if (!string.IsNullOrEmpty(result))
                    {
                        lstStringBase64 = JsonConvert.DeserializeObject<List<PhotoInconvenienteDTO>>(result);
                        if (lstStringBase64.Count > 1)
                        {
                            return lstStringBase64;
                        }
                    }
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnAuthorizedException();
                }
            }
            catch (UnAuthorizedException)
            {
                LogOutIfUnauthorized();
            }
            return lstStringBase64;

        }

        public async Task<bool> changeStatusCommessa(string numCommessa)
        {
            await SetAuthToken(0);


            Uri uri = new Uri(string.Format(App.ApiUrl + App.ApiAppName + "/Users/chiudi-commessa", string.Empty));
            string json = string.Empty;
            try
            {

                json = JsonConvert.SerializeObject(new CommessaClosedDTO { numeroCommessa = numCommessa });

                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, uri);

                if (!string.IsNullOrEmpty(json))
                {
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    httpRequest.Content = content;
                    var response = await _client.SendAsync(httpRequest);
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        throw new UnAuthorizedException();
                    }
                }
            }
            catch (UnAuthorizedException)
            {
                LogOutIfUnauthorized();
            }
            return false;
        }

        public async Task<List<PhotoInconvenienteDTO>> PreviewPhotoForInconvenienteAsync(string numeroInconveniente, bool extra)
        {
            List<PhotoInconvenienteDTO> lstStringBase64 = new List<PhotoInconvenienteDTO>();
            try
            {
                await SetAuthToken(0);
                Uri uri = new Uri(string.Format(App.ApiUrl + App.ApiAppName + "/Users/anteprima-foto", string.Empty));

                string json = string.Empty;
                if (!string.IsNullOrEmpty(numeroInconveniente))
                {
                    json = JsonConvert.SerializeObject(new InconvenienteWSDTO { NumeroInconveniente = numeroInconveniente, Extra = extra });
                }

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, uri);
                httpRequest.Content = content;

                var response = await _client.SendAsync(httpRequest);
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    if (!string.IsNullOrEmpty(result))
                    {
                        lstStringBase64 = JsonConvert.DeserializeObject<List<PhotoInconvenienteDTO>>(result);
                        if (lstStringBase64.Count > 1)
                        {
                            return lstStringBase64;
                        }
                    }
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnAuthorizedException();
                }
                else
                {
                    return lstStringBase64;
                }

            }
            catch (UnAuthorizedException)
            {
                LogOutIfUnauthorized();
            }
            return lstStringBase64;

        }


        public async Task<List<InconvenienteDTO>> CheckInconvenienteForCommessa(string idCommessa)
        {
            // metodo che controlla la scadenza del token
            await SetAuthToken(0);

            Uri uri = new Uri(string.Format(App.ApiUrl + App.ApiAppName + "/Users/get-inconvenienti", string.Empty));
            List<InconvenienteDTO> lstInconvenienti = null;
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(idCommessa))
                {
                    json = JsonConvert.SerializeObject(new InconvenienteRequest { NumeroCommessa = idCommessa });
                }
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, uri);
                httpRequest.Content = content;
                var response = await _client.SendAsync(httpRequest);
                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        Device.BeginInvokeOnMainThread(() => MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.Inconveniente.NoFoundInconveniente));
                    }
                    else
                    {
                        var result = response.Content.ReadAsStringAsync().Result;
                        if (!string.IsNullOrEmpty(result))
                        {
                            lstInconvenienti = JsonConvert.DeserializeObject<List<InconvenienteDTO>>(result);
                        }
                    }
                    
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnAuthorizedException();
                }
              
                if (response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    Device.BeginInvokeOnMainThread(() => MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, "Errore nella chiamata della ricerca inconvenienti ,riprovare e se il problema sussiste contattare l'assistenza"));
                }

            }
            catch (UnAuthorizedException)
            {
                LogOutIfUnauthorized();
            }
            return lstInconvenienti;
        }

     



        public async Task<bool> closeInconveniente(string numeroInconveniente, bool extra){
            // metodo che controlla la scadenza del token
            await SetAuthToken(0);
            Uri uri = new Uri(string.Format(App.ApiUrl + App.ApiAppName + "/Users/chiudi-inconveniente", string.Empty));
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(numeroInconveniente))
                {
                    json = JsonConvert.SerializeObject(new InconvenienteWSDTO { NumeroInconveniente = numeroInconveniente, Extra = extra });
                }

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, uri);
                httpRequest.Content = content;
                var response = await _client.SendAsync(httpRequest);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnAuthorizedException();
                }
                else
                {
                    MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, "Ci sono problemi nella chiusura dell'inconveniente, la chiamata alla chiusura ha dato esito negativo");
                    return false;
                }
            }
            catch (UnAuthorizedException)
            {
                LogOutIfUnauthorized();
            }
            return false;
        }

        public async Task<bool> setInconvenienteExtra(string idCommessa, string titolo, string descrizione, List<string> photo)
        {
            // metodo che controlla la scadenza del token
            await SetAuthToken(0);
            //_client.SetBearerToken();

            Uri uri = new Uri(string.Format(App.ApiUrl + App.ApiAppName + "/Users/crea-extra", string.Empty));
            string json = string.Empty;
            try
            {
                json = JsonConvert.SerializeObject(new InconvenienteExtraRequest { NumeroCommessa = idCommessa, Titolo = titolo, Descrizione = descrizione, Base64Foto = photo });
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, uri);
                httpRequest.Content = content;
                var response = await _client.SendAsync(httpRequest);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnAuthorizedException();
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() => MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, "Problemi nella chiamata al servizio per l'inserimento dell'inconveniente extra! Controllare i dati e riprovare!"));
                    return false;
                }
            }
            catch (UnAuthorizedException)
            {
                LogOutIfUnauthorized();
            }
            return false;
        }


        public async Task<bool> UpdateNoteInconveniente(string numInconveniente, bool extra, string note)
        {
            // metodo che controlla la scadenza del token
            await SetAuthToken(0);
            //_client.SetBearerToken();

            Uri uri = new Uri(string.Format(App.ApiUrl + App.ApiAppName + "/Users/aggiorna-note", string.Empty));
            string json = string.Empty;
            try
            {
                json = JsonConvert.SerializeObject(new InconvenienteNoteDTO { NumeroInconveniente = numInconveniente, Extra = extra, Note = note });

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, uri);
                httpRequest.Content = content;
                var response = await _client.SendAsync(httpRequest);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnAuthorizedException();
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() => MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, Messages.UnhandledExceptionError));
                    return false;
                }
            }
            catch (UnAuthorizedException)
            {
                LogOutIfUnauthorized();
            }

            return false;
        }

        public async Task<LavorazioneInconvenienteDTO> StartInconveniente(string numeroInconveniente, bool extra,DateTime dataInizio)
        {
            // metodo che controlla la scadenza del token
            await SetAuthToken(0);
            //_client.SetBearerToken();

            Uri uri = new Uri(string.Format(App.ApiUrl + App.ApiAppName + "/Users/start-inconveniente", string.Empty));
            string json = string.Empty;
            LavorazioneInconvenienteDTO lavorazioneInconvenienteDTO = null;
            try
            {
                json = JsonConvert.SerializeObject(new InconvenienteWSDTO { NumeroInconveniente = numeroInconveniente, Extra = extra, DataInizioLavoro= dataInizio });

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, uri);
                httpRequest.Content = content;
                var response = await _client.SendAsync(httpRequest);
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    if (!string.IsNullOrEmpty(result))
                    {
                        lavorazioneInconvenienteDTO = JsonConvert.DeserializeObject<LavorazioneInconvenienteDTO>(result);
                    }
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnAuthorizedException();
                }
                if (response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    Device.BeginInvokeOnMainThread(() => MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, "Errore nella chiamata dell'inizio lavorazione ,riprovare e se il problema sussiste contattare l'assistenza"));
                }
            }
            catch (UnAuthorizedException)
            {
                LogOutIfUnauthorized();
            }
            return lavorazioneInconvenienteDTO;
        }

        public async Task<bool> SendTimeWorking(string numeroInconveniente, bool extra, DateTime StartWorking, DateTime endWorking, string note,int idMarcatempo)
        {
            // metodo che controlla la scadenza del token
            await SetAuthToken(0);
            bool ret = false;
            Uri uri = new Uri(string.Format(App.ApiUrl + App.ApiAppName + "/Users/invia-marcatempo", string.Empty));
            string json = string.Empty;
            try
            {
                json = JsonConvert.SerializeObject(new InconvenienteInvioTempoDTO { NumeroInconveniente = numeroInconveniente, Extra = extra, DataInizioLavoro = StartWorking, DataFineLavoro = endWorking, Note = note, IdMarcatempo = idMarcatempo });

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, uri);
                httpRequest.Content = content;
                var response = await _client.SendAsync(httpRequest);
                if (response.IsSuccessStatusCode){
                    ret= true;
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized){
                    throw new UnAuthorizedException();
                }
                if(response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    Device.BeginInvokeOnMainThread(() => MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, "Errore nella chiamata dell'invio tempo lavorazione ,riprovare e se il problema sussiste contattare l'assistenza"));
                    ret = false;
                }
            }
            catch (UnAuthorizedException)
            {
                LogOutIfUnauthorized();
            }
            return ret;
        }


        public async Task<bool> waitRicambiInconveniente(string numeroInconveniente, bool extra)
        {
            // metodo che controlla la scadenza del token
            await SetAuthToken(0);
            //_client.SetBearerToken();
            bool ret = false;

            Uri uri = new Uri(string.Format(App.ApiUrl + App.ApiAppName + "/Users/attesa-inconveniente", string.Empty));
            string json = string.Empty;
            try
            {
                json = JsonConvert.SerializeObject(new InconvenienteWSDTO { NumeroInconveniente = numeroInconveniente, Extra = extra });

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, uri);
                httpRequest.Content = content;
                var response = await _client.SendAsync(httpRequest);
                if (response.IsSuccessStatusCode)
                {
                    ret = true;
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnAuthorizedException();
                }
                if (response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    Device.BeginInvokeOnMainThread(() => MessagingCenter.Send<string, string>(MessagingCenterEvents.Subscriber, MessagingCenterEvents.AlertError, "Errore nella chiamata attesa ricambi ,riprovare e se il problema sussiste contattare l'assistenza"));
                    ret = false;
                }

            }
            catch (UnAuthorizedException)
            {
                LogOutIfUnauthorized();
            }
            return ret;
        }
        #endregion

    }
}
