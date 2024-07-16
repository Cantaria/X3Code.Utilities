using System.Threading.Tasks;
using X3Code.UnitTests.HTTP.Models;
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

    [Fact]
    public async Task CanPostData()
    {
        var testData = new AddObjectApiModel
        {
            Name = "Unit-Test",
            Payload = new Data
            {
                Price = 15.4D,
                CpuModel = "AMD",
                HardDiskSpace = "5MB",
                Year = 2022
            }
        };

        var postResult = await ApiClient.PostAsync<AddObjectApiModel, AddObjectResponseModel>("objects", testData);
        Assert.NotNull(postResult);

        var res = await ApiClient.GetAsync($"objects?id={postResult.Id}");
        
        var getResult = await ApiClient.GetAsync<AddObjectResponseModel>($"objects?id={postResult.Id}");
        Assert.NotNull(getResult);
        Assert.NotNull(getResult.Payload);
    }
}