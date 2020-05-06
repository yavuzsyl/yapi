using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yapi.Contracts.V1;
using Yapi.Contracts.V1.Requests.Queries;

namespace YAPI.Services
{
    public class UriService : IUriService
    {
        private readonly string _baseUri;
        public UriService(string baseUri)//mvc installerdan requestten gelen bilgilerden baseuri gelecek 
        {
            _baseUri = baseUri;
        }
        public Uri GetAllPostsUri(PaginationQuery pagination = null)
        {
            if (pagination == null)
                return new Uri(_baseUri);

            var modifiedUri = QueryHelpers.AddQueryString(_baseUri, "pageNumber", pagination.PageNumber.ToString());
            modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "pageSize", pagination.PageSize.ToString());

            return new Uri(modifiedUri);

        }

        public Uri GetPostUri(string id)
        {
            return new Uri(_baseUri + ApiRoutes.Posts.Get.Replace("{postId}", id));
        }
    }
}
