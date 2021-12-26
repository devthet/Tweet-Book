﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tweet_Book.Contracts.v1.Requests
{
    public class CreatePostRequest
    {
        public string Name { get; set; }
       // public string UserId { get; set; }
        public IEnumerable<TagRequest> Tags { get; set; }
    }
}
