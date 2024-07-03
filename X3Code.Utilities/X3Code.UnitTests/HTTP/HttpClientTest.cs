using System.Threading.Tasks;
using X3Code.Utils.HTTP;
using Xunit;

namespace X3Code.UnitTests.HTTP;

public class HttpClientTest
{
    private HttpClientConfiguration ClientConfiguration => new() { BaseUrl = HttpConstValues.UnitTestBaseUrl };
    private RestFullApiClient ApiClient => new(ClientConfiguration);

    [Fact]
    public async Task CanGetDataFromRestService()
    {
        var result = await ApiClient.GetAll();

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }
    
    [Fact]
    public async Task CanGetDataUrlParamsFromRestService()
    {
        var result = await ApiClient.GetByIDs(3, 5, 10);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }
}