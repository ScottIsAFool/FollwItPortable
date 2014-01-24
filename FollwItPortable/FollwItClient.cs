using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Gets the similar movies.
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
        /// Gets the movie details.
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
        /// Gets the trending movies.
        /// </summary>
        /// <param name="timeInterval">The time interval.</param>
        /// <param name="locale">The locale.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">TimeInterval cannot be NewShow for Movies</exception>
        public async Task<List<FollwItMovie>> GetTrendingMoviesAsync(TimeInterval timeInterval, string locale = "en", int limit = 20, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (timeInterval == TimeInterval.NewShows)
            {
                throw new InvalidOperationException("TimeInterval cannot be NewShow for Movies");
            }

            var interval = timeInterval.ToString().ToLower();

            var methodParams = string.Format("{0}/{1}/{2}", interval, locale, limit);

            return await GetResponse<List<FollwItMovie>>(GetMethods.MovieTrending, methodParams, cancellationToken);
        }
        #endregion

        #region Get Show Methods
        /// <summary>
        /// Gets the show details.
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
        /// Gets the trending shows.
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
        /// Gets the list.
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
        /// Gets the user lists.
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
        /// Gets the user movie collection.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<List<FollwItMovie>> GetUserMovieCollectionAsync(string username = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(username))
            {
                username = Username;
            }

            return await GetResponse<List<FollwItMovie>>(GetMethods.UserMovieCollection, username, cancellationToken);
        }

        /// <summary>
        /// Gets the user tv collection.
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
        /// Gets the public profile.
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
        /// Gets the username available.
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
        /// Gets the user callendar follwing.
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
        /// Bulks the change episodes.
        /// </summary>
        /// <param name="episodes">The episodes.</param>
        /// <param name="inCollection">The in collection.</param>
        /// <param name="watched">The watched.</param>
        /// <param name="rating">The rating.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<List<BulkEpisodeResponse>> BulkChangeEpisodesAsync(List<FollwItEpisode> episodes, bool? inCollection = null, bool? watched = null, int? rating = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (episodes.IsNullOrEmpty())
            {
                return new List<BulkEpisodeResponse>();
            }

            var request = RequestManager.CreateRequestType<BulkEpisodeChangeRequest>();
            request.Episodes = episodes;
            request.InCollection = inCollection;
            request.Watched = watched;
            request.Rating = rating;
            var requestString = await request.SerialiseAsync();

            return await PostResponse<List<BulkEpisodeResponse>>(PostMethods.EpisodeBulkAction, requestString, cancellationToken);
        }

        /// <summary>
        /// Adds the episode to collection.
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

            var request = RequestManager.CreateRequestType<EpisodeBaseRequest>();
            request.EpisodeId = episode.FollwitEpisodeId;

            return await PostEpisodeCollectionInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Adds the epiosde to collection.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="identificationType">Type of the identification.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Imdb is not a valid identification type for this method</exception>
        public async Task<bool> AddEpisodeToCollectionAsync(int id, ShowIdentificationType identificationType, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = RequestManager.CreateRequestType<EpisodeBaseRequest>();
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
        /// Adds the episode to collection.
        /// </summary>
        /// <param name="tvdbId">The TVDB identifier.</param>
        /// <param name="seasonNumber">The season number.</param>
        /// <param name="episodeNumber">The episode number.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<bool> AddEpisodeToCollectionAsync(int tvdbId, int seasonNumber, int episodeNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = RequestManager.CreateRequestType<EpisodeBaseRequest>();
            request.TvdbEpisodeId = tvdbId;
            request.SeasonNumber = seasonNumber;
            request.EpisodeNumber = episodeNumber;

            return await PostEpisodeCollectionInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Adds the episode to collection.
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

            var request = RequestManager.CreateRequestType<EpisodeBaseRequest>();
            request.SeriesName = seriesName;
            request.EpisodeName = episodeName;
            request.SeasonNumber = seasonNumber;
            request.EpisodeNumber = episodeNumber;

            return await PostEpisodeCollectionInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Adds the episode to list.
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
        /// Adds the epiosde to list.
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
        /// Adds the episode to list.
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
        /// Adds the episode to list.
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

        /// <summary>
        /// Changes the episode rating.
        /// </summary>
        /// <param name="episode">The episode.</param>
        /// <param name="rating">The rating.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">episode;Episode cannot be null</exception>
        public async Task<bool> ChangeEpisodeRatingAsync(FollwItEpisode episode, int rating, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (episode == null)
            {
                throw new ArgumentNullException("episode", "Episode cannot be null");
            }

            var request = RequestManager.CreateRequestType<EpisodeRatingRequest>();
            request.EpisodeId = episode.FollwitEpisodeId;
            request.Rating = rating;

            return await PostEpisodeRatingInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Changes the episode rating.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="identificationType">Type of the identification.</param>
        /// <param name="rating">The rating.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Imdb is not a valid identification type for this method</exception>
        public async Task<bool> ChangeEpisodeRatingAsync(int id, ShowIdentificationType identificationType, int rating, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = RequestManager.CreateRequestType<EpisodeRatingRequest>();
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

            request.Rating = rating;

            return await PostEpisodeRatingInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Changes the episode rating.
        /// </summary>
        /// <param name="tvdbId">The TVDB identifier.</param>
        /// <param name="seasonNumber">The season number.</param>
        /// <param name="episodeNumber">The episode number.</param>
        /// <param name="rating">The rating.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<bool> ChangeEpisodeRatingAsync(int tvdbId, int seasonNumber, int episodeNumber, int rating, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = RequestManager.CreateRequestType<EpisodeRatingRequest>();
            request.TvdbEpisodeId = tvdbId;
            request.SeasonNumber = seasonNumber;
            request.EpisodeNumber = episodeNumber;
            request.Rating = rating;

            return await PostEpisodeRatingInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Changes the episode rating.
        /// </summary>
        /// <param name="seriesName">Name of the series.</param>
        /// <param name="seasonNumber">The season number.</param>
        /// <param name="episodeNumber">The episode number.</param>
        /// <param name="episodeName">Name of the episode.</param>
        /// <param name="rating">The rating.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// seriesName;Series Name cannot be null or empty
        /// or
        /// episodeName;Episode name cannot be null or empty
        /// </exception>
        public async Task<bool> ChangeEpisodeRatingAsync(string seriesName, int seasonNumber, int episodeNumber, string episodeName, int rating, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(seriesName))
            {
                throw new ArgumentNullException("seriesName", "Series Name cannot be null or empty");
            }

            if (string.IsNullOrEmpty(episodeName))
            {
                throw new ArgumentNullException("episodeName", "Episode name cannot be null or empty");
            }

            var request = RequestManager.CreateRequestType<EpisodeRatingRequest>();
            request.SeriesName = seriesName;
            request.EpisodeName = episodeName;
            request.SeasonNumber = seasonNumber;
            request.EpisodeNumber = episodeNumber;
            request.Rating = rating;

            return await PostEpisodeRatingInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Gets the episode details.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="identificationType">Type of the identification.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Imdb is not a valid identification type for this method</exception>
        public async Task<FollwItEpisode> GetEpisodeDetailsAsync(int id, ShowIdentificationType identificationType, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = RequestManager.CreateRequestType<EpisodeBaseRequest>();
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

            return await PostResponse<FollwItEpisode>(PostMethods.EpisodeSummary, await request.SerialiseAsync(), cancellationToken);
        }

        /// <summary>
        /// Gets the episode details.
        /// </summary>
        /// <param name="tvdbId">The TVDB identifier.</param>
        /// <param name="seasonNumber">The season number.</param>
        /// <param name="episodeNumber">The episode number.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<FollwItEpisode> GetEpisodeDetailsAsync(int tvdbId, int seasonNumber, int episodeNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = RequestManager.CreateRequestType<EpisodeBaseRequest>();
            request.TvdbEpisodeId = tvdbId;
            request.SeasonNumber = seasonNumber;
            request.EpisodeNumber = episodeNumber;

            return await PostResponse<FollwItEpisode>(PostMethods.EpisodeSummary, await request.SerialiseAsync(), cancellationToken);
        }

        /// <summary>
        /// Gets the episode details.
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
        public async Task<FollwItEpisode> GetEpisodeDetailsAsync(string seriesName, int seasonNumber, int episodeNumber, string episodeName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(seriesName))
            {
                throw new ArgumentNullException("seriesName", "Series Name cannot be null or empty");
            }

            if (string.IsNullOrEmpty(episodeName))
            {
                throw new ArgumentNullException("episodeName", "Episode name cannot be null or empty");
            }

            var request = RequestManager.CreateRequestType<EpisodeBaseRequest>();
            request.SeriesName = seriesName;
            request.EpisodeName = episodeName;
            request.SeasonNumber = seasonNumber;
            request.EpisodeNumber = episodeNumber;

            return await PostResponse<FollwItEpisode>(PostMethods.EpisodeSummary, await request.SerialiseAsync(), cancellationToken);
        }

        /// <summary>
        /// Removes the episode from collection.
        /// </summary>
        /// <param name="episode">The episode.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">episode;Episode cannot be null</exception>
        public async Task<bool> RemoveEpisodeFromCollectionAsync(FollwItEpisode episode, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (episode == null)
            {
                throw new ArgumentNullException("episode", "Episode cannot be null");
            }

            var request = RequestManager.CreateRequestType<EpisodeBaseRequest>();
            request.EpisodeId = episode.FollwitEpisodeId;

            return await PostEpisodeUncollectionInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Removes the episode from collection.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="identificationType">Type of the identification.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Imdb is not a valid identification type for this method</exception>
        public async Task<bool> RemoveEpisodeFromCollectionAsync(int id, ShowIdentificationType identificationType, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = RequestManager.CreateRequestType<EpisodeBaseRequest>();
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

            return await PostEpisodeUncollectionInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Removes the episode from collection.
        /// </summary>
        /// <param name="tvdbId">The TVDB identifier.</param>
        /// <param name="seasonNumber">The season number.</param>
        /// <param name="episodeNumber">The episode number.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<bool> RemoveEpisodeFromCollectionAsync(int tvdbId, int seasonNumber, int episodeNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = RequestManager.CreateRequestType<EpisodeBaseRequest>();
            request.TvdbEpisodeId = tvdbId;
            request.SeasonNumber = seasonNumber;
            request.EpisodeNumber = episodeNumber;

            return await PostEpisodeUncollectionInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Removes the episode from collection.
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
        public async Task<bool> RemoveEpisodeFromCollectionAsync(string seriesName, int seasonNumber, int episodeNumber, string episodeName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(seriesName))
            {
                throw new ArgumentNullException("seriesName", "Series Name cannot be null or empty");
            }

            if (string.IsNullOrEmpty(episodeName))
            {
                throw new ArgumentNullException("episodeName", "Episode name cannot be null or empty");
            }

            var request = RequestManager.CreateRequestType<EpisodeBaseRequest>();
            request.SeriesName = seriesName;
            request.EpisodeName = episodeName;
            request.SeasonNumber = seasonNumber;
            request.EpisodeNumber = episodeNumber;

            return await PostEpisodeUncollectionInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Removes the episode from list.
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
        public async Task<bool> RemoveEpisodeFromListAsync(FollwItEpisode episode, string listId, CancellationToken cancellationToken = default(CancellationToken))
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

            return await PostEpisodeUnlistInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Removes the episode from list.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="identificationType">Type of the identification.</param>
        /// <param name="listId">The list identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">listId;ListID cannot be null or empty</exception>
        /// <exception cref="System.InvalidOperationException">Imdb is not a valid identification type for this method</exception>
        public async Task<bool> RemoveEpisodeFromListAsync(int id, ShowIdentificationType identificationType, string listId, CancellationToken cancellationToken = default(CancellationToken))
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

            return await PostEpisodeUnlistInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Removes the episode from list.
        /// </summary>
        /// <param name="tvdbId">The TVDB identifier.</param>
        /// <param name="seasonNumber">The season number.</param>
        /// <param name="episodeNumber">The episode number.</param>
        /// <param name="listId">The list identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">listId;ListID cannot be null or empty</exception>
        public async Task<bool> RemoveEpisodeFromListAsync(int tvdbId, int seasonNumber, int episodeNumber, string listId, CancellationToken cancellationToken = default(CancellationToken))
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

            return await PostEpisodeUnlistInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Removes the episode from list.
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
        public async Task<bool> RemoveEpisodeFromListAsync(string seriesName, int seasonNumber, int episodeNumber, string episodeName, string listId, CancellationToken cancellationToken = default(CancellationToken))
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

            return await PostEpisodeUnlistInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Marks the episode as watched.
        /// </summary>
        /// <param name="episode">The episode.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">episode;Episode cannot be null</exception>
        public async Task<bool> MarkEpisodeAsWatchedAsync(FollwItEpisode episode, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (episode == null)
            {
                throw new ArgumentNullException("episode", "Episode cannot be null");
            }

            var request = RequestManager.CreateRequestType<EpisodeWatchedRequest>();
            request.EpisodeId = episode.FollwitEpisodeId;

            return await PostEpisodeWatchedInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Marks the episode as watched.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="identificationType">Type of the identification.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Imdb is not a valid identification type for this method</exception>
        public async Task<bool> MarkEpisodeAsWatchedAsync(int id, ShowIdentificationType identificationType, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = RequestManager.CreateRequestType<EpisodeWatchedRequest>();
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

            return await PostEpisodeWatchedInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Marks the episode as watched.
        /// </summary>
        /// <param name="tvdbId">The TVDB episode identifier.</param>
        /// <param name="seasonNumber">The season number.</param>
        /// <param name="episodeNumber">The episode number.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<bool> MarkEpisodeAsWatchedAsync(int tvdbId, int seasonNumber, int episodeNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = RequestManager.CreateRequestType<EpisodeWatchedRequest>();
            request.TvdbEpisodeId = tvdbId;
            request.SeasonNumber = seasonNumber;
            request.EpisodeNumber = episodeNumber;

            return await PostEpisodeWatchedInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Marks the episode as watched.
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
        public async Task<bool> MarkEpisodeAsWatchedAsync(string seriesName, int seasonNumber, int episodeNumber, string episodeName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(seriesName))
            {
                throw new ArgumentNullException("seriesName", "Series Name cannot be null or empty");
            }

            if (string.IsNullOrEmpty(episodeName))
            {
                throw new ArgumentNullException("episodeName", "Episode name cannot be null or empty");
            }

            var request = RequestManager.CreateRequestType<EpisodeWatchedRequest>();
            request.SeriesName = seriesName;
            request.EpisodeName = episodeName;
            request.SeasonNumber = seasonNumber;
            request.EpisodeNumber = episodeNumber;

            return await PostEpisodeWatchedInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Marks the episode as unwatched.
        /// </summary>
        /// <param name="episode">The episode.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">episode;Episode cannot be null</exception>
        public async Task<bool> MarkEpisodeAsUnwatchedAsync(FollwItEpisode episode, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (episode == null)
            {
                throw new ArgumentNullException("episode", "Episode cannot be null");
            }

            var request = RequestManager.CreateRequestType<EpisodeBaseRequest>();
            request.EpisodeId = episode.FollwitEpisodeId;

            return await PostEpisodeUnwatchedInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Marks the episode as unwatched.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="identificationType">Type of the identification.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Imdb is not a valid identification type for this method</exception>
        public async Task<bool> MarkEpisodeAsUnwatchedAsync(int id, ShowIdentificationType identificationType, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = RequestManager.CreateRequestType<EpisodeBaseRequest>();
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

            return await PostEpisodeUnwatchedInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Marks the episode as unwatched.
        /// </summary>
        /// <param name="tvdbId">The TVDB identifier.</param>
        /// <param name="seasonNumber">The season number.</param>
        /// <param name="episodeNumber">The episode number.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<bool> MarkEpisodeAsUnwatchedAsync(int tvdbId, int seasonNumber, int episodeNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = RequestManager.CreateRequestType<EpisodeBaseRequest>();
            request.TvdbEpisodeId = tvdbId;
            request.SeasonNumber = seasonNumber;
            request.EpisodeNumber = episodeNumber;

            return await PostEpisodeUnwatchedInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Marks the episode as unwatched.
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
        public async Task<bool> MarkEpisodeAsUnwatchedAsync(string seriesName, int seasonNumber, int episodeNumber, string episodeName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(seriesName))
            {
                throw new ArgumentNullException("seriesName", "Series Name cannot be null or empty");
            }

            if (string.IsNullOrEmpty(episodeName))
            {
                throw new ArgumentNullException("episodeName", "Episode name cannot be null or empty");
            }

            var request = RequestManager.CreateRequestType<EpisodeBaseRequest>();
            request.SeriesName = seriesName;
            request.EpisodeName = episodeName;
            request.SeasonNumber = seasonNumber;
            request.EpisodeNumber = episodeNumber;

            return await PostEpisodeUnwatchedInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Marks the episode as watching.
        /// </summary>
        /// <param name="episode">The episode.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">episode;Episode cannot be null</exception>
        public async Task<bool> MarkEpisodeAsWatchingAsync(FollwItEpisode episode, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (episode == null)
            {
                throw new ArgumentNullException("episode", "Episode cannot be null");
            }

            var request = RequestManager.CreateRequestType<EpisodeBaseRequest>();
            request.EpisodeId = episode.FollwitEpisodeId;

            return await PostEpisodeWatchingInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Marks the episode as watching.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="identificationType">Type of the identification.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Imdb is not a valid identification type for this method</exception>
        public async Task<bool> MarkEpisodeAsWatchingAsync(int id, ShowIdentificationType identificationType, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = RequestManager.CreateRequestType<EpisodeBaseRequest>();
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

            return await PostEpisodeWatchingInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Marks the episode as watching.
        /// </summary>
        /// <param name="tvdbId">The TVDB identifier.</param>
        /// <param name="seasonNumber">The season number.</param>
        /// <param name="episodeNumber">The episode number.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<bool> MarkEpisodeAsWatchingAsync(int tvdbId, int seasonNumber, int episodeNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = RequestManager.CreateRequestType<EpisodeBaseRequest>();
            request.TvdbEpisodeId = tvdbId;
            request.SeasonNumber = seasonNumber;
            request.EpisodeNumber = episodeNumber;

            return await PostEpisodeWatchingInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Marks the episode as watching.
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
        public async Task<bool> MarkEpisodeAsWatchingAsync(string seriesName, int seasonNumber, int episodeNumber, string episodeName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(seriesName))
            {
                throw new ArgumentNullException("seriesName", "Series Name cannot be null or empty");
            }

            if (string.IsNullOrEmpty(episodeName))
            {
                throw new ArgumentNullException("episodeName", "Episode name cannot be null or empty");
            }

            var request = RequestManager.CreateRequestType<EpisodeBaseRequest>();
            request.SeriesName = seriesName;
            request.EpisodeName = episodeName;
            request.SeasonNumber = seasonNumber;
            request.EpisodeNumber = episodeNumber;

            return await PostEpisodeWatchingInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Marks the episode as not watching.
        /// </summary>
        /// <param name="episode">The episode.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">episode;Episode cannot be null</exception>
        public async Task<bool> MarkEpisodeAsNotWatchingAsync(FollwItEpisode episode, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (episode == null)
            {
                throw new ArgumentNullException("episode", "Episode cannot be null");
            }

            var request = RequestManager.CreateRequestType<EpisodeBaseRequest>();
            request.EpisodeId = episode.FollwitEpisodeId;

            return await PostEpisodeUnwatchingInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Marks the episode as not watching.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="identificationType">Type of the identification.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Imdb is not a valid identification type for this method</exception>
        public async Task<bool> MarkEpisodeAsNotWatchingAsync(int id, ShowIdentificationType identificationType, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = RequestManager.CreateRequestType<EpisodeBaseRequest>();
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

            return await PostEpisodeUnwatchingInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Marks the episode as not watching.
        /// </summary>
        /// <param name="tvdbId">The TVDB identifier.</param>
        /// <param name="seasonNumber">The season number.</param>
        /// <param name="episodeNumber">The episode number.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<bool> MarkEpisodeAsNotWatchingAsync(int tvdbId, int seasonNumber, int episodeNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = RequestManager.CreateRequestType<EpisodeBaseRequest>();
            request.TvdbEpisodeId = tvdbId;
            request.SeasonNumber = seasonNumber;
            request.EpisodeNumber = episodeNumber;

            return await PostEpisodeUnwatchingInternalAsync(request, cancellationToken);
        }

        /// <summary>
        /// Marks the episode as not watching.
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
        public async Task<bool> MarkEpisodeAsNotWatchingAsync(string seriesName, int seasonNumber, int episodeNumber, string episodeName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(seriesName))
            {
                throw new ArgumentNullException("seriesName", "Series Name cannot be null or empty");
            }

            if (string.IsNullOrEmpty(episodeName))
            {
                throw new ArgumentNullException("episodeName", "Episode name cannot be null or empty");
            }

            var request = RequestManager.CreateRequestType<EpisodeBaseRequest>();
            request.SeriesName = seriesName;
            request.EpisodeName = episodeName;
            request.SeasonNumber = seasonNumber;
            request.EpisodeNumber = episodeNumber;

            return await PostEpisodeUnwatchingInternalAsync(request, cancellationToken);
        }
        #endregion

        #region Post Movie Methods
        /// <summary>
        /// Bulks the change movies.
        /// </summary>
        /// <param name="movies">The movies.</param>
        /// <param name="inCollection">The in collection.</param>
        /// <param name="watched">The watched.</param>
        /// <param name="rating">The rating.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<List<BulkMovieResponse>> BulkChangeMoviesAsync(List<FollwItMovie> movies, bool? inCollection = null, bool? watched = null, int? rating = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (movies.IsNullOrEmpty())
            {
                return new List<BulkMovieResponse>();
            }

            var bulkMovies = movies.ToBulkMovieList();
            var request = RequestManager.CreateRequestType<BulkMovieChangeRequest>();
            request.Movies = bulkMovies;
            request.InCollection = inCollection;
            request.Watched = watched;
            request.Rating = rating;

            return await PostResponse<List<BulkMovieResponse>>(PostMethods.MovieBulkAction, await request.SerialiseAsync(), cancellationToken);
        }

        /// <summary>
        /// Adds the movie to collection.
        /// </summary>
        /// <param name="movie">The movie.</param>
        /// <param name="insertInStream">if set to <c>true</c> [insert in stream].</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">movie;Movie cannot be null</exception>
        /// <exception cref="System.InvalidOperationException">Movie didn't contain a valid ID</exception>
        public async Task<bool> AddMovieToCollectionAsync(FollwItMovie movie, bool insertInStream = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (movie == null)
            {
                throw new ArgumentNullException("movie", "Movie cannot be null");
            }

            if (!string.IsNullOrEmpty(movie.Id))
            {
                return await AddMovieToCollectionAsync(movie.Id, MovieIdentificationType.FollwIt, insertInStream, cancellationToken);
            }
            if (!string.IsNullOrEmpty(movie.ImdbId))
            {
                return await AddMovieToCollectionAsync(movie.ImdbId, MovieIdentificationType.Imdb, insertInStream, cancellationToken);
            }
            if (!string.IsNullOrEmpty(movie.TmdbId))
            {
                return await AddMovieToCollectionAsync(movie.TmdbId, MovieIdentificationType.Tmdb, insertInStream, cancellationToken);
            }
            
            throw new InvalidOperationException("Movie didn't contain a valid ID");
        }

        /// <summary>
        /// Adds the movie to collection.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="identificationType">Type of the identification.</param>
        /// <param name="insertInStream">if set to <c>true</c> [insert in stream].</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<bool> AddMovieToCollectionAsync(string id, MovieIdentificationType identificationType, bool insertInStream = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = RequestManager.CreateRequestType<MovieCollectionRequest>();
            request.InsertInStream = insertInStream;
            switch (identificationType)
            {
                case MovieIdentificationType.FollwIt:
                    request.MovieId = int.Parse(id);
                    break;
                case MovieIdentificationType.Imdb:
                    request.ImdbId = id;
                    break;
                case MovieIdentificationType.Tmdb:
                    request.TmdbId = int.Parse(id);
                    break;
            }

            var response = await PostResponse<FollwItSuccessResponse>(PostMethods.MovieCollection, await request.SerialiseAsync(), cancellationToken);
            return response.Response.Contains("success");
        }

        /// <summary>
        /// Adds the movie to list.
        /// </summary>
        /// <param name="movie">The movie.</param>
        /// <param name="listId">The list identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// movie;Movie cannot be null
        /// or
        /// listId;ListID cannot be null or empty.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">Movie didn't contain a valid ID</exception>
        public async Task<bool> AddMovieToListAsync(FollwItMovie movie, string listId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (movie == null)
            {
                throw new ArgumentNullException("movie", "Movie cannot be null");
            }

            if (string.IsNullOrEmpty(listId))
            {
                throw new ArgumentNullException("listId", "ListID cannot be null or empty.");
            }

            if (!string.IsNullOrEmpty(movie.Id))
            {
                return await AddMovieToListAsync(movie.Id, MovieIdentificationType.FollwIt, listId, cancellationToken);
            }
            if (!string.IsNullOrEmpty(movie.ImdbId))
            {
                return await AddMovieToListAsync(movie.ImdbId, MovieIdentificationType.Imdb, listId, cancellationToken);
            }
            if (!string.IsNullOrEmpty(movie.TmdbId))
            {
                return await AddMovieToListAsync(movie.TmdbId, MovieIdentificationType.Tmdb, listId, cancellationToken);
            }

            throw new InvalidOperationException("Movie didn't contain a valid ID");
        }

        /// <summary>
        /// Adds the movie to list.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="identificationType">Type of the identification.</param>
        /// <param name="listId">The list identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">listId;ListID cannot be null or empty.</exception>
        public async Task<bool> AddMovieToListAsync(string id, MovieIdentificationType identificationType, string listId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(listId))
            {
                throw new ArgumentNullException("listId", "ListID cannot be null or empty.");
            }

            var request = RequestManager.CreateRequestType<MovieListRequest>();
            request.ListId = listId;
            switch (identificationType)
            {
                case MovieIdentificationType.FollwIt:
                    request.MovieId = int.Parse(id);
                    break;
                case MovieIdentificationType.Imdb:
                    request.ImdbId = id;
                    break;
                case MovieIdentificationType.Tmdb:
                    request.TmdbId = int.Parse(id);
                    break;
            }

            var response = await PostResponse<FollwItSuccessResponse>(PostMethods.MovieList, await request.SerialiseAsync(), cancellationToken);
            return response.Response.Contains("success");
        }

        /// <summary>
        /// Changes the movie rating.
        /// </summary>
        /// <param name="movie">The movie.</param>
        /// <param name="rating">The rating.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">movie;Movie cannot be null</exception>
        /// <exception cref="System.InvalidOperationException">Movie didn't contain a valid ID</exception>
        public async Task<bool> ChangeMovieRatingAsync(FollwItMovie movie, int rating, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (movie == null)
            {
                throw new ArgumentNullException("movie", "Movie cannot be null");
            }
            
            if (!string.IsNullOrEmpty(movie.Id))
            {
                return await ChangeMovieRatingAsync(movie.Id, MovieIdentificationType.FollwIt, rating, cancellationToken);
            }
            if (!string.IsNullOrEmpty(movie.ImdbId))
            {
                return await ChangeMovieRatingAsync(movie.ImdbId, MovieIdentificationType.Imdb, rating, cancellationToken);
            }
            if (!string.IsNullOrEmpty(movie.TmdbId))
            {
                return await ChangeMovieRatingAsync(movie.TmdbId, MovieIdentificationType.Tmdb, rating, cancellationToken);
            }

            throw new InvalidOperationException("Movie didn't contain a valid ID");
        }

        /// <summary>
        /// Changes the movie rating.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="identificationType">Type of the identification.</param>
        /// <param name="rating">The rating.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<bool> ChangeMovieRatingAsync(string id, MovieIdentificationType identificationType, int rating, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = RequestManager.CreateRequestType<MovieRatingRequest>();
            request.Rating = rating;
            switch (identificationType)
            {
                case MovieIdentificationType.FollwIt:
                    request.MovieId = int.Parse(id);
                    break;
                case MovieIdentificationType.Imdb:
                    request.ImdbId = id;
                    break;
                case MovieIdentificationType.Tmdb:
                    request.TmdbId = int.Parse(id);
                    break;
            }

            var response = await PostResponse<FollwItSuccessResponse>(PostMethods.MovieRate, await request.SerialiseAsync(), cancellationToken);
            return response.Response.Contains("success");
        }

        /// <summary>
        /// Gets the recommended movies.
        /// </summary>
        /// <param name="genres">The genres.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<List<FollwItMovie>> GetRecommendedMoviesAsync(List<FollwItGenre> genres, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (genres == null)
            {
                genres = new List<FollwItGenre>();
            }

            var request = RequestManager.CreateRequestType<RecommendedRequest>();

            var genreString = string.Join("|", genres.Select(x => x.GetDescription().ToLower()));
            request.Genres = genreString;

            return await PostResponse<List<FollwItMovie>>(PostMethods.MovieRecommendations, await request.SerialiseAsync(), cancellationToken);
        }

        /// <summary>
        /// Removes the movie from collection.
        /// </summary>
        /// <param name="movie">The movie.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">movie;Movie cannot be null</exception>
        /// <exception cref="System.InvalidOperationException">Movie didn't contain a valid ID</exception>
        public async Task<bool> RemoveMovieFromCollectionAsync(FollwItMovie movie, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (movie == null)
            {
                throw new ArgumentNullException("movie", "Movie cannot be null");
            }

            if (!string.IsNullOrEmpty(movie.Id))
            {
                return await RemoveMovieFromCollectionAsync(movie.Id, MovieIdentificationType.FollwIt, cancellationToken);
            }
            if (!string.IsNullOrEmpty(movie.ImdbId))
            {
                return await RemoveMovieFromCollectionAsync(movie.ImdbId, MovieIdentificationType.Imdb, cancellationToken);
            }
            if (!string.IsNullOrEmpty(movie.TmdbId))
            {
                return await RemoveMovieFromCollectionAsync(movie.TmdbId, MovieIdentificationType.Tmdb, cancellationToken);
            }

            throw new InvalidOperationException("Movie didn't contain a valid ID");
        }

        /// <summary>
        /// Removes the movie from collection.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="identificationType">Type of the identification.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<bool> RemoveMovieFromCollectionAsync(string id, MovieIdentificationType identificationType, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = RequestManager.CreateRequestType<MovieBaseRequest>();
            switch (identificationType)
            {
                case MovieIdentificationType.FollwIt:
                    request.MovieId = int.Parse(id);
                    break;
                case MovieIdentificationType.Imdb:
                    request.ImdbId = id;
                    break;
                case MovieIdentificationType.Tmdb:
                    request.TmdbId = int.Parse(id);
                    break;
            }

            var response = await PostResponse<FollwItSuccessResponse>(PostMethods.MovieUncollection, await request.SerialiseAsync(), cancellationToken);
            return response.Response.Contains("success");
        }

        /// <summary>
        /// Removes the movie from list.
        /// </summary>
        /// <param name="movie">The movie.</param>
        /// <param name="listId">The list identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// movie;Movie cannot be null
        /// or
        /// listId;ListID cannot be null or empty.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">Movie didn't contain a valid ID</exception>
        public async Task<bool> RemoveMovieFromListAsync(FollwItMovie movie, string listId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (movie == null)
            {
                throw new ArgumentNullException("movie", "Movie cannot be null");
            }

            if (string.IsNullOrEmpty(listId))
            {
                throw new ArgumentNullException("listId", "ListID cannot be null or empty.");
            }

            if (!string.IsNullOrEmpty(movie.Id))
            {
                return await AddMovieToListAsync(movie.Id, MovieIdentificationType.FollwIt, listId, cancellationToken);
            }
            if (!string.IsNullOrEmpty(movie.ImdbId))
            {
                return await AddMovieToListAsync(movie.ImdbId, MovieIdentificationType.Imdb, listId, cancellationToken);
            }
            if (!string.IsNullOrEmpty(movie.TmdbId))
            {
                return await AddMovieToListAsync(movie.TmdbId, MovieIdentificationType.Tmdb, listId, cancellationToken);
            }

            throw new InvalidOperationException("Movie didn't contain a valid ID");
        }

        /// <summary>
        /// Removes the movie from list.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="identificationType">Type of the identification.</param>
        /// <param name="listId">The list identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">listId;ListID cannot be null or empty.</exception>
        public async Task<bool> RemoveMovieFromListAsync(string id, MovieIdentificationType identificationType, string listId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(listId))
            {
                throw new ArgumentNullException("listId", "ListID cannot be null or empty.");
            }

            var request = RequestManager.CreateRequestType<MovieListRequest>();
            request.ListId = listId;
            switch (identificationType)
            {
                case MovieIdentificationType.FollwIt:
                    request.MovieId = int.Parse(id);
                    break;
                case MovieIdentificationType.Imdb:
                    request.ImdbId = id;
                    break;
                case MovieIdentificationType.Tmdb:
                    request.TmdbId = int.Parse(id);
                    break;
            }

            var response = await PostResponse<FollwItSuccessResponse>(PostMethods.MovieUnlist, await request.SerialiseAsync(), cancellationToken);
            return response.Response.Contains("success");
        }

        /// <summary>
        /// Marks the movie as unwatched.
        /// </summary>
        /// <param name="movie">The movie.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">movie;Movie cannot be null</exception>
        /// <exception cref="System.InvalidOperationException">Movie didn't contain a valid ID</exception>
        public async Task<bool> MarkMovieAsUnwatchedAsync(FollwItMovie movie, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (movie == null)
            {
                throw new ArgumentNullException("movie", "Movie cannot be null");
            }

            if (!string.IsNullOrEmpty(movie.Id))
            {
                return await MarkMovieAsUnwatchedAsync(movie.Id, MovieIdentificationType.FollwIt, cancellationToken);
            }
            if (!string.IsNullOrEmpty(movie.ImdbId))
            {
                return await MarkMovieAsUnwatchedAsync(movie.ImdbId, MovieIdentificationType.Imdb, cancellationToken);
            }
            if (!string.IsNullOrEmpty(movie.TmdbId))
            {
                return await MarkMovieAsUnwatchedAsync(movie.TmdbId, MovieIdentificationType.Tmdb, cancellationToken);
            }

            throw new InvalidOperationException("Movie didn't contain a valid ID");
        }

        /// <summary>
        /// Marks the movie as unwatched.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="identificationType">Type of the identification.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<bool> MarkMovieAsUnwatchedAsync(string id, MovieIdentificationType identificationType, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = RequestManager.CreateRequestType<MovieBaseRequest>();
            switch (identificationType)
            {
                case MovieIdentificationType.FollwIt:
                    request.MovieId = int.Parse(id);
                    break;
                case MovieIdentificationType.Imdb:
                    request.ImdbId = id;
                    break;
                case MovieIdentificationType.Tmdb:
                    request.TmdbId = int.Parse(id);
                    break;
            }

            var response = await PostResponse<FollwItSuccessResponse>(PostMethods.MovieUnwatched, await request.SerialiseAsync(), cancellationToken);
            return response.Response.Contains("success");
        }

        /// <summary>
        /// Marks the movie as not watching.
        /// </summary>
        /// <param name="movie">The movie.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">movie;Movie cannot be null</exception>
        /// <exception cref="System.InvalidOperationException">Movie didn't contain a valid ID</exception>
        public async Task<bool> MarkMovieAsNotWatchingAsync(FollwItMovie movie, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (movie == null)
            {
                throw new ArgumentNullException("movie", "Movie cannot be null");
            }

            if (!string.IsNullOrEmpty(movie.Id))
            {
                return await MarkMovieAsNotWatchingAsync(movie.Id, MovieIdentificationType.FollwIt, cancellationToken);
            }
            if (!string.IsNullOrEmpty(movie.ImdbId))
            {
                return await MarkMovieAsNotWatchingAsync(movie.ImdbId, MovieIdentificationType.Imdb, cancellationToken);
            }
            if (!string.IsNullOrEmpty(movie.TmdbId))
            {
                return await MarkMovieAsNotWatchingAsync(movie.TmdbId, MovieIdentificationType.Tmdb, cancellationToken);
            }

            throw new InvalidOperationException("Movie didn't contain a valid ID");
        }

        /// <summary>
        /// Marks the movie as not watching.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="identificationType">Type of the identification.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<bool> MarkMovieAsNotWatchingAsync(string id, MovieIdentificationType identificationType, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = RequestManager.CreateRequestType<MovieBaseRequest>();
            switch (identificationType)
            {
                case MovieIdentificationType.FollwIt:
                    request.MovieId = int.Parse(id);
                    break;
                case MovieIdentificationType.Imdb:
                    request.ImdbId = id;
                    break;
                case MovieIdentificationType.Tmdb:
                    request.TmdbId = int.Parse(id);
                    break;
            }

            var response = await PostResponse<FollwItSuccessResponse>(PostMethods.MovieUnwatching, await request.SerialiseAsync(), cancellationToken);
            return response.Response.Contains("success");
        }

        /// <summary>
        /// Marks the movie as watching.
        /// </summary>
        /// <param name="movie">The movie.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">movie;Movie cannot be null</exception>
        /// <exception cref="System.InvalidOperationException">Movie didn't contain a valid ID</exception>
        public async Task<bool> MarkMovieAsWatchingAsync(FollwItMovie movie, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (movie == null)
            {
                throw new ArgumentNullException("movie", "Movie cannot be null");
            }

            if (!string.IsNullOrEmpty(movie.Id))
            {
                return await MarkMovieAsWatchingAsync(movie.Id, MovieIdentificationType.FollwIt, cancellationToken);
            }
            if (!string.IsNullOrEmpty(movie.ImdbId))
            {
                return await MarkMovieAsWatchingAsync(movie.ImdbId, MovieIdentificationType.Imdb, cancellationToken);
            }
            if (!string.IsNullOrEmpty(movie.TmdbId))
            {
                return await MarkMovieAsWatchingAsync(movie.TmdbId, MovieIdentificationType.Tmdb, cancellationToken);
            }

            throw new InvalidOperationException("Movie didn't contain a valid ID");
        }

        /// <summary>
        /// Marks the movie as watching.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="identificationType">Type of the identification.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<bool> MarkMovieAsWatchingAsync(string id, MovieIdentificationType identificationType, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = RequestManager.CreateRequestType<MovieBaseRequest>();
            switch (identificationType)
            {
                case MovieIdentificationType.FollwIt:
                    request.MovieId = int.Parse(id);
                    break;
                case MovieIdentificationType.Imdb:
                    request.ImdbId = id;
                    break;
                case MovieIdentificationType.Tmdb:
                    request.TmdbId = int.Parse(id);
                    break;
            }

            var response = await PostResponse<FollwItSuccessResponse>(PostMethods.MovieWatching, await request.SerialiseAsync(), cancellationToken);
            return response.Response.Contains("success");
        }

        /// <summary>
        /// Marks the movie as watched.
        /// </summary>
        /// <param name="movie">The movie.</param>
        /// <param name="insertInStream">if set to <c>true</c> [insert in stream].</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">movie;Movie cannot be null</exception>
        /// <exception cref="System.InvalidOperationException">Movie didn't contain a valid ID</exception>
        public async Task<bool> MarkMovieAsWatchedAsync(FollwItMovie movie, bool insertInStream = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (movie == null)
            {
                throw new ArgumentNullException("movie", "Movie cannot be null");
            }

            if (!string.IsNullOrEmpty(movie.Id))
            {
                return await MarkMovieAsWatchedAsync(movie.Id, MovieIdentificationType.FollwIt, insertInStream, cancellationToken);
            }
            if (!string.IsNullOrEmpty(movie.ImdbId))
            {
                return await MarkMovieAsWatchedAsync(movie.ImdbId, MovieIdentificationType.Imdb, insertInStream, cancellationToken);
            }
            if (!string.IsNullOrEmpty(movie.TmdbId))
            {
                return await MarkMovieAsWatchedAsync(movie.TmdbId, MovieIdentificationType.Tmdb, insertInStream, cancellationToken);
            }

            throw new InvalidOperationException("Movie didn't contain a valid ID");
        }

        /// <summary>
        /// Marks the movie as watched.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="identificationType">Type of the identification.</param>
        /// <param name="insertInStream">if set to <c>true</c> [insert in stream].</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<bool> MarkMovieAsWatchedAsync(string id, MovieIdentificationType identificationType, bool insertInStream = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = RequestManager.CreateRequestType<MovieCollectionRequest>();
            request.InsertInStream = insertInStream;
            switch (identificationType)
            {
                case MovieIdentificationType.FollwIt:
                    request.MovieId = int.Parse(id);
                    break;
                case MovieIdentificationType.Imdb:
                    request.ImdbId = id;
                    break;
                case MovieIdentificationType.Tmdb:
                    request.TmdbId = int.Parse(id);
                    break;
            }

            var response = await PostResponse<FollwItSuccessResponse>(PostMethods.MovieWatched, await request.SerialiseAsync(), cancellationToken);
            return response.Response.Contains("success");
        }

        /// <summary>
        /// Gets the user stats for movie.
        /// </summary>
        /// <param name="movie">The movie.</param>
        /// <param name="username">The username.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">movie;Movie cannot be null</exception>
        /// <exception cref="System.InvalidOperationException">Movie didn't contain a valid ID</exception>
        public async Task<FollwItUserStats> GetUserStatsForMovieAsync(FollwItMovie movie, string username = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (movie == null)
            {
                throw new ArgumentNullException("movie", "Movie cannot be null");
            }

            if (!string.IsNullOrEmpty(movie.Id))
            {
                return await GetUserStatsForMovieAsync(movie.Id, MovieIdentificationType.FollwIt, username, cancellationToken);
            }
            if (!string.IsNullOrEmpty(movie.ImdbId))
            {
                return await GetUserStatsForMovieAsync(movie.ImdbId, MovieIdentificationType.Imdb, username, cancellationToken);
            }
            if (!string.IsNullOrEmpty(movie.TmdbId))
            {
                return await GetUserStatsForMovieAsync(movie.TmdbId, MovieIdentificationType.Tmdb, username, cancellationToken);
            }

            throw new InvalidOperationException("Movie didn't contain a valid ID");
        }

        /// <summary>
        /// Gets the user stats for movie.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="identificationType">Type of the identification.</param>
        /// <param name="username">The username.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<FollwItUserStats> GetUserStatsForMovieAsync(string id, MovieIdentificationType identificationType, string username = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = RequestManager.CreateRequestType<QueryUsernameRequest>();
            request.QueryUsername = username;
            switch (identificationType)
            {
                case MovieIdentificationType.FollwIt:
                    request.MovieId = int.Parse(id);
                    break;
                case MovieIdentificationType.Imdb:
                    request.ImdbId = id;
                    break;
                case MovieIdentificationType.Tmdb:
                    request.TmdbId = int.Parse(id);
                    break;
            }

            return await PostResponse<FollwItUserStats>(PostMethods.ShowUserStats, await request.SerialiseAsync(), cancellationToken);
        }
        #endregion

        #region Post Show Methods

        /// <summary>
        /// Adds the show to list.
        /// </summary>
        /// <param name="show">The show.</param>
        /// <param name="listId">The list identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// show;Show cannot be null
        /// or
        /// listId;ListID cannot be null or empty.
        /// </exception>
        public async Task<bool> AddShowToListAsync(FollwItTvShow show, string listId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (show == null)
            {
                throw new ArgumentNullException("show", "Show cannot be null");
            }

            if (string.IsNullOrEmpty(listId))
            {
                throw new ArgumentNullException("listId", "ListID cannot be null or empty.");
            }

            return await AddShowToListAsync(show.FollwitSeriesId, ShowIdentificationType.FollwIt, listId, cancellationToken);
        }

        /// <summary>
        /// Adds the show to list.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="identificationType">Type of the identification.</param>
        /// <param name="listId">The list identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">listId;ListID cannot be null or empty.</exception>
        /// <exception cref="System.InvalidOperationException">Imdb is not a supported type for this method</exception>
        public async Task<bool> AddShowToListAsync(int id, ShowIdentificationType identificationType, string listId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(listId))
            {
                throw new ArgumentNullException("listId", "ListID cannot be null or empty.");
            }

            var request = RequestManager.CreateRequestType<ShowListRequest>();
            request.ListId = listId;
            switch (identificationType)
            {
                case ShowIdentificationType.FollwIt:
                    request.ShowId = id;
                    break;
                case ShowIdentificationType.Imdb:
                    throw new InvalidOperationException("Imdb is not a supported type for this method");
                case ShowIdentificationType.Tvdb:
                    request.TvdbId = id;
                    break;
            }

            var response = await PostResponse<FollwItSuccessResponse>(PostMethods.ShowList, await request.SerialiseAsync(), cancellationToken);
            return response.Response.Contains("success");
        }

        /// <summary>
        /// Changes the show rating.
        /// </summary>
        /// <param name="show">The show.</param>
        /// <param name="rating">The rating.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">show;Show cannot be null</exception>
        public async Task<bool> ChangeShowRatingAsync(FollwItTvShow show, int rating, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (show == null)
            {
                throw new ArgumentNullException("show", "Show cannot be null");
            }

            return await ChangeShowRatingAsync(show.FollwitSeriesId, ShowIdentificationType.FollwIt, rating, cancellationToken);
        }

        /// <summary>
        /// Changes the show rating.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="identificationType">Type of the identification.</param>
        /// <param name="rating">The rating.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Imdb is not a supported type for this method</exception>
        public async Task<bool> ChangeShowRatingAsync(int id, ShowIdentificationType identificationType, int rating, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = RequestManager.CreateRequestType<ShowRatingRequest>();
            request.Rating = rating;
            switch (identificationType)
            {
                case ShowIdentificationType.FollwIt:
                    request.ShowId = id;
                    break;
                case ShowIdentificationType.Imdb:
                    throw new InvalidOperationException("Imdb is not a supported type for this method");
                case ShowIdentificationType.Tvdb:
                    request.TvdbId = id;
                    break;
            }

            var response = await PostResponse<FollwItSuccessResponse>(PostMethods.ShowRate, await request.SerialiseAsync(), cancellationToken);
            return response.Response.Contains("success");
        }

        /// <summary>
        /// Gets the recommended shows.
        /// </summary>
        /// <param name="genres">The genres.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<List<FollwItTvShow>> GetRecommendedShowsAsync(List<FollwItGenre> genres, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (genres == null)
            {
                genres = new List<FollwItGenre>();
            }

            var request = RequestManager.CreateRequestType<RecommendedRequest>();

            var genreString = string.Join("|", genres.Select(x => x.GetDescription().ToLower()));
            request.Genres = genreString;

            return await PostResponse<List<FollwItTvShow>>(PostMethods.ShowRecommendations, await request.SerialiseAsync(), cancellationToken);
        }

        /// <summary>
        /// Removes the show from list.
        /// </summary>
        /// <param name="show">The show.</param>
        /// <param name="listId">The list identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// show;Show cannot be null
        /// or
        /// listId;ListID cannot be null or empty.
        /// </exception>
        public async Task<bool> RemoveShowFromListAsync(FollwItTvShow show, string listId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (show == null)
            {
                throw new ArgumentNullException("show", "Show cannot be null");
            }

            if (string.IsNullOrEmpty(listId))
            {
                throw new ArgumentNullException("listId", "ListID cannot be null or empty.");
            }

            return await RemoveShowFromListAsync(show.FollwitSeriesId, ShowIdentificationType.FollwIt, listId, cancellationToken);
        }

        /// <summary>
        /// Removes the show from list.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="identificationType">Type of the identification.</param>
        /// <param name="listId">The list identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">listId;ListID cannot be null or empty.</exception>
        /// <exception cref="System.InvalidOperationException">Imdb is not a supported type for this method</exception>
        public async Task<bool> RemoveShowFromListAsync(int id, ShowIdentificationType identificationType, string listId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(listId))
            {
                throw new ArgumentNullException("listId", "ListID cannot be null or empty.");
            }

            var request = RequestManager.CreateRequestType<ShowListRequest>();
            request.ListId = listId;
            switch (identificationType)
            {
                case ShowIdentificationType.FollwIt:
                    request.ShowId = id;
                    break;
                case ShowIdentificationType.Imdb:
                    throw new InvalidOperationException("Imdb is not a supported type for this method");
                case ShowIdentificationType.Tvdb:
                    request.TvdbId = id;
                    break;
            }

            var response = await PostResponse<FollwItSuccessResponse>(PostMethods.ShowUnlist, await request.SerialiseAsync(), cancellationToken);
            return response.Response.Contains("success");
        }

        /// <summary>
        /// Gets the tv stats for user.
        /// </summary>
        /// <param name="show">The show.</param>
        /// <param name="username">The username.</param>
        /// <param name="includeEpisodes">if set to <c>true</c> [include episodes].</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">show;Show cannot be null</exception>
        public async Task<FollwItTvUserStats> GetTvStatsForUserAsync(FollwItTvShow show, string username = null, bool includeEpisodes = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (show == null)
            {
                throw new ArgumentNullException("show", "Show cannot be null");
            }

            return await GetTvStatsForUserAsync(show.FollwitSeriesId, ShowIdentificationType.FollwIt, username, includeEpisodes, cancellationToken);
        }

        /// <summary>
        /// Gets the tv stats for user.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="identificationType">Type of the identification.</param>
        /// <param name="username">The username.</param>
        /// <param name="includeEpisodes">if set to <c>true</c> [include episodes].</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Imdb is not a supported type for this method</exception>
        public async Task<FollwItTvUserStats> GetTvStatsForUserAsync(int id, ShowIdentificationType identificationType, string username = null, bool includeEpisodes = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(username))
            {
                username = Username;
            }

            var request = RequestManager.CreateRequestType<ShowUserStatsRequest>();
            request.QueryUsername = username;
            switch (identificationType)
            {
                case ShowIdentificationType.FollwIt:
                    request.ShowId = id;
                    break;
                case ShowIdentificationType.Imdb:
                    throw new InvalidOperationException("Imdb is not a supported type for this method");
                case ShowIdentificationType.Tvdb:
                    request.TvdbId = id;
                    break;
            }

            return await PostResponse<FollwItTvUserStats>(PostMethods.ShowUserStats, await request.SerialiseAsync(), cancellationToken);
        }
        #endregion

        #region Post User Methods

        /// <summary>
        /// Creates the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="locale">The locale.</param>
        /// <param name="privateProfile">if set to <c>true</c> [private profile].</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// True if the user was created successfully
        /// </returns>
        /// <exception cref="System.ArgumentNullException">username;Username cannot be null or empty
        /// or
        /// password;Password cannot be null or empty
        /// or
        /// emailAddress;Email address cannot be null or empty</exception>
        public async Task<bool> CreateUserAsync(string username, string password, string emailAddress, string locale = "en", bool privateProfile = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException("username", "Username cannot be null or empty");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("password", "Password cannot be null or empty");
            }

            if (string.IsNullOrEmpty(emailAddress))
            {
                throw new ArgumentNullException("emailAddress", "Email address cannot be null or empty");
            }

            var request = new CreateUserRequest
            {
                Username = username, 
                Password = password.Hash(), 
                EmailAddress = emailAddress, 
                Locale = locale, 
                PrivateProfile = privateProfile
            };

            var response = await PostResponse<CreateUserResponse>(PostMethods.UserCreate, await request.SerialiseAsync(), cancellationToken);
            if (response.Response != "success")
            {
                return false;
            }

            Username = username;
            Password = password;

            return true;
        }

        /// <summary>
        /// Gets the user list.
        /// </summary>
        /// <param name="listId">The list identifier.</param>
        /// <param name="username">The username.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">listId;ListID cannot be null or empty</exception>
        public async Task<FollwItList> GetUserListAsync(string listId, string username = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(listId))
            {
                throw new ArgumentNullException("listId", "ListID cannot be null or empty");
            }

            if (string.IsNullOrEmpty(username))
            {
                username = Username;
            }

            var request = RequestManager.CreateRequestType<UserListRequest>();
            request.QueryUsername = username;
            request.ListId = listId;

            return await PostResponse<FollwItList>(PostMethods.UserList, await request.SerialiseAsync(), cancellationToken);
        }

        /// <summary>
        /// Gets the users lists.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<List<FollwItList>> GetUsersListsAsync(string username = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(username))
            {
                username = Username;
            }

            var request = RequestManager.CreateRequestType<QueryUsernameRequest>();
            request.QueryUsername = username;

            return await PostResponse<List<FollwItList>>(PostMethods.UserLists, await request.SerialiseAsync(), cancellationToken);
        }

        /// <summary>
        /// Gets the online changes.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<List<FollwItOnlineChange>> GetOnlineChangesAsync(DateTime startDate, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = RequestManager.CreateRequestType<OnlineChangesRequest>();
            request.StartDate = startDate;

            var requestString = await request.SerialiseAsync(new DateConverter());

            return await PostResponse<List<FollwItOnlineChange>>(PostMethods.UserOnlineChanges, requestString, cancellationToken);
        }

        /// <summary>
        /// Gets the full profile.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<FollwItFullProfile> GetFullProfileAsync(string username = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(username))
            {
                username = Username;
            }

            var request = RequestManager.CreateRequestType<QueryUsernameRequest>();
            request.QueryUsername = username;

            return await PostResponse<FollwItFullProfile>(PostMethods.UserProfile, await request.SerialiseAsync(), cancellationToken);
        }

        /// <summary>
        /// Gets the user stream asynchronous.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<List<FollwItStreamItem>> GetUserStreamAsync(string username = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(username))
            {
                username = Username;
            }

            var request = RequestManager.CreateRequestType<QueryUsernameRequest>();
            request.QueryUsername = username;

            return await PostResponse<List<FollwItStreamItem>>(PostMethods.UserStream, await request.SerialiseAsync(), cancellationToken);
        }

        /// <summary>
        /// Updates the user asynchronous.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="locale">The locale.</param>
        /// <param name="privateProfile">The private profile.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<bool> UpdateUserAsync(string emailAddress = null, string locale = null, bool? privateProfile = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = RequestManager.CreateRequestType<CreateUserRequest>();
            request.EmailAddress = emailAddress;
            request.Locale = locale;
            request.PrivateProfile = privateProfile;

            var response = await PostResponse<FollwItSuccessResponse>(PostMethods.UserUpdate, await request.SerialiseAsync(), cancellationToken);

            return response.Response.Contains("success");
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

        #region Episode Post Methods
        private async Task<bool> PostEpisodeCollectionInternalAsync(EpisodeBaseRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            var requestString = await request.SerialiseAsync();

            var response = await PostResponse<FollwItSuccessResponse>(PostMethods.EpisodeCollection, requestString, cancellationToken);

            return response.Response.Contains("success");
        }

        private async Task<bool> PostEpisodeUncollectionInternalAsync(EpisodeBaseRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            var requestString = await request.SerialiseAsync();

            var response = await PostResponse<FollwItSuccessResponse>(PostMethods.EpisodeUncollection, requestString, cancellationToken);

            return response.Response.Contains("success");
        }

        private async Task<bool> PostEpisodeListInternalAsync(EpisodeListRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            var requestString = await request.SerialiseAsync();

            var response = await PostResponse<FollwItSuccessResponse>(PostMethods.EpisodeList, requestString, cancellationToken);

            return response.Response.Contains("success");
        }

        private async Task<bool> PostEpisodeUnlistInternalAsync(EpisodeListRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            var requestString = await request.SerialiseAsync();

            var response = await PostResponse<FollwItSuccessResponse>(PostMethods.EpisodeUnlist, requestString, cancellationToken);

            return response.Response.Contains("success");
        }

        private async Task<bool> PostEpisodeRatingInternalAsync(EpisodeRatingRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            var requestString = await request.SerialiseAsync();

            var response = await PostResponse<FollwItSuccessResponse>(PostMethods.EpisodeRate, requestString, cancellationToken);

            return response.Response.Contains("success");
        }

        private async Task<bool> PostEpisodeUnwatchedInternalAsync(EpisodeBaseRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            var requestString = await request.SerialiseAsync();

            var response = await PostResponse<FollwItSuccessResponse>(PostMethods.EpisodeUnwatched, requestString, cancellationToken);

            return response.Response.Contains("success");
        }

        private async Task<bool> PostEpisodeWatchedInternalAsync(EpisodeWatchedRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            var requestString = await request.SerialiseAsync();

            var response = await PostResponse<FollwItSuccessResponse>(PostMethods.EpisodeWatched, requestString, cancellationToken);

            return response.Response.Contains("success");
        }

        private async Task<bool> PostEpisodeUnwatchingInternalAsync(EpisodeBaseRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            var requestString = await request.SerialiseAsync();

            var response = await PostResponse<FollwItSuccessResponse>(PostMethods.EpisodeUnwatching, requestString, cancellationToken);

            return response.Response.Contains("success");
        }

        private async Task<bool> PostEpisodeWatchingInternalAsync(EpisodeBaseRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            var requestString = await request.SerialiseAsync();

            var response = await PostResponse<FollwItSuccessResponse>(PostMethods.EpisodeWatching, requestString, cancellationToken);

            return response.Response.Contains("success");
        }
        
        #endregion
        #endregion
    }
}