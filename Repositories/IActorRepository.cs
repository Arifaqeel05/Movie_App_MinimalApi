using Movie_App_MinimalApi.Entity;

namespace Movie_App_MinimalApi.Repositories
{
    public interface IActorRepository
    {
        Task<int> Create(Actor actor);
        Task Delete(int id);
        Task<List<Actor>> GetAll();
        Task<Actor?> GetById(int id);
        Task<bool> IsExist(int id);
        Task Update(Actor actor);
    }
}