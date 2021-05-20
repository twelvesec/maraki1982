namespace Maraki1982.Web.Utilities.Interfaces
{
    public interface IEmailUtility
    {
        /// <summary>
        /// Get the MS user email folders
        /// </summary>
        /// <param name="msUserId">The MS user id</param>
        void GetUserEmailFolders(int msUserId);

        /// <summary>
        /// Get the MS user emails for a given folder id
        /// </summary>
        /// <param name="msUserId">The MS user id</param>
        /// <param name="folderId">The folder id</param>
        void GetFolderEmails(int msUserId, int folderId);
    }
}
