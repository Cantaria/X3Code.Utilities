using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X3Code.UnitTests.HTTP.Models;
using X3Code.Utils.HTTP;

namespace X3Code.UnitTests.HTTP;

/// <summary>
/// Inherit from BasicHttpClient and implement/encapsulate you api
/// </summary>
internal class RestFullApiClient : BasicHttpClient
{
    public RestFullApiClient(HttpClientConfiguration configuration) : base(configuration)
    {
    }

    /// <summary>
    /// Due to the own implementation, it's possible to hide technical details and concentrate to the important stuff
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<UnitTestProduct>> GetAll()
    {
        var result = await GetAsync<IEnumerable<UnitTestProduct>>(HttpConstValues.UnitTestRoute);
        return result ?? new List<UnitTestProduct>();
    }

    /// <summary>
    /// Translate a bunch of parameters to everything you need for the API
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Do exception handling and/or hide technical details again.
    /// Keep api calls and api-paths organized on a centralized place
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<AddObjectResponseModel> AddObject(AddObjectApiModel model)
    {
        var postResult = await PostAsync<AddObjectApiModel, AddObjectResponseModel>("objects", model);
        if (postResult == null) throw new Exception("Nothing returned from REST service");

        return postResult;
    }
}