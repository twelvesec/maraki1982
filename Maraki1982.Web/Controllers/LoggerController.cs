using Maraki1982.Web.Helpers;
using Maraki1982.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Maraki1982.Web.Controllers
{
    [Authorize]
    [Route("api/log")]
    public class LoggerController : Controller
    {
        private readonly IConfiguration _configuration;

        public LoggerController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route("files")]
        [HttpGet]
        public IActionResult Index()
        {
            List<string> files = GetFiles();
            return View(files.Take(10).ToList());
        }

        [Route("file/{logFileId}")]
        [HttpGet]
        public IActionResult DisplayLogs(int logFileId, int? pageNumber)
        {
            ViewBag.Counter = pageNumber == null ? 0 : (pageNumber - 1) * 10;
            List<string> files = GetFiles().Take(10).ToList();
            var file = files[logFileId];
            var logs = new List<Log>();

            using (var fileStream = System.IO.File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    string line = string.Empty;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        logs.Add(JsonConvert.DeserializeObject<Log>(line));
                    }
                }
            }
            logs.Reverse();
            return View(PaginatedList<Log>.Create(logs.AsQueryable(), pageNumber ?? 1, Convert.ToInt32(_configuration["General:PagedResultsSize"])));
        }

        [Route("file/{logFileId}/download")]
        [HttpGet]
        public IActionResult Download(int logFileId)
        {
            List<string> files = GetFiles().Take(10).ToList();
            var file = files[logFileId];
            var fileStream = System.IO.File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return (File(fileStream, "application/octet-stream", Path.GetFileName(file)));
        }

        private static List<string> GetFiles()
        {
            string logsPath = Path.Combine(Environment.CurrentDirectory, "logs");
            List<string> files = Directory.GetFiles(logsPath).ToList();
            files.Reverse();
            return files;
        }
    }
}
