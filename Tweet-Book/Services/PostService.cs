using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_Book.Data;
using Tweetbook.Domain;

namespace Tweet_Book.Services
{
    public class PostService : IPostService
    {
        //private readonly List<Post> _posts;
        //public PostService()
        //{
        //    _posts = new List<Post>();
        //    for (int i = 0; i < 5; i++)
        //    {
        //        _posts.Add(new Post
        //        {
        //            Id = Guid.NewGuid(),
        //            Name = $"Post {i}"
        //        });
        //    }
        //}
        private readonly ApplicationDbContext _dataContext;

        public PostService(ApplicationDbContext dataContext)
        {
            _dataContext = dataContext;
        }

        

       

        public async Task<List<Post>> GetPostsAsync()
        {
            // return _posts;
            return await _dataContext.Posts.ToListAsync();
        }
        public async Task<Post> GetPostByIdAsync(Guid postId)
        {
            // return  _posts.SingleOrDefault(x => x.Id == postId);
            return await _dataContext.Posts.SingleOrDefaultAsync(x => x.Id == postId);
        }

        public async Task<bool> CreatePostAsync(Post post)
        {
           await _dataContext.Posts.AddAsync(post);
            var created = await _dataContext.SaveChangesAsync();
            return created > 0;

        }

        public async Task<bool> UpdatePostAsync(Post postToUpdate)
        {
            //var exists = GetPostById(postToUpdate.Id) != null;
            //if (!exists) return false;
            //var index = _posts.findindex(x => x.id == posttoupdate.id);
            //_posts[index] = postToUpdate;
            _dataContext.Posts.Update(postToUpdate);
            var updated = await _dataContext.SaveChangesAsync();
            // return true;
            return updated > 0;
        }
        public async Task<bool> DeletePostAsync(Guid postId)
        {
            //var post = await GetPostById(postId);
            //if (post == null) return false;
            //_posts.Remove(post);
            //return true;
            var post = await GetPostByIdAsync(postId);
            if (post == null) return false;
            _dataContext.Posts.Remove(post);
            var deleted = await _dataContext.SaveChangesAsync();
            return deleted > 0;
        }

        public async Task<bool> UserOwnPostAsync(Guid postId, string userId)
        {
            var post = await _dataContext.Posts.AsNoTracking().SingleOrDefaultAsync(x => x.Id == postId);
            if (post == null) return false;
            if (post.UserId != userId) return false;
            return true;
        }
    }
}
