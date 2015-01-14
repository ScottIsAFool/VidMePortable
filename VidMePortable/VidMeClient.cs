using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VidMePortable.Extensions;
using VidMePortable.Model;
using VidMePortable.Model.Responses;

namespace VidMePortable
{
    public class VidMeClient
    {
        private const string BaseUrl = "https://api.vid.me/";
        private readonly HttpClient _httpClient;

        public Auth AuthenticationInfo { get; private set; }

        public VidMeClient()
        {
            _httpClient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip });
        }

        #region Auth Methods

        public async Task<AuthResponse> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException("username", "username cannot be null or empty");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("password", "password cannot be null or empty");
            }

            var postData = new Dictionary<string, string>
            {
                {"username", username},
                {"password", password}
            };

            var response = await Post<AuthResponse>(postData, "auth/create", cancellationToken);
            if (response != null)
            {
                SetAuthentication(response.Auth);
            }

            return response;
        }

        public async Task<AuthResponse> CheckAuthTokenAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (AuthenticationInfo == null)
            {
                throw new VidMeException(HttpStatusCode.Unauthorized, "No AuthenticationInfo set");
            }

            var postData = CreatePostData();
            var response = await Post<AuthResponse>(postData, "auth/check", cancellationToken);

            if (response != null)
            {
                SetAuthentication(response.Auth);
            }

            return response;
        }

        public async Task<bool> DeleteAuthTokenAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (AuthenticationInfo == null)
            {
                throw new VidMeException(HttpStatusCode.Unauthorized, "No AuthenticationInfo set");
            }

            var postData = CreatePostData();
            var response = await Post<Response>(postData, "auth/delete", cancellationToken);

            return response != null && response.Status;
        }

        public void SetAuthentication(Auth authenticationInfo)
        {
            AuthenticationInfo = authenticationInfo;
        }

        #endregion

        #region User Methods

        public string GetUserAvatar(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId", "User ID cannot be null or empty");
            }

            var method = string.Format("user/{0}/avatar", userId);

            return CreateUrl(method);
        }

        public async Task<AuthResponse> CreateUserAsync(string username, string password, string email = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException("username", "username cannot be null or empty");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("password", "password cannot be null or empty");
            }

            var postData = new Dictionary<string, string>
            {
                {"username", username},
                {"password", password}
            };

            postData.AddIfNotNull("email", email);

            var response = await Post<AuthResponse>(postData, "user/create", cancellationToken);
            if (response != null)
            {
                SetAuthentication(response.Auth);
            }

            return response;
        }

        public async Task<User> GetUserAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId", "User Id cannot be null or empty");
            }

            var method = string.Format("user/{0}", userId);

            var response = await Post<UserResponse>(new Dictionary<string, string>(), method, cancellationToken);
            return response != null ? response.User : null;
        }

        public async Task<AuthResponse> EditUserAsync(string userId, string username = null, string currentPassword = null, string newPassword = null, string email = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId", "User Id cannot be null or empty");
            }

            var postData = CreatePostData();

            postData.AddIfNotNull("username", username);
            postData.AddIfNotNull("email", email);
            postData.AddIfNotNull("password", newPassword);
            postData.AddIfNotNull("passwordCurrent", currentPassword);
            postData.AddIfNotNull("bio", "");

            postData.AddIfNotNull("email", email);

            var response = await Post<AuthResponse>(postData, "user/edit", cancellationToken);
            if (response != null)
            {
                SetAuthentication(response.Auth);
            }

            return response;
        }

        #endregion

        #region API Call methods
        private async Task<TReturnType> Post<TReturnType>(Dictionary<string, string> postData, string method, CancellationToken cancellationToken = default(CancellationToken))
        {
            var url = CreateUrl(method);

            var response = await _httpClient.PostAsync(url, new FormUrlEncodedContent(postData), cancellationToken);

            return await HandleResponse<TReturnType>(response);
        }

        private async Task<TReturnType> Get<TReturnType>(string method, string options = "", CancellationToken cancellationToken = default(CancellationToken))
        {
            var url = CreateUrl(method, options);

            var response = await _httpClient.GetAsync(url, cancellationToken);

            return await HandleResponse<TReturnType>(response);
        }

        private static async Task<TReturnType> HandleResponse<TReturnType>(HttpResponseMessage response)
        {
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var returnItem = JsonConvert.DeserializeObject<TReturnType>(responseString);
                return returnItem;
            }

            var error = JsonConvert.DeserializeObject<ErrorResponse>(responseString);
            throw new VidMeException(response.StatusCode, error.Error);
        }
        #endregion

        private void CheckExpirationDateIsOk()
        {
            var now = DateTime.Now;
            if (AuthenticationInfo == null || AuthenticationInfo.Expires < now)
            {
                throw new VidMeException(HttpStatusCode.Unauthorized, "No valid AuthenticationInfo set");
            }
        }

        private static string CreateUrl(string method, string options = "")
        {
            return string.Format("{0}{1}?{2}", BaseUrl, method, options);
        }

        private Dictionary<string, string> CreatePostData()
        {
            return new Dictionary<string, string>
            {
                {"token", AuthenticationInfo.Token}
            };
        }
    }
}
