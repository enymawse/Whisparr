using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using NzbDrone.Core.Profiles.Releases;
using Whisparr.Http.REST;

namespace Whisparr.Api.V3.Profiles.Release
{
    public class ReleaseProfileResource : RestResource
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }

        // Is List<string>, string or JArray, we accept 'string' with POST and PUT for backwards compatibility
        public object Required { get; set; }
        public object Ignored { get; set; }
        public int IndexerId { get; set; }
        public HashSet<int> Tags { get; set; }

        public ReleaseProfileResource()
        {
            Tags = new HashSet<int>();
        }
    }

    public static class RestrictionResourceMapper
    {
        public static ReleaseProfileResource ToResource(this ReleaseProfile model)
        {
            if (model == null)
            {
                return null;
            }

            return new ReleaseProfileResource
            {
                Id = model.Id,
                Name = model.Name,
                Enabled = model.Enabled,
                Required = model.Required ?? new List<string>(),
                Ignored = model.Ignored ?? new List<string>(),
                IndexerId = model.IndexerId,
                Tags = new HashSet<int>(model.Tags)
            };
        }

        public static ReleaseProfile ToModel(this ReleaseProfileResource resource)
        {
            if (resource == null)
            {
                return null;
            }

            return new ReleaseProfile
            {
                Id = resource.Id,
                Name = resource.Name,
                Enabled = resource.Enabled,
                Required = resource.MapRequired(),
                Ignored = resource.MapIgnored(),
                IndexerId = resource.IndexerId,
                Tags = new HashSet<int>(resource.Tags)
            };
        }

        public static List<ReleaseProfileResource> ToResource(this IEnumerable<ReleaseProfile> models)
        {
            return models.Select(ToResource).ToList();
        }

        public static List<string> MapRequired(this ReleaseProfileResource profile) => ParseArray(profile.Required, "required");
        public static List<string> MapIgnored(this ReleaseProfileResource profile) => ParseArray(profile.Ignored, "ignored");

        private static List<string> ParseArray(object resource, string title)
        {
            if (resource == null)
            {
                return new List<string>();
            }

            if (resource is List<string> list)
            {
                return list;
            }

            if (resource is JsonElement array)
            {
                if (array.ValueKind == JsonValueKind.String)
                {
                    return array.GetString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }

                if (array.ValueKind == JsonValueKind.Array)
                {
                    return JsonSerializer.Deserialize<List<string>>(array);
                }
            }

            if (resource is string str)
            {
                return str.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            throw new BadRequestException($"Invalid field {title}, should be string or string array");
        }
    }
}
