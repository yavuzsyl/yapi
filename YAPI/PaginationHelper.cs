using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yapi.Contracts.V1.Requests.Queries;
using Yapi.Contracts.V1.Responses;
using YAPI.Domain;
using YAPI.Services;

namespace YAPI
{
    public class PaginationHelper
    {
        public static PagedResponse<T> CreatePaginatedResponse<T>(IUriService uriService, PaginationFilter pagination, List<T> response)
        {
            var nextPage = pagination.PageNumber >= 1
                ? uriService.GetAllPostsUri(new PaginationQuery(pagination.PageSize, pagination.PageNumber + 1)).ToString()
                : null;

            var previousPage = pagination.PageNumber - 1 > 1
                ? uriService.GetAllPostsUri(new PaginationQuery(pagination.PageSize, pagination.PageNumber - 1)).ToString()
                : null;

            return new PagedResponse<T>
            {
                Data = response,
                PageNumber = pagination.PageNumber >= 1 ? pagination.PageNumber : (int?)null,
                PageSize = pagination.PageSize >= 1 ? pagination.PageSize : (int?)null,
                NextPage = response.Any() ? nextPage : null,
                PreviousPage = previousPage
            };
        }
    }
}
