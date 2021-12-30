using System;
using System.Collections.Generic;
using Tweet_Book.Contracts.v1.Responses;


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
