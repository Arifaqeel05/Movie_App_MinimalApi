namespace Movie_App_MinimalApi.DTOs
{
    public class CreateMoviesDTO
    {
        public string Title { get; set; } = null!;
        public bool InTheater { get; set; }
        public DateTime ReleaseDate { get; set; }
        public IFormFile? Poster { get; set; }
    }
}
