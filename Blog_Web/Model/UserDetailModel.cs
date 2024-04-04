using Blog_Web.Entities;
using BlogApp.Entity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Blog_Web.Model
{
    public class UserDetailModel
    {
        public Post Post { get; set; }
        public List<Comment> Comments { get; set; }
        public User User { get; set; }

    }
}
