using AutoMapper;
using Movie_App_MinimalApi.DTOs;
using Movie_App_MinimalApi.Entity;

namespace Movie_App_MinimalApi.Utilities
{
    public class AutoMapperProfiles:Profile
    {
        //constructor to put all the automapping configurations
        public AutoMapperProfiles()
        {
            CreateMap<Genre, GenreDTO>();
            CreateMap<CreateUpdateGenreDTO, Genre>();
            CreateMap<Actor, ActorDTO>();
            CreateMap<CreateUpdateActorDTO, Actor>()
                .ForMember(p=>p.ActorPic,options=>options.Ignore());
            CreateMap<Movie, MovieDTO>();
            CreateMap<CreateMoviesDTO, Movie>()
                .ForMember(p => p.Poster, options => options.Ignore());
        }
    }
}

//WE WANT automapper to map all the properties from createupdateactordto except the picture file
/*
 PURPOSE OF THE CODE IN THIS CLASS IS SIMPLE: TO DEFINE AUTOMAPPING PROFILES BETWEEN DIFFERENT OBJECTS (ENTITIES AND DTOS)
 */