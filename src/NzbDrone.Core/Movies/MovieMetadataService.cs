using System.Collections.Generic;
using NzbDrone.Core.ImportLists.ImportListMovies;

namespace NzbDrone.Core.Movies
{
    public interface IMovieMetadataService
    {
        MovieMetadata Get(int id);
        MovieMetadata FindByTmdbId(int tmdbId);
        MovieMetadata FindByForeignId(string foreignId);
        MovieMetadata FindByImdbId(string imdbId);
        bool Upsert(MovieMetadata movie);
        bool UpsertMany(List<MovieMetadata> movies);
        void DeleteMany(List<MovieMetadata> movies);
    }

    public class MovieMetadataService : IMovieMetadataService
    {
        private readonly IMovieMetadataRepository _movieMetadataRepository;
        private readonly IMovieService _movieService;
        private readonly IImportListMovieService _importListMovieService;

        public MovieMetadataService(IMovieMetadataRepository movieMetadataRepository, IMovieService movieService, IImportListMovieService importListMovieService)
        {
            _movieMetadataRepository = movieMetadataRepository;
            _movieService = movieService;
            _importListMovieService = importListMovieService;
        }

        public MovieMetadata FindByTmdbId(int tmdbId)
        {
            return _movieMetadataRepository.FindByTmdbId(tmdbId);
        }

        public MovieMetadata FindByForeignId(string foreignId)
        {
            return _movieMetadataRepository.FindByForeignId(foreignId);
        }

        public MovieMetadata FindByImdbId(string imdbId)
        {
            return _movieMetadataRepository.FindByImdbId(imdbId);
        }

        public MovieMetadata Get(int id)
        {
            return _movieMetadataRepository.Get(id);
        }

        public bool Upsert(MovieMetadata movie)
        {
            return _movieMetadataRepository.UpsertMany(new List<MovieMetadata> { movie });
        }

        public bool UpsertMany(List<MovieMetadata> movies)
        {
            return _movieMetadataRepository.UpsertMany(movies);
        }

        public void DeleteMany(List<MovieMetadata> movies)
        {
            foreach (var movie in movies)
            {
                if (!_importListMovieService.ExistsByMetadataId(movie.Id) && !_movieService.ExistsByMetadataId(movie.Id))
                {
                    _movieMetadataRepository.Delete(movie);
                }
            }
        }
    }
}
