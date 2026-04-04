namespace Movie_App_MinimalApi.Entity
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public bool InTheater { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Poster { get; set; }
        public List<Comment> comments { get; set; } = new List<Comment>();
        //syntax explan: Comment is the type of the list, comments is name of list 
        //it means that the movie can have multiple comments, and we initialize the list to an empty list to avoid null reference issues when adding comments to a movie.


    }
}
