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
        public async Task<int> Create(Genre genre)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                //put the query here
                var query=@"INSERT INTO Genres (Name) VALUES (@Name); 
                            SELECT SCOPE_IDENTITY();";//to get the last inserted id, SCOPE_IDENTITY() is used
                

                //send the query to the database
                var resultfromquery=await connection.QuerySingleAsync<int>(query,genre); //here we will get i
                genre.Id = resultfromquery;

                return resultfromquery;
            }
           
        }
    }
}
