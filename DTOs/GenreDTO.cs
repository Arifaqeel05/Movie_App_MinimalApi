namespace Movie_App_MinimalApi.DTOs
{
    public class GenreDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}


/*purpose of this dto is to return final response to the client
without exposing any internal entity details eventhough in this case entity and dto are same*/