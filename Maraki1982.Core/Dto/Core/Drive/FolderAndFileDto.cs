namespace Maraki1982.Core.Dto.Core.Drive
{
    public class FolderAndFileDto
    {
        public virtual string Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string DownloadUrl { get; set; }

        public virtual dynamic Folder { get; set; }
    }
}
