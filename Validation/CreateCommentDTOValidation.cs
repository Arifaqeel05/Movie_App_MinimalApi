using FluentValidation;
using Movie_App_MinimalApi.DTOs;

namespace Movie_App_MinimalApi.Validation
{
    public class CreateCommentDTOValidation : AbstractValidator<CreateCommentDTO>
    {
        public CreateCommentDTOValidation()
        {
            RuleFor(x => x.Body)
                .NotEmpty().WithMessage(ValidationUtilities.NonEmptyMessage);

        }
    }
}
