using FluentAssertions;
using NUnit.Framework;
using NzbDrone.Core.MetadataSource.SkyHook;
using NzbDrone.Core.Movies;
using NzbDrone.Core.Test.Framework;
using NzbDrone.Test.Common.Categories;

namespace NzbDrone.Core.Test.MetadataSource.SkyHook
{
    [TestFixture]
    [IntegrationTest]
    public class SkyHookProxyFixture : CoreTest<SkyHookProxy>
    {
        [SetUp]
        public void Setup()
        {
            UseRealHttp();
        }

        [TestCase(42019, "Taboo")]
        [TestCase(37795, "Taboo II")]
        public void should_be_able_to_get_movie_detail(int tmdbId, string title)
        {
            var details = Subject.GetMovieInfo(tmdbId).Item1;

            ValidateMovie(details);

            details.Title.Should().Be(title);
        }

        private void ValidateMovie(MovieMetadata movie)
        {
            movie.Should().NotBeNull();
            movie.Title.Should().NotBeNullOrWhiteSpace();
            movie.CleanTitle.Should().Be(Parser.Parser.CleanMovieTitle(movie.Title));
            movie.SortTitle.Should().Be(MovieTitleNormalizer.Normalize(movie.Title, movie.ForeignId));
            movie.Overview.Should().NotBeNullOrWhiteSpace();
            movie.ReleaseDateUtc.Should().HaveValue();
            movie.Images.Should().NotBeEmpty();
            movie.StudioTitle.Should().NotBeNullOrWhiteSpace();
            movie.Runtime.Should().BeGreaterThan(0);

            // series.TvRageId.Should().BeGreaterThan(0);
            movie.ForeignId.Should().NotBeEmpty();
        }
    }
}
