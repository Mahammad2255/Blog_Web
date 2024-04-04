using Blog_Web.Entities;
using BlogApp.Entity;

namespace Blog_Web.Entities
{
    public interface IUserRepository
    {
        IQueryable<User> Users { get; }
        void CreateUser(User user);
      
    }
}
