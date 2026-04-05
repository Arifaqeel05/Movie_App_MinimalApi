using FluentValidation;
using Movie_App_MinimalApi.DTOs;
using Movie_App_MinimalApi.Repositories;

namespace Movie_App_MinimalApi.Validation
{
    
    
    public class CreateUpdateGenreDTOValidation : AbstractValidator<CreateUpdateGenreDTO>
    {
        //USE CONSTRUCTOR TO define the rules for validation
        public CreateUpdateGenreDTOValidation(
            IGenreRepository repository, IHttpContextAccessor httpContextAccessor
            //we can inject the repository here if we want to use it for validation,
            //for example, to check if a genre with the same name already exists in the database.
            )
        {
            var routeVaueId=httpContextAccessor.HttpContext!.Request.RouteValues["id"]; //we are fetching the id from the route values of the current HTTP request. This is useful when we want to perform validation that depends on the id of the genre being updated.
            var id = 0; //this variable will hold the integer value of the route value id after parsing. We initialize it to 0 in case the parsing fails or the route value id is not present.
            if (routeVaueId is string routeVaueIdString)
            {
                int.TryParse(routeVaueIdString, out id); //we are trying to parse the route value id as an integer and store it in the id variable. If the parsing is successful, the id variable will contain the integer value of the route value id. If the parsing fails, the id variable will remain 0.

            }

            RuleFor(p => p.Name)
                .NotEmpty()
                .WithMessage("The field {PropertyName} is required ")
                .MaximumLength(50).WithMessage("The field {PropertyName} must not exceed {MaxLength} characters")
                .Must(FirstLetterIsUpperCase).WithMessage("The first letter of {PropertyName} must be uppercase")
                .MustAsync(async (name, _) =>
                {
                    var exists = await repository.ExistGenre(id, name);
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
