namespace Movie_App_MinimalApi.Entity
{
    public class Comment
    {
        public int Id { get; set; }
        public string Body { get; set; }=null!;
        public int MovieId { get; set; } //foreign key to the movie that the comment belongs to
    }
}
