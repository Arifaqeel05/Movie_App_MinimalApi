using Movie_App_MinimalApi.DTOs;
using Movie_App_MinimalApi.Entity;

namespace Movie_App_MinimalApi.Repositories
{
    public interface IMovieRepository
    {
        Task<int> Create(Movie movie);
        Task Delete(int id);
        Task<List<Movie>> GetAll(PaginationDTO pagination);
        Task<Movie?> GetById(int id);
        Task<List<Movie>> GetByName(string title);
        Task<bool> IsExist(int id);
        Task Update(Movie movie);
    }
}