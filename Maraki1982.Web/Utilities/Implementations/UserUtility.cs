using Microsoft.EntityFrameworkCore;
using Maraki1982.Core.DAL;
using Maraki1982.Core.Models.Enum;
using Maraki1982.Core.VendorApi;
using Maraki1982.Web.Utilities.Interfaces;
using System;
using System.Collections.Generic;
using Maraki1982.Core.Dto.Core;
using Maraki1982.Core.Models.Database;

namespace Maraki1982.Web.Utilities.Implementations
{
    public class UserUtility : IUserUtility
    {
        private readonly OAuthServerContext _context;
        private Func<VendorEnum, IExternalApi> _externalApiDelegate;

        public UserUtility(OAuthServerContext context, Func<VendorEnum, IExternalApi> externalApiDelegate)
        {
            _context = context;
            _externalApiDelegate = externalApiDelegate;
        }

        public User CreateUpdateUser(string idToken, VendorEnum vendor)
        {
            IExternalApi externalApi = _externalApiDelegate(vendor);
            Token token = GetToken(idToken, externalApi);
            PersonalInfoDto personalInfoDto = GetPersonalInfo(token.AccessToken, externalApi);

            var user = _context.Users.Include(x => x.EmailFolders)
                .ThenInclude(x => x.Emails)
                .Include(x => x.Drives)
                .ThenInclude(x => x.Folders)
                .ThenInclude(x => x.Files)
                .FirstOrDefaultAsync(x => x.VendorId == personalInfoDto.Id).Result;

            if (user != null)
            {
                user.AccessToken = token.AccessToken;
                user.RefreshToken = token.RefreshToken;
                _context.Update(user);
                _context.SaveChanges();
            }
            else
            {
                user = new User()
                {
                    Vendor = vendor,
                    VendorId = personalInfoDto.Id,
                    Name = personalInfoDto.Name ?? string.Empty,
                    Email = personalInfoDto.Email,
                    AccessToken = token.AccessToken,
                    RefreshToken = token.RefreshToken,
                    EmailFolders = new List<EmailFolder>(),
                    Drives = new List<Drive>()
                };
                _context.Users.Add(user);
                _context.SaveChanges();
            }

            return user;
        }


        private Token GetToken(string idToken, IExternalApi externalApi)
        {
            return externalApi.GetToken(idToken);
        }

        private PersonalInfoDto GetPersonalInfo(string accessToken, IExternalApi externalApi)
        {
            return externalApi.GetMe(accessToken);
        }
    }
}
