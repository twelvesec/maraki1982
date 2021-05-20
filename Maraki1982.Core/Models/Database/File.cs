using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maraki1982.Core.Models.Database
{
    public class File
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string MsId { get; set; }
        public string Name { get; set; }
        public string DownloadUrl { get; set; }

        [ForeignKey("Folder")]
        public int FolderId { get; set; }
        public virtual Folder Folder { get; set; }
    }
}
