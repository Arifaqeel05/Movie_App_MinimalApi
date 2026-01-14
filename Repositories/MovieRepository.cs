
using Dapper;
using Microsoft.Data.SqlClient;
using Movie_App_MinimalApi.DTOs;
using Movie_App_MinimalApi.Entity;
using System.Data;

namespace Movie_App_MinimalApi.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly string? connectionString;
        private readonly HttpContext httpContext;

        public MovieRepository(IConfiguration configuration, HttpContextAccessor httpContextAccessor)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")!;
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<int> Create(Movie movie)
        {
            using (var connection = new SqlConnection(connectionString))//establishing connection with database
            {
                var query = "Movies_Create";//to get the last inserted id
                var parameters = new
                {
                    Title = movie.Title,
                    ReleaseDate = movie.ReleaseDate,
                    InTheater = movie.InTheater,
                    Poster = movie.Poster

                };

                var id = await connection.QuerySingleAsync<int>(query, parameters, commandType: CommandType.StoredProcedure);
                movie.Id = id;//set the id of the actor object ,it will come from database.
                return id;

            }
        }

        public async Task<List<Movie>> GetAll(PaginationDTO pagination)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = "Movies_GetAll"; //we had modified stored procedure to accept pagination parameters
                var movies = await connection.QueryAsync<Movie>(query,
                new { pagination.Page, pagination.RecordsPerPage },
                commandType: CommandType.StoredProcedure);
                var movieCount = await connection.QuerySingleAsync<int>("Movies_Count", commandType: CommandType.StoredProcedure);
                //we will use the header of http response to send total count of records to client

                httpContext.Response.Headers.Append("TotalAmountOfMoviea", movieCount.ToString());
                return movies.ToList();
            }
        }
        public async Task<bool> IsExist(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = "Movies_Exist";
                var parameter = new { Id = id };
                var exists = await connection.ExecuteScalarAsync<bool>(query, parameter);
                return exists;
            }
        }

        public async Task<Movie?> GetById(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = "Movies_GetById";
                var parameter = new { Id = id };
                var movies = await connection.QuerySingleOrDefaultAsync<Movie>(query, parameter, commandType: CommandType.StoredProcedure);
                return movies;
            }
        }
        public async Task Update(Movie movie)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = "Movies_Update";
                var parameters = new
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    ReleaseDate = movie.ReleaseDate,
                    Poster = movie.Poster,
                    InTheater = movie.InTheater

                };
                await connection.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task Delete(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = "Movies_Delete";
                var parameter = new { Id = id };
                await connection.ExecuteAsync(query, parameter, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<List<Movie>> GetByName(string title)
        {
            using (var connection = new SqlConnection(connectionString))//establishing connection with database
            {
                var query = "Movies_GetByName";//stored procedure name
                var parameter = new { Title = title };//parameter object
                var movies = await connection.QueryAsync<Movie>(query, parameter, commandType: CommandType.StoredProcedure);
                return movies.ToList();
            }

        }
    }
}
