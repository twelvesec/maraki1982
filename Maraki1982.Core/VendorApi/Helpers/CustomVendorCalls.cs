using Newtonsoft.Json;

namespace Maraki1982.Core.VendorApi.Helpers
{
    public class CustomVendorCalls<T> : ICustomVendorCalls<T>
    {
        private readonly IExternalApi _externalApi;

        public CustomVendorCalls(IExternalApi externalApi)
        {
            _externalApi = externalApi;
        }

        public T GetCustomData(string accessToken, string url)
        {
            string result = _externalApi.GetApi(accessToken, url);
            T customObject = JsonConvert.DeserializeObject<T>(result);
            return customObject;
        }
    }
}
