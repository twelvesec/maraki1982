using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Maraki1982.Core.DAL;
using Maraki1982.Web.Helpers;
using Maraki1982.Web.Utilities.Interfaces;
using Maraki1982.Core.Models.Database;

namespace Maraki1982.Web.Controllers
{
    [Authorize]
    [Route("api/drives")]
    public class DrivesController : Controller
    {
        private readonly OAuthServerContext _context;
        private readonly IDriveUtility _driveUtility;
        private readonly IConfiguration _configuration;

        public DrivesController(OAuthServerContext context, IDriveUtility driveUtility, IConfiguration configuration)
        {
            _context = context;
            _driveUtility = driveUtility;
            _configuration = configuration;
        }

        [Route("{userId}")]
        [HttpGet]
        public async Task<IActionResult> Index(int userId, int? pageNumber)
        {
            var user = await _context.Users.Include(x => x.EmailFolders)
                .ThenInclude(x => x.Emails)
                .Include(x => x.Drives)
                .ThenInclude(x => x.Folders)
                .ThenInclude(x => x.Files)
                .FirstOrDefaultAsync(m => m.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            ViewBag.UserId = userId;
            return View(PaginatedList<Drive>.Create(user.Drives.AsQueryable(), pageNumber ?? 1, Convert.ToInt32(_configuration["General:PagedResultsSize"])));
        }

        [Route("{userId}/get")]
        [HttpGet]
        public async Task<IActionResult> GetDrives(int userId)
        {
            _driveUtility.GetUserDrives(userId);

            var routeValues = new RouteValueDictionary {
              { "userId", userId }
            };
            return RedirectToAction("Index", routeValues);
        }
    }
}
