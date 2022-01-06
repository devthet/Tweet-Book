using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_book.Contracts.v1.Requests.Queries;
using Tweet_Book.Domain;

namespace Tweet_Book.MappingProfiles
{
    public class RequestToDomainProfile:Profile
    {
        public RequestToDomainProfile()
        {
            CreateMap<PaginationQuery,PaginationFilter>();
            CreateMap<GetAllPostsQuery, GetAllPostFilter>();
        }
    }
}
