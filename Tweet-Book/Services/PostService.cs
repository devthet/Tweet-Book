using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_book.Contracts.v1.Requests.Queries;
using Tweet_Book.Data;
using Tweet_Book.Domain;
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
        public async Task<List<Post>> GetPostsAsync(GetAllPostFilter filter =null,PaginationFilter paginationFilter=null)
        {
            var quarable = _dataContext.Posts.AsQueryable();

            // return _posts;
            // return await _dataContext.Posts.ToListAsync();
            if (paginationFilter == null)
            {
                return await quarable
                 .Include(x => x.Tags).ToListAsync();
            }
            quarable = AddFilterOnQuery(filter, quarable);
            var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
            return await quarable
                 .Include(x => x.Tags)
                 .Skip(skip).Take(paginationFilter.PageSize)
                 .ToListAsync();

        }

        private static IQueryable<Post> AddFilterOnQuery(GetAllPostFilter filter, IQueryable<Post> quarable)
        {
            if (!string.IsNullOrEmpty(filter?.UserId))
            {
                quarable = quarable.Where(x => x.UserId == filter.UserId);
            }

            return quarable;
        }

        public async Task<Post> GetPostByIdAsync(Guid postId)
        {
            // return  _posts.SingleOrDefault(x => x.Id == postId);
            //return await _dataContext.Posts.SingleOrDefaultAsync(x => x.Id == postId);
            return await _dataContext.Posts
                .Include(x => x.Tags)
                .SingleOrDefaultAsync(x => x.Id == postId);
        }

        public async Task<bool> CreatePostAsync(Post post)
        {
            //await _dataContext.Posts.AddAsync(post);
            // var created = await _dataContext.SaveChangesAsync();
            // return created > 0;
            post.Tags?.ForEach(x => x.TagName = x.TagName.ToLower());
            await AddNewTag(post);
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
            postToUpdate.Tags?.ForEach(x => x.TagName = x.TagName.ToLower());
           // await AddNewTag(postToUpdate);
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

       

        public async Task<List<Tag>> GetAllTagsAsync()
        {
           // throw new NotImplementedException();
          //return await _dataContext.Tags.AsNoTracking().ToListAsync();
          return await _dataContext.Tags.ToListAsync();
        }

        public async Task<Tag> GetTagByNameAsync(string name)
        {
            //throw new NotImplementedException();
            return await _dataContext.Tags.SingleOrDefaultAsync(x => x.TagName == name);
        }

        public async Task AddNewTag(Post post)
        {
            foreach(var tags in post.Tags)
            {
                var tag = new Tag
                {
                    TagName = tags.TagName,
                    CreatorId = Guid.NewGuid(),
                    CreatedOn = DateTime.Now,
                    CreatedBy = post.Name

                };
                await _dataContext.Tags.AddAsync(tag);
                await _dataContext.SaveChangesAsync();
            }
            
           
        }

        public async Task<bool> CreateTagAsync(Tag tag)
        {
            // throw new NotImplementedException();
            await _dataContext.Tags.AddAsync(tag);
            var created = await _dataContext.SaveChangesAsync();
            return created > 0;

        }

        public  async Task<bool> UpdateTagAsync(Tag tagToUpdate)
        {
            // throw new NotImplementedException();
            _dataContext.Tags.Update(tagToUpdate);
            var updated = await _dataContext.SaveChangesAsync();
            // return true;
            return updated > 0;
        }

        public async Task<bool> DeleteTagAsync(string tagName)
        {
            // throw new NotImplementedException();
            var tag = await GetTagByNameAsync(tagName);
            if (tag == null) return false;
            _dataContext.Tags.Remove(tag);
            var deleted = await _dataContext.SaveChangesAsync();
            return deleted > 0;
        }

        public Task<List<Tag>> GetTagsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
