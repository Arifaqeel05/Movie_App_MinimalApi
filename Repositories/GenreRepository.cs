using Dapper;
using Microsoft.Data.SqlClient;
using Movie_App_MinimalApi.Entity;

namespace Movie_App_MinimalApi.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly string? connectionString;
        //constructor to get the connection string from appsettings.json.this is dependency injection.
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
                var query = "SELECT Id, Name FROM Genres ORDER BY NAME";//this is SQL query simply.
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

        public async Task<bool> ExistGenre(int id)
        {
            // Create a SQL Server connection using the connection string
            using (var connection = new SqlConnection(connectionString))
            {
                // This SQL checks if a record with the given Id exists in Genres table
                // IF EXISTS → returns TRUE if at least one row is found
                // If found → SELECT 1
                // If not found → SELECT 0
                var query = @"IF EXISTS (SELECT 1 FROM Genres WHERE Id = @Id)
                        SELECT 1
                      ELSE
                        SELECT 0";

                // Anonymous object to safely pass parameter to SQL
                // @Id in SQL will get value of 'id'
                var parameter = new { Id = id };

                // QuerySingleAsync is used because:
                // 1️⃣ SQL ALWAYS returns exactly ONE value (1 or 0)
                // 2️⃣ We are expecting a single result
                // 3️⃣ If SQL returns more or zero rows → it should throw an error
                var existGenre = await connection.QuerySingleAsync<bool>(query, parameter);

                // existGenre will be:
                // true  → if SELECT 1
                // false → if SELECT 0
                return existGenre;
            }
        }

        public async Task Update(Genre genre)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = "UPDATE GENRES SET Name = @Name WHERE Id = @Id";
                var parameter = new { Name = genre.Name, Id = genre.Id };
                await connection.ExecuteAsync(query,parameter);//ExecuteAsync is used for commands that do not return any data.
            }

        }

        public async Task Delete(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = "DELETE FROM GENRES WHERE Id = @Id";
                var parameter = new { Id = id };
                await connection.ExecuteAsync(query,parameter);
            }
        }
    }
}
