var builder = DistributedApplication.CreateBuilder(args);

var dbUser = builder.AddParameter("db-user", secret: true);
var dbPassword = builder.AddParameter("db-pw", secret: true);

var dbContainer = builder.AddPostgres("postgres", dbUser, dbPassword)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    .WithPgAdmin();

var identityDb = dbContainer.AddDatabase("IdentityDb");

var authService = builder.AddProject<Projects.AuthService_API>("AuthService")
    .WithReference(identityDb)
    .WaitFor(identityDb);

var shopUi = builder.AddProject<Projects.Shop_UI>("ShopUI")
    .WithReference(authService)
    .WaitFor(authService);

var sellerUi = builder.AddProject<Projects.Seller_UI>("SellerUI")
    .WithReference(authService)
    .WaitFor(authService);

builder.Build().Run();