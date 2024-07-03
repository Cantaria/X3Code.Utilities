using System.Collections.Generic;

namespace X3Code.UnitTests.HTTP;

internal class UnitTestProduct
{
    public string? Id { get; set; }

    public string? Name { get; set; }
    
    public Dictionary<string, object>? Data { get; set; }
}