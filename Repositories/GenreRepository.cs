using Dapper;
using Microsoft.Data.SqlClient;
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
            using (var connection = new SqlConnection(connectionString))
            {
                var q = connection.Query("SELECT 1").FirstOrDefault();
            }
            return Task.FromResult(0);
        }
    }
}
