using AutoMapper;
using Movie_App_MinimalApi.DTOs;
using Movie_App_MinimalApi.Entity;

namespace Movie_App_MinimalApi.Utilities
{
    //purpose of this class is to define the mapping(meaning which property of source type will be mapped to which property of destination type)
    //between different objects(entities and dtos) using
    //automapper(automapper is a library that helps us to map one object to another object
    //without writing any code for it.we just have to define the mapping configuration in this class and automapper will take care of the rest).
    public class AutoMapperProfiles:Profile
    {
        //constructor to put all the automapping configurations
        public AutoMapperProfiles()
        {
            CreateMap<Genre, GenreDTO>(); //FIRST Parameter is the source type meaning the type we want to map from and the second parameter is the destination type meaning the type we want to map to.
            CreateMap<CreateUpdateGenreDTO, Genre>();
            CreateMap<Actor, ActorDTO>();
            CreateMap<CreateUpdateActorDTO, Actor>()
                .ForMember(p=>p.ActorPic,options=>options.Ignore());
            CreateMap<Movie, MovieDTO>();
            CreateMap<CreateMoviesDTO, Movie>()
                .ForMember(p => p.Poster, options => options.Ignore());
            CreateMap<Comment, CommentDTO>();
            CreateMap<CreateCommentDTO, Comment>();

        }
    }
}

//WE WANT automapper to map all the properties from createupdateactordto except the picture file
/*
 PURPOSE OF THE CODE IN THIS CLASS IS SIMPLE: TO DEFINE AUTOMAPPING PROFILES BETWEEN DIFFERENT OBJECTS (ENTITIES AND DTOS)
 */