using Microsoft.AspNetCore.Mvc;


namespace Tweet_book.Contracts.v1.Requests.Queries
{
    public class GetAllPostsQuery
    {
        [FromQuery(Name="userId")]
        public string UserId { get; set; }
    }
}
