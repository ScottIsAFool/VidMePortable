using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VidMePortable.Extensions;
using VidMePortable.Model;
using VidMePortable.Model.Requests;
using VidMePortable.Model.Responses;

namespace VidMePortable
{
    public class VidMeClient : IVidMeClient
    {
        private const string BaseUrl = "https://api.vid.me/";
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Gets the authentication information.
        /// </summary>
        /// <value>
        /// The authentication information.
        /// </value>
        public Auth AuthenticationInfo { get; private set; }
        /// <summary>
        /// Gets the device identifier.
        /// </summary>
        /// <value>
        /// The device identifier.
        /// </value>
        public string DeviceId { get; private set; }
        /// <summary>
        /// Gets the platform.
        /// </summary>
        /// <value>
        /// The platform.
        /// </value>
        public string Platform { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VidMeClient"/> class.
        /// </summary>
        public VidMeClient()
            : this(string.Empty, string.Empty) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="VidMeClient"/> class.
        /// </summary>
        /// <param name="deviceName">Name of the device.</param>
        /// <param name="platform">The platform.</param>
        public VidMeClient(string deviceName, string platform)
        {
            DeviceId = deviceName;
            Platform = platform;
            _httpClient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip });
        }

        /// <summary>
        /// Sets the device name and platform.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="platform">The platform.</param>
        public void SetDeviceNameAndPlatform(string deviceId, string platform)
        {
            DeviceId = deviceId;
            Platform = platform;
        }

        #region Auth Methods

