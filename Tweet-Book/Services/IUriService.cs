using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_book.Contracts.v1.Requests.Queries;

namespace Tweet_Book.Services
{
    public interface IUriService
    {
        Uri GetPostUri(string postId);
        Uri GetAllPostsUri(PaginationQuery pagination=null);
    }
}
