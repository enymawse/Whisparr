using FluentValidation.Validators;
using NzbDrone.Core.Movies;

namespace NzbDrone.Core.Validation.Paths
{
    public class MovieExistsValidator : PropertyValidator
    {
        private readonly IMovieService _movieService;

        public MovieExistsValidator(IMovieService movieService)
        {
            _movieService = movieService;
        }

        protected override string GetDefaultMessageTemplate() => "This item has already been added";

        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context.PropertyValue == null)
            {
                return true;
            }

            var foreignId = (string)context.PropertyValue;

            return _movieService.FindByForeignId(foreignId) == null;
        }
    }
}
