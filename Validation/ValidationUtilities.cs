namespace Movie_App_MinimalApi.Validation
{
    public static class ValidationUtilities
    {
        public static string NonEmptyMessage = "The field {PropertyName} is required ";
        public static string MaxLengthMessage = "The field {PropertyName} must not exceed {MaxLength} characters";
        public static string FirstLetterUpperCaseMessage = "The first letter of {PropertyName} must be uppercase";
    }

    
}
