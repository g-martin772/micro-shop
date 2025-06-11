var builder = DistributedApplication.CreateBuilder(args);

var dbUser = builder.AddParameter("db-user", secret: true);
var dbPassword = builder.AddParameter("db-pw", secret: true);

var dbContainer = builder.AddPostgres("postgres", dbUser, dbPassword)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    .WithPgAdmin();

var identityDb = dbContainer.AddDatabase("IdentityDb");
var catalogDb = dbContainer.AddDatabase("CatalogDb");

var storage = builder.AddAzureStorage("Storage")
    .RunAsEmulator(c => c.WithLifetime(ContainerLifetime.Persistent));

var objectStore = storage.AddBlobs("ObjectStore");

var redis = builder.AddAzureRedis("Redis")
    .RunAsContainer(c => c.WithLifetime(ContainerLifetime.Persistent));

var authService = builder.AddProject<Projects.AuthService_API>("AuthService")
    .WithReference(identityDb)
    .WaitFor(identityDb);

var catalogService = builder.AddProject<Projects.CatalogService_API>("CatalogService")
    .WithReference(catalogDb)
    .WithReference(authService)
    .WithReference(objectStore)
    .WaitFor(storage)
    .WaitFor(dbContainer);

var shopUi = builder.AddProject<Projects.Shop_UI>("ShopUI")
    .WithReference(authService)
    .WaitFor(authService);

var sellerUi = builder.AddProject<Projects.Seller_UI>("SellerUI")
    .WithReference(authService)
    .WaitFor(authService);

builder.Build().Run();