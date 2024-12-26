using Dapper;
using Npgsql;

namespace Web.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();
        builder.AddNpgsqlDataSource("stocks");

        builder.Services.AddProblemDetails();

		// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
		builder.Services.AddOpenApi();

		// Add services to the container.
		builder.Services.AddAuthentication()
			.AddKeycloakJwtBearer(
                serviceName: "keycloak", 
                realm: "stocks", 
                options =>
                {
                    options.RequireHttpsMetadata = false;
					options.Audience = "account";
			    });

        builder.Services.AddAuthorizationBuilder();

        var app = builder.Build();

        app.UseExceptionHandler();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.MapGet("/stock-prices", async (NpgsqlDataSource dataSource) =>
        {
            using var connection = await dataSource.OpenConnectionAsync();
            var stockPrices = await connection.QueryAsync<StockPrice>("SELECT * FROM stock_prices");
            return Results.Ok(stockPrices);
        }).RequireAuthorization();

        app.MapDefaultEndpoints();

		app.Run();
    }
}
