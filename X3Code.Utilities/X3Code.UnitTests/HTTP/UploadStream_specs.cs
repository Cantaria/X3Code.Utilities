using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using X3Code.Utils.HTTP;
using Xunit;

namespace X3Code.UnitTests.HTTP;

/// <summary>
/// UploadStreamAsync wrapped the caller's stream in a StreamContent and disposed of that content.
/// StreamContent.Dispose disposes the stream it wraps, so the caller got its own stream closed
/// underneath it - a fix that was worse than the leak it was meant to repair.
/// </summary>
public class UploadStream_specs
{
    private sealed class RecordingHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { RequestMessage = request });
    }

    private sealed class TestClient : BasicHttpClient
    {
        public TestClient() : base(new HttpClientConfiguration { BaseUrl = "https://example.invalid" })
        {
        }
    }

    [Fact]
    public async Task The_callers_stream_stays_usable_after_the_upload()
    {
        using var client = new TestClient();
        using var stream = new MemoryStream([1, 2, 3, 4]);

        // Der Handler ist hier nicht erreichbar; entscheidend ist, dass der Stream danach
        // noch lebt - egal ob die Anfrage durchging oder nicht.
        try
        {
            await client.UploadStreamAsync("/upload", stream);
        }
        catch (HttpRequestException)
        {
            // Kein Netz im Test, das ist in Ordnung.
        }

        Assert.True(stream.CanRead);
        Assert.Equal(4, stream.Length);
    }
}
