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
        }
    }
}
