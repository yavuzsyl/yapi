using System;
using System.Collections.Generic;
using System.Text;

namespace Yapi.Contracts.V1.Responses
{
    public class Response<T>
    {
        public Response()
        {

        }
        public Response(T response)
        {
            Data = response;
        }

        private T Data { get; set; }
    }
}
