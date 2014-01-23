using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using FollwItPortable.Converters;
using FollwItPortable.Extensions;
using FollwItPortable.Logging;
using FollwItPortable.Model;
using FollwItPortable.Model.Requests;
using FollwItPortable.Model.Responses;

namespace FollwItPortable
{
    /// <summary>
    /// 
    /// </summary>
    public class FollwItClient
    {
        private const string BaseUrlFormat = "http://follw.it/api/3/{0}/{1}/{2}";
        internal const string DateFormat = "YYYY-mm-dd";

        #region Public Properties
        
        /// <summary>
        /// Gets the HTTP client.
        /// </summary>
        /// <value>
        /// The HTTP client.
        /// </value>
        public HttpClient HttpClient { get; private set; }
        
        /// <summary>
        /// Gets the API key.
        /// </summary>
        /// <value>
        /// The API key.
        /// </value>
        public string ApiKey { get; private set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username
        {
            get { return RequestManager.Username; }
            set { RequestManager.Username = value; }
        }

        /// <summary>
        /// Sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password
        {
            set { RequestManager.Password = value.Hash(); }
        }
        #endregion

        internal readonly ILogger Logger;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FollwItClient"/> class.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        public FollwItClient(string apiKey)
            : this(apiKey, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FollwItClient"/> class.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        /// <param name="handler">The handler.</param>
        public FollwItClient(string apiKey, HttpMessageHandler handler)
            : this (apiKey, handler, new NullLogger())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FollwItClient"/> class.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        /// <param name="handler">The handler.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="System.ArgumentNullException">apiKey;API key cannot be null or empty</exception>
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
        /// <summary>
        /// Authenticates the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>True if authenticated</returns>
        /// <exception cref="System.ArgumentNullException">
        /// username;Username cannot be null or empty
        /// or
        /// password;Password cannot be null or empty
        /// </exception>
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

            var response = await PostResponse<AuthenticationResponse>(PostMethods.UserAuthenticate, await request.SerialiseAsync(), cancellationToken: cancellationToken);

            return response.Response.ToLower().Equals("success");
        }
        #endregion

        #region Get Calendar Method

        /// <summary>
        /// Gets the popular episodes.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="locale">The locale.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of episodes</returns>
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
        /// <summary>
        /// Gets the similar movies asynchronous.
        /// </summary>
        /// <param name="identificationType">Type of the identification.</param>
        /// <param name="movieId">The movie identifier.</param>
        /// <param name="locale">The locale.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">movieId;MovieID cannot be null or empty</exception>
        public async Task<List<FollwItMovie>> GetSimilarMoviesAsync(MovieIdentificationType identificationType, string movieId, string locale = "en", CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(movieId))
            {
                throw new ArgumentNullException("movieId", "MovieID cannot be null or empty");
            }

            var methodParams = string.Format("{0}/{1}/{2}", identificationType.GetDescription(), movieId, locale);

            return await GetResponse<List<FollwItMovie>>(GetMethods.MovieSimilarMovies, methodParams, cancellationToken);
        }

        /// <summary>
        /// Gets the movie details asynchronous.
        /// </summary>
        /// <param name="identificationType">Type of the identification.</param>
        /// <param name="movieId">The movie identifier.</param>
        /// <param name="locale">The locale.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">movieId;MovieID cannot be null or empty</exception>
        public async Task<FollwItMovie> GetMovieDetailsAsync(MovieIdentificationType identificationType, string movieId, string locale = "en", CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(movieId))
            {
                throw new ArgumentNullException("movieId", "MovieID cannot be null or empty");
            }

            var methodParams = string.Format("{0}/{1}/{2}", identificationType.GetDescription(), movieId, locale);

            return await GetResponse<FollwItMovie>(GetMethods.MovieSimilarMovies, methodParams, cancellationToken);
        }

        /// <summary>
        /// Gets the trending movies asynchronous.
        /// </summary>
        /// <param name="timeInterval">The time interval.</param>
        /// <param name="locale">The locale.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">TimeInterval cannot be NewShow for Movies</exception>
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
        /// <summary>
        /// Gets the show details asynchronous.
        /// </summary>
        /// <param name="identificationType">Type of the identification.</param>
        /// <param name="showId">The show identifier.</param>
        /// <param name="includeEpisodes">if set to <c>true</c> [include episodes].</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">showId;ShowID cannot be null or empty</exception>
        public async Task<FollwItTvShow> GetShowDetailsAsync(ShowIdentificationType identificationType, string showId, bool includeEpisodes = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(showId))
            {
                throw new ArgumentNullException("showId", "ShowID cannot be null or empty");
            }

