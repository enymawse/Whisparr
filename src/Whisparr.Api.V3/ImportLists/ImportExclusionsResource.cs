using System.Collections.Generic;
using System.Linq;
using NzbDrone.Core.ImportLists.ImportExclusions;

namespace Whisparr.Api.V3.ImportLists
{
    public class ImportExclusionsResource : ProviderResource<ImportExclusionsResource>
    {
        // public int Id { get; set; }
        public string ForeignId { get; set; }
        public string MovieTitle { get; set; }
        public int MovieYear { get; set; }
    }

    public static class ImportExclusionsResourceMapper
    {
        public static ImportExclusionsResource ToResource(this ImportExclusion model)
        {
            if (model == null)
            {
                return null;
            }

            return new ImportExclusionsResource
            {
                Id = model.Id,
                ForeignId = model.ForeignId,
                MovieTitle = model.MovieTitle,
                MovieYear = model.MovieYear
            };
        }

        public static List<ImportExclusionsResource> ToResource(this IEnumerable<ImportExclusion> exclusions)
        {
            return exclusions.Select(ToResource).ToList();
        }

        public static ImportExclusion ToModel(this ImportExclusionsResource resource)
        {
            return new ImportExclusion
            {
                Id = resource.Id,
                ForeignId = resource.ForeignId,
                MovieTitle = resource.MovieTitle,
                MovieYear = resource.MovieYear
            };
        }

        public static List<ImportExclusion> ToModel(this IEnumerable<ImportExclusionsResource> resources)
        {
            return resources.Select(ToModel).ToList();
        }
    }
}
