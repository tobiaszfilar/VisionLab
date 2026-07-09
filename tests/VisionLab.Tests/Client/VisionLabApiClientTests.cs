using System.Net;
using System.Net.Http.Json;
using VisionLab.Client;
using VisionLab.Shared.Images;

namespace VisionLab.Tests.Client;

public sealed class VisionLabApiClientTests
{
    [Fact]
    public async Task GetImagesAsync_should_return_images()
    {
        var expected = new[]
        {
            new ImageAssetDto(
                Guid.NewGuid(),
                "cat.png",
                "stored-cat.png",
                "image/png",
                123,
                DateTimeOffset.UtcNow)
        };

        using var httpClient = new HttpClient(
            new StubHttpMessageHandler(request =>
            {
                Assert.Equal(HttpMethod.Get, request.Method);
                Assert.Equal("/api/images", request.RequestUri?.AbsolutePath);

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(expected)
                };
            }))
        {
            BaseAddress = new Uri("http://localhost")
        };

        var client = new VisionLabApiClient(httpClient);

        var images = await client.GetImagesAsync();

        var image = Assert.Single(images);

        Assert.Equal(expected[0].Id, image.Id);
        Assert.Equal("cat.png", image.OriginalFileName);
    }

    [Fact]
    public async Task GetImageByIdAsync_should_return_null_when_not_found()
    {
        using var httpClient = new HttpClient(
            new StubHttpMessageHandler(_ =>
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }))
        {
            BaseAddress = new Uri("http://localhost")
        };

        var client = new VisionLabApiClient(httpClient);

        var image = await client.GetImageByIdAsync(Guid.NewGuid());

        Assert.Null(image);
    }

    [Fact]
    public async Task UploadImageAsync_should_send_multipart_request()
    {
        var expected = new ImageAssetDto(
            Guid.NewGuid(),
            "cat.png",
            "stored-cat.png",
            "image/png",
            4,
            DateTimeOffset.UtcNow);

        using var httpClient = new HttpClient(
            new StubHttpMessageHandler(request =>
            {
                Assert.Equal(HttpMethod.Post, request.Method);
                Assert.Equal("/api/images", request.RequestUri?.AbsolutePath);
                Assert.IsType<MultipartFormDataContent>(request.Content);

                return new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = JsonContent.Create(expected)
                };
            }))
        {
            BaseAddress = new Uri("http://localhost")
        };

        var client = new VisionLabApiClient(httpClient);

        await using var stream = new MemoryStream([1, 2, 3, 4]);

        var uploaded = await client.UploadImageAsync(
            stream,
            "cat.png",
            "image/png");

        Assert.Equal(expected.Id, uploaded.Id);
        Assert.Equal("cat.png", uploaded.OriginalFileName);
    }

    [Fact]
    public async Task GetImagesAsync_should_throw_api_exception_on_error()
    {
        using var httpClient = new HttpClient(
            new StubHttpMessageHandler(_ =>
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("Something went wrong.")
                };
            }))
        {
            BaseAddress = new Uri("http://localhost")
        };

        var client = new VisionLabApiClient(httpClient);

        var exception = await Assert.ThrowsAsync<VisionLabApiException>(
            () => client.GetImagesAsync());

        Assert.Equal(HttpStatusCode.InternalServerError, exception.StatusCode);
        Assert.Equal("Something went wrong.", exception.ResponseBody);
    }

    [Fact]
    public async Task GetImageContentAsync_should_return_stream()
    {
        var bytes = new byte[] { 1, 2, 3, 4 };

        using var httpClient = new HttpClient(
            new StubHttpMessageHandler(request =>
            {
                Assert.Equal(HttpMethod.Get, request.Method);
                Assert.EndsWith("/content", request.RequestUri?.AbsolutePath);

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(bytes)
                };
            }))
        {
            BaseAddress = new Uri("http://localhost")
        };

        var client = new VisionLabApiClient(httpClient);

        await using var stream = await client.GetImageContentAsync(Guid.NewGuid());

        Assert.NotNull(stream);

        using var memoryStream = new MemoryStream();

        await stream.CopyToAsync(memoryStream);

        Assert.Equal(bytes, memoryStream.ToArray());
    }

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

        public StubHttpMessageHandler(
            Func<HttpRequestMessage, HttpResponseMessage> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(_handler(request));
        }
    }
}