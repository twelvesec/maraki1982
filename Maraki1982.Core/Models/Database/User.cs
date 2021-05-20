using Maraki1982.Core.Models.Enum;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maraki1982.Core.Models.Database
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public VendorEnum Vendor { get; set; }
        public string VendorId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public virtual ICollection<EmailFolder> EmailFolders { get; set; }

        public virtual ICollection<Drive> Drives { get; set; }
    }
}
