using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace aspnetcore_2_webapi.Controllers
{
    [ApiController]
    public class StatusController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        [Route("api/Status")]
        public object Status()
        {
            return new { Message = "Running", 
                Time = DateTime.Now,
                Version="",
                DeployedOn=DateTime.Now,
                DeployedBy="Automated" 
            };
        }

    }
}
