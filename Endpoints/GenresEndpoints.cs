using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Movie_App_MinimalApi.Entity;
using Movie_App_MinimalApi.Repositories;

namespace Movie_App_MinimalApi.Endpoints
{
    public static class GenresEndpoints
    {
        public static RouteGroupBuilder MapGenres(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll)
                    .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("genre-get"));
            // Cache the response for 60 seconds
            //tag is used to evict or clear the cache when data is changed.


            group.MapGet("/{id:int}", GetById);
            /*async (int id, IGenreRepository genreRepository) =>
            {
            var genre = await genreRepository.GetById(id);
            if (genre is null)
            {
                return Results.NotFound();
            }
            return Results.Ok(genre);
            });*/



            group.MapPost("/createGenre", Create);
            /*async (Genre genre, IGenreRepository genreRepository, IOutputCacheStore cachecleanig) =>
            {
            await genreRepository.Create(genre);
            await cachecleanig.EvictByTagAsync("genre-get", default);//evict the cache with tag "genre-get"
            return Results.Created($"/genre/{genre.Id}", genre);
            });*/


            group.MapPut("/updateGenre/{id:int}", Update);
            /* async (int id,Genre genre, IGenreRepository genreRepository, IOutputCacheStore cachecleanig) =>
            {
            var existingGenre = await genreRepository.GetById(id);
            if (existingGenre is null)
            {
                return Results.NotFound();
            }
            await genreRepository.Update(genre);//here we  call the update method of repository and pass the genre object
            await cachecleanig.EvictByTagAsync("genre-get", default);//evict the cache with tag "genre-get"
            return Results.NoContent();//204 no content because we are not returning any content
            });*/

            group.MapDelete("/deleteGenre/{id:int}", Delete);
            /* async (int id, IGenreRepository genreRepository, IOutputCacheStore cachecleanig) =>
            {
                //var existingGenre = await genreRepository.GetById(id);

                var existingGenre=await genreRepository.ExistGenre(id);
                if (!existingGenre)
                {
                    return Results.NotFound();
                }
                await genreRepository.Delete(id);
                await cachecleanig.EvictByTagAsync("genre-get", default);//evict the cache with tag "genre-get"
                return Results.NoContent();
            });*/


            return group;
        }
        static async Task<Ok<List<Genre>>> GetAll(IGenreRepository genreRepository)
        {
            var genres = await genreRepository.GetAll();
            return TypedResults.Ok(genres);
        }

        static async Task<Results<Ok<Genre>, NotFound>> GetById(int id, IGenreRepository genreRepository)

        //Task<Results<ok, not found>>--->it will return either ok or not found result.we mention the possible return types in the angle brackets because there are multiple return types.
        {
            var genre = await genreRepository.GetById(id);
            if (genre is null)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(genre);
        }


        static async Task<Created<Genre>> Create(Genre genre, IGenreRepository genreRepository, IOutputCacheStore cachecleanig)
        {
            await genreRepository.Create(genre);
            await cachecleanig.EvictByTagAsync("genre-get", default);//evict the cache with tag "genre-get"
            return TypedResults.Created($"/genre/{genre.Id}", genre);
        }


        static async Task<Results<NoContent, NotFound>> Update(int id, Genre genre, IGenreRepository genreRepository, IOutputCacheStore cachecleanig)
        {
            var existingGenre = await genreRepository.GetById(id);
            if (existingGenre is null)
            {
                return TypedResults.NotFound();
            }
            await genreRepository.Update(genre);//here we  call the update method of repository and pass the genre object
            await cachecleanig.EvictByTagAsync("genre-get", default);//evict the cache with tag "genre-get"
            return TypedResults.NoContent();//204 no content because we are not returning any content
        }

        static async Task<Results<NotFound, NoContent>> Delete(int id, IGenreRepository genreRepository, IOutputCacheStore cachecleanig)
        {
            //var existingGenre = await genreRepository.GetById(id);

            var existingGenre = await genreRepository.ExistGenre(id);
            if (!existingGenre)
            {
                return TypedResults.NotFound();
            }
            await genreRepository.Delete(id);
            await cachecleanig.EvictByTagAsync("genre-get", default);//evict the cache with tag "genre-get"
            return TypedResults.NoContent();
        } };
    }

