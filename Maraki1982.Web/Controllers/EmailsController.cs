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
    [Route("api/emails")]
    public class EmailsController : Controller
    {
        private readonly OAuthServerContext _context;
        private readonly IEmailUtility _emailUtility;
        private readonly IConfiguration _configuration;

        public EmailsController(OAuthServerContext context, IEmailUtility emailUtility, IConfiguration configuration)
        {
            _context = context;
            _emailUtility = emailUtility;
            _configuration = configuration;
        }

        [Route("{userId}/folder/{folderId}")]
        [HttpGet]
        public async Task<IActionResult> Index(int userId, int folderId, int? pageNumber)
        {
            var user = await _context.Users.Include(x => x.EmailFolders)
                .ThenInclude(x => x.Emails)
                .Include(x => x.Drives)
                .ThenInclude(x => x.Folders)
                .ThenInclude(x => x.Files)
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            var emailFolder = user.EmailFolders.FirstOrDefault(x => x.Id == folderId);
            if (emailFolder == null)
            {
                return NotFound();
            }

            ViewBag.UserId = userId;
            ViewBag.FolderId = folderId;
            ViewBag.FolderName = emailFolder.Name;
            return View(PaginatedList<Email>.Create(emailFolder.Emails.AsQueryable(), pageNumber ?? 1, Convert.ToInt32(_configuration["General:PagedResultsSize"])));
        }

        [Route("{userId}/folder/{folderId}/get")]
        [HttpGet]
        public async Task<IActionResult> GetEmails(int userId, int folderId)
        {
            _emailUtility.GetFolderEmails(userId, folderId);

            var routeValues = new RouteValueDictionary {
              { "userId", userId },
              { "folderId", folderId }
            };
            return RedirectToAction("Index", routeValues);
        }

        [Route("{userId}/folder/{folderId}/file/{fileId}")]
        public async Task<IActionResult> DisplayEmail(int userId, int folderId, int fileId)
        {
            var user = await _context.Users.Include(x => x.EmailFolders)
                .ThenInclude(x => x.Emails)
                .Include(x => x.Drives)
                .ThenInclude(x => x.Folders)
                .ThenInclude(x => x.Files)
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            var emailFolder = user.EmailFolders.FirstOrDefault(x => x.Id == folderId);
            if (emailFolder == null)
            {
                return NotFound();
            }

            var email = emailFolder.Emails.FirstOrDefault(x => x.Id == fileId);
            if (email == null)
            {
                return NotFound();
            }

            return View(email);
        }

        [Route("{userId}/folder/{folderId}/file/{fileId}/raw")]
        public async Task<IActionResult> DisplayRawEmail(int userId, int folderId, int fileId)
        {
            var user = await _context.Users.Include(x => x.EmailFolders)
                .ThenInclude(x => x.Emails)
                .Include(x => x.Drives)
                .ThenInclude(x => x.Folders)
                .ThenInclude(x => x.Files)
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            var emailFolder = user.EmailFolders.FirstOrDefault(x => x.Id == folderId);
            if (emailFolder == null)
            {
                return NotFound();
            }

            var email = emailFolder.Emails.FirstOrDefault(x => x.Id == fileId);
            if (email == null)
            {
                return NotFound();
            }

            return View(email);
        }
    }
}
