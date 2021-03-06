using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tweet_book.Contracts.v1.Requests.Queries
{
    public class PaginationQuery
    {
        public PaginationQuery() {
            PageNumber = 1;
            PageSize = 100;
        }
        public PaginationQuery(int pageNumber,int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
