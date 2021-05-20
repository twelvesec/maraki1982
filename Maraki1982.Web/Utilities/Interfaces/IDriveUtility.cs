namespace Maraki1982.Web.Utilities.Interfaces
{
    public interface IDriveUtility
    {
        /// <summary>
        /// Get Ms User drives.
        /// </summary>
        /// <param name="msUserId">The MS user id.</param>
        void GetUserDrives(int msUserId);

        /// <summary>
        /// Get drive root folders recursive
        /// </summary>
        /// <param name="msUserId">The MS user id.</param>
        /// <param name="driveId">The drive id.</param>
        void GetUserDriveRootFolders(int msUserId, int driveId);

        /// <summary>
        /// Get drive subfolders 
        /// </summary>
        /// <param name="msUserId">The MS user id.</param>
        /// <param name="driveId">The drive id.</param>
        /// <param name="folderId">The folder id.</param>
        void GetUserDriveSubFolders(int msUserId, int driveId, int folderId);

        /// <summary>
        /// Get drive folder files
        /// </summary>
        /// <param name="msUserId">The MS user id.</param>
        /// <param name="driveId">The drive id.</param>
        /// <param name="folderId">The folder id.</param>
        void GetUserDriveFolderFiles(int msUserId, int driveId, int folderId);
    }
}
