using FluentValidation;
using Movie_App_MinimalApi.DTOs;

namespace Movie_App_MinimalApi.Validation
{
    public class CreateMovieDTOValidations : AbstractValidator<CreateMoviesDTO>
    {
        public CreateMovieDTOValidations()
        {
            RuleFor(x => x.Title).NotEmpty()
                .WithMessage(ValidationUtilities.NonEmptyMessage)
                .MaximumLength(100).WithMessage(ValidationUtilities.MaxLengthMessage)
                ;
        }
    }
}
