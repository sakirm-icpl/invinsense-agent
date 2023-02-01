/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers
 * for more information concerning the license and the contributors participating to this project.
 */

namespace AspNet.Security.OAuth.Keycloak;

public class KeycloakTests : OAuthTests<KeycloakAuthenticationOptions>
{
    public KeycloakTests(ITestOutputHelper outputHelper)
    {
        OutputHelper = outputHelper;
    }

    public override string DefaultScheme => KeycloakAuthenticationDefaults.AuthenticationScheme;

    protected internal override void RegisterAuthentication(AuthenticationBuilder builder)
    {
        builder.AddKeycloak(options =>
        {
            ConfigureDefaults(builder, options);
            options.Domain = "localhost";
            options.Realm = "invinsense";
        });
    }

    [Theory]
    [InlineData(ClaimTypes.NameIdentifier, "237029ac-192f-4678-af5b-c552f3fbcea9")]
    [InlineData(ClaimTypes.Email, "test123@test.com")]
    [InlineData(ClaimTypes.GivenName, "Yash")]
    [InlineData(ClaimTypes.Role, "admin")]
    [InlineData(ClaimTypes.Name, "Yash Bhatt")]

    public async Task Can_Sign_In_Using_Keycloak_BaseAddress(string claimType, string claimValue)
    {
        // Arrange
        static void ConfigureServices(IServiceCollection services)
        {
            services.PostConfigureAll<KeycloakAuthenticationOptions>((options) =>
            {
                options.BaseAddress = new Uri("http://localhost:8080");
            });
        }

        using var server = CreateTestServer(ConfigureServices);

        // Act
        var claims = await AuthenticateUserAsync(server);

        // Assert
        AssertClaim(claims, claimType, claimValue);
    }

    [Theory]
    [InlineData(null, ClaimTypes.NameIdentifier, "237029ac-192f-4678-af5b-c552f3fbcea9")]
    [InlineData(null, ClaimTypes.Email, "test123@test.com")]
    [InlineData(null, ClaimTypes.GivenName, "Yash")]
    [InlineData(null, ClaimTypes.Role, "admin")]
    [InlineData(null, ClaimTypes.Name, "Yash Bhatt")]
   /* [InlineData("17.0", ClaimTypes.NameIdentifier, "237029ac-192f-4678-af5b-c552f3fbcea9")]
    [InlineData("17.0", ClaimTypes.Email, "test123@test.com")]
    [InlineData("17.0", ClaimTypes.GivenName, "Yash")]
    [InlineData("17.0", ClaimTypes.Role, "admin")]
    [InlineData("17.0", ClaimTypes.Name, "Yash Bhatt")]
    [InlineData("18.0", ClaimTypes.NameIdentifier, "237029ac-192f-4678-af5b-c552f3fbcea9")]
    [InlineData("18.0", ClaimTypes.Email, "test123@test.com")]
    [InlineData("18.0", ClaimTypes.GivenName, "Yash")]
    [InlineData("18.0", ClaimTypes.Role, "admin")]
    [InlineData("18.0", ClaimTypes.Name, "Yash Bhatt")]
    [InlineData("19.0", ClaimTypes.NameIdentifier, "237029ac-192f-4678-af5b-c552f3fbcea9")]
    [InlineData("19.0", ClaimTypes.Email, "test123@test.com")]
    [InlineData("19.0", ClaimTypes.GivenName, "Yash")]
    [InlineData("19.0", ClaimTypes.Role, "admin")]
    [InlineData("19.0", ClaimTypes.Name, "Yash Bhatt")]*/
    public async Task Can_Sign_In_Using_Keycloak_Domain(string? version, string claimType, string claimValue)
    {
        // Arrange
        void ConfigureServices(IServiceCollection services)
        {
            services.PostConfigureAll<KeycloakAuthenticationOptions>((options) =>
            {
                options.Domain = "localhost";

                if (version is not null)
                {
                    options.Version = Version.Parse(version);
                }
            });
        }

        using var server = CreateTestServer(ConfigureServices);

        // Act
        var claims = await AuthenticateUserAsync(server);

        // Assert
        AssertClaim(claims, claimType, claimValue);
    }

    [Theory]
    [InlineData(ClaimTypes.NameIdentifier, "237029ac-192f-4678-af5b-c552f3fbcea9")]
    [InlineData(ClaimTypes.Email, "test123@test.com")]
    [InlineData(ClaimTypes.GivenName, "Yash")]
    [InlineData(ClaimTypes.Role, "admin")]
    [InlineData(ClaimTypes.Name, "Yash Bhatt")]
    public async Task Can_Sign_In_Using_Keycloak_Public_AccessType(string claimType, string claimValue)
    {
        // Arrange
        static void ConfigureServices(IServiceCollection services)
        {
            services.PostConfigureAll<KeycloakAuthenticationOptions>((options) =>
            {
                options.AccessType = KeycloakAuthenticationAccessType.Public;
                options.ClientSecret = string.Empty;
                options.Domain = "localhost";
            });
        }

        using var server = CreateTestServer(ConfigureServices);

        // Act
        var claims = await AuthenticateUserAsync(server);

        // Assert
        AssertClaim(claims, claimType, claimValue);
    }
}
