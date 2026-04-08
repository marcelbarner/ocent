using System.Text.Json;

using ocent.DevEnvironment.AcceptanceTests.TestProject.Models;

namespace ocent.DevEnvironment.AcceptanceTests.TestProject.Tests;

[ClassDataSource<AppFixture>(Shared = SharedType.PerTestSession)]
public class ApiTests(AppFixture fixture)
{
    private static readonly string[] ValidSummaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    [Test]
    public async Task GetWeatherForecastReturnsOkStatusCode()
    {
        var httpClient = fixture.CreateHttpClient("webapi");

        var response = await httpClient.GetAsync("/weatherforecast");

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }

    [Test]
    public async Task GetWeatherForecastReturnsFiveItems()
    {
        var httpClient = fixture.CreateHttpClient("webapi");

        var response = await httpClient.GetAsync("/weatherforecast");
        var content = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<IEnumerable<WeatherForecast>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        await Assert.That(data).IsNotNull();
        await Assert.That(data!.Count()).IsEqualTo(5);
    }

    [Test]
    public async Task GetWeatherForecastReturnsOnlyValidSummaries()
    {
        var httpClient = fixture.CreateHttpClient("webapi");

        var response = await httpClient.GetAsync("/weatherforecast");
        var content = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<IEnumerable<WeatherForecast>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        await Assert.That(data).IsNotNull();
        await Assert.That(data!.All(w => ValidSummaries.Contains(w.Summary))).IsTrue();
    }
}