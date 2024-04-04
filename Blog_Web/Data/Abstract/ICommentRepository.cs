using Blog_Web.Entities;
using BlogApp.Entity;

namespace Blog_Web.Data.Abstract
{
    public interface ICommentRepository
    {
        IQueryable<Comment> Comments { get; }
        void CreateComment(Comment comment);
    }
}
