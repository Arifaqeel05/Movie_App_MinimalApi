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

        public async Task<List<Genre>> GetAll()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = "SELECT Id, Name FROM Genres ORDER BY NAME DESC";//this is SQL query simply.
                var resultfromquery = await connection.QueryAsync<Genre>(query);//return result of Genre type and it is asynchronous.
                return resultfromquery.ToList();
            }
        }

        public async Task<Genre?> GetById(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = "SELECT Id, Name FROM Genres WHERE Id = @Id";//parameterized query to prevent SQL injection
                var parameter=new {Id=id };//anonymous object to hold the parameter value
                var findedResult = await connection.QueryFirstOrDefaultAsync<Genre>(query, parameter);
                //@Id is coming from client side and it is mapped to id parameter of this method.
                //QueryFirstOrDefaultAsync will return null if no record is found.
                //we can't pass simple id directly to the query because it may lead to SQL injection.so we pass anyonymous object.
                
                return findedResult; //object of Genre type will be returned corrosponding to the given id.
            }
        }
    }
}
