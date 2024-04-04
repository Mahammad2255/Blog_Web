using Blog_Web.Data.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.EntityFrameworkCore;

namespace Blog_Web.ViewComponents
{
    public class NewPosts : ViewComponent
    {
        private IPostRepository _postRepository;
        public NewPosts(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var posts = _postRepository.Posts.Where(x => x.IsActive);    
            return View(await posts
                .OrderByDescending(p => p.PublishedOn)
                .Take(2)
                .ToListAsync()
                );
        }

    }
}
