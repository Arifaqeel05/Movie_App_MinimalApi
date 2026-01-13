using Dapper;
using Microsoft.Data.SqlClient;
using Movie_App_MinimalApi.DTOs;
using Movie_App_MinimalApi.Entity;
using System.Data;

namespace Movie_App_MinimalApi.Repositories
{
    public class ActorRepository : IActorRepository
    {
        private readonly string connectionString;
        private readonly HttpContext httpContext;

        //constructor to get the connection string from appsettings.json.this is dependency injection.
        public ActorRepository(IConfiguration config,IHttpContextAccessor httpContextAccessor)
        {
            connectionString = config.GetConnectionString("DefaultConnection")!;
            httpContext=httpContextAccessor.HttpContext!;
        }

        public async Task<int> Create(Actor actor)
        {
            using (var connection = new SqlConnection(connectionString))//establishing connection with database
            {
                var query = "Actors_Create";//to get the last inserted id
                var parameters = new
                {
                    Name = actor.Name,
                    DateOfBirth = actor.DateOfBirth,
                    ActorPic = actor.ActorPic
                };

                var id = await connection.QuerySingleAsync<int>(query, parameters, commandType: CommandType.StoredProcedure);
                actor.Id = id;//set the id of the actor object ,it will come from database.
                return id;

            }
        }

        public async Task<List<Actor>> GetAll(PaginationDTO pagination)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = "Actors_GetAll"; //we had modified stored procedure to accept pagination parameters
                var actors = await connection.QueryAsync<Actor>(query,
                    new { pagination.Page, pagination.RecordsPerPage },
                    commandType: CommandType.StoredProcedure);
                    var actorsCount= await connection.QueryAsync<int>("Actors_Count",commandType: CommandType.StoredProcedure);
                //we will use the header of http response to send total count of records to client

                httpContext.Response.Headers.Append("TotalAmountOfActors", actorsCount.ToString());
                return actors.ToList();
            }
        }

        public async Task<bool> IsExist(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = @"Actors_Exist";
                var parameter = new { Id = id };
                var exists = await connection.ExecuteScalarAsync<bool>(query, parameter);
                return exists;
            }
        }

        public async Task<Actor?> GetById(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = "Actors_GetById";
                var parameter = new { Id = id };
                var actor = await connection.QuerySingleOrDefaultAsync<Actor>(query, parameter, commandType: CommandType.StoredProcedure);
                return actor;
            }
        }

        public async Task Update(Actor actor)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = "Actors_Update";
                var parameters = new
                {
                    Id = actor.Id,
                    Name = actor.Name,
                    DateOfBirth = actor.DateOfBirth,
                    ActorPic = actor.ActorPic
                };
                await connection.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task Delete(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = "Actors_Delete";
                var parameter = new { Id = id };
                await connection.ExecuteAsync(query, parameter, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<List<Actor>>GetByName(string name)
        {
            using (var connection = new SqlConnection(connectionString))//establishing connection with database
            {
                var query = "Actors_GetByName";//stored procedure name
                var parameter = new { Name = name };//parameter object
                var actors = await connection.QueryAsync<Actor>(query, parameter, commandType: CommandType.StoredProcedure);
                return actors.ToList();
            }
        }
    }
}
