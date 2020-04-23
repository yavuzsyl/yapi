using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YAPI.Domain;

namespace YAPI.Contracts.V1.Responses
{
    public class PostResponse
    {
        public Guid Id { get; set; }
        public string Name { get; internal set; }
        public string AppUserId { get; internal set; }
        public List<TagResponse> Tags { get; internal set; }
    }
}
