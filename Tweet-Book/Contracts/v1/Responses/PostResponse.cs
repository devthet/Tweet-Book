using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_Book.Contracts.v1.Responses;
using Tweet_Book.Domain;

namespace Tweetbook.Contracts.v1.Responses
{
    public class PostResponse
    {
        public Guid Id { get; set; }
        // Modified
        public string Name { get; set; }
        public string UserId { get; set; }
        public IEnumerable<TagResponse> Tags { get; set; }
    }
}
