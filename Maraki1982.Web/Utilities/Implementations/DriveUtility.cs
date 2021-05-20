using Microsoft.EntityFrameworkCore;
using Maraki1982.Core.DAL;
using Maraki1982.Core.Models.Enum;
using Maraki1982.Core.VendorApi;
using Maraki1982.Web.Utilities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Maraki1982.Core.Models.Database;
using Maraki1982.Core.VendorApi.Helpers;
using Maraki1982.Core.Dto.Core.Drive;

namespace Maraki1982.Web.Utilities.Implementations
{
    public class DriveUtility : IDriveUtility
    {
        private readonly OAuthServerContext _context;
        private Func<VendorEnum, IExternalApi> _externalApiDelegate;

        public DriveUtility(OAuthServerContext context, Func<VendorEnum, IExternalApi> externalApiDelegate)
        {
            _context = context;
            _externalApiDelegate = externalApiDelegate;
        }

        public void GetUserDrives(int msUserId)
        {
            User user = _context.Users.Include(x => x.EmailFolders).ThenInclude(x => x.Emails).Include(x => x.Drives).ThenInclude(x => x.Folders).First(x => x.Id == msUserId);
            IExternalApi externalApi = _externalApiDelegate(user.Vendor);
            ICustomVendorCalls<DrivesDto> customVendorCalls = new CustomVendorCalls<DrivesDto>(externalApi);
            DrivesDto drives = externalApi.GetDrives(user.AccessToken);
            GetDrivesRecursive(drives, user);

            while (drives.NextLink != null)
            {
                drives = customVendorCalls.GetCustomData(user.AccessToken, drives.NextLink);
                GetDrivesRecursive(drives, user);
            }
        }

        public void GetUserDriveRootFolders(int msUserId, int driveId)
        {
            User user = _context.Users.Include(x => x.EmailFolders)
                .ThenInclude(x => x.Emails)
                .Include(x => x.Drives)
                .ThenInclude(x => x.Folders)
                .ThenInclude(x => x.Files)
                .First(x => x.Id == msUserId);
            IExternalApi externalApi = _externalApiDelegate(user.Vendor);
            ICustomVendorCalls<FoldersAndFilesDto> customVendorCalls = new CustomVendorCalls<FoldersAndFilesDto>(externalApi);

            CreateRootFolder(driveId, user);
            Drive drive = user.Drives.FirstOrDefault(x => x.Id == driveId);
            FoldersAndFilesDto foldersAndFilesDto = externalApi.GetDriveRootFolder(user.AccessToken, drive.MsId);
            GetDriveFoldersRecursive(driveId, "/", foldersAndFilesDto, user);

            while (foldersAndFilesDto.NextLink != null)
            {
                foldersAndFilesDto = customVendorCalls.GetCustomData(user.AccessToken, foldersAndFilesDto.NextLink);
                GetDriveFoldersRecursive(driveId, "/", foldersAndFilesDto, user);
            }
        }

        public void GetUserDriveSubFolders(int msUserId, int driveId, int folderId)
        {
            User user = _context.Users.Include(x => x.EmailFolders)
                .ThenInclude(x => x.Emails)
                .Include(x => x.Drives)
                .ThenInclude(x => x.Folders)
                .ThenInclude(x => x.Files)
                .First(x => x.Id == msUserId);
            IExternalApi externalApi = _externalApiDelegate(user.Vendor);
            ICustomVendorCalls<FoldersAndFilesDto> customVendorCalls = new CustomVendorCalls<FoldersAndFilesDto>(externalApi);

            Drive drive = user.Drives.FirstOrDefault(x => x.Id == driveId);
            Folder folder = drive.Folders.FirstOrDefault(x => x.Id == folderId);
            FoldersAndFilesDto foldersAndFilesDto = externalApi.GetDriveSubFolders(user.AccessToken, drive.MsId, folder.MsId);
            string folderPath = string.Format(@"{0}{1}/", folder.FolderPath, folder.Name);
            GetDriveFoldersRecursive(driveId, folderPath, foldersAndFilesDto, user);

            while (foldersAndFilesDto.NextLink != null)
            {
                foldersAndFilesDto = customVendorCalls.GetCustomData(user.AccessToken, foldersAndFilesDto.NextLink);
                GetDriveFoldersRecursive(driveId, folderPath, foldersAndFilesDto, user);
            }
        }

