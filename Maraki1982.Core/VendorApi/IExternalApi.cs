using Maraki1982.Core.Dto.Core;
using Maraki1982.Core.Dto.Core.Drive;
using Maraki1982.Core.Dto.Core.Mail;
using Maraki1982.Core.Models.Database;

namespace Maraki1982.Core.VendorApi
{
    public interface IExternalApi
    {
        /// <summary>
        /// Get API access token
        /// </summary>
        /// <param name="idToken">The id token from vendor</param>
        /// <returns>The token in json string</returns>
        Token GetToken(string idToken);

        /// <summary>
        /// Refresh API access token
        /// </summary>
        /// <param name="user">The vendor user that requests the refresh token</param>
        /// <returns>The refreshed token in json string</returns>
        Token RefreshToken(User user);

        /// <summary>
        /// Make a get request to Graph API
        /// </summary>
        /// <param name="accessToken">The access token aquired from GetToken method</param>
        /// <param name="url">The requested url</param>
        /// <returns>The stringified response</returns>
        string GetApi(string accessToken, string url);

        /// <summary>
        /// Get the user's profile.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns>The Personal Info Dto.</returns>
        PersonalInfoDto GetMe(string accessToken);

        /// <summary>
        /// Get the user's email folders.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns>The Email Folders Dto.</returns>
        EmailFoldersDto GetEmailFolders(string accessToken);

        /// <summary>
        /// Get the user's emails.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="folderId">The MS folder id.</param>
        /// <returns>The Emails Dto.</returns>
        EmailsDto GetEmails(string accessToken, string folderId);

        /// <summary>
        /// Get users drives.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns>The Drives Dto.</returns>
        DrivesDto GetDrives(string accessToken);

        /// <summary>
        /// Get user's drive root folder.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="driveId">The drive id.</param>
        /// <returns>The Folders Dto.</returns>
        FoldersAndFilesDto GetDriveRootFolder(string accessToken, string driveId);

        /// <summary>
        /// Get user's drive subfolders.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="driveId">The drive id.</param>
        /// <param name="folderId">The folder id.</param>
        /// <returns>The Folders Dto.</returns>
        FoldersAndFilesDto GetDriveSubFolders(string accessToken, string driveId, string folderId);

        /// <summary>
        /// Get user's folder data
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="driveId">The drive id.</param>
        /// <param name="folderId">The folderId</param>
        /// <returns>The Folders Dto.</returns>
        FoldersAndFilesDto GetFolderFiles(string accessToken, string driveId, string folderId);
    }
}
