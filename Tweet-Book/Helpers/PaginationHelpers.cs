using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_book.Contracts.v1.Requests.Queries;
using Tweet_book.Contracts.v1.Responses;
using Tweet_Book.Domain;
using Tweet_Book.Services;
using Tweetbook.Contracts.v1.Responses;

namespace Tweet_Book.Helpers
{
    public class PaginationHelpers
    {
        internal static PagedResponse<T> CreatePaginatedResponse<T>(IUriService uriService, PaginationFilter pagination, List<T> response)
        {
            var nextPage = pagination.PageNumber >= 1 ? uriService
                .GetAllPostsUri(new PaginationQuery(pagination.PageNumber + 1, pagination.PageSize)).ToString()
                : null;
            var previousPage = pagination.PageNumber - 1 >= 1 ? uriService
               .GetAllPostsUri(new PaginationQuery(pagination.PageNumber - 1, pagination.PageSize)).ToString()
               : null;
           
            return new PagedResponse<T>(response)
            {
                Data = response,
                PageNumber = pagination.PageNumber >= 1 ? pagination.PageNumber : (int?)null,
                PageSize = pagination.PageSize >= 1 ? pagination.PageSize : (int?)null,
                NextPage = response.Any() ? nextPage : null,
                PreviousPage = previousPage
            };
        }
    }
}
