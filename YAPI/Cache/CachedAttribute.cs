using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAPI.Services;

namespace YAPI.Cache
{
    [AttributeUsage(validOn:AttributeTargets.Class | AttributeTargets.Method)]
    public class CachedAttribute :Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveSeconds;
        public CachedAttribute(int timeToLiveSeconds)
        {
            _timeToLiveSeconds = timeToLiveSeconds;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //if the request cached return it
            //instead of getting service from ctor we are getting it from request services from service container cuz we cant inject it in attribute ctor otherwise we have to inject it everywhere we use the attribute
            var cacheSettings = context.HttpContext.RequestServices.GetRequiredService<RedisCacheSettings>();
            if (!cacheSettings.Enabled)
            {
                await next();//go my son to the other middlewares you dont have anything to do here
                return;
            }

            var cacheSerivece = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
            var cachedResponse = await cacheSerivece.GetCachedResponseAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedResponse))
            {
                var contentResult = new ContentResult
                {
                     Content = cachedResponse,
                     ContentType = "application/json",
                     StatusCode = 200
                };
                context.Result = contentResult;
                return;
            }

            //execute edilen actiondan dönen result cachelenir
            var executedContext = await next();
            //get value cache the response if not cached before
            if (executedContext.Result is OkObjectResult okObjectResult)
            {
                await cacheSerivece.CacheResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(_timeToLiveSeconds));
            }

        }

        private static string GenerateCacheKeyFromRequest(HttpRequest httpRequest)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append($"{httpRequest.Path}");
            foreach (var (key,value) in httpRequest.Query.OrderBy(x=> x.Key))
            {
                keyBuilder.Append($"{key}-{value}");
            }
            return keyBuilder.ToString();
        }
    }
}
