using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yapi.Contracts.V1.Requests;

namespace YAPI.SwaggerExamples.Requests
{
    //swagger örnek request modelleri installerda service eklenir
    public class CreateTagRequestExample : IExamplesProvider<CreateTagRequest>
    {
        public CreateTagRequest GetExamples()
        {
            return new CreateTagRequest
            {
                 TagName="nameTag"
            };
        }
    }
}
