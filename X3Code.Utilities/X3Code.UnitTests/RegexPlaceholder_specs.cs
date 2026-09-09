using X3Code.Utils;
using Xunit;

namespace X3Code.UnitTests;

/// <summary>
/// A placeholder whose property exists but holds null used to be replaced with the entire input
/// string, so "Hello %Nickname% World" came back as "Hello Hello %Nickname% World World".
/// </summary>
/// <remarks>
/// A placeholder naming a property that does not exist at all resolves to an empty string - that
/// is the documented contract of TryReadPropertyAsString and is unchanged.
/// </remarks>
public class RegexPlaceholder_specs
{
    private class Entity
    {
        public string Name { get; set; } = "Clemens";
        public string? Nickname { get; set; }
    }

    [Fact]
    public void A_known_placeholder_is_replaced_with_the_property()
    {
        var result = new Entity().FillPlaceholderFromEntity("Hello %Name% World");

        Assert.Equal("Hello Clemens World", result);
    }

    [Fact]
    public void A_null_property_leaves_the_placeholder_standing_instead_of_duplicating_the_input()
    {
        var result = new Entity().FillPlaceholderFromEntity("Hello %Nickname% World");

        Assert.Equal("Hello %Nickname% World", result);
    }

    [Fact]
    public void A_known_and_a_null_placeholder_side_by_side()
    {
        var result = new Entity().FillPlaceholderFromEntity("%Name% and %Nickname%");

        Assert.Equal("Clemens and %Nickname%", result);
    }

    [Fact]
    public void An_unknown_property_resolves_to_nothing()
    {
        var result = new Entity().FillPlaceholderFromEntity("Hello %Missing% World");

        Assert.Equal("Hello  World", result);
    }
}
