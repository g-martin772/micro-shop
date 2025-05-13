using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var dbUser = builder.AddParameter("db-user", secret: true);
var dbPassword = builder.AddParameter("db-pw", secret: true);

var dbContainer = builder.AddPostgres("postgres", dbUser, dbPassword)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

var identityDb = dbContainer.AddDatabase("IdentityDb");


var authService = builder.AddProject<Projects.AuthService_API>("AuthService")
    .WithReference(identityDb);


builder.Build().Run();