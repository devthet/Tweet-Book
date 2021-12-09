using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tweetbook.Contracts.v1
{
    public static class ApiRoutes
    {
        public const string Root = "api";
        public const string Version = "v1";
        public const string baseUrl = Root+"/"+Version+"/";
        public static class Posts
        {
            public const string GetAll = baseUrl+"posts";
            public const string Get = baseUrl + "posts/{postId}";
            public const string Create = baseUrl + "posts";
            public const string Update = baseUrl + "posts/{postId}";
            public const string Delete = baseUrl + "posts/{postId}";
            
        }
    }
}
