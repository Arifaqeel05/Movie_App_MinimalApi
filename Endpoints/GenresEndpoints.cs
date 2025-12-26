using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Movie_App_MinimalApi.DTOs;
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

        /*--------------------------------GET ALL START-----------------------------------------------*/
        static async Task<Ok<List<GenreDTO>>> GetAll(IGenreRepository genreRepository, IMapper mapper)
        {
            var genres = await genreRepository.GetAll();

            var genresDTO = mapper.Map<List<GenreDTO>>(genres);//please map the "genres" list to "genresDTO" list
        /*
        genres.Select(genre => new GenreDTO{Id = genre.Id, Name = genre.Name}).ToList();*/

            return TypedResults.Ok(genresDTO);
        }

        /*----------------------------------END------------------------------------------------*/



        /*-----------------------------------GETBY ID------------------------------------------*/
        static async Task<Results<Ok<GenreDTO>, NotFound>> GetById(int id, IGenreRepository genreRepository,
                                                                    IMapper mapper)

        //Task<Results<ok, not found>>--->it will return either ok or not found result.we mention the possible return types in the angle brackets because there are multiple return types.
        {
            var genre = await genreRepository.GetById(id);
            if (genre is null)
            {
                return TypedResults.NotFound();
            }

            /*map genre entity to genreDTO, so that we dont expose internal entity details to the client.
            if we do not do so, any changes in entity will directly impact the client. */

            //object of GenreDTO so that we can return GenreDTO to the client
            var genreDTO =mapper.Map<GenreDTO>(genre);//please map the "genre" object to "genreDTO" object
            /*new GenreDTO 
            {
                Id = genre.Id, //it means genreDTO.Id= genre.Id-->go to entity and get the id and set it to genreDTO.Id
                Name = genre.Name//similarly set the Name property
            };*/

            return TypedResults.Ok(genreDTO);
        }
        /*----------------------------------------END HERE------------------------------------------*/




        /*-------------------------------------CREATE START HERE--------------------------------------*/
        static async Task<Created<GenreDTO>> Create(CreateUpdateGenreDTO creatupdaetgenreDTO,
                                                    IGenreRepository genreRepository,
                                                    IOutputCacheStore cachecleanig,IMapper mapper)
        {
            //THIS WILL CREATE A NEW GENRE BASED ON THE DATA RECEIVED FROM THE CLIENT IN THE DTO
            var genre =mapper.Map<Genre>(creatupdaetgenreDTO);
            /* new Genre
        {
            //here we have not mentioned Id because it will be generated by the database,
            //but in update we have to mention id because we are updating existing genre.

            Name = creatupdaetgenreDTO.Name
        };*/

            await genreRepository.Create(genre); //here we pass the genre object to the create method of repository so that it can be saved to the database

            var genreDTO = mapper.Map<GenreDTO>(genre);

            /*new GenreDTO
        {
            Id = genre.Id,
            Name = genre.Name
        };*/

            await cachecleanig.EvictByTagAsync("genre-get", default);//evict the cache with tag "genre-get"
            return TypedResults.Created($"/genre/{genre.Id}", genreDTO);//return 201 created response with location header and genreDTO in the response body
        }

        /*-----------------------------------END--------------------------------------------------------*/

        static async Task<Results<NoContent, NotFound>> Update(int id, CreateUpdateGenreDTO creatupdaetgenreDTO, 
                                                                IGenreRepository genreRepository, IOutputCacheStore cachecleanig,
                                                                IMapper mapper)
        {
            var existingGenre = await genreRepository.GetById(id);
            if (existingGenre is null)
            {
                return TypedResults.NotFound();
            }

            
            var genre = mapper.Map<Genre>(creatupdaetgenreDTO);
            genre.Id = id; //set the id of the genre object to the id received from the route parameter
            /*new Genre
        {
            Id = id, //THIS will come from the route parameter and no need to set from DTO
            Name = creatupdaetgenreDTO.Name //this will come from the DTO, we only allow updating the Name property
        };*/
            await genreRepository.Update(genre);//here we  call the update method of repository and pass the genre object
            await cachecleanig.EvictByTagAsync("genre-get", default);//evict the cache with tag "genre-get"
            return TypedResults.NoContent();//204 no content because we are not returning any content
        }

        static async Task<Results<NotFound, NoContent>> Delete(int id, IGenreRepository genreRepository, 
                                                                    IOutputCacheStore cachecleanig)
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

