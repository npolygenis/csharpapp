using CSharpApp.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace CSharpApp.Api.Tests;

public class PerformanceMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_LogsRequestPerformance()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<PerformanceMiddleware>>();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Method = "GET";
        httpContext.Request.Path = "/test";
        var requestDelegate = new RequestDelegate((innerHttpContext) =>
        {
            return Task.CompletedTask;
        });

        var middleware = new PerformanceMiddleware(requestDelegate, loggerMock.Object);

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Request GET /test executed in")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
