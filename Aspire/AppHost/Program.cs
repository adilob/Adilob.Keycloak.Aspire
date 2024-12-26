var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder
	.AddPostgres("postgres")
	.WithDataVolume();

var stocksDb = postgres.AddDatabase("stocks");

var keycloak = builder.AddKeycloak("keycloak", 8080)
	.WithDataVolume()
	.WithExternalHttpEndpoints();

builder.AddProject<Projects.Web_Api>("web-api")
	.WithExternalHttpEndpoints()
	.WithReference(stocksDb)
	.WithReference(keycloak)
	.WaitFor(keycloak);

builder.Build().Run();
