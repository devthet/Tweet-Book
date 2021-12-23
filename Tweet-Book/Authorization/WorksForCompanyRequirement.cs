using Microsoft.AspNetCore.Authorization;


namespace Tweet_Book.Authorization
{
    public class WorksForCompanyRequirement:IAuthorizationRequirement
    {
        public string DomainName { get;  }
        public WorksForCompanyRequirement(string domainName)
        {
            DomainName = domainName;
        }
    }
}
