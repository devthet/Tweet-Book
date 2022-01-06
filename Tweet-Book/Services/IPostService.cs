using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_Book.Domain;
using Tweetbook.Domain;

namespace Tweet_Book.Services
{
    public interface IPostService
    {
        Task<List<Post>> GetPostsAsync(GetAllPostFilter userId =null,PaginationFilter paginationFilter=null);
        Task<Post> GetPostByIdAsync(Guid postId);
        Task<bool> CreatePostAsync(Post post);
        Task<bool> UpdatePostAsync(Post postToUpdate);
        Task<bool> DeletePostAsync(Guid postId);
        Task<bool> UserOwnPostAsync(Guid postId, string userId);
        Task<List<Tag>> GetAllTagsAsync();
        Task<Tag> GetTagByNameAsync(string tagName);
        Task AddNewTag(Post post);
        Task<bool> CreateTagAsync(Tag tag);
        Task<bool> UpdateTagAsync(Tag tagToUpdate);
        Task<bool> DeleteTagAsync(string tagName);
        //Task GetAllTagsAsync();
    }
}
