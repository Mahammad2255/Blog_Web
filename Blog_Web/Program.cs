using Blog_Web.Data.Abstract;
using Microsoft.AspNetCore.Identity;

using Blog_Web.Data.Concrete.EfCore;
using Blog_Web.Entities;
using BlogApp.Data.Concrete.EfCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<BlogContext>(options =>
{
    var config = builder.Configuration;
    var connectionString = config.GetConnectionString("sql_connection");
    options.UseSqlServer(connectionString);
});

builder.Services.AddScoped<IPostRepository, EFPostRepository>();    
builder.Services.AddScoped<ITagRepository, EFTagRepository>();
builder.Services.AddScoped<ICommentRepository, EfCommentRepository>();
builder.Services.AddScoped<IUserRepository, EFTUserRepository>();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/Users/Login";
});  

var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();   
app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultControllerRoute();


app.MapControllerRoute(
    name: "post_details",
    pattern: "posts/detail/{postUrl}",
    defaults: new {controller = "Posts", action = "Details"}
    
        );





app.MapControllerRoute(
    name: "post_by_tag",
    pattern: "posts/tag/{tag}",
    defaults: new { controller = "Posts", action = "Index" }

        );

app.MapControllerRoute(
    name: "user_profile",
    pattern: "profile/{username}",
    defaults: new { controller = "Users", action = "Profile" }

        );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Posts}/{action=Index}"

        );


app.MapControllerRoute(
    name: "post_create",
    pattern: "posts/create",
    defaults: new { controller = "Posts", action = "Create" }
);
app.MapControllerRoute(
    name: "delete_comment",
    pattern: "posts/DeleteComment/{commentId}",
    defaults: new { controller = "Posts", action = "DeleteComment"}

);




app.Run();
