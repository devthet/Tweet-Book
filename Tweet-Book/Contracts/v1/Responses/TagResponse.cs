﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tweet_Book.Contracts.v1.Responses
{
    public class TagResponse
    {
        public Guid TagId { get; set; }
        public string TagName { get; set; }
    }
}