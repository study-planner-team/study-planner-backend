using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyPlannerAPI.Models;
using System.Security.Claims;

namespace StudyPlannerAPI.Controllers
{
    [Route("api/profile")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        [HttpGet("test")]
        [Authorize]
        public IActionResult GetProfile()
        {
            return Ok("This is a protected route!");
        }
    }
}
