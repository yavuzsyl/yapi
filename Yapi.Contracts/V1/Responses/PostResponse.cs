using System;
using System.Collections.Generic;
using System.Text;

namespace Yapi.Contracts.V1.Responses
{
    public class PostResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string AppUserId { get; set; }
        public List<TagResponse> Tags { get; set; }
    }
}