            var methodParams = string.Format("{0}/{1}/{2}", identificationType.GetDescription(), showId, includeEpisodes);

            return await GetResponse<FollwItTvShow>(GetMethods.ShowSummary, methodParams, cancellationToken);
        }

        /// <summary>
        /// Gets the trending shows asynchronous.
        /// </summary>
        /// <param name="timeInterval">The time interval.</param>
        /// <param name="locale">The locale.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<List<FollwItTvShow>> GetTrendingShowsAsync(TimeInterval timeInterval, string locale = "en", int limit = 20, CancellationToken cancellationToken = default(CancellationToken))
        {
            var interval = timeInterval.ToString().ToLower();
            var methodParams = string.Format("{0}/{1}/{2}", interval, locale, limit);

            return await GetResponse<List<FollwItTvShow>>(GetMethods.ShowTrending, methodParams, cancellationToken);
        } 
        #endregion

        #region Get User Methods
        /// <summary>
        /// Gets the list asynchronous.
        /// </summary>
        /// <param name="listId">The list identifier.</param>
        /// <param name="username">The username.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">listId;ListID cannot be null or empty</exception>
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

        /// <summary>
        /// Gets the user lists asynchronous.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<List<FollwItList>> GetUserListsAsync(string username = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(username))
            {
                username = Username;
            }

            return await GetResponse<List<FollwItList>>(GetMethods.UserLists, username, cancellationToken);
        }

        /// <summary>
        /// Gets the user movie collection asynchronous.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<List<FollwItMovieSummary>> GetUserMovieCollectionAsync(string username = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(username))
            {
                username = Username;
            }

            return await GetResponse<List<FollwItMovieSummary>>(GetMethods.UserMovieCollection, username, cancellationToken);
        }

        /// <summary>
        /// Gets the user tv collection asynchronous.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="includeEpisodes">if set to <c>true</c> [include episodes].</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<List<FollwItTvShow>> GetUserTvCollectionAsync(string username = null, bool includeEpisodes = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(username))
            {
                username = Username;
            }

            var methodParams = string.Format("{0}/{1}", username, includeEpisodes);

            return await GetResponse<List<FollwItTvShow>>(GetMethods.UserTvCollection, methodParams, cancellationToken);
        }

        /// <summary>
        /// Gets the public profile asynchronous.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<FollwItUser> GetPublicProfileAsync(string username = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(username))
            {
                username = Username;
            }

            return await GetResponse<FollwItUser>(GetMethods.UserPublicProfile, username, cancellationToken);
        }

        /// <summary>
        /// Gets the username available asynchronous.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">username;Username cannot be null or empty</exception>
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

        #region Post Calendar Method

        /// <summary>
        /// Gets the user callendar follwing asynchronous.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="locale">The locale.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of episodes</returns>
        public async Task<List<FollwItEpisode>> GetUserCallendarFollwingAsync(DateTime? startDate = null, DateTime? endDate = null, string locale = "en", CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!startDate.HasValue)
            {
                startDate = DateTime.Now.Date;
            }

            if (!endDate.HasValue)
            {
                endDate = startDate.Value.AddDays(7);
            }

            var request = RequestManager.CreateRequestType<CalendarFollwingRequest>();
            request.StartDate = startDate.Value;
            request.EndDate = endDate.Value;
            request.Locale = locale;
            var requestString = await request.SerialiseAsync(new DateConverter());

            return await PostResponse<List<FollwItEpisode>>(PostMethods.CalendarFollowing, requestString, cancellationToken);
        } 
        #endregion

        #region Post Episode Methods

        /// <summary>
        /// Bulks the change episodes asynchronous.
        /// </summary>
        /// <param name="episodes">The episodes.</param>
        /// <param name="inCollection">The in collection.</param>
        /// <param name="watched">The watched.</param>
        /// <param name="rating">The rating.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<List<EpisodeResponse>> BulkChangeEpisodesAsync(List<FollwItEpisode> episodes, bool? inCollection = null, bool? watched = null, int? rating = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (episodes.IsNullOrEmpty())
            {
                return new List<EpisodeResponse>();
            }

            var request = RequestManager.CreateRequestType<BulkEpisodeChangeRequest>();
            request.Episodes = episodes;
            request.InCollection = inCollection;
            request.Watched = watched;
            request.Rating = rating;
            var requestString = await request.SerialiseAsync();

            return await PostResponse<List<EpisodeResponse>>(PostMethods.EpisodeBulkAction, requestString, cancellationToken);
        }

        /// <summary>
        /// Adds the episode to collection asynchronous.
        /// </summary>
        /// <param name="episode">The episode.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">episode;Episode cannot be null</exception>
        public async Task<bool> AddEpisodeToCollectionAsync(FollwItEpisode episode, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (episode == null)
            {
                throw new ArgumentNullException("episode", "Episode cannot be null");
            }

            var request = RequestManager.CreateRequestType<EpisodeCollectionRequest>();
            request.EpisodeId = episode.FollwitEpisodeId;

            return await PostEpisodeCollectionInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Adds the epiosde to collection asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="identificationType">Type of the identification.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Imdb is not a valid identification type for this method</exception>
        public async Task<bool> AddEpiosdeToCollectionAsync(int id, ShowIdentificationType identificationType, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = RequestManager.CreateRequestType<EpisodeCollectionRequest>();
            switch (identificationType)
            {
                case ShowIdentificationType.Imdb:
                    throw new InvalidOperationException("Imdb is not a valid identification type for this method");
                case ShowIdentificationType.FollwIt:
                    request.EpisodeId = id;
                    break;
                case ShowIdentificationType.Tvdb:
                    request.TvdbEpisodeId = id;
                    break;
            }

            return await PostEpisodeCollectionInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Adds the episode to collection asynchronous.
        /// </summary>
        /// <param name="tvdbId">The TVDB identifier.</param>
        /// <param name="seasonNumber">The season number.</param>
        /// <param name="episodeNumber">The episode number.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<bool> AddEpisodeToCollectionAsync(int tvdbId, int seasonNumber, int episodeNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = RequestManager.CreateRequestType<EpisodeCollectionRequest>();
            request.TvdbEpisodeId = tvdbId;
            request.SeasonNumber = seasonNumber;
            request.EpisodeNumber = episodeNumber;

            return await PostEpisodeCollectionInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Adds the episode to collection asynchronous.
        /// </summary>
        /// <param name="seriesName">Name of the series.</param>
        /// <param name="seasonNumber">The season number.</param>
        /// <param name="episodeNumber">The episode number.</param>
        /// <param name="episodeName">Name of the episode.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// seriesName;Series Name cannot be null or empty
        /// or
        /// episodeName;Episode name cannot be null or empty
        /// </exception>
        public async Task<bool> AddEpisodeToCollectionAsync(string seriesName, int seasonNumber, int episodeNumber, string episodeName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(seriesName))
            {
                throw new ArgumentNullException("seriesName", "Series Name cannot be null or empty");
            }

            if (string.IsNullOrEmpty(episodeName))
            {
                throw new ArgumentNullException("episodeName", "Episode name cannot be null or empty");
            }

            var request = RequestManager.CreateRequestType<EpisodeCollectionRequest>();
            request.SeriesName = seriesName;
            request.EpisodeName = episodeName;
            request.SeasonNumber = seasonNumber;
            request.EpisodeNumber = episodeNumber;

            return await PostEpisodeCollectionInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Adds the episode to list asynchronous.
        /// </summary>
        /// <param name="episode">The episode.</param>
        /// <param name="listId">The list identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// episode;Episode cannot be null
        /// or
        /// listId;ListID cannot be null or empty
        /// </exception>
        public async Task<bool> AddEpisodeToListAsync(FollwItEpisode episode, string listId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (episode == null)
            {
                throw new ArgumentNullException("episode", "Episode cannot be null");
            }

            if (string.IsNullOrEmpty(listId))
            {
                throw new ArgumentNullException("listId", "ListID cannot be null or empty");
            }

            var request = RequestManager.CreateRequestType<EpisodeListRequest>();
            request.EpisodeId = episode.FollwitEpisodeId;
            request.ListId = listId;

            return await PostEpisodeListInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Adds the epiosde to list asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="identificationType">Type of the identification.</param>
        /// <param name="listId">The list identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">listId;ListID cannot be null or empty</exception>
        /// <exception cref="System.InvalidOperationException">Imdb is not a valid identification type for this method</exception>
        public async Task<bool> AddEpiosdeToListAsync(int id, ShowIdentificationType identificationType, string listId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(listId))
            {
                throw new ArgumentNullException("listId", "ListID cannot be null or empty");
            }

            var request = RequestManager.CreateRequestType<EpisodeListRequest>();
            switch (identificationType)
            {
                case ShowIdentificationType.Imdb:
                    throw new InvalidOperationException("Imdb is not a valid identification type for this method");
                case ShowIdentificationType.FollwIt:
                    request.EpisodeId = id;
                    break;
                case ShowIdentificationType.Tvdb:
                    request.TvdbEpisodeId = id;
                    break;
            }

            request.ListId = listId;

            return await PostEpisodeListInternalAsync(request, cancellationToken);
        }


        /// <summary>
        /// Adds the episode to list asynchronous.
        /// </summary>
        /// <param name="tvdbId">The TVDB identifier.</param>
        /// <param name="seasonNumber">The season number.</param>
        /// <param name="episodeNumber">The episode number.</param>
        /// <param name="listId">The list identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">listId;ListID cannot be null or empty</exception>
        public async Task<bool> AddEpisodeToListAsync(int tvdbId, int seasonNumber, int episodeNumber, string listId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(listId))
            {
                throw new ArgumentNullException("listId", "ListID cannot be null or empty");
            }

            var request = RequestManager.CreateRequestType<EpisodeListRequest>();
            request.TvdbEpisodeId = tvdbId;
            request.SeasonNumber = seasonNumber;
            request.EpisodeNumber = episodeNumber;
            request.ListId = listId;

            return await PostEpisodeListInternalAsync(request, cancellationToken);
        }


        /// <summary>
        /// Adds the episode to list asynchronous.
        /// </summary>
        /// <param name="seriesName">Name of the series.</param>
        /// <param name="seasonNumber">The season number.</param>
        /// <param name="episodeNumber">The episode number.</param>
        /// <param name="episodeName">Name of the episode.</param>
        /// <param name="listId">The list identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// seriesName;Series Name cannot be null or empty
        /// or
        /// episodeName;Episode name cannot be null or empty
        /// or
        /// listId;ListID cannot be null or empty
        /// </exception>
        public async Task<bool> AddEpisodeToListAsync(string seriesName, int seasonNumber, int episodeNumber, string episodeName, string listId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(seriesName))
            {
                throw new ArgumentNullException("seriesName", "Series Name cannot be null or empty");
            }

            if (string.IsNullOrEmpty(episodeName))
            {
                throw new ArgumentNullException("episodeName", "Episode name cannot be null or empty");
            }

            if (string.IsNullOrEmpty(listId))
            {
                throw new ArgumentNullException("listId", "ListID cannot be null or empty");
            }

            var request = RequestManager.CreateRequestType<EpisodeListRequest>();
            request.SeriesName = seriesName;
            request.EpisodeName = episodeName;
            request.SeasonNumber = seasonNumber;
            request.EpisodeNumber = episodeNumber;
            request.ListId = listId;

            return await PostEpisodeListInternalAsync(request, cancellationToken);
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

        private async Task<TReturnType> PostResponse<TReturnType>(string endPoint, string requestBody, CancellationToken cancellationToken = default(CancellationToken), [CallerMemberName] string callingMethod = "")
        {
            Logger.Debug(callingMethod);
            var url = GetUrl(endPoint, string.Empty);

            Logger.Debug("POST: {0}", url);
            var requestTime = DateTime.Now;

            var response = await HttpClient.PostAsync(url, new StringContent(requestBody), cancellationToken);
            var duration = DateTime.Now - requestTime;

            Logger.Debug("Received {0} status after {1}ms from {2}: {3}", response.StatusCode, duration.TotalMilliseconds, "POST", url);

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            var item = await responseString.DeserialiseAsync<TReturnType>();

            return item;
        }
        #endregion

        #region Private Methods
        private string GetUrl(string endPoint, string methodParams)
        {
            return string.Format(BaseUrlFormat, ApiKey, endPoint, methodParams);
        }

        private async Task<bool> PostEpisodeCollectionInternalAsync(EpisodeCollectionRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            var requestString = await request.SerialiseAsync();

            var response = await PostResponse<string>(PostMethods.EpisodeCollection, requestString, cancellationToken);

            return response.ToLower().Contains("success");
        }

        private async Task<bool> PostEpisodeListInternalAsync(EpisodeListRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            var requestString = await request.SerialiseAsync();

            var response = await PostResponse<string>(PostMethods.EpisodeList, requestString, cancellationToken);

            return response.ToLower().Contains("success");
        }
        #endregion
    }
}
