using Movie_App_MinimalApi.Entity;

namespace Movie_App_MinimalApi.Repositories
{
    public interface IGenreRepository
    {
        //any class that implements this interface must implement this method
        Task<int> Create(Genre genre);
    }
}
