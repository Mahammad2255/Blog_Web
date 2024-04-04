using Blog_Web.Data.Abstract;
using Blog_Web.Entities;
using Blog_Web.Model;
using BlogApp.Entity;
using Humanizer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Blog_Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;
        private Stream memoryStream;
        private string uniqueFileName;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public UsersController(IUserRepository userRepository, IWebHostEnvironment hostingEnvironment)
        {
            _userRepository = userRepository;
                _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Posts");
            }
            return View();
        }
        public IActionResult Register()
        {
            
            return View();
        }
        [HttpPost]
        [HttpPost]
public async Task<IActionResult> Register(RegisterViewModel model)
{
    if (ModelState.IsValid)
    {
        var user = await _userRepository.Users.FirstOrDefaultAsync(x => x.UserName == model.UserName || x.Email == model.Email);
        if(user == null) 
        {
           
            
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                string imagePath = "/img/" + uniqueFileName;

                var folderPath = Path.Combine(_hostingEnvironment.WebRootPath, "img");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "img", uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }
                        var newUser = new User
                        {
                            UserName = model.UserName,
                            Email = model.Email,
                            Password = model.Password,
                            ImagePath = uniqueFileName
                        };

                        _userRepository.CreateUser(newUser);
                   
            }
                    else
                    {
                        var newUser = new User
                        {
                            UserName = model.UserName,
                            Email = model.Email,
                            Password = model.Password
                        };

                        _userRepository.CreateUser(newUser);
                    }

            return RedirectToAction("Login");
        }
        else
        {
            ModelState.AddModelError("", "Username or Email is already used.");
        }
    }
    
    return View(model);
}

    
        public async Task<IActionResult> logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var isUser = _userRepository.Users.FirstOrDefault(x => x.Email == model.Email && x.Password == model.Password);
                if (isUser != null)
                {
                    var userClaims = new List<Claim>();
                    userClaims.Add(new Claim(ClaimTypes.NameIdentifier, isUser.UserId.ToString()));
                    userClaims.Add(new Claim(ClaimTypes.Name, isUser.UserName ?? ""));
                    userClaims.Add(new Claim(ClaimTypes.GivenName, isUser.UserName ?? ""));
                    userClaims.Add(new Claim(ClaimTypes.UserData, isUser.ImagePath ?? ""));
                   
                    if (isUser.Email == "admin@gmail.com")
                    {
                        userClaims.Add(new Claim(ClaimTypes.Role, "admin"));

                    }

                    var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var autProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                    };

                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        autProperties);
                    return RedirectToAction("Index", "Posts");


                }
                else
                {
                    ModelState.AddModelError("", "Email or password is incorrect");
                }
            }

            return View();
        }

        public IActionResult Profile(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return NotFound();
            }

            var user = _userRepository.Users.Include(x => x.Posts).Include(x => x.Comments).ThenInclude(x => x.Post).FirstOrDefault(x=>x.UserName == username);

            if (user == null) 
            {
                return NotFound();
            }
            return View(user);
        }
    }
}
