using Blog_Web.Data.Abstract;
using Blog_Web.Entities;
using BlogApp.Data.Concrete.EfCore;
using BlogApp.Entity;

namespace Blog_Web.Data.Concrete.EfCore
{
    public class EFTagRepository : ITagRepository
    {
        private BlogContext _context;
        public EFTagRepository(BlogContext context)
        {
            _context = context;
        }
        public IQueryable<Tag> Tags => _context.Tags;

        public void CreateTag(Tag post)
        {
            _context.Tags.Add(post);
            _context.SaveChanges();
        }
    }
}
