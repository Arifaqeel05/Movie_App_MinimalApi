using FluentValidation;
using Movie_App_MinimalApi.DTOs;

namespace Movie_App_MinimalApi.Validation
{
    public class CreateUpdateActorDTOValidation : AbstractValidator<CreateUpdateActorDTO>
    {
        public CreateUpdateActorDTOValidation()
        {
            RuleFor(p=>p.Name)
                .NotEmpty()
                .WithMessage(ValidationUtilities.NonEmptyMessage)
                .MaximumLength(50).WithMessage(ValidationUtilities.MaxLengthMessage);
            

            var minimumDate=new DateTime(1900, 1, 1);
            RuleFor(p=>p.DateOfBirth)
                .GreaterThan(minimumDate)
                .WithMessage(ValidationUtilities.MinimumDOBMessage+ minimumDate.ToString("yyyy-MM-dd"));
        }
    }
}
