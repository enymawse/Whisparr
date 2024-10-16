using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NzbDrone.Core.Configuration;
using NzbDrone.Core.ImportLists;
using NzbDrone.Core.ImportLists.ImportExclusions;
using NzbDrone.Core.ImportLists.ImportListMovies;
using NzbDrone.Core.MediaCover;
using NzbDrone.Core.MetadataSource;
using NzbDrone.Core.Movies;
using NzbDrone.Core.Organizer;
using Whisparr.Api.V3.Movies;
using Whisparr.Http;

namespace Whisparr.Api.V3.ImportLists
{
    [V3ApiController("importlist/movie")]
    public class ImportListMoviesController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly IAddMovieService _addMovieService;
        private readonly IProvideMovieInfo _movieInfo;
        private readonly IBuildFileNames _fileNameBuilder;
        private readonly IImportListMovieService _listMovieService;
        private readonly IImportListFactory _importListFactory;
        private readonly IImportExclusionsService _importExclusionService;
        private readonly INamingConfigService _namingService;
        private readonly IConfigService _configService;

        public ImportListMoviesController(IMovieService movieService,
                                    IAddMovieService addMovieService,
                                    IProvideMovieInfo movieInfo,
                                    IBuildFileNames fileNameBuilder,
                                    IImportListMovieService listMovieService,
                                    IImportListFactory importListFactory,
                                    IImportExclusionsService importExclusionsService,
                                    INamingConfigService namingService,
                                    IConfigService configService)
        {
            _movieService = movieService;
            _addMovieService = addMovieService;
            _movieInfo = movieInfo;
            _fileNameBuilder = fileNameBuilder;
            _listMovieService = listMovieService;
            _importListFactory = importListFactory;
            _importExclusionService = importExclusionsService;
            _namingService = namingService;
            _configService = configService;
        }

        [HttpGet]
        public object GetDiscoverMovies()
        {
            var realResults = new List<ImportListMoviesResource>();
            var listExclusions = _importExclusionService.GetAllExclusions();
            var existingTmdbIds = _movieService.AllMovieTmdbIds();

            var listMovies = MapToResource(_listMovieService.GetAllForLists(_importListFactory.Enabled().Select(x => x.Definition.Id).ToList())).ToList();

            realResults.AddRange(listMovies);

            var groupedListMovies = realResults.GroupBy(x => x.TmdbId);

            // Distinct Movies
            realResults = groupedListMovies.Select(x =>
            {
                var movie = x.First();

                movie.Lists = x.SelectMany(m => m.Lists).ToHashSet();
                movie.IsExcluded = listExclusions.Any(e => e.ForeignId == movie.ForeignId);
                movie.IsExisting = existingTmdbIds.Any(e => e == movie.TmdbId);
                movie.IsRecommendation = x.Any(m => m.IsRecommendation);

                return movie;
            }).ToList();

            return realResults;
        }

        [HttpPost]
        public object AddMovies([FromBody] List<MovieResource> resource)
        {
            var newMovies = resource.ToModel();

            return _addMovieService.AddMovies(newMovies, true).ToResource(0);
        }

        private IEnumerable<ImportListMoviesResource> MapToResource(IEnumerable<Movie> movies)
        {
            // Avoid calling for naming spec on every movie in filenamebuilder
            var namingConfig = _namingService.GetConfig();

            foreach (var currentMovie in movies)
            {
                var resource = currentMovie.ToResource();

                var poster = currentMovie.MovieMetadata.Value.Images.FirstOrDefault(c => c.CoverType == MediaCoverTypes.Poster);
                if (poster != null)
                {
                    resource.RemotePoster = poster.RemoteUrl;
                }

                resource.Title =  resource.Title;
                resource.Overview = resource.Overview;
                resource.Folder = _fileNameBuilder.GetMovieFolder(currentMovie, namingConfig);

                yield return resource;
            }
        }

        private IEnumerable<ImportListMoviesResource> MapToResource(IEnumerable<ImportListMovie> movies)
        {
            // Avoid calling for naming spec on every movie in filenamebuilder
            var namingConfig = _namingService.GetConfig();

            foreach (var currentMovie in movies)
            {
                var resource = currentMovie.ToResource();

                var poster = currentMovie.MovieMetadata.Value.Images.FirstOrDefault(c => c.CoverType == MediaCoverTypes.Poster);
                if (poster != null)
                {
                    resource.RemotePoster = poster.RemoteUrl;
                }

                resource.Title = resource.Title;
                resource.Overview = resource.Overview;
                resource.Folder = _fileNameBuilder.GetMovieFolder(new Movie
                {
                    MovieMetadata = currentMovie.MovieMetadata
                }, namingConfig);

                yield return resource;
            }
        }
    }
}
