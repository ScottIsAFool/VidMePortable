using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VidMePortable.Model;
using VidMePortable.Model.Responses;

namespace VidMePortable
{
    public class VidMeClient
    {
        private const string BaseUrl = "https://api.vid.me/";
        private readonly HttpClient _httpClient;

        public VidMeClient()
        {
            _httpClient = new HttpClient(new HttpClientHandler {AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip});
        }

        #region Auth Methods

        public async Task<AuthResponse> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default(CancellationToken))
        {
            return new AuthResponse();
        }

        #endregion

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

        private static string CreateUrl(string method, string options = "")
        {
            return string.Format("{0}{1}?{2}", BaseUrl, method, options);
        }
    }
}
