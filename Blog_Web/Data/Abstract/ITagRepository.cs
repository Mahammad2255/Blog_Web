using Blog_Web.Entities;
using BlogApp.Entity;

namespace Blog_Web.Data.Abstract
{
    public interface ITagRepository
    {
        IQueryable<Tag> Tags { get; }

        void CreateTag(Tag tag);
    }
}
