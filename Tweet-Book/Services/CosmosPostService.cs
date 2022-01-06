using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_Book.Domain;
using Tweetbook.Domain;

namespace Tweet_Book.Services
{
    public class CosmosPostService : IPostService
    {
        public Task AddNewTag(Post post)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreatePostAsync(Post post)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreateTagAsync(Tag tag)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeletePostAsync(Guid postId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteTagAsync(string tagName)
        {
            throw new NotImplementedException();
        }

        public Task GetAllTagsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Post> GetPostByIdAsync(Guid postId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Post>> GetPostsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<Post>> GetPostsAsync(PaginationFilter paginationFilter = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<Post>> GetPostsAsync(string userId = null, PaginationFilter paginationFilter = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<Post>> GetPostsAsync(GetAllPostFilter userId = null, PaginationFilter paginationFilter = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<Post>> GetTagAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Tag> GetTagByIdAsync(string tagName)
        {
            throw new NotImplementedException();
        }

        public Task<Tag> GetTagByNameAsync(string tagName)
        {
            throw new NotImplementedException();
        }

        public Task<List<Tag>> GetTagsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdatePostAsync(Post postToUpdate)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateTagAsync(Tag tagToUpdate)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UserOwnPostAsync(Guid postId, string userId)
        {
            throw new NotImplementedException();
        }

        Task<List<Tag>> IPostService.GetAllTagsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
