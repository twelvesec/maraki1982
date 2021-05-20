namespace Maraki1982.Core.Dto.Core.Mail
{
    public class EmailFoldersDto
    {
        public virtual EmailFolderDto[] EmailFolders { get; set; }

        public virtual string NextLink { get; set; }
    }
}
