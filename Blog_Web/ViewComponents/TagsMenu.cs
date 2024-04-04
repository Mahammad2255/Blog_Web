using Blog_Web.Data.Abstract;
using BlogApp.Data.Concrete.EfCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog_Web.ViewComponents
{
    public class TagsMenu : ViewComponent
    {
        private BlogContext _context;
        private ITagRepository _tagRepository;
        public TagsMenu(ITagRepository tagRepository, BlogContext context)
        {
            _tagRepository = tagRepository;
            _context = context;

        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var tagWithPosts = await _context.Tags.Where(tag=>tag.Posts.Any(post=>post.IsActive)).ToListAsync();
            return View(tagWithPosts);
        }
    }
}
