using Blog_Web.Data.Abstract;
using Blog_Web.Data.Concrete.EfCore;
using Blog_Web.Entities;
using Blog_Web.Model;
using BlogApp.Data.Concrete.EfCore;
using BlogApp.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Drawing;
using System.Security.Claims;

namespace Blog_Web.Controllers
{
    public class PostsController : Controller
    {
        private IPostRepository _postRepository;
        private ITagRepository _tagRepository;
        private ICommentRepository _commentRepository;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private BlogContext _context;

        public PostsController(IPostRepository postRepository, ITagRepository tagRepository, IWebHostEnvironment hostingEnvironment, ICommentRepository commentRepository, BlogContext context)

        {
            _postRepository = postRepository;
            _tagRepository = tagRepository;
            _commentRepository = commentRepository;
            _hostingEnvironment = hostingEnvironment;
            _context = context; 
        }
        public async Task<IActionResult> Index(string tag)
        
        {
            var posts = _postRepository.Posts.Where(x=>x.IsActive);
            if (!string.IsNullOrEmpty(tag))
            {
                posts = posts.Where(x => x.Tags.Any(t => t.Url == tag));
            }
            return View(new PostViewModel
            {
                Posts = await posts.ToListAsync()
            });
        }
        public async Task<IActionResult> Details(string? postUrl)
        {
            
            var currentUserImagePath = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData)?.Value;

            ViewData["CurrentUserImagePath"] = currentUserImagePath;


            if (postUrl == null)
            {
                return NotFound();
            }
            return View(await _postRepository
                .Posts.Include(x => x.Tags)
                .Include(x => x.User)
                .Include(x => x.Comments)
                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(p => p.Url == postUrl));

        }

        //[HttpPost]
        //public JsonResult AddComment(int PostId, string Text)
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var userName = User.FindFirstValue(ClaimTypes.Name);


        //    var entity = new Comment
        //    {
        //        Text = Text,
        //        PublishedOn = DateTime.Now,
        //        PostId = PostId,

        //        UserId = int.Parse(userId?? "")


        //    };
        //    var pht = _commentRepository
        //        .Comments
        //        .Include(x => x.User)
        //        .FirstOrDefault(x => x.UserId == entity.UserId);
        //    var avatar = pht?.User?.ImagePath ?? "";
        //    _commentRepository.CreateComment(entity);

        //    return Json(new
        //    {
        //        userName,
        //        Text,
        //        entity.PublishedOn,
        //        avatar
        //    });
        //}



        [HttpPost]
        public async Task<JsonResult> AddComment(int PostId, string Text)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.FindFirstValue(ClaimTypes.Name);

            var entity = new Comment
            {
                Text = Text,
                PublishedOn = DateTime.Now,
                PostId = PostId,
                UserId = int.Parse(userId ?? "")
            };

            //var pht = await _commentRepository.Comments
            //    .Include(x => x.User)
            //    .FirstOrDefaultAsync(x => x.UserId == entity.UserId);

            //var avatar = pht?.User?.ImagePath ?? "";

            _commentRepository.CreateComment(entity);

