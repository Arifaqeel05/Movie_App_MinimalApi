using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Movie_App_MinimalApi.DTOs;
using Movie_App_MinimalApi.Entity;
using Movie_App_MinimalApi.Repositories;
using Movie_App_MinimalApi.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
namespace Movie_App_MinimalApi.Endpoints
{
    public static class ActorsEndpoints
    {
        private readonly static string containerName = "actors-pics";//folder name to store actor pictures
        public static RouteGroupBuilder MapActors(this RouteGroupBuilder group)
        {
           
            //group.MapGet("/", GetAll)
            //.CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("actors-get"));
            //group.MapGet("/{id:int}", GetById);
            group.MapPost("/createActor", Create).DisableAntiforgery();//disable antiforgery for testing in postman
            //group.MapPut("/updateActor/{id:int}", Update);
            //group.MapDelete("/deleteActor/{id:int}", Delete);
            return group;
        }

        static async Task<Created<ActorDTO>> Create([FromForm] //use fromform to receive form data including file
                                            CreateUpdateActorDTO creatupdaetactorDTO,//data received from client
                                            IActorRepository actorRepository,//for database operations
                                            IFileStorage fileStorage,//for file storage operations
                                            IOutputCacheStore cachecleanig, IMapper mapper)
        {
            //THIS WILL CREATE A NEW Actor BASED ON THE DATA RECEIVED FROM THE CLIENT IN THE DTO and map to actor entity
            var actor = mapper.Map<Actor>(creatupdaetactorDTO);

            if(creatupdaetactorDTO.ActorPic is not null)
            {
                var url = await fileStorage.Store(containerName, creatupdaetactorDTO.ActorPic);
                actor.ActorPic = url; 
                //set the ActorPic property of actor entity to the url returned by file storage service.url will be saved in the database
            }

            await actorRepository.Create(actor); //here we pass the actor object to the create method of repository so that it can be saved to the database

            var actorDTO = mapper.Map<ActorDTO>(actor);


            await cachecleanig.EvictByTagAsync("actors-get", default);//evict the cache with tag "genre-get"
            return TypedResults.Created($"/actor/{actor.Id}", actorDTO);//return 201 created response with location header and genreDTO in the response body
        }

        
    }
}
