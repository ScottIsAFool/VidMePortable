using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using VidMePortable.Model;
using VidMePortable.Model.Responses;

namespace VidMePortable
{
    public interface IVidMeClient
    {
        Auth AuthenticationInfo { get; }
        string DeviceId { get; }
        string Platform { get; }
        void SetDeviceNameAndPlatform(string deviceId, string platform);

        [Obsolete("This method will work, but the oauth way is preferred")]
        Task<AuthResponse> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default(CancellationToken));

        Task<AuthResponse> CheckAuthTokenAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> DeleteAuthTokenAsync(CancellationToken cancellationToken = default(CancellationToken));
        string GetAuthUrl(string clientId, string redirectUrl, List<Scope> scopes, AuthType type = AuthType.Code);
        Task<AuthResponse> ExchangeCodeForTokenAsync(string code, string clientId, string clientSecret, CancellationToken cancellationToken = default(CancellationToken));
        void SetAuthentication(Auth authenticationInfo);
        Task<Channel> GetChannelAsync(string channelId, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> FollowChannelAsync(string channelId, CancellationToken cancellationToken = default(CancellationToken));
        Task<VideosResponse> GetChannelsHotVideosAsync(string channelId, int? offset = null, int? limit = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<VideosResponse> GetChannelsNewVideosAsync(string channelId, int? offset = null, int? limit = null, CancellationToken cancellationToken = default(CancellationToken));
        string GetChannelUrl(string channelId);
        Task<bool> UnFollowChannelAsync(string channelId, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<Channel>> ListChannelsAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<List<Channel>> ListSuggestedChannelsAsync(string text = null, int? number = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<Comment> CreateCommentAsync(string videoId, string commentText, TimeSpan timeOfComment, string inReplyToCommentId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> DeleteCommentAsync(string commentId, CancellationToken cancellationToken = default(CancellationToken));
        Task<Comment> GetCommentAsync(string commentId, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<Comment>> GetCommentsAsync(string videoId, SortDirection? sortDirection, CancellationToken cancellationToken = default(CancellationToken));
        string GetCommentUrl(string commentId);
        Task<Comment> VoteCommentAsync(string commentId, Vote vote, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<Geofence>> GetGeoFencesAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<List<Geofence>> SuggestGeoFencesAsync(string searchText = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<Video> GrabExternalVideoAsync(string externalUrl, string title = null, string description = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<VideoInfoResponse> GrabExternalVideoInfoAsync(string externalUrl, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<Notification>> GetNotificationsAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> MarkNotificationsAsReadAsync(List<string> notificationIds, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> MarkAllNotificationsAsReadAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<List<Tag>> SuggestedTagsAsync(string searchText = null, CancellationToken cancellationToken = default(CancellationToken));
        string GetUserAvatar(string userId);
        Task<AuthResponse> CreateUserAsync(string username, string password, string email = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<User> GetUserAsync(string userId, CancellationToken cancellationToken = default(CancellationToken));
        Task<AuthResponse> EditUserAsync(string userId, string username = null, string currentPassword = null, string newPassword = null, string email = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> FollowUserAsync(string userId, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<Channel>> GetUsersFollowedChannelsAsync(string userId, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> RemoveAvatarAsync(string userId, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> UnfollowUserAsync(string userId, CancellationToken cancellationToken = default(CancellationToken));
        Task<User> UpdateAvatarAsync(string userId, Stream imageStream, CancellationToken cancellationToken = default(CancellationToken));
        Task<User> UpdateAvatarAsync(string userId, byte[] imageStream, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<UserTag>> SuggestedUsersAsync(string searchText = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<VideosResponse> GetUserVideosAsync(string userId, int? offset = null, int? limit = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<VideosResponse> GetAnonymouseVideosAsync(int? offset = null, int? limit = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> DeleteVideoAsync(string videoId, string deletionToken = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<Video> GetVideoAsync(string videoId, CancellationToken cancellationToken = default(CancellationToken));
        Task<Video> EditVideoAsync(string videoId, VideoRequest request = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<Video> FlagVideoAsync(string videoId, bool isFlagged, CancellationToken cancellationToken = default(CancellationToken));
        Task<VideoRequestResponse> RequestVideoAsync(VideoRequest request, CancellationToken cancellationToken = default(CancellationToken));
        string GetVideoThumbnail(string videoId);
        Task<bool> UpdateVideoTitleAsync(string videoCode, string title, CancellationToken cancellationToken = default(CancellationToken));
        Task<VideoUploadResponse> UploadVideoAsync(string videoCode, Stream videoStream, CancellationToken cancellationToken = default(CancellationToken));
        Task<VideoUploadResponse> UploadVideoAsync(VideoRequest request, Stream videoStream, CancellationToken cancellationToken = default(CancellationToken));
        Task<VideosResponse> LocationSearchAsync(LocationRequest request, CancellationToken cancellationToken = default(CancellationToken));
    }
}