using Movie_App_MinimalApi.DTOs;
using Movie_App_MinimalApi.Entity;

namespace Movie_App_MinimalApi.Repositories
{
    public interface IActorRepository
    {
        Task<int> Create(Actor actor);
        Task Delete(int id);
        Task<List<Actor>> GetAll(PaginationDTO pagination);
        Task<Actor?> GetById(int id);
        Task<List<Actor>> GetByName(string name);
        Task<bool> IsExist(int id);
        Task Update(Actor actor);
    }
}