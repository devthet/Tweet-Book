using Swashbuckle.AspNetCore.Filters;
using System;
using Tweet_Book.Contracts.v1.Responses;

namespace Tweet_Book.SwaggerExamples.Responses
{
    public class TagResponseExample : IExamplesProvider<TagResponse>
    {
        public TagResponse GetExamples()
        {
            return new TagResponse
            {
                Name = "new tag response example"
            };
        }
    }
}
