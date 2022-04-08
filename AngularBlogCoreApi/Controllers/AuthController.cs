using AngularBlogCoreApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularBlogCoreApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost]
        public IActionResult IsAuthenticated(AdminUser adminUser)
        {
            bool status = false;
            if (adminUser.Email == "cyptometa@gmail.com" && adminUser.Password == "1235")
            {
                status = true;
            }
            var result = new {
                status = status
            };
            return Ok(result);
        }
    }
}
