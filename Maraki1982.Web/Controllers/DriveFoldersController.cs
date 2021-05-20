using System;
using System.Collections.Generic;
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
    [Route("api/folders")]
    public class DriveFoldersController : Controller
    {
        private readonly OAuthServerContext _context;
        private readonly IDriveUtility _driveUtility;
        private readonly IConfiguration _configuration;

        public DriveFoldersController(OAuthServerContext context, IDriveUtility driveUtility, IConfiguration configuration)
        {
            _context = context;
            _driveUtility = driveUtility;
            _configuration = configuration;
        }

        [Route("{userId}/drive/{driveId}")]
        [HttpGet]
        public async Task<IActionResult> Index(int userId, int driveId, int? pageNumber)
        {
            User user = await _context.Users.Include(x => x.EmailFolders)
                .ThenInclude(x => x.Emails)
                .Include(x => x.Drives)
                .ThenInclude(x => x.Folders)
                .ThenInclude(x => x.Files)
                .FirstOrDefaultAsync(m => m.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            var drive = user.Drives.FirstOrDefault(x => x.Id == driveId);
            if (drive == null)
            {
                return NotFound();
            }

            ViewBag.UserId = userId;
            ViewBag.DriveId = driveId;
            return View(PaginatedList<Folder>.Create(drive.Folders.AsQueryable(), pageNumber ?? 1, Convert.ToInt32(_configuration["General:PagedResultsSize"])));
        }

        [Route("{userId}/drive/{driveId}/get")]
        [HttpGet]
        public async Task<IActionResult> GetDriveFolders(int userId, int driveId)
        {
            _driveUtility.GetUserDriveRootFolders(userId, driveId);

            //TODO: Optimize recursive code
            User user = await _context.Users.Include(x => x.EmailFolders)
                .ThenInclude(x => x.Emails)
                .Include(x => x.Drives)
                .ThenInclude(x => x.Folders)
                .ThenInclude(x => x.Files)
                .FirstOrDefaultAsync(m => m.Id == userId);

            var drive = user.Drives.FirstOrDefault(x => x.Id == driveId);
            if (drive == null)
            {
                return NotFound("Drive not found.");
            }

            var doneFolders = new List<Folder>();
            var toDoFolders = new List<Folder>();
            drive.Folders.ToList().ForEach(x =>
            {
                toDoFolders.Add(x);
            });

            do
            {
                toDoFolders.ForEach(x => 
                {
                    if (!doneFolders.Contains(x))
                    {
                        _driveUtility.GetUserDriveSubFolders(userId, driveId, x.Id);
                        doneFolders.Add(x);
                    }
                });

                toDoFolders = new List<Folder>();
                drive.Folders.ToList().ForEach(x =>
                {
                    toDoFolders.Add(x);
                });
            } while (toDoFolders.Count > doneFolders.Count);

            var routeValues = new RouteValueDictionary {
              { "userId", userId },
              { "driveId", driveId }
            };
            return RedirectToAction("Index", routeValues);
        }

        [Route("{userId}/drive/{driveId}/get/files")]
        [HttpGet]
        public async Task<IActionResult> GetFoldersFiles(int userId, int driveId)
        {
            var user = await _context.Users.Include(x => x.EmailFolders)
                .ThenInclude(x => x.Emails)
                .Include(x => x.Drives)
                .ThenInclude(x => x.Folders)
                .ThenInclude(x => x.Files)
                .FirstOrDefaultAsync(m => m.Id == userId);

            var drive = user.Drives.FirstOrDefault(x => x.Id == driveId);
            if (drive == null)
            {
                return NotFound();
            }

            var folders = drive.Folders.ToList();
            folders.ForEach(x =>
            {
                _driveUtility.GetUserDriveFolderFiles(user.Id, driveId, x.Id);
            });

            var routeValues = new RouteValueDictionary {
              { "userId", userId },
              { "driveId", driveId }
            };
            return RedirectToAction("Index", routeValues);
        }
    }
}
