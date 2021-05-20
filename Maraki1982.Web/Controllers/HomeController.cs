using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System;
using System.Linq;
using Newtonsoft.Json;
using Maraki1982.Web.Models.Changelog;

namespace Maraki1982.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IConfiguration configuration, ILogger<HomeController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public IActionResult Index([FromQuery] string errorMessage = null)
        {
            ChangelogModel changelog = GetChangelogDetails();
            ViewBag.ErrorMessage = errorMessage;
            return View(changelog);
        }

        public IActionResult MaliciousUrls()
        {
            string microsoftUrl = CraftMicrosoftUrl();
            ViewBag.MicrosoftUrl = microsoftUrl;

            string googleUrl = CraftGoogleUrl();
            ViewBag.GoogleUrl = googleUrl;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Terms()
        {
            return View();
        }

        private string CraftMicrosoftUrl()
        {
            var values = new Dictionary<string, string>
            {
                { "client_id", _configuration["Authentication:Microsoft:ClientId"] },
                { "response_type", _configuration["Authentication:Microsoft:MaliciousUrlCraft:ResponseType"] },
                { "redirect_uri", _configuration["Authentication:Microsoft:RedirectUri"] },
                { "scope", _configuration["Authentication:Microsoft:MaliciousUrlCraft:Scope"] },
                { "state", _configuration["Authentication:Microsoft:MaliciousUrlCraft:State"] },
                { "nonce", _configuration["Authentication:Microsoft:MaliciousUrlCraft:Nonce"] },
                { "response_mode", _configuration["Authentication:Microsoft:MaliciousUrlCraft:ResponseMode"] }
            };

            var content = new FormUrlEncodedContent(values);
            var microsoftUrl = string.Format("{0}?{1}", 
                _configuration["Authentication:Microsoft:MaliciousUrlCraft:AuthorizationUrl"], 
                content.ReadAsStringAsync().Result);

            return microsoftUrl;
        }

        private string CraftGoogleUrl()
        {
            var values = new Dictionary<string, string>
            {
                { "scope", _configuration["Authentication:Google:MaliciousUrlCraft:Scope"] },
                { "access_type", _configuration["Authentication:Google:MaliciousUrlCraft:AccessType"] },
                { "include_granted_scopes", _configuration["Authentication:Google:MaliciousUrlCraft:IncludeGrantedScopes"] },
                { "response_type", _configuration["Authentication:Google:MaliciousUrlCraft:ResponseType"] },
                { "state", _configuration["Authentication:Google:MaliciousUrlCraft:State"] },
                { "redirect_uri", _configuration["Authentication:Google:RedirectUri"] },
                { "client_id", _configuration["Authentication:Google:ClientId"] },
                { "prompt", _configuration["Authentication:Google:MaliciousUrlCraft:Prompt"] }
            };

            var content = new FormUrlEncodedContent(values);
            var microsoftUrl = string.Format("{0}?{1}",
                _configuration["Authentication:Google:MaliciousUrlCraft:AuthorizationUrl"],
                content.ReadAsStringAsync().Result);

            return microsoftUrl;
        }

        private ChangelogModel GetChangelogDetails()
        {
            var changelogPath = Path.Combine(Environment.CurrentDirectory, @"Changelog");
            List<string> files = Directory.GetFiles(changelogPath).ToList();
            return JsonConvert.DeserializeObject<ChangelogModel>(System.IO.File.ReadAllText(files.First()));
        }
    }
}
