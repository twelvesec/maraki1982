using Maraki1982.Core.Models.Database;
using Maraki1982.Core.Models.Enum;

namespace Maraki1982.Web.Utilities.Interfaces
{
    public interface IUserUtility
    {
        /// <summary>
        /// Create or update an vendor user
        /// </summary>
        /// <param name="idToken">The id token of the victim</param>
        /// <param name="vendor">The victim's vendor</param>
        /// <returns>The MS user</returns>
        User CreateUpdateUser(string idToken, VendorEnum vendor);
    }
}
