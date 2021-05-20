using Maraki1982.Web.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Maraki1982.Web.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IConfiguration _configuration;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
                if (httpContext.Response.StatusCode >= 400)
                {
                    string path = httpContext.Request.Path;
                    string method = httpContext.Request.Method;
                    string body = string.Empty;
                    using (StreamReader reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
                    {
                        body = reader.ReadToEndAsync().Result;
                    }
                    throw new StatusCodeException(string.Format("Path:{0} | Method: {1} | Status Code: {2}, Body: {3}", path, method, httpContext.Response.StatusCode, body));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                if (ex is StatusCodeException)
                {
                    //Exception with handling
                    httpContext.Response.Redirect(_configuration["General:GlobalExceptionRedirectUrl"]);
                }
                else
                {
                    //Exception without handling
                    httpContext.Response.Redirect(string.Format("/Home/Index/?errorMessage={0}", ex.Message));
                }
            }
        }
    }
}
