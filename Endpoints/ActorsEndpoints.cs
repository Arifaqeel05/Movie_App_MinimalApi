using AutoMapper;
using FluentValidation;
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
        private readonly static string containerName = "actors-pics";
        /*folder name to store actor pictures, we will see this name inside the azure container ,
          this will act as a parent foloder of storing actor pictures
        */
        public static RouteGroupBuilder MapActors(this RouteGroupBuilder group)
        {

            group.MapGet("/", GetAll)
                .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("actors-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapGet("/searchByName/{name}", GetByName);
                
            group.MapPost("/createActor", Create).DisableAntiforgery();//disable antiforgery for testing in postman
            group.MapPut("/updateActor/{id:int}", Update).DisableAntiforgery(); ;
            group.MapDelete("/deleteActor/{id:int}", Delete);
            return group;
        }


        /*Task<Ok<List<ActorDTO>>>-->this means that task is a asynchronous method that will return a result of type Ok<List<ActorDTO>>.Ok is a helper class
        that represents a 200 OK response with a value of type List<ActorDTO> in the response body.*/
        static async Task<Ok<List<ActorDTO>>>GetAll(
            IActorRepository actorRepository,  
            IMapper mapper, 
            int page=1, int recordsPerPage=10) //we didn't use PaginationDTO here to keep it simple
        {
            //create instance of paginationDTO and now set its properties
            var pagination = new PaginationDTO{ Page = page, RecordsPerPage = recordsPerPage };

            //now pass pagination to repository method so it can fetch number of records based on pagination

            var actors = await actorRepository.GetAll(pagination);//fetch all actors from database
            var actorsDTO = mapper.Map<List<ActorDTO>>(actors);//map the list of actor entities to list of actorDTOs
            return TypedResults.Ok(actorsDTO);//return 200 ok response with list of actorDTOs in the response body
        }

        static async Task<Results<Ok<ActorDTO>,NotFound>> GetById(int id ,IActorRepository actorRepository, 
            IMapper mapper)
        {
            var actor = await actorRepository.GetById(id);//fetch actor by id from database
            if (actor is null)
            {
                return TypedResults.NotFound();//return 404 not found if actor is not found
            }
            var actorDTO = mapper.Map<ActorDTO>(actor);//map the actor entity to actorDTO
            return TypedResults.Ok(actorDTO);//return 200 ok response with actorDTO in the response body

        }

        static async Task<Ok<List<ActorDTO>>>GetByName(string name, IActorRepository actorRepository, IMapper mapper)
        {
            var actors = await actorRepository.GetByName(name);//fetch actors by name from database
            var actorsDTO = mapper.Map<List<ActorDTO>>(actors);//map the list of actor entities to list of actorDTOs
            return TypedResults.Ok(actorsDTO);//return 200 ok response with list of actorDTOs in the response body
        }

        static async Task<Results<Created<ActorDTO>,ValidationProblem>> Create([FromForm] //use fromform to receive form data including file
                                            CreateUpdateActorDTO creatupdaetactorDTO,//data received from client
                                            IActorRepository actorRepository,//for database operations
                                            IFileStorage fileStorage,//for file storage operations
                                            IOutputCacheStore cachecleanig, IMapper mapper,
                                            IValidator<CreateUpdateActorDTO> validator
                                            )
        {
            var validationResult= await validator.ValidateAsync(creatupdaetactorDTO);//validate the incoming data using fluent validation

            if (!validationResult.IsValid)
            {
                return TypedResults.ValidationProblem(validationResult.ToDictionary());
            }

            var actor = mapper.Map<Actor>(creatupdaetactorDTO);//here we are mapping the createUpdateActorDTO to actor entity,automapper will automatically map the properties with same name and type

            if (creatupdaetactorDTO.ActorPic is not null)
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

        static async Task<Results<NotFound,NoContent >> Update(int id,
            [FromForm] CreateUpdateActorDTO createUpdateActorDTO, IActorRepository actorRepository,
            IFileStorage fileStorage, IOutputCacheStore cachecleanig, IMapper mapper
            )
        {
            var actotDB= await actorRepository.GetById(id);//fetch actor by id from database
            if (actotDB is null)
            {
                return TypedResults.NotFound();//return 404 not found if actor is not found
            }
            var actorForUpdate=mapper.Map<Actor>(createUpdateActorDTO);//map the existing actor entity to a new actor entity for update
            actorForUpdate.Id = id;
            actorForUpdate.ActorPic = actotDB.ActorPic;//set the existing actor picture url to the new actor entity

            if(createUpdateActorDTO.ActorPic is not null)
            {
                //if new picture is provided, then we need to update the picture
                var url = await fileStorage.Edit(containerName, createUpdateActorDTO.ActorPic, actorForUpdate.ActorPic);      
                actorForUpdate.ActorPic = url;
                //set the ActorPic property of actor entity to the url returned by file storage service.url will be saved in the database
            }
            await actorRepository.Update(actorForUpdate);//update the actor in the database
            await cachecleanig.EvictByTagAsync("actors-get", default);//evict the cache with tag "genre-get"
            return TypedResults.NoContent();//return 204 no content response
        }

        static async Task<Results<NotFound, NoContent>> Delete(int id, IActorRepository actorRepository,
            IFileStorage fileStorage, IOutputCacheStore cachecleanig)
        {
            var actor = await actorRepository.GetById(id);//fetch actor by id from database
            if (actor is null)
            {
                return TypedResults.NotFound();//return 404 not found if actor is not found
            }

            await actorRepository.Delete(id);//delete the actor from database
            await fileStorage.Delete(actor.ActorPic, containerName);//delete the actor picture from file storage
            await cachecleanig.EvictByTagAsync("actors-get", default);//evict the cache with tag "genre-get"
            return TypedResults.NoContent();//return 204 no content response


        }

        }
}
