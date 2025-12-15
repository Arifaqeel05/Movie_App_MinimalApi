using Movie_App_MinimalApi.Entity;

namespace Movie_App_MinimalApi.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly string? connectionString;

        public GenreRepository(IConfiguration config)
        {
             connectionString = config.GetConnectionString("DefaultConnection")!;
        }
        public Task<int> Create(Genre genre)
        {
           
        }
    }
}
