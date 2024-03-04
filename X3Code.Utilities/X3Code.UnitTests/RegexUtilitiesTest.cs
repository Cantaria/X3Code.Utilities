using System;
using X3Code.Utils;
using Xunit;

namespace X3Code.UnitTests;

public class RegexUtilitiesTest
{
    [Theory]
    [InlineData("david.jones@proseware.com", true)]
    [InlineData("d.j@server1.proseware.com", true)]
    [InlineData("jones@ms1.proseware.com", true)]
    [InlineData("j@proseware.com9", true)]
    [InlineData("js#internal@proseware.com", true)]
    [InlineData("j_9@[129.126.118.1]", true)]
    [InlineData("js@proseware.com9", true)]
    [InlineData("j.s@server1.proseware.com", true)]
    [InlineData("js@contoso.中国", true)]
    [InlineData("js@proseware..com", false)]
    [InlineData("js*@proseware.com", false)]
    [InlineData("j..s@proseware.com", false)]
    [InlineData("j.@server1.proseware.com", false)]
    public void ValidateEmails(string email, bool exceptedResult)
    {
        var result = RegexUtilities.IsValidEmail(email);
        Assert.Equal(exceptedResult, result);
    }

    private class PlaceHolderTester
    {
        public string Color => "Green";
        public int Value => 12;
        public DateTime AvailableAt => new (2015, 05, 21);
    }
    
    [Fact]
    public void CanReplacePlaceholder()
    {
        var tester = new PlaceHolderTester();
        var placeholderString = "The customer ordered the color %Color% for a value of %Value% at %AvailableAt%";
        var expectedString = $"The customer ordered the color {tester.Color} for a value of {tester.Value} at {tester.AvailableAt:yyyyMMdd HH:mm}";

        var result = tester.FillPlaceholderFromEntity(placeholderString, dateTimeFormat: "yyyyMMdd HH:mm");
        var result2 = placeholderString.FillPlaceholderStringFromEntity(tester, dateTimeFormat: "yyyyMMdd HH:mm");
        Assert.Equal(expectedString, result);
        Assert.Equal(expectedString, result2);
    }
    
    [Fact]
    public void CanHandleNullOrEmptyValues()
    {
        var tester = new PlaceHolderTester();
        var nullResult = tester.FillPlaceholderFromEntity(null);
        var emptyResult = tester.FillPlaceholderFromEntity(string.Empty);

        Assert.Null(nullResult);
        Assert.NotNull(emptyResult);
        Assert.Empty(emptyResult);
    }
}