using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_Book.Contracts.v1.Responses;
using Tweet_Book.Domain;
using Tweetbook.Contracts.v1.Responses;
using Tweetbook.Domain;

namespace Tweet_Book.MappingProfiles
{
    public class DomainToResponseProfile:Profile
    {
        public DomainToResponseProfile()
        {
            CreateMap<Post, PostResponse>()
                .ForMember(dest=>dest.Tags,opt=>
                opt.MapFrom(src=>src.Tags.Select(x=> new TagResponse { Name=x.TagName })))
                ;
            CreateMap<Tag, TagResponse>();
        }
    }
}
