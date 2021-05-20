namespace Maraki1982.Core.Dto.Core.Mail
{
    public class EmailsDto
    {
        public virtual EmailDto[] Emails { get; set; }

        public virtual string NextLink { get; set; }
    }
}
