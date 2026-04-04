namespace Movie_App_MinimalApi.Entity
{
    public class GenreMovie
    {
        public int MovieId { get; set; }
        public int GenreId { get; set; }

        //navigation properties--> these are added to establish the relationship between the entities, so that we can easily access the related data when we query the database.
        public Movie Movie { get; set; } = null!;
        public Genre Genre { get; set; } = null!;
    }
}
