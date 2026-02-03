using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Movie_App_MinimalApi.DTOs;
using Movie_App_MinimalApi.Entity;
using Movie_App_MinimalApi.Repositories;
using Movie_App_MinimalApi.Services;

namespace Movie_App_MinimalApi.Endpoints
{
    public static class MoviesEndpoints
    {
        private readonly static string containerName = "movies-pics";
        public static RouteGroupBuilder MapMovies(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll)
                .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("movies-get"));
            group.MapGet("/{id:int}", GetById);

            group.MapPost("/createMovie", Create).DisableAntiforgery();
            group.MapPut("/updateMovie/{id:int}", Update).DisableAntiforgery();
            group.MapDelete("/deleteMovie/{id:int}", Delete);
            return group;
        }

        static async Task<Ok<List<MovieDTO>>> GetAll(IMovieRepository movieRepository,IMapper mapper, int page=1, 
            int recordsPerPage=10) 
        {
            var pagination = new PaginationDTO();
            pagination.Page = page;
            pagination.RecordsPerPage = recordsPerPage;

            var movies = await movieRepository.GetAll(pagination);//on the backend of this code, 
            var movieDTO = mapper.Map<List<MovieDTO>>(movies); //it will map movies that we have fetched into list having moviedto
            return TypedResults.Ok(movieDTO);
        }

        static async Task<Results<Ok<MovieDTO>, NotFound>> GetById(int id, IMovieRepository movieRepository, IMapper mapper)
        {
            var movie = await movieRepository.GetById(id);
            if (movie is null)
            {
                return TypedResults.NotFound();
            }
            var movieDTO = mapper.Map<MovieDTO>(movie);//MAP movie-->movieDTO
            return TypedResults.Ok(movieDTO);
        }
        static async Task<Created<MovieDTO>> Create([FromForm] CreateMoviesDTO createMoviesDTO,
            IFileStorage fileStorage, IMovieRepository movieRepository,
             IOutputCacheStore outputCacheStore, IMapper mapper) 
        {
            var movie=mapper.Map<Movie>(createMoviesDTO);
            //in simple words:it copies data from createMoviesDTO to movie entity for saving in database 
            if (createMoviesDTO.Poster is not null)
            {
                var url=await fileStorage.Store(containerName, createMoviesDTO.Poster);//store the poster file and get its url
                movie.Poster=url;
            }

            var id=await movieRepository.Create(movie);
            //we have passed movie to repository create method to save in database. it will return the id of the newly created movie.movie is entity object of class Movie
            await outputCacheStore.EvictByTagAsync("movies-get", default);//empty the cache with tag "movies-get",purpose of tag is to identify the cache entries related to movies
            var movieDTO=mapper.Map<MovieDTO>(movie);//map the movie entity to movieDTO to send in response
            return TypedResults.Created($"/movies/{id}", movieDTO);
        }

        static async Task<Results<NoContent, NotFound>>Update(int id, [FromForm] CreateMoviesDTO createMoviesDTO, IMovieRepository movieRepository
            ,IFileStorage fileStorage, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var checkMovieExist=await movieRepository.GetById(id);
            if (checkMovieExist is null)
            {
                return TypedResults.NotFound();
            }
            var movieForUpdate=mapper.Map<Movie>(createMoviesDTO);//map the incoming dto to movie entity
            movieForUpdate.Id=id;//set the id of movie entity to the id passed in url
            movieForUpdate.Poster=checkMovieExist.Poster;//set the poster of movie entity to the existing poster in database
            if (createMoviesDTO.Poster is not null)
            {
                var url=await fileStorage.Edit(containerName, createMoviesDTO.Poster, checkMovieExist.Poster);
                //createMoviesDTO.Poster is the new poster file to be updated
                //checkMovieExist.Poster is the existing poster url in database.
                //in simple words, we are replacing the existing poster with the new poster
                movieForUpdate.Poster=url;
            }
            await movieRepository.Update(movieForUpdate);
            await outputCacheStore.EvictByTagAsync("movies-get", default);
            return TypedResults.NoContent();
        }
        static async Task<Results<NoContent, NotFound>> Delete(int id, IMovieRepository movieRepository,
            IFileStorage fileStorage, IOutputCacheStore outputCacheStore)
        {
            var movieExist=await movieRepository.GetById(id);
            if (movieExist is null)
            {
                return TypedResults.NotFound();
            }
            await movieRepository.Delete(id);//goto repository or database and delete the movie with given id
            await fileStorage.Delete(movieExist.Poster, containerName);//deleting the poster file from storage
            await outputCacheStore.EvictByTagAsync("movies-get", default);
            return TypedResults.NoContent();
        }
    }
}
