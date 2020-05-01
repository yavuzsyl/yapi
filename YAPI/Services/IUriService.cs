using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yapi.Contracts.V1.Requests.Queries;

namespace YAPI.Services
{
    public interface IUriService
    {
        Uri GetPostUri(string id);
        Uri GetAllPostsUri(PaginationQuery pagination = null);
    }
}
