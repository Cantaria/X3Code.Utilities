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
}