using System;
using System.Collections.Generic;
using System.Text;

namespace Yapi.Contracts.V1.Requests.Queries
{
    public class PaginationQuery
    {
        public PaginationQuery()
        {
            PageNumber = 1;
            PageSize = 100;
        }
        public PaginationQuery(int pageSize, int pageNumber)
        {
            PageSize = pageSize;
            PageNumber = pageNumber;
        }

        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }
    }
}
