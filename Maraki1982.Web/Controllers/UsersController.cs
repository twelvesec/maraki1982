using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Maraki1982.Core.DAL;
using Maraki1982.Core.Models.Enum;
using Maraki1982.Core.VendorApi;
using Maraki1982.Web.Helpers;
using Maraki1982.Core.Models.Database;

namespace Maraki1982.Web.Controllers
{
    [Authorize]
    [Route("api/users")]
    public class UsersController : Controller
    {
        private readonly OAuthServerContext _context;
        private readonly IConfiguration _configuration;
        private Func<VendorEnum, IExternalApi> _externalApiDelegate;

        public UsersController(OAuthServerContext context,
                               IConfiguration configuration,
                               Func<VendorEnum, IExternalApi> externalApiDelegate)
        {
            _context = context;
            _configuration = configuration;
            _externalApiDelegate = externalApiDelegate;
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> Index(VendorEnum? vendor, int? pageNumber)
        {
            var users = _context.Users.Include(x => x.EmailFolders)
                .ThenInclude(x => x.Emails)
                .Include(x => x.Drives)
                .ThenInclude(x => x.Folders)
                .ThenInclude(x => x.Files)
#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                .Where(x => x.Vendor != null);
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
            ViewData["TotalUsers"] = users.Count();

            if (vendor != null)
            {
                users = users.Where(x => x.Vendor == vendor);
            }

            ViewData["VendorSortParm"] = vendor;

            return View(await PaginatedList<User>.CreateAsync(users, pageNumber ?? 1, Convert.ToInt32(_configuration["General:PagedResultsSize"])));
        }

        [Route("{id}/tokens")]
        [HttpGet]
        public async Task<IActionResult> Tokens(int id)
        {
            var user = await _context.Users.Include(x => x.EmailFolders)
                .ThenInclude(x => x.Emails)
                .Include(x => x.Drives)
                .ThenInclude(x => x.Folders)
                .ThenInclude(x => x.Files)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [Route("tokens/refresh")]
        [HttpGet]
        public async Task<IActionResult> RefreshTokens()
        {
            var users = await _context.Users.Include(x => x.EmailFolders)
                .ThenInclude(x => x.Emails)
                .Include(x => x.Drives)
                .ThenInclude(x => x.Folders)
                .ThenInclude(x => x.Files)
                .ToListAsync();

            users.ForEach(x =>
            {
                IExternalApi externalApi = _externalApiDelegate(x.Vendor);
                externalApi.RefreshToken(x);
            });
            return RedirectToAction("Index");
        }

        [Route("{id}/token/refresh")]
        [HttpGet]
        public async Task<IActionResult> RefreshToken(int id)
        {
            var user = await _context.Users.Include(x => x.EmailFolders)
                .ThenInclude(x => x.Emails)
                .Include(x => x.Drives)
                .ThenInclude(x => x.Folders)
                .ThenInclude(x => x.Files)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }
            IExternalApi externalApi = _externalApiDelegate(user.Vendor);
            externalApi.RefreshToken(user);

            var routeValues = new RouteValueDictionary {
              { "id", user.Id }
            };
            return RedirectToAction("Tokens", routeValues);
        }
    }
}
