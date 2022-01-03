﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tweet_book.Contracts.v1.Responses
{
    public class Response<T>
    {
        public Response() { }
        public Response(T response)
        {
            Data = response;
        }
        public T Data { get; set; }
    }
}
