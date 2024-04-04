using Blog_Web.Data.Abstract;
using Blog_Web.Entities;
using BlogApp.Data.Concrete.EfCore;
using BlogApp.Entity;

namespace Blog_Web.Data.Concrete.EfCore
{
    public class EFTUserRepository : IUserRepository
    {
        private BlogContext _context;
        public EFTUserRepository(BlogContext context)
        {
            _context = context;
        }
        public IQueryable<User> Users => _context.Users;
        public void CreateUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

      

       
    }
}
