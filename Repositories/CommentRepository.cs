using Dapper;
using Microsoft.Data.SqlClient;
using Movie_App_MinimalApi.Entity;
using System.Data;

namespace Movie_App_MinimalApi.Repositories
{
    public class CommentRepository
    {
        private readonly string myConnectionString;

        //constructor injection for the database context configuration
        public CommentRepository(IConfiguration configuration)
        {
            //connection string to the database
            myConnectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        //as we are in the repository/data access layer, we will write the code to interact with the database using ADO.NET

        public async Task<int> Create(Comment comment)
        {
            //using statement to ensure that the connection is properly disposed of after use
            using (var connection = new SqlConnection(myConnectionString))
            //establishing a connection to the database using the connection string
            {
                var query = "Comments_Create"; //name of the stored procedure to execute
                var parameters = new
                {
                    Body = comment.Body,
                    MovieId = comment.MovieId
                };
                var id = await connection.QuerySingleAsync<int>(query, parameters, commandType: CommandType.StoredProcedure);
                comment.Id = id; //set the id of the comment object, it will come from database.
                return id;

            }
        } //purpose of this create method is to insert a new comment into the database and return the id of the newly created comment. It uses Dapper to execute a stored procedure named "Comments_Create" with the parameters for the comment's body and associated movie id. The result of the query is expected to be the id of the newly created comment, which is then assigned to the comment object and returned.

        public asy
    }
}
