using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YAPI.CustomAuthorization
{
    public class WorksForCompanyRequirement : IAuthorizationRequirement
    {
        public string DomainName { get; }
        public WorksForCompanyRequirement(string domainName)
        {
            DomainName = domainName;
        }
    }
}
