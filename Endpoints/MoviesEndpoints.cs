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
    }
}
