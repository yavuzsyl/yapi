using System;
using System.Collections.Generic;
using System.Text;

namespace Yapi.Contracts.V1.Responses
{
    public class PostResponse
    {
        public Guid Id { get; set; }
        public string Name { get; internal set; }
        public string AppUserId { get; internal set; }
        public List<TagResponse> Tags { get; internal set; }
    }
}
