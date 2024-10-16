using System.Collections.Generic;
using System.Linq;
using NLog;
using NzbDrone.Core.Datastore;
using NzbDrone.Core.Messaging.Events;

namespace NzbDrone.Core.Movies
{
    public interface IMovieMetadataRepository : IBasicRepository<MovieMetadata>
    {
        MovieMetadata FindByTmdbId(int tmdbId);
        MovieMetadata FindByForeignId(string foreignId);
        MovieMetadata FindByImdbId(string imdbId);
        List<MovieMetadata> FindById(List<string> tmdbIds);
        bool UpsertMany(List<MovieMetadata> data);
    }

    public class MovieMetadataRepository : BasicRepository<MovieMetadata>, IMovieMetadataRepository
    {
        private readonly Logger _logger;

        public MovieMetadataRepository(IMainDatabase database, IEventAggregator eventAggregator, Logger logger)
            : base(database, eventAggregator)
        {
            _logger = logger;
        }

        public MovieMetadata FindByTmdbId(int tmdbId)
        {
            return Query(x => x.TmdbId == tmdbId).FirstOrDefault();
        }

        public MovieMetadata FindByForeignId(string foreignId)
        {
            return Query(x => x.ForeignId == foreignId).FirstOrDefault();
        }

        public MovieMetadata FindByImdbId(string imdbId)
        {
            return Query(x => x.ImdbId == imdbId).FirstOrDefault();
        }

        public List<MovieMetadata> FindById(List<string> tmdbIds)
        {
            return Query(x => Enumerable.Contains(tmdbIds, x.ForeignId));
        }

        public bool UpsertMany(List<MovieMetadata> data)
        {
            var existingMetadata = FindById(data.Select(x => x.ForeignId).ToList());
            var updateMetadataList = new List<MovieMetadata>();
            var addMetadataList = new List<MovieMetadata>();
            var upToDateMetadataCount = 0;

            foreach (var meta in data)
            {
                var existing = existingMetadata.SingleOrDefault(x => x.ForeignId == meta.ForeignId);
                if (existing != null)
                {
                    meta.UseDbFieldsFrom(existing);
                    if (!meta.Equals(existing))
                    {
                        updateMetadataList.Add(meta);
                    }
                    else
                    {
                        upToDateMetadataCount++;
                    }
                }
                else
                {
                    addMetadataList.Add(meta);
                }
            }

            UpdateMany(updateMetadataList);
            InsertMany(addMetadataList);

            _logger.Debug($"{upToDateMetadataCount} movie metadata up to date; Updating {updateMetadataList.Count}, Adding {addMetadataList.Count} movie metadata entries.");

            return updateMetadataList.Count > 0 || addMetadataList.Count > 0;
        }
    }
}
