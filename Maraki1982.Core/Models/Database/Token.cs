namespace Maraki1982.Core.Models.Database
{
    public class Token
    {
        public virtual string AccessToken { get; set; }

        public virtual string RefreshToken { get; set; }
    }
}
