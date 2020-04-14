using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace YAPI.Domain
{
    public class PostTag
    {
        [ForeignKey(nameof(TagName))]
        public virtual Tag Tag { get; set; }

        public string TagName { get; set; }

        public virtual Post Post { get; set; }

        public Guid PostId { get; set; }
    }
}
