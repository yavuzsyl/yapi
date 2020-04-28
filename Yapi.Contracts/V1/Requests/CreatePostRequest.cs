using System;
using System.Collections.Generic;
using System.Text;

namespace Yapi.Contracts.V1.Requests
{
    public class CreatePostRequest
    {
        public string Name { get; set; }
        public IEnumerable<string> Tags { get; set; }

    }
}
