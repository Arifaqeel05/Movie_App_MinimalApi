namespace Movie_App_MinimalApi.DTOs
{
    public class MovieDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public bool InTheater { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string? Poster { get; set; } 

        public List<CommentDTO> Comments { get; set; } = new List<CommentDTO>();
        //we will use this property to include the comments of the movie in the response when we get the details of a movie by id,
        //so that we can avoid making another request to get the comments of the movie separately.
    }
}
