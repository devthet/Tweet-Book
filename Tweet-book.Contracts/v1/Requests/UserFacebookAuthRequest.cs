using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tweet_book.Contracts.v1.Requests
{
    public class UserFacebookAuthRequest
    {
        public string AccessToken { get; set; }
    }
}
