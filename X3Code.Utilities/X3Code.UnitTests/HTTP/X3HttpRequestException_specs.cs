using System;
using System.Net;
using X3Code.Utils.Exceptions;
using Xunit;

namespace X3Code.UnitTests.HTTP;

/// <summary>
/// The exception used to hold on to the HttpResponseMessage - an IDisposable nobody disposed and
/// that could already be gone by the time somebody inspected the exception. It carries the body
/// as a string now, captured while the response was still alive.
/// </summary>
// ReSharper disable once InconsistentNaming
public class X3HttpRequestException_specs
{
    private class Problem
    {
        public string Title { get; set; } = string.Empty;
    }

    private static X3HttpRequestException Create(string? content) =>
        new("boom", new InvalidOperationException("inner"), HttpStatusCode.BadRequest, content);

    [Fact]
    public void Carries_status_code_and_body()
    {
        var exception = Create("""{"Title":"nope"}""");

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal("""{"Title":"nope"}""", exception.ResponseContent);
    }

    [Fact]
    public void Parses_the_body_without_going_back_to_the_network()
    {
        var problem = Create("""{"Title":"nope"}""").TryParseResponseContent<Problem>();

        Assert.NotNull(problem);
        Assert.Equal("nope", problem.Title);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("this is not json")]
    public void Returns_the_default_instead_of_throwing(string? content)
    {
        // "Try" im Namen heißt jetzt auch "wirft nicht".
        Assert.Null(Create(content).TryParseResponseContent<Problem>());
    }
}
