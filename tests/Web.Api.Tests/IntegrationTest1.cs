using Projects;
using System.Text.Json;

namespace Web.Api.Tests
{
	public class IntegrationTest1
	{
		// Instructions:
		// 1. Add a project reference to the target AppHost project, e.g.:
		//
		//    <ItemGroup>
		//        <ProjectReference Include="../MyAspireApp.AppHost/MyAspireApp.AppHost.csproj" />
		//    </ItemGroup>
		//
		// 2. Uncomment the following example test and update 'Projects.MyAspireApp_AppHost' to match your AppHost project:
		//
		[Test]
		public async Task GetWebResourceRootReturnsOkStatusCode()
		{
			// Arrange
			var appHost = await DistributedApplicationTestingBuilder.CreateAsync<AppHost>();
			appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
			{
				clientBuilder.AddStandardResilienceHandler();
			});
			await using var app = await appHost.BuildAsync();
			var resourceNotificationService = app.Services.GetRequiredService<ResourceNotificationService>();
			await app.StartAsync();

			// Act
			var httpClient = app.CreateHttpClient("web-api");
			await resourceNotificationService.WaitForResourceAsync("web-api", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
			var response = await httpClient.GetAsync("/weatherforecast");

			var content = await response.Content.ReadAsStringAsync();
			var weatherForecasts = JsonSerializer.Deserialize<WeatherForecast[]>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

			// Assert
			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
			Assert.That(weatherForecasts, Has.Length.EqualTo(5));
			Assert.That(weatherForecasts, Has.All.Property(nameof(WeatherForecast.Date)).Not.Null);
		}
	}
}
