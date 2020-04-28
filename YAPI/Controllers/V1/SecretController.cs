using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YAPI.Filters;

namespace YAPI.Controllers.V1
{
    [ApiKeyAuth]
    [ApiController]
    public class SecretController : ControllerBase
    {
        [HttpGet(template:"secret")]
        public IActionResult Secret()
        {
            return Ok("No secrets anymore");
        }
    }
}