        public void GetUserDriveFolderFiles(int msUserId, int driveId, int folderId)
        {
            User user = _context.Users.Include(x => x.EmailFolders)
                .ThenInclude(x => x.Emails)
                .Include(x => x.Drives)
                .ThenInclude(x => x.Folders)
                .ThenInclude(x => x.Files)
                .First(x => x.Id == msUserId);
            IExternalApi externalApi = _externalApiDelegate(user.Vendor);
            ICustomVendorCalls<FoldersAndFilesDto> customVendorCalls = new CustomVendorCalls<FoldersAndFilesDto>(externalApi);

            Drive drive = user.Drives.FirstOrDefault(x => x.Id == driveId);
            Folder folder = drive.Folders.FirstOrDefault(x => x.Id == folderId);
            FoldersAndFilesDto foldersAndFilesDto = externalApi.GetFolderFiles(user.AccessToken, drive.MsId, folder.MsId);
            GetDriveFolderFilesRecursive(driveId, folderId, foldersAndFilesDto, user);

            while (foldersAndFilesDto.NextLink != null)
            {
                foldersAndFilesDto = customVendorCalls.GetCustomData(user.AccessToken, foldersAndFilesDto.NextLink);
                GetDriveFolderFilesRecursive(driveId, folderId, foldersAndFilesDto, user);
            }
        }

        private void CreateRootFolder(int driveId, User user)
        {
            if (user.Drives.First(x => x.Id == driveId).Folders.FirstOrDefault(x => x.Name == "Root" && x.FolderPath == string.Empty && x.MsId == "root") == null)
            {
                user.Drives.First(x => x.Id == driveId).Folders.Add(new Folder() { MsId = "root", DriveId = driveId, FolderPath = string.Empty, Name = "Root", DownloadUrl = string.Empty });

                _context.Update(user);
                _context.SaveChanges();
            }
        }

        private void GetDrivesRecursive(DrivesDto drivesDto, User user)
        {
            if (drivesDto.Drives != null)
            {
                List<DriveDto> drives = drivesDto.Drives.ToList();
                drives.ForEach(x =>
                {
                    if (user.Drives != null && user.Drives.FirstOrDefault(y => y.MsId == x.Id) == null)
                    {
                        user.Drives.Add(new Drive() { MsId = x.Id, DriveType = x.DriveType, UserId = user.Id });
                    }
                });

                _context.Update(user);
                _context.SaveChanges();
            }
        }

        private void GetDriveFoldersRecursive(int driveId, string folderPath, FoldersAndFilesDto foldersAndFilesDto, User user)
        {
            if (foldersAndFilesDto.FoldersAndFiles != null)
            {
                List<FolderAndFileDto> foldersAndFiles = foldersAndFilesDto.FoldersAndFiles.ToList();
                foldersAndFiles.ForEach(x =>
                {
                    if (user.Drives != null &&
                        user.Drives.FirstOrDefault(y => y.Id == driveId) != null &&
                        user.Drives.FirstOrDefault(y => y.Id == driveId).Folders != null &&
                        user.Drives.FirstOrDefault(y => y.Id == driveId).Folders.FirstOrDefault(y => y.MsId == x.Id) == null &&
                        x.Folder != null)
                    {
                        user.Drives.First(y => y.Id == driveId).Folders.Add(new Folder() { MsId = x.Id, DriveId = driveId, FolderPath = folderPath, Name = x.Name, DownloadUrl = x.DownloadUrl });
                    }
                });

                _context.Update(user);
                _context.SaveChanges();
            }
        }

        private void GetDriveFolderFilesRecursive(int driveId, int folderId, FoldersAndFilesDto foldersAndFilesDto, User user)
        {
            if (foldersAndFilesDto.FoldersAndFiles != null)
            {
                List<FolderAndFileDto> foldersAndFiles = foldersAndFilesDto.FoldersAndFiles.ToList();
                foldersAndFiles.ForEach(x =>
                {
                    if (user.Drives != null &&
                        user.Drives.FirstOrDefault(y => y.Id == driveId) != null &&
                        user.Drives.FirstOrDefault(y => y.Id == driveId).Folders != null &&
                        user.Drives.FirstOrDefault(y => y.Id == driveId).Folders.FirstOrDefault(y => y.Id == folderId) != null &&
                        user.Drives.FirstOrDefault(y => y.Id == driveId).Folders.First(y => y.Id == folderId).Files.FirstOrDefault(y => y.MsId == x.Id) == null &&
                        x.Folder == null)
                    {
                        user.Drives.First(y => y.Id == driveId).Folders.First(x => x.Id == folderId).Files.Add(new File() { MsId = x.Id, FolderId = folderId, Name = x.Name, DownloadUrl = x.DownloadUrl });
                    }
                });

                _context.Update(user);
                _context.SaveChanges();
            }
        }
    }
}
