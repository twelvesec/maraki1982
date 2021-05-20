using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maraki1982.Core.Models.Database
{
    public class Folder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string MsId { get; set; }
        public string Name { get; set; }
        public string FolderPath { get; set; }
        public string DownloadUrl { get; set; }

        public virtual ICollection<File> Files { get; set; }

        [ForeignKey("Drive")]
        public int DriveId { get; set; }
        public virtual Drive Drive { get; set; }
    }
}
