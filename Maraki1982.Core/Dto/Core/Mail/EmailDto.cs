namespace Maraki1982.Core.Dto.Core.Mail
{
    public class EmailDto
    {
        public virtual string Id { get; set; }

        public virtual string Subject { get; set; }

        public virtual EmailBodyDto Body { get; set; }
    }
}
