using System;
using System.Linq;
using System.Threading.Tasks;
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
    [Route("api/folderfiles")]
    public class DriveFolderFilesController : Controller
    {
        private readonly OAuthServerContext _context;
        private readonly IDriveUtility _driveUtility;
        private readonly IConfiguration _configuration;

        public DriveFolderFilesController(OAuthServerContext context, IDriveUtility driveUtility, IConfiguration configuration)
        {
            _context = context;
            _driveUtility = driveUtility;
            _configuration = configuration;
        }

        [Route("{userId}/drive/{driveId}/folder/{folderId}")]
        [HttpGet]
        public async Task<IActionResult> Index(int userId, int driveId, int folderId, int? pageNumber)
        {
            User user = await _context.Users.Include(x => x.EmailFolders)
                .ThenInclude(x => x.Emails)
                .Include(x => x.Drives)
                .ThenInclude(x => x.Folders)
                .ThenInclude(x => x.Files)
                .FirstOrDefaultAsync(m => m.Id == userId);

            var drive = user.Drives.FirstOrDefault(x => x.Id == driveId);
            if (drive == null)
            {
                return NotFound("Drive not found");
            }

            var folder = drive.Folders.FirstOrDefault(x => x.Id == folderId);
            if (folder == null)
            {
                return NotFound("Folder not found");
            }

            ViewBag.UserId = userId;
            ViewBag.DriveId = driveId;
            ViewBag.FolderId = folderId;
            ViewBag.FolderName = folder.Name;
            ViewBag.FolderPath = string.Format("{0}{1}", folder.FolderPath, folder.Name);
            return View(PaginatedList<File>.Create(folder.Files.AsQueryable(), pageNumber ?? 1, Convert.ToInt32(_configuration["General:PagedResultsSize"])));
        }

        [Route("{userId}/drive/{driveId}/folder/{folderId}/get")]
        [HttpGet]
        public async Task<IActionResult> GetDriveFolderFiles(int userId, int driveId, int folderId)
        {
            _driveUtility.GetUserDriveFolderFiles(userId, driveId, folderId);

            var routeValues = new RouteValueDictionary {
              { "userId", userId },
              { "driveId", driveId },
              { "folderId", folderId }
            };
            return RedirectToAction("Index", routeValues);
        }
    }
}
