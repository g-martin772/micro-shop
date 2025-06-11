using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var identityUrl = "http://localhost:5001";

builder.Services.AddAuthentication()
    .AddJwtBearer(o =>
    {
        o.Authority = identityUrl;
        o.RequireHttpsMetadata = false;
        o.Audience = "catalog";
        o.TokenValidationParameters.ValidIssuers = [identityUrl];
        o.TokenValidationParameters.ValidateIssuer = false;
        o.TokenValidationParameters.ValidateAudience = false;
        o.TokenValidationParameters.ValidateIssuerSigningKey = false;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/api/test", async (HttpContext httpContext) =>
    {
        var user = httpContext.User;

        if (user.Identity is { IsAuthenticated: false })
            return Results.Unauthorized();
        
        var accessToken = await httpContext.GetTokenAsync("access_token");

        var claims = user.Claims.Select(c => new { Type = c.Type, Value = c.Value }).ToList();

        return Results.Ok(new
        {
            Message = "User is authenticated.",
            UserName = user.Identity?.Name,
            IsAuthenticated = true,
            AccessToken = accessToken,
            Claims = claims
        });
    })
    .WithName("GetAuthStatus")
    .RequireAuthorization();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultEndpoints();

app.Run();