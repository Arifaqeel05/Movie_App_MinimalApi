using FluentValidation;
using Movie_App_MinimalApi.DTOs;
namespace Movie_App_MinimalApi.Validation
{
    
    
    public class CreateUpdateGenreDTOValidation : AbstractValidator<CreateUpdateGenreDTO>
    {
        //USE CONSTRUCTOR TO define the rules for validation
        public CreateUpdateGenreDTOValidation()
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .WithMessage("The field {PropertyName} is required ")
                .MaximumLength(50).WithMessage("The field {PropertyName} must not exceed {MaxLength} characters")
                .Must(FirstLetterIsUpperCase).WithMessage("The first letter of {PropertyName} must be uppercase")
                ;
            //THIS MEANS THAT THE NAME PROPERTY CANNOT BE EMPTY IN THE CreateUpdateGenreDTO OBJECT.
            //IF IT IS EMPTY, THE VALIDATION WILL FAIL AND AN ERROR MESSAGE WILL BE GENERATED.

        }
        

        //custom validation:
        public bool FirstLetterIsUpperCase(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return true; // Let the NotEmpty rule handle this case
            }

            var firstLetter = value[0].ToString();
            return firstLetter == firstLetter.ToUpper();
        }
    }
}
