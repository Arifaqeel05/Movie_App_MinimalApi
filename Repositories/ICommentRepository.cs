using Movie_App_MinimalApi.Entity;

namespace Movie_App_MinimalApi.Repositories
{
    public interface ICommentRepository
    {
        Task<int> Create(Comment comment);
        Task Delete(int id);
        Task<List<Comment>> GetAll(int movieId);
        Task<Comment?> GetById(int id);
        Task<bool> IsExist(int id);
        Task Update(Comment comment);
    }
}