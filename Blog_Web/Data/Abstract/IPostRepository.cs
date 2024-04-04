using Blog_Web.Entities;

namespace Blog_Web.Data.Abstract
{
    public interface IPostRepository
    {
        IQueryable<Post> Posts { get; }
        void CreatePost(Post post);
        void CreatePost(Post post, int[] tagIds);
        void EditPost(Post post);
        void EditPostNoPhoto(Post post, int[] tagIds);    

        void EditPost(Post post, int[] tagIds);  
        void DeletePost(Post id); 

    }
}
