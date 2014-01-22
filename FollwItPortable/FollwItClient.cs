using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using FollwItPortable.Attributes;
using FollwItPortable.Extensions;
using FollwItPortable.Logging;
using FollwItPortable.Model;
using FollwItPortable.Model.Requests;
using FollwItPortable.Model.Responses;

namespace FollwItPortable
{
    public class FollwItClient
    {
        private const string BaseUrlFormat = "http://follw.it/api/3/{0}/{1}/{2}";
        private const string DateFormat = "YYYY-mm-dd";

        #region Public Properties
        public HttpClient HttpClient { get; private set; }
        public string ApiKey { get; private set; }

        public string Username
        {
            get { return RequestManager.Username; }
            set { RequestManager.Username = value; }
        }

        public string Password
        {
            set { RequestManager.Password = value.Hash(); }
        }
        #endregion

        internal readonly ILogger Logger;

        #region Constructors
        public FollwItClient(string apiKey)
            : this(apiKey, null)
        {
        }

        public FollwItClient(string apiKey, HttpMessageHandler handler)
            : this (apiKey, handler, new NullLogger())
        {
        }

        public FollwItClient(string apiKey, HttpMessageHandler handler, ILogger logger)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentNullException("apiKey", "API key cannot be null or empty");
            }

            ApiKey = apiKey;
            Logger = logger;
            HttpClient = handler == null
                ? new HttpClient(new HttpClientHandler {AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip})
                : new HttpClient(handler);
        }
        #endregion

        #region Authentication method
        public async Task<bool> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException("username", "Username cannot be null or empty");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("password", "Password cannot be null or empty");
            }

            Username = username;
            Password = password;

            var request = RequestManager.CreateRequestType<AuthenticationRequest>();

            var response = await PostResponse<AuthenticationResponse>(PostMethods.UserAuthenticate, string.Empty, await request.SerialiseAsync(), cancellationToken);

            return response.Response.ToLower().Equals("success");
        }
        #endregion

        #region Calendar Method

        public async Task<List<FollwItEpisode>> GetPopularEpisodesAsync(DateTime? startDate = null, DateTime? endDate = null, string locale = "en", CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!startDate.HasValue)
            {
                startDate = DateTime.Now.Date;
            }

            if (!endDate.HasValue)
            {
                endDate = startDate.Value.AddDays(7);
            }

            var methodParams = string.Format("{0}/{1}/{2}", startDate.Value.ToString(DateFormat), endDate.Value.ToString(DateFormat), locale);

            return await GetResponse<List<FollwItEpisode>>(GetMethods.CalendarPopular, methodParams, cancellationToken);
        } 
        #endregion

        #region Get Movie Methods
        public async Task<List<FollwItMovie>> GetSimilarMoviesAsync(MovieIdentificationType identificationType, string movieId, string locale = "en", CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(movieId))
            {
                throw new ArgumentNullException("movieId", "MovieID cannot be null or empty");
            }

            var methodParams = string.Format("{0}/{1}/{2}", identificationType.GetDescription(), movieId, locale);

            return await GetResponse<List<FollwItMovie>>(GetMethods.MovieSimilarMovies, methodParams, cancellationToken);
        }

        public async Task<FollwItMovie> GetMovieDetailsAsync(MovieIdentificationType identificationType, string movieId, string locale = "en", CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(movieId))
            {
                throw new ArgumentNullException("movieId", "MovieID cannot be null or empty");
            }

            var methodParams = string.Format("{0}/{1}/{2}", identificationType.GetDescription(), movieId, locale);

            return await GetResponse<FollwItMovie>(GetMethods.MovieSimilarMovies, methodParams, cancellationToken);
        }

        public async Task<List<FollwItMovieSummary>> GetTrendingMoviesAsync(TimeInterval timeInterval, string locale = "en", int limit = 20, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (timeInterval == TimeInterval.NewShows)
            {
                throw new InvalidOperationException("TimeInterval cannot be NewShow for Movies");
            }

            var interval = timeInterval.ToString().ToLower();

            var methodParams = string.Format("{0}/{1}/{2}", interval, locale, limit);

            return await GetResponse<List<FollwItMovieSummary>>(GetMethods.MovieTrending, methodParams, cancellationToken);
        }
        #endregion

        #region Get Show Methods
        public async Task<FollwItTvShow> GetShowDetailsAsync(ShowIdentificationType identificationType, string showId, bool includeEpisodes = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(showId))
            {
                throw new ArgumentNullException("showId", "ShowID cannot be null or empty");
            }

            var methodParams = string.Format("{0}/{1}/{2}", identificationType.GetDescription(), showId, includeEpisodes);

            return await GetResponse<FollwItTvShow>(GetMethods.ShowSummary, methodParams, cancellationToken);
        }

        public async Task<List<FollwItTvShow>> GetTrendingShowsAsync(TimeInterval timeInterval, string locale = "en", int limit = 20, CancellationToken cancellationToken = default(CancellationToken))
        {
            var interval = timeInterval.ToString().ToLower();
            var methodParams = string.Format("{0}/{1}/{2}", interval, locale, limit);

            return await GetResponse<List<FollwItTvShow>>(GetMethods.ShowTrending, methodParams, cancellationToken);
        } 
        #endregion

        #region Get User Methods
        public async Task<FollwItList> GetListAsync(string listId, string username = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(username))
            {
                username = Username;
            }

            if (string.IsNullOrEmpty(listId))
            {
                throw new ArgumentNullException("listId", "ListID cannot be null or empty");
            }

            var methodParams = string.Format("{0}/{1}", username, listId);

            return await GetResponse<FollwItList>(GetMethods.UserList, methodParams, cancellationToken);
        }

        public async Task<List<FollwItList>> GetUserListsAsync(string username = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(username))
            {
                username = Username;
            }

            return await GetResponse<List<FollwItList>>(GetMethods.UserLists, username, cancellationToken);
        }

        public async Task<List<FollwItMovieSummary>> GetUserMovieCollectionAsync(string username = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(username))
            {
                username = Username;
            }

            return await GetResponse<List<FollwItMovieSummary>>(GetMethods.UserMovieCollection, username, cancellationToken);
        }

        public async Task<List<FollwItTvShow>> GetUserTvCollectionAsync(string username = null, bool includeEpisodes = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(username))
            {
                username = Username;
            }

            var methodParams = string.Format("{0}/{1}", username, includeEpisodes);

            return await GetResponse<List<FollwItTvShow>>(GetMethods.UserTvCollection, methodParams, cancellationToken);
        }

        public async Task<FollwItUser> GetPublicProfileAsync(string username = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(username))
            {
                username = Username;
            }

            return await GetResponse<FollwItUser>(GetMethods.UserPublicProfile, username, cancellationToken);
        }

        public async Task<bool> GetUsernameAvailableAsync(string username, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException("username", "Username cannot be null or empty");
            }

            var response = await GetResponse<FollwItUsernameAvailable>(GetMethods.UserUsernameAvailable, username, cancellationToken);

            return response.Available;
        }
        #endregion

        #region Web Requests
        private async Task<TReturnType> GetResponse<TReturnType>(string endPoint, string methodParams, CancellationToken cancellationToken = default(CancellationToken), [CallerMemberName] string callingMethod = "")
        {
            Logger.Debug(callingMethod);
            var url = GetUrl(endPoint, methodParams);

            Logger.Debug("GET: {0}", url);
            var requestTime = DateTime.Now;

            var response = await HttpClient.GetAsync(url, cancellationToken);
            var duration = DateTime.Now - requestTime;

            Logger.Debug("Received {0} status after {1}ms from {2}: {3}", response.StatusCode, duration.TotalMilliseconds, "GET", url);

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            var item = await responseString.DeserialiseAsync<TReturnType>();

            return item;
        }

        private async Task<TReturnType> PostResponse<TReturnType>(string endPoint, string methodParams, string requestBody, CancellationToken cancellationToken = default(CancellationToken), [CallerMemberName] string callingMethod = "")
        {
            Logger.Debug(callingMethod);
            var url = GetUrl(endPoint, methodParams);

            Logger.Debug("POST: {0}", url);
            var requestTime = DateTime.Now;

            var response = await HttpClient.PostAsync(url, new StringContent(""), cancellationToken);
            var duration = DateTime.Now - requestTime;

            Logger.Debug("Received {0} status after {1}ms from {2}: {3}", response.StatusCode, duration.TotalMilliseconds, "POST", url);

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            var item = await responseString.DeserialiseAsync<TReturnType>();

            return item;
        }
        #endregion

        private string GetUrl(string endPoint, string methodParams)
        {
            return string.Format(BaseUrlFormat, ApiKey, endPoint, methodParams);
        }
    }

    public enum MovieIdentificationType
    {
        [Description("movie_id")]
        FollwIt,
        [Description("imdb_id")]
        Imdb,
        [Description("tmdb_id")]
        Tmdb
    }

    public enum ShowIdentificationType
    {
        [Description("movie_id")]
        FollwIt,
        [Description("imdb_id")]
        Imdb,
        [Description("tvdb_id")]
        Tvdb
    }

    public enum TimeInterval
    {
        Week,
        Month,
        AllTime,
        NewShows
    }
}
