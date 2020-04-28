using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yapi.Contracts.V1.Responses;
using YAPI.Domain;

namespace YAPI.MappingProfiles
{
    public class DomainToResponseProfile :Profile
    {
        public DomainToResponseProfile()
        {
            CreateMap<Post, PostResponse>()
                .ForMember(p=> p.Tags,options => 
                    options.MapFrom(src=> src.Tags.Select(t=> new TagResponse {Name = src.Name })));

            CreateMap<Tag, TagResponse>();
        }
    }
}
