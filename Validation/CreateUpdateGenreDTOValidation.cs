using FluentValidation;
using Movie_App_MinimalApi.DTOs;
using Movie_App_MinimalApi.Repositories;

namespace Movie_App_MinimalApi.Validation
{
    
    
    public class CreateUpdateGenreDTOValidation : AbstractValidator<CreateUpdateGenreDTO>
    {
        //USE CONSTRUCTOR TO define the rules for validation
        public CreateUpdateGenreDTOValidation(
            IGenreRepository repository //we can inject the repository here if we want to use it for validation,
                                        //for example, to check if a genre with the same name already exists in the database.
            )
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .WithMessage("The field {PropertyName} is required ")
                .MaximumLength(50).WithMessage("The field {PropertyName} must not exceed {MaxLength} characters")
                .Must(FirstLetterIsUpperCase).WithMessage("The first letter of {PropertyName} must be uppercase")
                .MustAsync(async (name, _) =>
                {
                    var exists = await repository.ExistGenre(0, name);
                    //we pass 0 for id because we are creating a new genre and we want to check if any genre with the same name already exists in the database.
                    return !exists; //if exists is true, it means a genre with the same name already exists, so we return false to indicate that the validation has failed. If exists is false, it means no genre with the same name exists, so we return true to indicate that the validation has passed.
                }).WithMessage(g=>$"A genre with {g.Name} already exists");

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
