namespace Movie_App_MinimalApi.Validation
{
    public static class ValidationUtilities
    {
        public static string NonEmptyMessage = "The field {PropertyName} is required ";
        public static string MaxLengthMessage = "The field {PropertyName} must not exceed {MaxLength} characters";
        public static string FirstLetterUpperCaseMessage = "The first letter of {PropertyName} must be uppercase";

        public static string MinimumDOBMessage = "The field {PropertyName} must be greater than";
        //custom validation:
        public static bool FirstLetterIsUpperCase(string value)
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
