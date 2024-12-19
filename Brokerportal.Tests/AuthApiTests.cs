using System.Net;
using System.Text;
using Xunit;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using FluentAssertions;

namespace Brokerportal.Tests;

public class AuthApiTests
{
    const string BASE_URL = "https://brokerportal.leverate.com/api/";
    const string SAMPLE_TOKEN = "sample-token";

    private readonly HttpClient _httpClient;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    
    public AuthApiTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri(BASE_URL)
        };
    }

    [Fact]
    public async Task Login_ShouldReturnToken_OnValidCredentials()
    {
        // Arrange
         var loginResponse = new { token = SAMPLE_TOKEN };
        var responseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(loginResponse), Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        var loginPayload = new { username = "testuser", password = "testpassword" };

        // Act
        var response = await _httpClient.PostAsync("auth/login", new StringContent(JsonConvert.SerializeObject(loginPayload), Encoding.UTF8, "application/json"));
        var responseContent = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseContent.Should().Contain(SAMPLE_TOKEN);
    }

    [Fact]
    public async Task RenewToken_ShouldReturnNewToken()
    {
        // Arrange
        var renewResponse = new { token = SAMPLE_TOKEN };
        var responseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(renewResponse), Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act
        var response = await _httpClient.GetAsync("auth/renewToken");
        var responseContent = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseContent.Should().Contain(SAMPLE_TOKEN);
    }

    [Fact]
    public async Task Logout_ShouldReturnSuccess()
    {
        // Arrange
        var responseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act
        var response = await _httpClient.GetAsync("auth/logout");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GenerateApiToken_ShouldReturnToken()
    {
        // Arrange
        var apiTokenResponse = new { apiToken = SAMPLE_TOKEN };
        var responseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(apiTokenResponse), Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act
        var response = await _httpClient.GetAsync("auth/generateAPIToken");
        var responseContent = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseContent.Should().Contain(responseContent);
    }
}