        /// <summary>
        /// Authenticates the.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// username;username cannot be null or empty
        /// or
        /// password;password cannot be null or empty
        /// </exception>
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
                {"password", password},
                {"remember", "1"},
                {"nocookie", "1"}
            };

            var response = await Post<AuthResponse>(postData, "auth/create", cancellationToken);
            if (response != null)
            {
                SetAuthentication(response.Auth);
            }

            return response;
        }

        /// <summary>
        /// Checks the authentication token.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="VidMeException">No AuthenticationInfo set</exception>
        public async Task<AuthResponse> CheckAuthTokenAsync(string token, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException("token", "You must provide a token");
            }

            var postData = new Dictionary<string, string> { { "token", token } };
            var response = await Post<AuthResponse>(postData, "auth/check", cancellationToken);

            if (response != null)
            {
                SetAuthentication(response.Auth);
            }

            return response;
        }

        /// <summary>
        /// Deletes the authentication token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="VidMeException">No AuthenticationInfo set</exception>
        public async Task<bool> DeleteAuthTokenAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (AuthenticationInfo == null)
            {
                throw new VidMeException(HttpStatusCode.Unauthorized, new ErrorResponse { Error = "No AuthenticationInfo set" });
            }

            var postData = await CreatePostData();
            var response = await Post<Response>(postData, "auth/delete", cancellationToken);

            return response != null && response.Status;
        }

        /// <summary>
        /// Gets the authentication URL.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <param name="scopes">The scopes.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// clientId;Client ID cannot be null or empty
        /// or
        /// scopes;Scopes must be provided
        /// </exception>
        public string GetAuthUrl(string clientId, string redirectUrl, List<Scope> scopes, AuthType type = AuthType.Code)
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

            return string.Format("https://vid.me/oauth/authorize?client_id={0}&redirect_uri={1}&scope={2}&response_type={3}", clientId, redirectUrl, scopesString, type.ToString().ToLower());
        }

        /// <summary>
        /// Exchanges the code for token.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// code;code cannot be null or empty
        /// or
        /// clientId;Client ID cannot be null or empty
        /// or
        /// clientSecret;Client Secret cannot be null or empty
        /// </exception>
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

        /// <summary>
        /// Sets the authentication.
        /// </summary>
        /// <param name="authenticationInfo">The authentication information.</param>
        public void SetAuthentication(Auth authenticationInfo)
        {
            AuthenticationInfo = authenticationInfo;
        }

        #endregion

        #region Channel Methods

        /// <summary>
        /// Determines whether [is user following channel] [the specified channel identifier].
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="otherUser">The other user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">channelId;A base channel ID must be provided</exception>
        public async Task<bool> IsUserFollowingChannelAsync(string channelId, string otherUser = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(channelId))
            {
                throw new ArgumentNullException("channelId", "A base channel ID must be provided");
            }

            var options = await CreatePostData(false);
            var method = string.Format("channel/{0}/follow/{1}", channelId, otherUser);

            var response = await Get<FollowResponse>(method, options.ToQueryString(), cancellationToken);
            return response != null && response.IsFollowing;
        }

        /// <summary>
        /// Gets the channel.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">channelId;Channel ID cannot be null or empty</exception>
        public async Task<ChannelResponse> GetChannelAsync(string channelId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(channelId))
            {
                throw new ArgumentNullException("channelId", "Channel ID cannot be null or empty");
            }

            var options = await CreatePostData(false);
            var method = string.Format("channel/{0}", channelId);

            var response = await Get<ChannelResponse>(method, options.ToQueryString(), cancellationToken);
            return response;
        }

        /// <summary>
        /// Gets the channels.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<List<Channel>> GetChannelsAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var response = await Get<ChannelsResponse>("channels/list", cancellationToken: cancellationToken);

            if (response != null)
            {
                return response.Channels ?? new List<Channel>();
            }

            return new List<Channel>();
        }

        /// <summary>
        /// Follows the channel.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">channelId;Channel ID cannot be null or empty</exception>
        public async Task<bool> FollowChannelAsync(string channelId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(channelId))
            {
                throw new ArgumentNullException("channelId", "Channel ID cannot be null or empty");
            }

            var postData = await CreatePostData();
            var method = string.Format("channel/{0}/follow", channelId);

            var response = await Post<Response>(postData, method, cancellationToken);

            return response != null && response.Status;
        }

        /// <summary>
        /// Gets the channels hot videos.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">channelId;Channel ID cannot be null or empty</exception>
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

        /// <summary>
        /// Gets the channels new videos.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">channelId;Channel ID cannot be null or empty</exception>
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

        /// <summary>
        /// Gets the channel URL.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <returns></returns>
        public string GetChannelUrl(string channelId)
        {
            return CreateUrl(string.Format("channel/{0}/url", channelId));
        }

        /// <summary>
        /// Uns the follow channel.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">channelId;Channel ID cannot be null or empty</exception>
        public async Task<bool> UnFollowChannelAsync(string channelId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(channelId))
            {
                throw new ArgumentNullException("channelId", "Channel ID cannot be null or empty");
            }

            var postData = await CreatePostData();
            var method = string.Format("channel/{0}/unfollow", channelId);

            var response = await Post<Response>(postData, method, cancellationToken);

            return response != null && response.Status;
        }

        /// <summary>
        /// Lists the channels.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<List<Channel>> ListChannelsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await Get<ChannelsResponse>("channels", cancellationToken: cancellationToken);
            if (response != null)
            {
                return response.Channels ?? new List<Channel>();
            }

            return new List<Channel>();
        }

        /// <summary>
        /// Lists the suggested channels.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="number">The number.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the channel moderators.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">channelId;Channel ID cannot be null or empty</exception>
        public async Task<UsersResponse> GetChannelModeratorsAsync(string channelId, int? offset = null, int? limit = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(channelId))
            {
                throw new ArgumentNullException("channelId", "Channel ID cannot be null or empty");
            }

            var options = new Dictionary<string, string>();
            options.AddIfNotNull("offset", offset);
            options.AddIfNotNull("limit", limit);

            var method = string.Format("channel/{0}/moderators", channelId);

            var response = await Get<UsersResponse>(method, options.ToQueryString(), cancellationToken);
            return response;
        }

        #endregion

        #region Comment Methods

        /// <summary>
        /// Creates the comment.
        /// </summary>
        /// <param name="videoId">The video identifier.</param>
        /// <param name="commentText">The comment text.</param>
        /// <param name="timeOfComment">The time of comment.</param>
        /// <param name="inReplyToCommentId">The in reply to comment identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// videoId;Video ID cannot be null or empty
        /// or
        /// commentText;Empty comments are not allowed
        /// </exception>
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

            var postData = await CreatePostData();
            postData.AddIfNotNull("video", videoId);
            postData.AddIfNotNull("comment", inReplyToCommentId);
            postData.AddIfNotNull("body", commentText);
            postData.AddIfNotNull("at", timeOfComment.TotalSeconds);

            var response = await Post<CommentResponse>(postData, "comment/create", cancellationToken);

            return response != null ? response.Comment : null;
        }

        /// <summary>
        /// Deletes the comment.
        /// </summary>
        /// <param name="commentId">The comment identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">commentId;Comment ID cannot be null or empty</exception>
        public async Task<bool> DeleteCommentAsync(string commentId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(commentId))
            {
                throw new ArgumentNullException("commentId", "Comment ID cannot be null or empty");
            }

            var postData = await CreatePostData();
            var method = string.Format("comment/{0}/delete", commentId);

            var response = await Post<Response>(postData, method, cancellationToken);
            return response != null && response.Status;
        }

        /// <summary>
        /// Gets the comment.
        /// </summary>
        /// <param name="commentId">The comment identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">commentId;Comment ID cannot be null or empty</exception>
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

        /// <summary>
        /// Gets the comments.
        /// </summary>
        /// <param name="videoId">The video identifier.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">videoId;Video ID cannot be null or empty</exception>
        public async Task<CommentsResponse> GetCommentsAsync(string videoId, SortDirection? sortDirection = null, int? offset = null, int? limit = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(videoId))
            {
                throw new ArgumentNullException("videoId", "Video ID cannot be null or empty");
            }

            var options = await CreatePostData(false);
            options.AddIfNotNull("video", videoId);
            options.AddIfNotNull("direction", sortDirection);
            options.AddIfNotNull("offset", offset);
            options.AddIfNotNull("limit", limit);

            var response = await Post<CommentsResponse>(options, "comments/list", cancellationToken);
            return response;
        }

        /// <summary>
        /// Gets the comment URL.
        /// </summary>
        /// <param name="commentId">The comment identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">commentId;Comment ID cannot be null or empty</exception>
        public string GetCommentUrl(string commentId)
        {
            if (string.IsNullOrEmpty(commentId))
            {
                throw new ArgumentNullException("commentId", "Comment ID cannot be null or empty");
            }

            return CreateUrl(string.Format("comment/{0}/url", commentId));
        }

        /// <summary>
        /// Votes the comment.
        /// </summary>
        /// <param name="commentId">The comment identifier.</param>
        /// <param name="vote">The vote.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">commentId;Comment ID cannot be null or empty</exception>
        public async Task<Comment> VoteCommentAsync(string commentId, Vote vote, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(commentId))
            {
                throw new ArgumentNullException("commentId", "Comment ID cannot be null or empty");
            }

            var postData = await CreatePostData();
            postData.AddIfNotNull("value", vote.GetDescription());

            var method = string.Format("comment/{0}/vote", commentId);

            var response = await Post<CommentResponse>(postData, method, cancellationToken);

            return response != null ? response.Comment : null;
        }

        #endregion

        #region GeoFences Methods

        /// <summary>
        /// Gets the geo fences.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<List<Geofence>> GetGeoFencesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await Get<GeoFencesResponse>("geofences", cancellationToken: cancellationToken);
            if (response != null)
            {
                return response.Geofences ?? new List<Geofence>();
            }

            return new List<Geofence>();
        }

        /// <summary>
        /// Suggests the geo fences.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
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

        #region Grab Methods

        /// <summary>
        /// Grabs the external video.
        /// </summary>
        /// <param name="externalUrl">The external URL.</param>
        /// <param name="title">The title.</param>
        /// <param name="description">The description.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">externalUrl;External URL cannot be null or empty</exception>
        public async Task<Video> GrabExternalVideoAsync(string externalUrl, string title = null, string description = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(externalUrl))
            {
                throw new ArgumentNullException("externalUrl", "External URL cannot be null or empty");
            }

            var postData = await CreatePostData(false);
            postData.AddIfNotNull("url", externalUrl);
            postData.AddIfNotNull("title", title);
            postData.AddIfNotNull("description", description);

            var response = await Post<VideoResponse>(postData, "grab", cancellationToken);
            return response != null ? response.Video : null;
        }

        /// <summary>
        /// Grabs the external video information.
        /// </summary>
        /// <param name="externalUrl">The external URL.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">externalUrl;External URL cannot be null or empty</exception>
        public async Task<VideoInfoResponse> GrabExternalVideoInfoAsync(string externalUrl, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(externalUrl))
            {
                throw new ArgumentNullException("externalUrl", "External URL cannot be null or empty");
            }

            var options = new Dictionary<string, string>();
            options.AddIfNotNull("url", externalUrl);

            var response = await Get<VideoInfoResponse>("grab/preview", options.ToQueryString(), cancellationToken);
            return response;
        }

        #endregion

        #region Notifications Methods

        /// <summary>
        /// Gets the notifications.
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<NotificationsResponse> GetNotificationsAsync(int? limit = null, int? offset = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var postData = await CreatePostData();
            postData.AddIfNotNull("limit", limit);
            postData.AddIfNotNull("offset", offset);

            var response = await Post<NotificationsResponse>(postData, "notifications", cancellationToken);
            return response;
        }

        /// <summary>
        /// Marks the notifications as read.
        /// </summary>
        /// <param name="notificationIds">The notification ids.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">notificationIds;You must provide a list of notification IDs</exception>
        public async Task<bool> MarkNotificationsAsReadAsync(List<string> notificationIds, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (notificationIds.IsNullOrEmpty())
            {
                throw new ArgumentNullException("notificationIds", "You must provide a list of notification IDs");
            }

            var postData = await CreatePostData();
            postData.AddIfNotNull("notifications", notificationIds);

            var response = await Post<Response>(postData, "notifications/mark-read", cancellationToken);
            return response != null && response.Status;
        }

        /// <summary>
        /// Marks all notifications as read.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<bool> MarkAllNotificationsAsReadAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var postData = await CreatePostData();
            postData.Add("notifications", "all");

            var response = await Post<Response>(postData, "notifications/mark-read", cancellationToken);
            return response != null && response.Status;
        }

        #endregion

        #region Tags Methods

        /// <summary>
        /// Suggesteds the tags.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<List<Tag>> SuggestedTagsAsync(string searchText = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var postData = await CreatePostData(false);
            postData.AddIfNotNull("text", searchText);

            var response = await Post<TagsResponse>(postData, "tags/suggest", cancellationToken);
            if (response != null)
            {
                return response.Tags ?? new List<Tag>();
            }

            return new List<Tag>();
        }

        #endregion

        #region App/Client Methods

        /// <summary>
        /// Gets the authorised apps.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<List<Application>> GetAuthorisedAppsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var postData = await CreatePostData();

            var response = await Post<AppsResponse>(postData, "oauth/apps", cancellationToken);

            if (response != null)
            {
                return response.Applications ?? new List<Application>();
            }

            return new List<Application>();
        }

        /// <summary>
        /// Gets the owned apps.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<List<Application>> GetOwnedAppsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var postData = await CreatePostData();

            var response = await Post<AppsResponse>(postData, "oauth/clients", cancellationToken);

            if (response != null)
            {
                return response.Applications ?? new List<Application>();
            }

            return new List<Application>();
        }

        /// <summary>
        /// Registers the application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// app;You must provide some app details
        /// or
        /// app.name;You must provide a name for the app
        /// or
        /// app.redirecturl;You must provide a redirect url
        /// </exception>
        public async Task<CreateAppResponse> RegisterAppAsync(AppRequest app, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (app == null)
            {
                throw new ArgumentNullException("app", "You must provide some app details");
            }

            if (string.IsNullOrEmpty(app.Name))
            {
                throw new ArgumentNullException("app.name", "You must provide a name for the app");
            }

            if (string.IsNullOrEmpty(app.RedirectUri))
            {
                throw new ArgumentNullException("app.redirecturl", "You must provide a redirect url");
            }

            var postData = await CreatePostData();
            postData.AddIfNotNull("name", app.Name);
            postData.AddIfNotNull("website", app.Website);
            postData.AddIfNotNull("description", app.Description);
            postData.AddIfNotNull("organization", app.Organisation);
            postData.AddIfNotNull("accept_terms", "yes");
            postData.AddIfNotNull("redirect_uri_prefix", app.RedirectUri);

            var response = await Post<CreateAppResponse>(postData, "oauth/register", cancellationToken);
            return response;
        }

        /// <summary>
        /// Revokes the application token.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">clientId;A Client ID must be provided</exception>
        public async Task<bool> RevokeAppTokenAsync(string clientId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentNullException("clientId", "A Client ID must be provided");
            }

            var postData = await CreatePostData();
            postData.AddIfNotNull("client_id", clientId);

            var response = await Post<Response>(postData, "oauth/revoke", cancellationToken);
            return response != null && response.Status;
        }

        #endregion

        #region User Methods

        /// <summary>
        /// Gets the user avatar.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">userId;User ID cannot be null or empty</exception>
        public string GetUserAvatar(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId", "User ID cannot be null or empty");
            }

            var method = string.Format("user/{0}/avatar", userId);

            return CreateUrl(method);
        }

        /// <summary>
        /// Gets the user cover.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">userId;User ID cannot be null or empty</exception>
        public string GetUserCover(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId", "User ID cannot be null or empty");
            }

            var method = string.Format("user/{0}/cover", userId);

            return CreateUrl(method);
        }

        /// <summary>
        /// Removes the cover.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">userId;A user ID must be provided</exception>
        public async Task<User> RemoveCoverAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId", "A user ID must be provided");
            }

            var postData = await CreatePostData();
            postData.AddIfNotNull("user", userId);

            var method = string.Format("user/{0}/cover/remove", userId);

            var response = await Post<UserResponse>(postData, method, cancellationToken);
            return response != null ? response.User : null;
        }

        /// <summary>
        /// Updates the cover asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="imageStream">The image stream.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">userId;User ID cannot be null or empty</exception>
        public async Task<User> UpdateCoverAsync(string userId, Stream imageStream, string contentType, string filename, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId", "User ID cannot be null or empty");
            }

            using (var memoryStream = new MemoryStream())
            {
                await imageStream.CopyToAsync(memoryStream);
                return await UpdateCoverAsync(userId, memoryStream.ToArray(), contentType, filename, cancellationToken);
            }
        }

        /// <summary>
        /// Updates the cover asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="imageStream">The image stream.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">userId;User ID cannot be null or empty</exception>
        public async Task<User> UpdateCoverAsync(string userId, byte[] imageStream, string contentType, string filename, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId", "User ID cannot be null or empty");
            }

            var postData = await CreatePostData();

            var method = string.Format("user/{0}/cover/update", userId);

            var response = await PostFile<UserResponse>(postData, method, imageStream, contentType, filename, cancellationToken);

            return response != null ? response.User : null;
        }

        /// <summary>
        /// Creates the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="email">The email.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// username;username cannot be null or empty
        /// or
        /// password;password cannot be null or empty
        /// </exception>
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

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">userId;User Id cannot be null or empty</exception>
        public async Task<UserResponse> GetUserAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId", "User Id cannot be null or empty");
            }

            var options = await CreatePostData(false);
            var method = string.Format("user/{0}", userId);

            var response = await Post<UserResponse>(options, method, cancellationToken);
            return response;
        }

        /// <summary>
        /// Edits the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="username">The username.</param>
        /// <param name="currentPassword">The current password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <param name="email">The email.</param>
        /// <param name="bio"></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">userId;User Id cannot be null or empty</exception>
        public async Task<User> EditUserAsync(string userId, string username = null, string currentPassword = null, string newPassword = null, string email = null, string bio = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId", "User Id cannot be null or empty");
            }

            var postData = await CreatePostData();

            postData.AddIfNotNull("username", username);
            postData.AddIfNotNull("email", email);
            postData.AddIfNotNull("password", newPassword);
            postData.AddIfNotNull("passwordCurrent", currentPassword);
            postData.AddIfNotNull("bio", bio);

            postData.AddIfNotNull("email", email);

            var method = string.Format("user/{0}/edit", userId);

            var response = await Post<UserResponse>(postData, method, cancellationToken);
            
            return response != null ? response.User : null;
        }

        /// <summary>
        /// Follows the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">userId;User ID cannot be null or empty</exception>
        public async Task<bool> FollowUserAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId", "User ID cannot be null or empty");
            }

            var postData = await CreatePostData();

            var method = string.Format("user/{0}/follow", userId);

            var response = await Post<Response>(postData, method, cancellationToken);

            return response != null && response.Status;
        }

        /// <summary>
        /// Gets the users followed channels.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">userId;User ID cannot be null or empty</exception>
        public async Task<List<Channel>> GetUsersFollowedChannelsAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId", "User ID cannot be null or empty");
            }

            var postData = await CreatePostData();
            var method = string.Format("user/{0}/follows-channels", userId);

            var response = await Post<ChannelsResponse>(postData, method, cancellationToken);

            if (response != null)
            {
                return response.Channels ?? new List<Channel>();
            }

            return new List<Channel>();
        }

        /// <summary>
        /// Removes the avatar.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">userId;User ID cannot be null or empty</exception>
        public async Task<User> RemoveAvatarAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId", "User ID cannot be null or empty");
            }

            var postData = await CreatePostData();
            var method = string.Format("user/{0}/avatar/remove", userId);

            var response = await Post<UserResponse>(postData, method, cancellationToken);

            return response != null ? response.User : null;
        }

        /// <summary>
        /// Unfollows the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">userId;User ID cannot be null or empty</exception>
        public async Task<bool> UnfollowUserAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId", "User ID cannot be null or empty");
            }

            var postData = await CreatePostData();
            var method = string.Format("user/{0}/unfollow", userId);

            var response = await Post<Response>(postData, method, cancellationToken);

            return response != null && response.Status;
        }

        /// <summary>
        /// Updates the avatar.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="imageStream">The image stream.</param>
        /// <param name="filename"></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">userId;User ID cannot be null or empty</exception>
        public async Task<User> UpdateAvatarAsync(string userId, Stream imageStream, string contentType, string filename, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId", "User ID cannot be null or empty");
            }

            using (var memoryStream = new MemoryStream())
            {
                await imageStream.CopyToAsync(memoryStream);
                return await UpdateAvatarAsync(userId, memoryStream.ToArray(), contentType, filename, cancellationToken);
            }
        }

        /// <summary>
        /// Updates the avatar.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="imageStream">The image stream.</param>
        /// <param name="contentType"></param>
        /// <param name="filename"></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">userId;User ID cannot be null or empty</exception>
        public async Task<User> UpdateAvatarAsync(string userId, byte[] imageStream, string contentType, string filename, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId", "User ID cannot be null or empty");
            }

            var postData = await CreatePostData();

            var method = string.Format("user/{0}/avatar/update", userId);

            var response = await PostFile<UserResponse>(postData, method, imageStream, contentType, filename, cancellationToken);

            return response != null ? response.User : null;
        }

        /// <summary>
        /// Suggesteds the users.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<List<UserTag>> SuggestedUsersAsync(string searchText = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var postData = await CreatePostData(false);
            postData.AddIfNotNull("text", searchText);

            var response = await Post<UserTagsResponse>(postData, "users/suggest", cancellationToken);
            if (response != null)
            {
                return response.UserTags ?? new List<UserTag>();
            }

            return new List<UserTag>();
        }

        /// <summary>
        /// Gets the user videos.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="sortDirection"></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">userId;User ID cannot be null or empty</exception>
        public async Task<VideosResponse> GetUserVideosAsync(string userId, int? offset = null, int? limit = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId", "User ID cannot be null or empty");
            }

            var options = await CreatePostData(false, false);
            options.AddIfNotNull("user", userId);
            options.AddIfNotNull("offset", offset);
            options.AddIfNotNull("limit", limit);
            options.AddIfNotNull("state", "success");
            options.AddIfNotNull("minVideoId", "0");
            options.AddIfNotNull("direction", sortDirection);

            var response = await Post<VideosResponse>(options, "videos/list", cancellationToken);
            return response;
        }

        /// <summary>
        /// Gets the anonymous videos.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<VideosResponse> GetAnonymousVideosAsync(int? offset = null, int? limit = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var options = await CreatePostData(false);
            options.AddIfNotNull("user", "0");
            options.AddIfNotNull("offset", offset);
            options.AddIfNotNull("limit", limit);
            options.AddIfNotNull("moderated", "0");
            options.AddIfNotNull("state", "success");
            options.AddIfNotNull("minVideoId", "0");

            var response = await Post<VideosResponse>(options, "videos/list", cancellationToken);
            return response;
        }

        /// <summary>
        /// Gets the user feed.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<VideosResponse> GetUserFeedAsync(int? offset = null, int? limit = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var postData = await CreatePostData();
            postData.AddIfNotNull("offset", offset);
            postData.AddIfNotNull("limit", limit);

            var response = await Post<VideosResponse>(postData, "videos/feed", cancellationToken);
            return response;
        }

        /// <summary>
        /// Determines whether [is user following user] [the specified user identifier].
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="otherUser">The other user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">userId;A base user ID must be provided</exception>
        public async Task<bool> IsUserFollowingUserAsync(string userId, string otherUser = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId", "A base user ID must be provided");
            }

            var options = await CreatePostData(false);
            var method = string.Format("user/{0}/follow/{1}", userId, otherUser);

            var response = await Get<FollowResponse>(method, options.ToQueryString(), cancellationToken);
            return response != null && response.IsFollowing;
        }

        /// <summary>
        /// Gets the users followers.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">userId;You must provide a valid user id</exception>
        public async Task<UsersResponse> GetUsersFollowersAsync(string userId, int? offset = null, int? limit = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId", "You must provide a valid user id");
            }

            var postData = await CreatePostData(false);
            postData.AddIfNotNull("offset", offset);
            postData.AddIfNotNull("limit", limit);

            var method = string.Format("user/{0}/followers", userId);

            var response = await Get<UsersResponse>(method, postData.ToQueryString(), cancellationToken);
            return response;
        }

        /// <summary>
        /// Gets the users following.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">userId;You must provide a valid user id</exception>
        public async Task<UsersResponse> GetUsersFollowingAsync(string userId, int? offset = null, int? limit = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId", "You must provide a valid user id");
            }

            var postData = await CreatePostData(false);
            postData.AddIfNotNull("offset", offset);
            postData.AddIfNotNull("limit", limit);

            var method = string.Format("user/{0}/following", userId);

            var response = await Get<UsersResponse>(method, postData.ToQueryString(), cancellationToken);
            return response;
        }

        #endregion

        #region Video Methods

        /// <summary>
        /// Deletes the video.
        /// </summary>
        /// <param name="videoId">The video identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">videoId;Video ID cannot be null or empty</exception>
        public async Task<bool> DeleteVideoAsync(string videoId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(videoId))
            {
                throw new ArgumentNullException("videoId", "Video ID cannot be null or empty");
            }

            var postData = await CreatePostData(true);

            var method = string.Format("video/{0}/delete", videoId);
            var response = await Post<Response>(postData, method, cancellationToken);
            return response != null && response.Status;
        }

        /// <summary>
        /// Deletes the anonymous video.
        /// </summary>
        /// <param name="videoId">The video identifier.</param>
        /// <param name="deletionToken">The deletion token.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// videoId;Video ID cannot be null or empty
        /// or
        /// deletionToken;Deletion token cannot be null or empty
        /// </exception>
        /// <exception cref="System.InvalidOperationException">No Device ID was set</exception>
        public async Task<bool> DeleteAnonymousVideoAsync(string videoId, string deletionToken, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(videoId))
            {
                throw new ArgumentNullException("videoId", "Video ID cannot be null or empty");
            }

            if (string.IsNullOrEmpty(deletionToken))
            {
                throw new ArgumentNullException("deletionToken", "Deletion token cannot be null or empty");
            }

            if (string.IsNullOrEmpty(DeviceId))
            {
                throw new InvalidOperationException("No Device ID was set");
            }

            var postData = await CreatePostData(false);
            postData.AddIfNotNull("deleteToken", deletionToken);

            var method = string.Format("video/{0}/delete", videoId);
            var response = await Post<Response>(postData, method, cancellationToken);
            return response != null && response.Status;
        }

        /// <summary>
        /// Gets the video.
        /// </summary>
        /// <param name="videoId">The video identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">videoId;Video ID cannot be null or empty</exception>
        public async Task<VideoResponse> GetVideoAsync(string videoId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(videoId))
            {
                throw new ArgumentNullException("videoId", "Video ID cannot be null or empty");
            }

            var options = await CreatePostData(false);
            var method = string.Format("video/{0}", videoId);

            var response = await Get<VideoResponse>(method, options.ToQueryString(), cancellationToken);
            return response;
        }

        /// <summary>
        /// Edits the video.
        /// </summary>
        /// <param name="videoId">The video identifier.</param>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">videoId;Video ID cannot be null or empty</exception>
        public async Task<Video> EditVideoAsync(string videoId, VideoRequest request = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(videoId))
            {
                throw new ArgumentNullException("videoId", "Video ID cannot be null or empty");
            }

            var postData = await CreatePostData(false);
            if (request != null)
            {
                postData.AddIfNotNull("title", request.Title);
                postData.AddIfNotNull("description", request.Description);
                postData.AddIfNotNull("source", request.VideoSource);
                postData.AddIfNotNull("latitude", request.Latitude);
                postData.AddIfNotNull("longitude", request.Longitude);
                postData.AddIfNotNull("place_id", request.FourSquarePlaceId);
                postData.AddIfNotNull("place_name", request.FourSquarePlaceName);
                postData.AddIfNotNull("private", request.IsPrivate);
            }

            var method = string.Format("video/{0}/edit", videoId);

            var response = await Post<VideoResponse>(postData, method, cancellationToken);
            return response != null ? response.Video : null;
        }

        /// <summary>
        /// Updates the video thumbnail.
        /// </summary>
        /// <param name="videoId">The video identifier.</param>
        /// <param name="thumbnailStream">The thumbnail stream.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// videoId;Video ID cannot be null or empty
        /// or
        /// thumbnailStream;Must provide a valid image
        /// </exception>
        public async Task<Video> UpdateVideoThumbnailAsync(string videoId, Stream thumbnailStream, string contentType, string filename, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(videoId))
            {
                throw new ArgumentNullException("videoId", "Video ID cannot be null or empty");
            }

            if (thumbnailStream == null)
            {
                throw new ArgumentNullException("thumbnailStream", "Must provide a valid image");
            }

            var postData = await CreatePostData(false);
            var method = string.Format("video/{0}/edit", videoId);

            using (var memoryStream = new MemoryStream())
            {
                await thumbnailStream.CopyToAsync(memoryStream);
                var response = await PostFile<VideoResponse>(postData, method, memoryStream.ToArray(), contentType, filename, cancellationToken);
                return response != null ? response.Video : null;
            }
        }

        /// <summary>
        /// Flags the video.
        /// </summary>
        /// <param name="videoId">The video identifier.</param>
        /// <param name="isFlagged">if set to <c>true</c> [is flagged].</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">videoId;Video ID cannot be null or empty</exception>
        public async Task<Video> FlagVideoAsync(string videoId, bool isFlagged, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(videoId))
            {
                throw new ArgumentNullException("videoId", "Video ID cannot be null or empty");
            }

            var postData = await CreatePostData();
            postData.Add("flagged", isFlagged ? "1" : "0");

            var method = string.Format("video/{0}/flag", videoId);

            var response = await Post<VideoResponse>(postData, method, cancellationToken);
            return response != null ? response.Video : null;
        }

        /// <summary>
        /// Requests the video.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<VideoRequestResponse> RequestVideoAsync(VideoRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            var postData = await CreatePostData(false);
            if (request != null)
            {
                postData.AddIfNotNull("title", request.Title);
                postData.AddIfNotNull("description", request.Description);
                postData.AddIfNotNull("thumbnail", await request.ThumbnailStream.ToBase64String());
                postData.AddIfNotNull("source", request.VideoSource);
                postData.AddIfNotNull("latitude", request.Latitude);
                postData.AddIfNotNull("longitude", request.Longitude);
                postData.AddIfNotNull("place_id", request.FourSquarePlaceId);
                postData.AddIfNotNull("place_name", request.FourSquarePlaceName);
                postData.AddIfNotNull("private", request.IsPrivate);
            }

            var response = await Post<VideoRequestResponse>(postData, "video/request", cancellationToken);
            return response;
        }

        /// <summary>
        /// Gets the video thumbnail.
        /// </summary>
        /// <param name="videoId">The video identifier.</param>
        /// <returns></returns>
        public string GetVideoThumbnail(string videoId)
        {
            return CreateUrl(string.Format("video/{0}/thumbnail", videoId));
        }

        /// <summary>
        /// Updates the video title.
        /// </summary>
        /// <param name="videoCode">The video code.</param>
        /// <param name="title">The title.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// videoCode;A video code must be provided
        /// or
        /// title;Title cannot be null or empty
        /// </exception>
        /// <exception cref="System.InvalidOperationException">You must have set a device id</exception>
        public async Task<bool> UpdateVideoTitleAsync(string videoCode, string title, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(videoCode))
            {
                throw new ArgumentNullException("videoCode", "A video code must be provided");
            }

            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException("title", "Title cannot be null or empty");
            }

            if (string.IsNullOrEmpty(DeviceId))
            {
                throw new InvalidOperationException("You must have set a device id");
            }

            var postData = await CreatePostData();
            postData.AddIfNotNull("code", videoCode);
            postData.AddIfNotNull("title", title);

            var response = await Post<Response>(postData, "video/update-title", cancellationToken);
            return response != null && response.Status;
        }

        /// <summary>
        /// Uploads the video.
        /// </summary>
        /// <param name="videoCode">The video code.</param>
        /// <param name="contentType"></param>
        /// <param name="filename"></param>
        /// <param name="videoStream">The video stream.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// videoCode;A valid video code must be used from the RequestVideo method.
        /// or
        /// videoStream;Invalid video stream passed through
        /// </exception>
        public async Task<VideoUploadResponse> UploadVideoAsync(string videoCode, string contentType, string filename, Stream videoStream, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(videoCode))
            {
                throw new ArgumentNullException("videoCode", "A valid video code must be used from the RequestVideo method.");
            }

            if (videoStream == null || videoStream.Length == 0)
            {
                throw new ArgumentNullException("videoStream", "Invalid video stream passed through");
            }

            var postData = await CreatePostData(false);
            postData.AddIfNotNull("code", videoCode);

            using (var m = await videoStream.ToMemoryStream())
            {
                var response = await PostFile<VideoUploadResponse>(postData, "video/upload", m.ToArray(), contentType, filename, cancellationToken);
                return response;
            }
        }

        /// <summary>
        /// Uploads the video.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="videoStream">The video stream.</param>
        /// <param name="contentType"></param>
        /// <param name="fileName"></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">videoStream;Invalid video stream passed through</exception>
        public async Task<VideoUploadResponse> UploadVideoAsync(VideoRequest request, Stream videoStream, string contentType, string fileName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (videoStream == null || videoStream.Length == 0)
            {
                throw new ArgumentNullException("videoStream", "Invalid video stream passed through");
            }

            var postData = await CreatePostData(false);
            if (request != null)
            {
                postData.AddIfNotNull("channel", request.ChannelId);
                postData.AddIfNotNull("size", request.VideoSize);
                postData.AddIfNotNull("title", request.Title);
                postData.AddIfNotNull("description", request.Description);
                postData.AddIfNotNull("source", request.VideoSource);
                postData.AddIfNotNull("latitude", request.Latitude);
                postData.AddIfNotNull("longitude", request.Longitude);
                postData.AddIfNotNull("place_id", request.FourSquarePlaceId);
                postData.AddIfNotNull("place_name", request.FourSquarePlaceName);
                postData.AddIfNotNull("private", request.IsPrivate);
                postData.AddIfNotNull("filename", request.FileName);
            }

            using (var m = await videoStream.ToMemoryStream())
            {
                var response = await PostFile<VideoUploadResponse>(postData, "video/upload", m.ToArray(), contentType, fileName, cancellationToken);
                return response;
            }
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// Locations the search.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">You must supply a valid request
        /// or
        /// You must supply either long/lat or a geofence ID</exception>
        public async Task<VideosResponse> LocationSearchAsync(LocationRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
            {
                throw new InvalidOperationException("You must supply a valid request");
            }

            if (request.Latitude == null && request.Longitude == null && string.IsNullOrEmpty(request.GeofenceId))
            {
                throw new InvalidOperationException("You must supply either long/lat or a geofence ID");
            }

            var postData = await CreatePostData(false);
            if (string.IsNullOrEmpty(request.GeofenceId))
            {
                postData.AddIfNotNull("latitude", request.Latitude);
                postData.AddIfNotNull("longitude", request.Longitude);
                postData.AddIfNotNull("from", request.From);
                postData.AddIfNotNull("to", request.To);
                postData.AddIfNotNull("distance", request.Distance);
            }
            else
            {
                postData.AddIfNotNull("geofence", request.GeofenceId);
            }

            postData.AddIfNotNull("offset", request.Offset);
            postData.AddIfNotNull("limit", request.Limit);
            postData.AddIfNotNull("order", request.LocationOrderBy);

            var response = await Post<VideosResponse>(postData, "videos/location", cancellationToken);
            return response;
        }

        /// <summary>
        /// Searches the videos.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="includeNsfw"></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">searchText;Search text cannot be null or empty</exception>
        public async Task<VideosResponse> SearchVideosAsync(string searchText, int? offset = null, int? limit = null, bool? includeNsfw = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(searchText))
            {
                throw new ArgumentNullException("searchText", "Search text cannot be null or empty");
            }

            var postData = await CreatePostData(false);
            postData.AddIfNotNull("query", searchText);
            postData.AddIfNotNull("offset", offset);
            postData.AddIfNotNull("limit", limit);

            if (includeNsfw.HasValue)
            {
                if (includeNsfw.Value)
                {
                    postData.AddIfNotNull("nsfw", "true");
                }
                else
                {
                    postData.Add("nsfw", string.Empty);
                }
            }

            var response = await Get<VideosResponse>("videos/search", postData.ToQueryString(), cancellationToken);
            return response;
        }

        public event EventHandler<AuthResponse> AuthDetailsUpdated;

        #endregion

        #region API Call methods
        private async Task<TReturnType> Post<TReturnType>(Dictionary<string, string> postData, string method, CancellationToken cancellationToken = default(CancellationToken))
        {
            var url = CreateUrl(method);

            var response = await _httpClient.PostAsync(url, new FormUrlEncodedContent(postData), cancellationToken);

            return await HandleResponse<TReturnType>(response);
        }

        private async Task<TReturnType> PostFile<TReturnType>(Dictionary<string, string> postData, string method, byte[] fileData, string contentType, string filename, CancellationToken cancellationToken = default(CancellationToken))
        {
            var url = CreateUrl(method);

            var form = new MultipartFormDataContent();

            foreach (var item in postData)
            {
                form.Add(new StringContent(item.Value), item.Key);
            }

            var byteContent = new ByteArrayContent(fileData, 0, fileData.Count());
            byteContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            form.Add(byteContent, "filedata", filename);

            var response = await _httpClient.PostAsync(url, form, cancellationToken);

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

            if (response.IsSuccessStatusCode || responseString.Contains("\"status\":true"))
            {
                var returnItem = JsonConvert.DeserializeObject<TReturnType>(responseString);
                return returnItem;
            }

            var error = JsonConvert.DeserializeObject<ErrorResponse>(responseString);
            throw new VidMeException(response.StatusCode, error);
        }
        #endregion

        private async Task CheckExpirationDateIsOk()
        {
            var now = DateTime.Now;
            if (AuthenticationInfo == null)
            {
                throw new VidMeException(HttpStatusCode.Unauthorized, new ErrorResponse { Error = "No valid AuthenticationInfo set" });
            }

            if (AuthenticationInfo.Expires < now)
            {
                var token = AuthenticationInfo.Token;
                AuthenticationInfo = null;
                var response = await CheckAuthTokenAsync(token);
                if (response != null)
                {
                    SetAuthentication(response.Auth);

                    var authChanged = AuthDetailsUpdated;
                    if (authChanged != null)
                    {
                        authChanged(this, response);
                    }
                }
            }
        }

        private static string CreateUrl(string method, string options = "")
        {
            return string.Format("{0}{1}?{2}", BaseUrl, method, options);
        }

        private async Task<Dictionary<string, string>> CreatePostData(bool tokenRequred = true, bool includeDevice = true)
        {
            var postData = new Dictionary<string, string>();
            if (tokenRequred)
            {
                await CheckExpirationDateIsOk();
            }

            if (AuthenticationInfo != null)
            {
                postData.Add("token", AuthenticationInfo.Token);
            }

            if (includeDevice)
            {
                postData.AddIfNotNull("DEVICE".ToLower(), DeviceId);
            }

            postData.AddIfNotNull("device_PLATFORM".ToLower(), Platform);

            return postData;
        }
    }
}
