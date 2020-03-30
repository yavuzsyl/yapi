using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YAPI.Controllers
{
    public class TestController : Controller
    {
        [HttpGet("api/User")]//direk olarak bu urlden ulaşılabilir
        public IActionResult Get()
        {
            return Ok(new { name = "meh" });
        }
    }
}
