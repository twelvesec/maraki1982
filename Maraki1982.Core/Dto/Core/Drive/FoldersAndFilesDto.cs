namespace Maraki1982.Core.Dto.Core.Drive
{
    public class FoldersAndFilesDto
    {
        public virtual FolderAndFileDto[] FoldersAndFiles { get; set; }

        public virtual string NextLink { get; set; }
    }
}
