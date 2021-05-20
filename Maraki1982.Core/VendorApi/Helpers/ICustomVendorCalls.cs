namespace Maraki1982.Core.VendorApi.Helpers
{
    public interface ICustomVendorCalls<T>
    {
        T GetCustomData(string accessToken, string url);
    }
}
