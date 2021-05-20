using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Maraki1982.Core.DAL;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using Maraki1982.Core.Dto.Core;
using Maraki1982.Core.Dto.Google;
using Maraki1982.Core.Dto.Core.Mail;
using Maraki1982.Core.Models.Database;
using Maraki1982.Core.Dto.Core.Drive;

namespace Maraki1982.Core.VendorApi
{
    public class GoogleApi : IExternalApi
    {
        private readonly IConfiguration _configuration;
        private readonly OAuthServerContext _context;

        public GoogleApi(OAuthServerContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public Token GetToken(string idToken)
        {
            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                    { "client_id", _configuration["Authentication:Google:ClientId"] },
                    { "client_secret", _configuration["Authentication:Google:ClientSecret"] },
                    { "code", idToken },
                    { "grant_type", "authorization_code" },
                    { "redirect_uri", _configuration["Authentication:Google:RedirectUri"] }
                };

                var content = new FormUrlEncodedContent(values);
                var response = client.PostAsync(_configuration["Authentication:Google:TokenUri"], content).Result;
                string gToken = response.Content.ReadAsStringAsync().Result;

                return JsonConvert.DeserializeObject<GoogleToken>(gToken);
            }
        }

        public Token RefreshToken(User user)
        {
            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                    { "client_id", _configuration["Authentication:Google:ClientId"] },
                    { "client_secret", _configuration["Authentication:Google:ClientSecret"] },
                    { "grant_type", "refresh_token" },
                    { "refresh_token", user.RefreshToken }
                };

                var content = new FormUrlEncodedContent(values);
                var response = client.PostAsync(_configuration["Authentication:Google:TokenUri"], content).Result;
                string gToken = response.Content.ReadAsStringAsync().Result;

                var token = JsonConvert.DeserializeObject<GoogleToken>(gToken);

                user.AccessToken = token.AccessToken;
                _context.Update(user);
                _context.SaveChanges();

                return token;
            }
        }

        public string GetApi(string accessToken, string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", "Bearer " + accessToken);

            var httpResponse = (HttpWebResponse)request.GetResponse();
            var response = string.Empty;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                response = streamReader.ReadToEnd();
            }

            return response;
        }

        public PersonalInfoDto GetMe(string accessToken)
        {
            string result = GetApi(accessToken, GetUrl(_configuration["Authentication:Google:BaseUri"], "/oauth2/v3/userinfo"));
            return JsonConvert.DeserializeObject<GooglePersonalInfoDto>(result);
        }

        public EmailFoldersDto GetEmailFolders(string accessToken)
        {
            var emailFolders = new EmailFoldersDto();
            emailFolders.EmailFolders = new EmailFolderDto[1];
            emailFolders.EmailFolders[0] = new EmailFolderDto() { Id = "", Name = "Inbox" };
            emailFolders.NextLink = null;
            return emailFolders;
        }

        public EmailsDto GetEmails(string accessToken, string folderId)
        {
            //TODO: Gmail is incomplete
            var result = GetApi(accessToken, GetUrl("", string.Format("/me/mailFolders/{0}/messages", folderId)));
            return new EmailsDto();
        }

        public DrivesDto GetDrives(string accessToken)
        {
            //TODO: Gmail is incomplete
            var result = GetApi(accessToken, GetUrl("", "/me/drives"));
            return new DrivesDto();
        }

        public FoldersAndFilesDto GetDriveRootFolder(string accessToken, string driveId)
        {
            //TODO: Gmail is incomplete
            var result = GetApi(accessToken, GetUrl("", "/me/drives/" + driveId + "/root/children"));
            return new FoldersAndFilesDto();
        }

        public FoldersAndFilesDto GetDriveSubFolders(string accessToken, string driveId, string folderId)
        {
            //TODO: Gmail is incomplete
            var result = GetApi(accessToken, GetUrl("", string.Format("/drives/{0}/items/{1}/children", driveId, folderId)));
            return new FoldersAndFilesDto();
        }

        public FoldersAndFilesDto GetFolderFiles(string accessToken, string driveId, string folderId)
        {
            return GetDriveSubFolders(accessToken, driveId, folderId);
        }

        private string GetUrl(string baseUri, string path)
        {
            return string.Format(@"{0}{1}", _configuration["Authentication:Google:BaseUri"], path);
        }
    }
}
