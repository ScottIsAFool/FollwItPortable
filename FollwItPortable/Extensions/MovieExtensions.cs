using System.Collections.Generic;
using System.Linq;
using FollwItPortable.Model;
using FollwItPortable.Model.Requests;

namespace FollwItPortable.Extensions
{
    internal static class MovieExtensions
    {
        internal static List<BulkMovie> ToBulkMovieList(this IList<FollwItMovie> movies)
        {
            var bulkMovies = movies.Cast<BulkMovie>().ToList();
            foreach (var movie in bulkMovies)
            {
                var list = new List<string>();
                if (!string.IsNullOrEmpty(movie.ImdbId))
                {
                    list.Add("imdb.com=" + movie.ImdbId);
                }

                if (!string.IsNullOrEmpty(movie.TmdbId))
                {
                    list.Add("themoviedb.org=" + movie.TmdbId);
                }

                if (!string.IsNullOrEmpty(movie.Id))
                {
                    list.Add("movie_id=" + movie.Id);
                }

                var resources = string.Join("|", list);
                movie.Resources = resources;
            }

            return bulkMovies;
        } 
    }
}
