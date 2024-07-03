using System.Collections.Generic;
using System.Threading.Tasks;
using X3Code.Utils.HTTP;

namespace X3Code.UnitTests.HTTP;

internal class RestFullApiClient : BasicHttpClient
{
    public RestFullApiClient(HttpClientConfiguration configuration) : base(configuration)
    {
    }

    public async Task<IEnumerable<UnitTestProduct>> GetAll()
    {
        var result = await GetAsync<IEnumerable<UnitTestProduct>>(HttpConstValues.UnitTestRoute);
        return result ?? new List<UnitTestProduct>();
    }

    public async Task<IEnumerable<UnitTestProduct>> GetByIDs(params int[] ids)
    {
        // https://api.restful-api.dev/objects?id=3&id=5&id=10
        
        //Can be done more beautifully, but it's enough for the unit test
        var url = HttpConstValues.UnitTestRoute;
        var isFirst = true;
        foreach (var id in ids)
        {
            if (isFirst)
            {
                url += $"?id={id}";
                isFirst = false;
                continue;
            }
            url += $"&id={id}";
        }

        var result = await GetAsync<IEnumerable<UnitTestProduct>>(url);
        return result ?? new List<UnitTestProduct>();
    }
}