            return Json(new
            {
                userName,
                Text,
                entity.PublishedOn
                //avatar
            });
        }



       
        public  JsonResult Delete_Comment( int CommentId)
        {
            try
            {
                
                var commentId = _commentRepository.Comments.FirstOrDefault(c=>c.CommentId == CommentId);    
                if (commentId == null)
                {
                    // Yorum bulunamadıysa hata mesajı ver ve uygun bir işlem yap
                    TempData["ErrorMessage"] = "Comment not found";
                    return Json(new { success = false, message = "Comment not found" });
                }

                _context.Comments.Remove(commentId);
                 _context.SaveChanges();

                // Yorum başarıyla silindiğinde başarılı mesajı ver ve uygun bir işlem yap
                TempData["SuccessMessage"] = "Comment deleted successfully";
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Hata durumunda hata mesajı ver ve uygun bir işlem yap
                TempData["ErrorMessage"] = "An error occurred: " + ex.Message;
                return Json(new { success = false, message = "An error occurred" });
            }
        }



        //public async Task<JsonResult> DeleteComment(int commentId)
        //{
        //    try
        //    {
        //        var comment = await _commentRepository.Comments.FirstOrDefaultAsync(c => c.CommentId == commentId);
        //        if (comment == null)
        //        {
        //            return Json(new { success = false, message = "duzgun gelmir" });
        //        }

        //        _context.Comments.Remove(comment);
        //        await _context.SaveChangesAsync();

        //        return Json(new { success = true });
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        // Veritabanı işlemi sırasında bir hata oluştu
        //        return Json(new { success = false, message = "Database error occurred: " + ex.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        // Diğer tüm istisnalar
        //        return Json(new { success = false, message = "An error occurred: " + ex.Message });
        //    }
        //}



        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            ViewBag.Tags = _tagRepository.Tags.ToList();
            return View();

        }

        [HttpPost]
        [Authorize]

        public async Task<IActionResult> Create(PostCreateViewModel model, int[] tagIds)
        {
            
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


                if (model.BlogImageFile != null && model.BlogImageFile.Length > 0)
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.BlogImageFile.FileName;
                    string imagePath = "/img/" + uniqueFileName;
                    var folderPath = Path.Combine(_hostingEnvironment.WebRootPath, "img");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "img", uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.BlogImageFile.CopyToAsync(stream);
                    }
                    //_postRepository.CreatePost(
                    //    new Post
                    //    {
                    //        Title = model.Title,
                    //        Content = model.Content,
                    //        Description = model.Description,
                    //        Url = model.Url,
                    //        UserId = int.TryParse(userId, out var id) ? id : (int?)null,
                    //        PublishedOn = DateTime.Now,
                    //        Image = uniqueFileName,
                    //        IsActive = false,
                    //    }
                    //);

                    _postRepository.CreatePost(
                        new Post
                        {
                            Title = model.Title,
                            Content = model.Content,
                            Description = model.Description,
                            Url = model.Url,
                            UserId = int.TryParse(userId, out var id) ? id : (int?)null,
                            PublishedOn = DateTime.Now,
                            Image = uniqueFileName,
                            IsActive = false,
                        },
                        tagIds


                        );

                    return RedirectToAction("Index");
                }

            }
            ViewBag.Tags = _tagRepository.Tags.ToList();

            return View(model);
        }


        [Authorize]
        public async Task<IActionResult> List()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "");
            var role = User.FindFirstValue(ClaimTypes.Role);
            var posts = _postRepository.Posts;
            if (string.IsNullOrEmpty(role))
            {
                posts = posts.Where(i=> i.UserId == userId);    
            }

            return View(await posts.ToListAsync());
        }
        [Authorize]
        public IActionResult Edit(int? id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return NotFound();
                }
                var posts = _postRepository.Posts.Include(x => x.Tags).FirstOrDefault(i => i.PostId == id);
                if (posts == null)
                {
                    NotFound();
                }
                else
                {
                    ViewBag.Tags = _tagRepository.Tags.ToList();
                    return View(new PostEditViewModel
                    {
                        PostId = posts.PostId,
                        Title = posts.Title,
                        Content = posts.Content,
                        Description = posts.Description,
                        Url = posts.Url,
                        Tags = posts.Tags,
                        isActive = posts.IsActive,
                        GetImage = posts.Image

                    });

                }
            }

            return NotFound();

          
        }

        public IActionResult Delete(int? id) 
        {
            if (id == null)
            {
                return NotFound();
            }
            var deletedPost = _postRepository.Posts.FirstOrDefault(x=>x.PostId == id);  

            if (deletedPost != null)
            {
                _postRepository.DeletePost(deletedPost);
            }



            
            return RedirectToAction("List");
        }

        [Authorize]
        [HttpPost]

        public async Task<IActionResult> Edit(PostEditViewModel model, int[] tagIds)
        {
            bool valid0 = ModelState["Title"].Errors.Count == 0;
            bool valid1 = ModelState["Description"].Errors.Count == 0;
            bool valid2 = ModelState["Url"].Errors.Count == 0;
            bool valid3 = ModelState["Content"].Errors.Count == 0;

            if (valid1 && valid2 && valid3)
            {
                if (model.BlogImageFile != null && model.BlogImageFile.Length > 0)
                {
                    
                  
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.BlogImageFile.FileName;
                    string imagePath = "/img/" + uniqueFileName;
                    var folderPath = Path.Combine(_hostingEnvironment.WebRootPath, "img");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "img", uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.BlogImageFile.CopyToAsync(stream);
                    }

                    var entityToUpdate = new Post
                    {
                        PostId = model.PostId,
                        Title = model.Title,
                        Description = model.Description,
                        Content = model.Content,
                        Url = model.Url,
                        Tags = model.Tags,
                        Image = uniqueFileName
                    };

                    if (User.FindFirstValue(ClaimTypes.Role) == "admin")
                    {
                        entityToUpdate.IsActive = model.isActive;
                    }

                    _postRepository.EditPost(entityToUpdate, tagIds);
                    return RedirectToAction("List");
                }
                else
                {
                    var entityToUpdateNoPhoto = new Post
                    {
                        PostId = model.PostId,
                        Title = model.Title,
                        Description = model.Description,
                        Content = model.Content,
                        Url = model.Url,
                        Tags = model.Tags
                    };
                    if (User.FindFirstValue(ClaimTypes.Role) == "admin")
                    {
                        entityToUpdateNoPhoto.IsActive = model.isActive;
                    }

                    _postRepository.EditPostNoPhoto(entityToUpdateNoPhoto, tagIds);
                    return RedirectToAction("List");
                }
            }

            ViewBag.Tags = _tagRepository.Tags.ToList();
          
            PostEditViewModel mdl = new PostEditViewModel();
            var posts = _postRepository.Posts.Include(x => x.Tags).FirstOrDefault(i => i.PostId == model.PostId);
            ViewBag.Image = posts.Image;


            return View(mdl);
        }



    }


}
