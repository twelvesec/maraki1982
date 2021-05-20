using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maraki1982.Core.Models.Database
{
    public class Email
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string MsId { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

        [ForeignKey("EmailFolder")]
        public int EmailFolderId { get; set; }
        public virtual EmailFolder EmailFolder { get; set; }
    }
}
