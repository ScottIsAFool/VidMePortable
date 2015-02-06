using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using VidMePortable.Model;
using VidMePortable.Model.Requests;
using VidMePortable.Model.Responses;

namespace VidMePortable
{
    public interface IVidMeClient
    {
        /// <summary>
        /// Gets the authentication information.
        /// </summary>
        /// <value>
        /// The authentication information.
        /// </value>
        Auth AuthenticationInfo { get; }

        /// <summary>
        /// Gets the device identifier.
        /// </summary>
        /// <value>
        /// The device identifier.
        /// </value>
        string DeviceId { get; }

        /// <summary>
        /// Gets the platform.
        /// </summary>
        /// <value>
        /// The platform.
        /// </value>
        string Platform { get; }

        /// <summary>
        /// Sets the device name and platform.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="platform">The platform.</param>
        void SetDeviceNameAndPlatform(string deviceId, string platform);

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
        Task<AuthResponse> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Checks the authentication token.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="VidMeException">No AuthenticationInfo set</exception>
        Task<AuthResponse> CheckAuthTokenAsync(string token, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes the authentication token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="VidMeException">No AuthenticationInfo set</exception>
        Task<bool> DeleteAuthTokenAsync(CancellationToken cancellationToken = default(CancellationToken));

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
        string GetAuthUrl(string clientId, string redirectUrl, List<Scope> scopes, AuthType type = AuthType.Code);

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
        Task<AuthResponse> ExchangeCodeForTokenAsync(string code, string clientId, string clientSecret, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Sets the authentication.
        /// </summary>
        /// <param name="authenticationInfo">The authentication information.</param>
        void SetAuthentication(Auth authenticationInfo);

        /// <summary>
        /// Determines whether [is user following channel] [the specified channel identifier].
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="otherUser">The other user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">channelId;A base channel ID must be provided</exception>
        Task<bool> IsUserFollowingChannelAsync(string channelId, string otherUser = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the channel.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">channelId;Channel ID cannot be null or empty</exception>
        Task<ChannelResponse> GetChannelAsync(string channelId, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the channels.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<List<Channel>> GetChannelsAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Follows the channel.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">channelId;Channel ID cannot be null or empty</exception>
        Task<bool> FollowChannelAsync(string channelId, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the channels hot videos.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">channelId;Channel ID cannot be null or empty</exception>
        Task<VideosResponse> GetChannelsHotVideosAsync(string channelId, int? offset = null, int? limit = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the channels new videos.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">channelId;Channel ID cannot be null or empty</exception>
        Task<VideosResponse> GetChannelsNewVideosAsync(string channelId, int? offset = null, int? limit = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the channel URL.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <returns></returns>
        string GetChannelUrl(string channelId);

        /// <summary>
        /// Uns the follow channel.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">channelId;Channel ID cannot be null or empty</exception>
        Task<bool> UnFollowChannelAsync(string channelId, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Lists the channels.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<List<Channel>> ListChannelsAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Lists the suggested channels.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="number">The number.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<List<Channel>> ListSuggestedChannelsAsync(string text = null, int? number = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the channel moderators.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<UsersResponse> GetChannelModeratorsAsync(string channelId, int? offset = null, int? limit = null, CancellationToken cancellationToken = default(CancellationToken));

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
        Task<Comment> CreateCommentAsync(string videoId, string commentText, TimeSpan timeOfComment, string inReplyToCommentId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes the comment.
        /// </summary>
        /// <param name="commentId">The comment identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">commentId;Comment ID cannot be null or empty</exception>
        Task<bool> DeleteCommentAsync(string commentId, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the comment.
        /// </summary>
        /// <param name="commentId">The comment identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">commentId;Comment ID cannot be null or empty</exception>
        Task<Comment> GetCommentAsync(string commentId, CancellationToken cancellationToken = default(CancellationToken));

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
        Task<CommentsResponse> GetCommentsAsync(string videoId, SortDirection? sortDirection = null, int? offset = null, int? limit = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the comment URL.
        /// </summary>
        /// <param name="commentId">The comment identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">commentId;Comment ID cannot be null or empty</exception>
        string GetCommentUrl(string commentId);

        /// <summary>
        /// Votes the comment.
        /// </summary>
        /// <param name="commentId">The comment identifier.</param>
        /// <param name="vote">The vote.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">commentId;Comment ID cannot be null or empty</exception>
        Task<Comment> VoteCommentAsync(string commentId, Vote vote, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the geo fences.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<List<Geofence>> GetGeoFencesAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Suggests the geo fences.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<List<Geofence>> SuggestGeoFencesAsync(string searchText = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Grabs the external video.
        /// </summary>
        /// <param name="externalUrl">The external URL.</param>
        /// <param name="title">The title.</param>
        /// <param name="description">The description.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">externalUrl;External URL cannot be null or empty</exception>
        Task<Video> GrabExternalVideoAsync(string externalUrl, string title = null, string description = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Grabs the external video information.
        /// </summary>
        /// <param name="externalUrl">The external URL.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">externalUrl;External URL cannot be null or empty</exception>
        Task<VideoInfoResponse> GrabExternalVideoInfoAsync(string externalUrl, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the notifications.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<List<Notification>> GetNotificationsAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Marks the notifications as read.
        /// </summary>
        /// <param name="notificationIds">The notification ids.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">notificationIds;You must provide a list of notification IDs</exception>
        Task<bool> MarkNotificationsAsReadAsync(List<string> notificationIds, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Marks all notifications as read.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<bool> MarkAllNotificationsAsReadAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Suggesteds the tags.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<List<Tag>> SuggestedTagsAsync(string searchText = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the authorised apps.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<List<Application>> GetAuthorisedAppsAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the owned apps.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<List<Application>> GetOwnedAppsAsync(CancellationToken cancellationToken = default(CancellationToken));

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
        Task<CreateAppResponse> RegisterAppAsync(AppRequest app, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Revokes the application token.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">clientId;A Client ID must be provided</exception>
        Task<bool> RevokeAppTokenAsync(string clientId, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the user avatar.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">userId;User ID cannot be null or empty</exception>
        string GetUserAvatar(string userId);

        /// <summary>
        /// Gets the user cover.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">userId;User ID cannot be null or empty</exception>
        string GetUserCover(string userId);

        /// <summary>
        /// Removes the cover.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">userId;A user ID must be provided</exception>
        Task<bool> RemoveCoverAsync(string userId, CancellationToken cancellationToken = default(CancellationToken));

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
        Task<User> UpdateCoverAsync(string userId, Stream imageStream, string contentType, string filename, CancellationToken cancellationToken = default(CancellationToken));

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
        Task<User> UpdateCoverAsync(string userId, byte[] imageStream, string contentType, string filename, CancellationToken cancellationToken = default(CancellationToken));

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
        Task<AuthResponse> CreateUserAsync(string username, string password, string email = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">userId;User Id cannot be null or empty</exception>
        Task<UserResponse> GetUserAsync(string userId, CancellationToken cancellationToken = default(CancellationToken));

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
        Task<AuthResponse> EditUserAsync(string userId, string username = null, string currentPassword = null, string newPassword = null, string email = null, string bio = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Follows the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">userId;User ID cannot be null or empty</exception>
        Task<bool> FollowUserAsync(string userId, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the users followed channels.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">userId;User ID cannot be null or empty</exception>
        Task<List<Channel>> GetUsersFollowedChannelsAsync(string userId, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Removes the avatar.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">userId;User ID cannot be null or empty</exception>
        Task<bool> RemoveAvatarAsync(string userId, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Unfollows the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">userId;User ID cannot be null or empty</exception>
        Task<bool> UnfollowUserAsync(string userId, CancellationToken cancellationToken = default(CancellationToken));

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
        Task<User> UpdateAvatarAsync(string userId, Stream imageStream, string contentType, string filename, CancellationToken cancellationToken = default(CancellationToken));

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
        Task<User> UpdateAvatarAsync(string userId, byte[] imageStream, string contentType, string filename, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Suggesteds the users.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<List<UserTag>> SuggestedUsersAsync(string searchText = null, CancellationToken cancellationToken = default(CancellationToken));

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
        Task<VideosResponse> GetUserVideosAsync(string userId, int? offset = null, int? limit = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the anonymous videos.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<VideosResponse> GetAnonymousVideosAsync(int? offset = null, int? limit = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the user feed.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<VideosResponse> GetUserFeedAsync(int? offset = null, int? limit = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Determines whether [is user following user] [the specified user identifier].
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="otherUser">The other user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">userId;A base user ID must be provided</exception>
        Task<bool> IsUserFollowingUserAsync(string userId, string otherUser = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes the video.
        /// </summary>
        /// <param name="videoId">The video identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">videoId;Video ID cannot be null or empty</exception>
        Task<bool> DeleteVideoAsync(string videoId, CancellationToken cancellationToken = default(CancellationToken));

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
        Task<bool> DeleteAnonymousVideoAsync(string videoId, string deletionToken, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the video.
        /// </summary>
        /// <param name="videoId">The video identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">videoId;Video ID cannot be null or empty</exception>
        Task<VideoResponse> GetVideoAsync(string videoId, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Edits the video.
        /// </summary>
        /// <param name="videoId">The video identifier.</param>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">videoId;Video ID cannot be null or empty</exception>
        Task<Video> EditVideoAsync(string videoId, VideoRequest request = null, CancellationToken cancellationToken = default(CancellationToken));

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
        Task<Video> UpdateVideoThumbnailAsync(string videoId, Stream thumbnailStream, string contentType, string filename, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Flags the video.
        /// </summary>
        /// <param name="videoId">The video identifier.</param>
        /// <param name="isFlagged">if set to <c>true</c> [is flagged].</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">videoId;Video ID cannot be null or empty</exception>
        Task<Video> FlagVideoAsync(string videoId, bool isFlagged, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Requests the video.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<VideoRequestResponse> RequestVideoAsync(VideoRequest request, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the video thumbnail.
        /// </summary>
        /// <param name="videoId">The video identifier.</param>
        /// <returns></returns>
        string GetVideoThumbnail(string videoId);

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
        Task<bool> UpdateVideoTitleAsync(string videoCode, string title, CancellationToken cancellationToken = default(CancellationToken));

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
        Task<VideoUploadResponse> UploadVideoAsync(string videoCode, string contentType, string filename, Stream videoStream, CancellationToken cancellationToken = default(CancellationToken));

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
        Task<VideoUploadResponse> UploadVideoAsync(VideoRequest request, Stream videoStream, string contentType, string fileName, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Locations the search.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">You must supply a valid request
        /// or
        /// You must supply either long/lat or a geofence ID</exception>
        Task<VideosResponse> LocationSearchAsync(LocationRequest request, CancellationToken cancellationToken = default(CancellationToken));

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
        Task<VideosResponse> SearchVideosAsync(string searchText, int? offset = null, int? limit = null, bool? includeNsfw = null, CancellationToken cancellationToken = default(CancellationToken));

        event EventHandler<AuthResponse> AuthDetailsUpdated;
    }
}