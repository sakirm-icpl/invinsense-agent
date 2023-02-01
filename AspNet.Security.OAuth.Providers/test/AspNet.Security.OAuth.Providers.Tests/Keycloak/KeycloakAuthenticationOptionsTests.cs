/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers
 * for more information concerning the license and the contributors participating to this project.
 */

namespace AspNet.Security.OAuth.Keycloak;

public static class KeycloakAuthenticationOptionsTests
{
    public static IEnumerable<object[]> AccessTypes => new object[][]
    {
            new object[] { KeycloakAuthenticationAccessType.BearerOnly },
            new object[] { KeycloakAuthenticationAccessType.Confidential },
            new object[] { KeycloakAuthenticationAccessType.Public },
    };

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public static void Validate_Does_Not_Throw_If_ClientSecret_Is_Not_Provided_For_Public_Access_Type(string clientSecret)
    {
        // Arrange
        var options = new KeycloakAuthenticationOptions()
        {
            AccessType = KeycloakAuthenticationAccessType.Public,
            ClientId = "test-client",
            ClientSecret = clientSecret,
        };

        // Act (no Assert)
        options.Validate();
    }

    [Theory]
    [InlineData(KeycloakAuthenticationAccessType.BearerOnly)]
    [InlineData(KeycloakAuthenticationAccessType.Confidential)]
    public static void Validate_Throws_If_ClientSecret_Is_Null(KeycloakAuthenticationAccessType accessType)
    {
        // Arrange
        var options = new KeycloakAuthenticationOptions()
        {
            AccessType = accessType,
            ClientId = "test-client",
            ClientSecret = null!,
        };

        // Act and Assert
        Assert.Throws<ArgumentException>("ClientSecret", () => options.Validate());
    }

    [Theory]
    [MemberData(nameof(AccessTypes))]
    public static void Validate_Throws_If_AuthorizationEndpoint_Is_Null(KeycloakAuthenticationAccessType accessType)
    {
        // Arrange
        var options = new KeycloakAuthenticationOptions()
        {
            AccessType = accessType,
            AuthorizationEndpoint = null!,
            ClientId = "test-client"
        };

        // Act and Assert
        Assert.Throws<ArgumentException>("AuthorizationEndpoint", () => options.Validate());
    }

    [Theory]
    [MemberData(nameof(AccessTypes))]
    public static void Validate_Throws_If_TokenEndpoint_Is_Null(KeycloakAuthenticationAccessType accessType)
    {
        // Arrange
        var options = new KeycloakAuthenticationOptions()
        {
            AccessType = accessType,
            ClientId = "test-client",
            ClientSecret = "6LHT0sjBsgrDzQGhii4Vnbxa5XSZzOn7",
            TokenEndpoint = null!,
        };

        // Act and Assert
        Assert.Throws<ArgumentException>("TokenEndpoint", () => options.Validate());
    }

    [Theory]
    [MemberData(nameof(AccessTypes))]
    public static void Validate_Throws_If_CallbackPath_Is_Null(KeycloakAuthenticationAccessType accessType)
    {
        // Arrange
        var options = new KeycloakAuthenticationOptions()
        {
            AccessType = accessType,
            CallbackPath = null,
            ClientId = "test-client",
            ClientSecret = "6LHT0sjBsgrDzQGhii4Vnbxa5XSZzOn7",
        };

        // Act and Assert
        Assert.Throws<ArgumentException>("CallbackPath", () => options.Validate());
    }
}
