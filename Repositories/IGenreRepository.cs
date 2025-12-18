using Movie_App_MinimalApi.Entity;

namespace Movie_App_MinimalApi.Repositories
{
    public interface IGenreRepository
    {
        //any class that implements this interface must implement this method
        Task<int> Create(Genre genre);
        //read
        
        Task<List<Genre>>GetAll();//task means its asynchronous, List will be returned after some time.
                                  //Genre is the type of object in the list.
        Task<Genre?>GetById(int id); //it will return a object of Genre 

        Task<bool>ExistGenre(int id);
        Task Update(Genre genre); //update method, no return type so void
    }
}
