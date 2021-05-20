using Microsoft.EntityFrameworkCore;
using Maraki1982.Core.DAL;
using Maraki1982.Core.Models.Enum;
using Maraki1982.Core.VendorApi;
using Maraki1982.Web.Utilities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Maraki1982.Core.Dto.Core.Mail;
using Maraki1982.Core.VendorApi.Helpers;
using Maraki1982.Core.Models.Database;

namespace Maraki1982.Web.Utilities.Implementations
{
    public class EmailUtility : IEmailUtility
    {
        private readonly OAuthServerContext _context;
        private Func<VendorEnum, IExternalApi> _externalApiDelegate;

        public EmailUtility(OAuthServerContext context, Func<VendorEnum, IExternalApi> externalApiDelegate)
        {
            _context = context;
            _externalApiDelegate = externalApiDelegate;
        }

        public void GetUserEmailFolders(int msUserId)
        {
            User user = _context.Users.Include(x => x.EmailFolders)
                .ThenInclude(x => x.Emails)
                .Include(x => x.Drives)
                .ThenInclude(x => x.Folders)
                .ThenInclude(x => x.Files)
                .First(x => x.Id == msUserId);
            IExternalApi externalApi = _externalApiDelegate(user.Vendor);
            ICustomVendorCalls<EmailFoldersDto> customVendorCalls = new CustomVendorCalls<EmailFoldersDto>(externalApi);

            EmailFoldersDto emailFolders = externalApi.GetEmailFolders(user.AccessToken);
            GetEmailFoldersRecursive(emailFolders, user);

            while (emailFolders.NextLink != null)
            {
                emailFolders = customVendorCalls.GetCustomData(user.AccessToken, emailFolders.NextLink);
                GetEmailFoldersRecursive(emailFolders, user);
            }
        }

        public void GetFolderEmails(int msUserId, int folderId)
        {
            User user = _context.Users.Include(x => x.EmailFolders)
                .ThenInclude(x => x.Emails)
                .Include(x => x.Drives)
                .ThenInclude(x => x.Folders)
                .ThenInclude(x => x.Files)
                .First(x => x.Id == msUserId);
            IExternalApi externalApi = _externalApiDelegate(user.Vendor);
            ICustomVendorCalls<EmailsDto> customVendorCalls = new CustomVendorCalls<EmailsDto>(externalApi);

            EmailFolder emailFolder = user.EmailFolders.FirstOrDefault(x => x.Id == folderId);
            EmailsDto emails = externalApi.GetEmails(user.AccessToken, emailFolder.MsId);
            GetEmailsRecursive(emails, user, folderId);

            while (emails.NextLink != null)
            {
                emails = customVendorCalls.GetCustomData(user.AccessToken, emails.NextLink);
                GetEmailsRecursive(emails, user, folderId);
            }            
        }

        private void GetEmailFoldersRecursive(EmailFoldersDto emailFoldersDto, User user)
        {
            if (emailFoldersDto.EmailFolders != null)
            {
                List<EmailFolderDto> emailFolders = emailFoldersDto.EmailFolders.ToList();
                emailFolders.ForEach(x =>
                {
                    if (user.EmailFolders != null && user.EmailFolders.FirstOrDefault(y => y.MsId == x.Id) == null)
                    {
                        user.EmailFolders.Add(new EmailFolder() { MsId = x.Id, Name = x.Name, UserId = user.Id });
                    }
                });

                _context.Update(user);
                _context.SaveChanges();
            }
        }

        private void GetEmailsRecursive(EmailsDto emailsDto, User user, int folderId)
        {

            if (emailsDto.Emails != null)
            {
                List<EmailDto> mailModels = emailsDto.Emails.ToList();
                EmailFolder emailFolder = user.EmailFolders.FirstOrDefault(x => x.Id == folderId);

                mailModels.ForEach(x =>
                {
                    if (emailFolder != null && emailFolder.Emails != null && emailFolder.Emails.FirstOrDefault(y => y.MsId == x.Id) == null)
                    {
                        emailFolder.Emails.Add(new Email() { MsId = x.Id, Subject = x.Subject, Content = HttpUtility.HtmlEncode(x.Body.Content), EmailFolderId = emailFolder.Id });
                    }
                });

                _context.Update(user);
                _context.SaveChanges();
            }
        }
    }
}
