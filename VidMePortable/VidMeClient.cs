using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public string DeviceName { get; private set; }
        public string Platform { get; private set; }

        public VidMeClient()
            : this(string.Empty, string.Empty) { }

        public VidMeClient(string deviceName, string platform)
        {
            DeviceName = deviceName;
            Platform = platform;
            _httpClient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip });
        }

        #region Auth Methods

        [Obsolete("This method will work, but the oauth way is preferred")]
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

        public string GetAuthUrl(string clientId, string redirectUrl, List<Scope> scopes)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentNullException("clientId", "Client ID cannot be null or empty");
            }

            if (scopes.IsNullOrEmpty())
            {
                throw new ArgumentNullException("scopes", "Scopes must be provided");
            }

            var scopesString = string.Join(" ", scopes.Select(x => x.GetDescription()));

            return string.Format("https://vid.me/oauth/authorize?client_id={0}&redirect_uri={1}&scope={2}&response_type=token", clientId, redirectUrl, scopesString);
        }

        public async Task<AuthResponse> ExchangeCodeForTokenAsync(string code, string clientId, string clientSecret, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException("code", "code cannot be null or empty");
            }

            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentNullException("clientId", "Client ID cannot be null or empty");
            }

            if (string.IsNullOrEmpty(clientSecret))
            {
                throw new ArgumentNullException("clientSecret", "Client Secret cannot be null or empty");
            }

            var postData = new Dictionary<string, string>
            {
                {"grant_type", "client_credentials"},
                {"client_id", clientId},
                {"client_secret", clientSecret},
                {"code", code}
            };

            var response = await Post<AuthResponse>(postData, "oauth/token", cancellationToken);

            if (response != null)
            {
                SetAuthentication(response.Auth);
            }

            return response;
        }

        public void SetAuthentication(Auth authenticationInfo)
        {
            AuthenticationInfo = authenticationInfo;
        }

        #endregion

        #region Channel Methods

        public async Task<Channel> GetChannelAsync(string channelId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(channelId))
            {
                throw new ArgumentNullException("channelId", "Channel ID cannot be null or empty");
            }

            var method = string.Format("channel/{0}", channelId);

            var response = await Get<ChannelResponse>(method, cancellationToken: cancellationToken);

            return response != null ? response.Channel : null;
        }

        public async Task<bool> FollowChannelAsync(string channelId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(channelId))
            {
                throw new ArgumentNullException("channelId", "Channel ID cannot be null or empty");
            }

            var postData = CreatePostData();
            var method = string.Format("channel/{0}/follow", channelId);

            var response = await Post<Response>(postData, method, cancellationToken);

            return response != null && response.Status;
        }

        public async Task<VideosResponse> GetChannelsHotVideosAsync(string channelId, int? offset = null, int? limit = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(channelId))
            {
                throw new ArgumentNullException("channelId", "Channel ID cannot be null or empty");
            }

            var method = string.Format("channel/{0}/hot", channelId);
            var options = new Dictionary<string, string>();
            options.AddIfNotNull("offset", offset);
            options.AddIfNotNull("limit", limit);

            var response = await Get<VideosResponse>(method, options.ToQueryString(), cancellationToken);
            return response;
        }

        public async Task<VideosResponse> GetChannelsNewVideosAsync(string channelId, int? offset = null, int? limit = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(channelId))
            {
                throw new ArgumentNullException("channelId", "Channel ID cannot be null or empty");
            }

            var method = string.Format("channel/{0}/new", channelId);
            var options = new Dictionary<string, string>();
            options.AddIfNotNull("offset", offset);
            options.AddIfNotNull("limit", limit);

            var response = await Get<VideosResponse>(method, options.ToQueryString(), cancellationToken);
            return response;
        }

        public string GetChannelUrl(string channelId)
        {
            return CreateUrl(string.Format("channel/{0}/url", channelId));
        }

        public async Task<bool> UnFollowChannelAsync(string channelId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(channelId))
            {
                throw new ArgumentNullException("channelId", "Channel ID cannot be null or empty");
            }

            var postData = CreatePostData();
            var method = string.Format("channel/{0}/unfollow", channelId);

            var response = await Post<Response>(postData, method, cancellationToken);

            return response != null && response.Status;
        }

        public async Task<List<Channel>> ListChannelsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await Get<ChannelsResponse>("channels", cancellationToken: cancellationToken);
            if (response != null)
            {
                return response.Channels ?? new List<Channel>();
            }

            return new List<Channel>();
        }

        public async Task<List<Channel>> ListSuggestedChannelsAsync(string text = null, int? number = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var options = new Dictionary<string, string>();
            options.AddIfNotNull("text", text);
            options.AddIfNotNull("number", number);

            var response = await Get<ChannelsResponse>("channels/suggest", options.ToQueryString(), cancellationToken);
            if (response != null)
            {
                return response.Channels ?? new List<Channel>();
            }

            return new List<Channel>();
        }

        #endregion

        #region Comment Methods

        public async Task<Comment> CreateCommentAsync(string videoId, string commentText, TimeSpan timeOfComment, string inReplyToCommentId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(videoId))
            {
                throw new ArgumentNullException("videoId", "Video ID cannot be null or empty");
            }

            if (string.IsNullOrEmpty(commentText))
            {
                throw new ArgumentNullException("commentText", "Empty comments are not allowed");
            }

            if (timeOfComment == TimeSpan.MinValue)
            {
                timeOfComment = TimeSpan.FromSeconds(0);
            }

            var postData = CreatePostData();
            postData.AddIfNotNull("video", videoId);
            postData.AddIfNotNull("comment", inReplyToCommentId);
            postData.AddIfNotNull("body", commentText);
            postData.AddIfNotNull("at", timeOfComment.TotalSeconds);

            var response = await Post<CommentResponse>(postData, "comment/create", cancellationToken);

            return response != null ? response.Comment : null;
        }

        public async Task<bool> DeleteCommentAsync(string commentId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(commentId))
            {
                throw new ArgumentNullException("commentId", "Comment ID cannot be null or empty");
            }

            var postData = CreatePostData();
            var method = string.Format("comment/{0}/delete", commentId);

            var response = await Post<Response>(postData, method, cancellationToken);
            return response != null && response.Status;
        }

        public async Task<Comment> GetCommentAsync(string commentId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(commentId))
            {
                throw new ArgumentNullException("commentId", "Comment ID cannot be null or empty");
            }

            var method = string.Format("comment/{0}", commentId);
            var response = await Get<CommentResponse>(method, cancellationToken: cancellationToken);

            return response != null ? response.Comment : null;
        }

        public async Task<List<Comment>> GetCommentsAsync(string videoId, SortDirection? sortDirection, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(videoId))
            {
                throw new ArgumentNullException("videoId", "Video ID cannot be null or empty");
            }

            var options = new Dictionary<string, string>();
            options.AddIfNotNull("video", videoId);
            options.AddIfNotNull("direction", sortDirection.GetDescription());
            
            var response = await Get<CommentsResponse>("comments/list", options.ToQueryString(), cancellationToken);

            if (response != null)
            {
                return response.Comments ?? new List<Comment>();
            }

            return new List<Comment>();
        }

        public string GetCommentUrl(string commentId)
        {
            if (string.IsNullOrEmpty(commentId))
            {
                throw new ArgumentNullException("commentId", "Comment ID cannot be null or empty");
            }

            return CreateUrl(string.Format("comment/{0}/url", commentId));
        }

        public async Task<Comment> VoteCommentAsync(string commentId, Vote vote, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(commentId))
            {
                throw new ArgumentNullException("commentId", "Comment ID cannot be null or empty");
            }

            var postData = CreatePostData();
            postData.AddIfNotNull("value", vote.GetDescription());

            var method = string.Format("comment/{0}/vote", commentId);

            var response = await Post<CommentResponse>(postData, method, cancellationToken);

            return response != null ? response.Comment : null;
        }

        #endregion

        #region GeoFences Methods

        public async Task<List<Geofence>> GetGeoFencesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await Get<GeoFencesResponse>("geofences", cancellationToken: cancellationToken);
            if (response != null)
            {
                return response.Geofences ?? new List<Geofence>();
            }

            return new List<Geofence>();
        }

        public async Task<List<Geofence>> SuggestGeoFencesAsync(string searchText = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var options = new Dictionary<string, string>();
            options.AddIfNotNull("text", searchText);

            var response = await Get<GeoFencesResponse>("geofences/suggest", options.ToQueryString(), cancellationToken);
            if (response != null)
            {
                return response.Geofences ?? new List<Geofence>();
            }

            return new List<Geofence>();
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

        public async Task<bool> FollowUserAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId", "User ID cannot be null or empty");
            }

            var postData = CreatePostData();

            var method = string.Format("user/{0}/follow", userId);

            var response = await Post<Response>(postData, method, cancellationToken);

            return response != null && response.Status;
        }

        public async Task<List<Channel>> GetUsersFollowedChannelsAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId", "User ID cannot be null or empty");
            }

            var postData = CreatePostData();
            var method = string.Format("user/{0}/follows-channels", userId);

            var response = await Post<ChannelsResponse>(postData, method, cancellationToken);

            if (response != null)
            {
                return response.Channels ?? new List<Channel>();
            }

            return new List<Channel>();
        }

        public async Task<bool> RemoveAvatarAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId", "User ID cannot be null or empty");
            }

            var postData = CreatePostData();
            var method = string.Format("user/{0}/avatar/remove", userId);

            var response = await Post<AuthResponse>(postData, method, cancellationToken);

            return response != null && response.Status;
        }

        public async Task<bool> UnfollowUserAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId", "User ID cannot be null or empty");
            }

            var postData = CreatePostData();
            var method = string.Format("user/{0}/unfollow", userId);

            var response = await Post<Response>(postData, method, cancellationToken);

            return response != null && response.Status;
        }

        public async Task<User> UpdateAvatarAsync(string userId, Stream imageStream, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId", "User ID cannot be null or empty");
            }

            using (var memoryStream = new MemoryStream())
            {
                await imageStream.CopyToAsync(memoryStream);
                return await UpdateAvatarAsync(userId, memoryStream.ToArray(), cancellationToken);
            }
        }

        public async Task<User> UpdateAvatarAsync(string userId, byte[] imageStream, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId", "User ID cannot be null or empty");
            }

            var postData = CreatePostData();
            var imageData = Convert.ToBase64String(imageStream);
            postData.Add("filedata", imageData);

            var method = string.Format("user/{0}/avatar/update", userId);

            var response = await Post<UserResponse>(postData, method, cancellationToken);

            return response != null ? response.User : null;
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
            CheckExpirationDateIsOk();

            var postData = new Dictionary<string, string>
            {
                {"token", AuthenticationInfo.Token}
            };
            postData.AddIfNotNull("DEVICE", DeviceName);
            postData.AddIfNotNull("PLATFORM", Platform);

            return postData;
        }
    }
}
