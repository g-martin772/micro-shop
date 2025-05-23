﻿namespace AuthService.API.Configuration;

public class Config
{
    public static IEnumerable<ApiResource> GetApis()
    {
        return new List<ApiResource>
        {
            new ApiResource("orders", "Orders Service") { Scopes = { "orders" } },
            new ApiResource("basket", "Basket Service") { Scopes = { "Basket" } },
        };
    }

    // ApiScope is used to protect the API 
    // The effect is the same as that of API resources in IdentityServer 3.x
    public static IEnumerable<ApiScope> GetApiScopes()
    {
        return new List<ApiScope>
        {
            new ApiScope("orders", "Orders Service"),
            new ApiScope("basket", "Basket Service"),
        };
    }

    // Identity resources are data like user ID, name, or email address of a user
    // see: http://docs.identityserver.io/en/release/configuration/resources.html
    public static IEnumerable<IdentityResource> GetResources()
    {
        return new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResource("Seller", new List<string> { "seller", "Seller" }),
        };
    }

    // client want to access resources (aka scopes)
    public static IEnumerable<Client> GetClients()
    {
        return new List<Client>
        {
            new Client()
            {
                ClientId = "shop",
                ClientName = "Shop UI",
                ClientSecrets = new List<Secret>
                {
                    new Secret("secret".Sha256())
                },
                ClientUri = "https://localhost:7119",
                AllowedGrantTypes = GrantTypes.Code,
                AllowAccessTokensViaBrowser = false,
                RequireConsent = false,
                AllowOfflineAccess = true,
                AlwaysIncludeUserClaimsInIdToken = true,
                RequirePkce = false,
                RedirectUris = new List<string>
                {
                    "https://localhost:7119/signin-oidc"
                },
                PostLogoutRedirectUris = new List<string>
                {
                    "https://localhost:7119/signout-callback-oidc"
                },
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.OfflineAccess,
                    "orders",
                    "basket"
                },
                AccessTokenLifetime = 60 * 60 * 2, // 2 hours
                IdentityTokenLifetime = 60 * 60 * 2 // 2 hours
            },
            new Client()
            {
                ClientId = "seller",
                ClientName = "Seller UI",
                ClientSecrets = new List<Secret>
                {
                    new Secret("secret".Sha256())
                },
                ClientUri = "https://localhost:7272",
                AllowedGrantTypes = GrantTypes.Code,
                AllowAccessTokensViaBrowser = false,
                RequireConsent = false,
                AllowOfflineAccess = true,
                AlwaysIncludeUserClaimsInIdToken = true,
                RequirePkce = false,
                RedirectUris = new List<string>
                {
                    "https://localhost:7272/signin-oidc"
                },
                PostLogoutRedirectUris = new List<string>
                {
                    "https://localhost:7272/signout-callback-oidc"
                },
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.OfflineAccess,
                    "Seller",
                    "orders",
                    "basket"
                },
                AccessTokenLifetime = 60 * 60 * 2, // 2 hours
                IdentityTokenLifetime = 60 * 60 * 2 // 2 hours
            }
        };
    }
}