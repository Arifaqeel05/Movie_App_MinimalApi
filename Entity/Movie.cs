namespace Movie_App_MinimalApi.Entity
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public bool InTheater { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Poster { get; set; }


    }
}
