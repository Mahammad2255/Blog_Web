using Blog_Web.Data.Abstract;
using Blog_Web.Entities;
using BlogApp.Data.Concrete.EfCore;
using Microsoft.EntityFrameworkCore;

namespace Blog_Web.Data.Concrete.EfCore
{
    public class EFPostRepository : IPostRepository
    {
        private BlogContext _context;
        public EFPostRepository(BlogContext context)
        {
            _context = context;
        }
        public IQueryable<Post> Posts => _context.Posts;

        

        public void CreatePost(Post post, int[] tagIds)
        {
            _context.Posts.Add(post);

            foreach (var tagId in tagIds)
            {
                var tag = _context.Tags.FirstOrDefault(t => t.TagId == tagId);
                if (tag != null)
                {
                    post.Tags.Add(tag);
                }
            }

            _context.SaveChanges();

        }

        public void CreatePost(Post post)
        {
            _context.Posts.Add(post);
            _context.SaveChanges();

        }

        public void DeletePost(Post id)
        {
            _context.Posts?.Remove(id);
            _context.SaveChanges(); 
        }

        public void EditPost(Post post)
        {
            var entity = _context.Posts.FirstOrDefault(i => i.PostId == post.PostId);
            if (entity != null) 
            {
                entity.Title = post.Title;
                entity.Description = post.Description;
                entity.IsActive = post.IsActive;
                entity.Url = post.Url;  
                entity.Content = post.Content;  
                entity.Image = post.Image;

                _context.SaveChanges();
            }

        }

        public void EditPost(Post post, int[] tagIds)
        {
            var entity = _context.Posts.Include(i=>i.Tags).FirstOrDefault(i => i.PostId == post.PostId);
            if (entity != null)
            {
                entity.Title = post.Title;
                entity.Description = post.Description;
                entity.IsActive = post.IsActive;
                entity.Url = post.Url;
                entity.Content = post.Content;
                entity.Image = post.Image;  

                entity.Tags = _context.Tags.Where(t=> tagIds.Contains(t.TagId)).ToList();   
                _context.SaveChanges();
            }
        }

        public void EditPostNoPhoto(Post post, int[] tagIds)
        {
            var entity = _context.Posts.Include(i => i.Tags).FirstOrDefault(i => i.PostId == post.PostId);
            if (entity != null)
            {
                entity.Title = post.Title;
                entity.Description = post.Description;
                entity.IsActive = post.IsActive;
                entity.Url = post.Url;
                entity.Content = post.Content;
                

                entity.Tags = _context.Tags.Where(t => tagIds.Contains(t.TagId)).ToList();
                _context.SaveChanges();
            }
        }
    }
}
