using System.Collections.Generic;
using System.Threading.Tasks;
using X3Code.Utils.HTTP;

namespace X3Code.UnitTests.HTTP;

public class RestFullApiClient : BasicHttpClient
{
    public RestFullApiClient(HttpClientConfiguration configuration) : base(configuration)
    {
    }

    internal async Task<IEnumerable<UnitTestProduct>> GetAll()
    {
        var result = await GetAsync<IEnumerable<UnitTestProduct>>(HttpConstValues.UnitTestRoute);
        return result ?? new List<UnitTestProduct>();
    }
}