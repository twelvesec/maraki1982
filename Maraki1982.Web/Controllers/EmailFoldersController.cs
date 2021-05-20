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
    [Route("api/emailfolders")]
    public class EmailFoldersController : Controller
    {
        private readonly OAuthServerContext _context;
        private readonly IEmailUtility _emailUtility;
        private readonly IConfiguration _configuration;

        public EmailFoldersController(OAuthServerContext context, IEmailUtility emailUtility, IConfiguration configuration)
        {
            _context = context;
            _emailUtility = emailUtility;
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
            return View(PaginatedList<EmailFolder>.Create(user.EmailFolders.AsQueryable(), pageNumber ?? 1, Convert.ToInt32(_configuration["General:PagedResultsSize"])));
        }

        [Route("{userId}/get")]
        [HttpGet]
        public async Task<IActionResult> GetEmailFolders(int userId)
        {
            _emailUtility.GetUserEmailFolders(userId);

            var routeValues = new RouteValueDictionary {
              { "userId", userId }
            };
            return RedirectToAction("Index", routeValues);
        }

        [Route("{userId}/get/emails")]
        [HttpGet]
        public async Task<IActionResult> GetFoldersEmails(int userId)
        {
            var user = await _context.Users.Include(x => x.EmailFolders)
                .ThenInclude(x => x.Emails)
                .Include(x => x.Drives)
                .ThenInclude(x => x.Folders)
                .ThenInclude(x => x.Files)
                .FirstOrDefaultAsync(m => m.Id == userId);

            var emailFolders = user.EmailFolders.ToList();
            emailFolders.ForEach(x =>
            {
                _emailUtility.GetFolderEmails(user.Id, x.Id);
            });

            var routeValues = new RouteValueDictionary {
              { "userId", userId }
            };
            return RedirectToAction("Index", routeValues);
        }
    }
}
