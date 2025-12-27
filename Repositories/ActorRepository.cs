using Dapper;
using Microsoft.Data.SqlClient;
using Movie_App_MinimalApi.Entity;
using System.Data;

namespace Movie_App_MinimalApi.Repositories
{
    public class ActorRepository : IActorRepository
    {
        private readonly string connectionString;
        //constructor to get the connection string from appsettings.json.this is dependency injection.
        public ActorRepository(IConfiguration config)
        {
            connectionString = config.GetConnectionString("DefaultConnection")!;
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

                var id = await connection.QuerySingleAsync(query, parameters,commandType:CommandType.StoredProcedure);
                actor.Id = id;//set the id of the actor object ,it will come from database.
                return id;

            }
        }

        public async Task<List<Actor>> GetAll()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = "Actors_GetAll";
                var actors = await connection.QueryAsync<Actor>(query, commandType:CommandType.StoredProcedure);
                return actors.ToList();
            }
        }

        public async Task<bool> IsExist(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = @"IF EXISTS(SELECT 1 FROM ACTORS WHERE Id = @Id)
                                SELECT 1
                              ELSE
                                SELECT 0";
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
                var actor = await connection.QuerySingleOrDefaultAsync<Actor>(query, parameter, commandType:CommandType.StoredProcedure);
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
                await connection.ExecuteAsync(query, parameters, commandType:CommandType.StoredProcedure);
            }
        }
        public async Task Delete(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var query = @"DELETE FROM Actors
                            WHERE Id=@Id";
                var parameter = new { Id = id };
                await connection.ExecuteAsync(query, parameter);
            }
        }
    }
}
