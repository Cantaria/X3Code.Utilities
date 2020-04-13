using X3Code.Utils;
using Xunit;

// ReSharper disable InconsistentNaming

namespace X3Code.UnitTests
{
    public class RegesUtilities_specs
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
        public void ValideEmails(string email, bool exceptedResult)
        {
            var result = RegexUtilities.IsValidEmail(email);
            Assert.Equal(exceptedResult, result);
        }
    }
}
