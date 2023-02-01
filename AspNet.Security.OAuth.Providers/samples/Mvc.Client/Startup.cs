/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers
 * for more information concerning the license and the contributors participating to this project.
 */

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Logging;

namespace Mvc.Client;

public class Startup
{
    public Startup(IConfiguration configuration, IHostEnvironment hostingEnvironment)
    {
        Configuration = configuration;
        HostingEnvironment = hostingEnvironment;
    }

    public IConfiguration Configuration { get; }

    private IHostEnvironment HostingEnvironment { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRouting();

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie(options =>
        {
            options.LoginPath = "/signin";
            options.LogoutPath = "/signout";
        })
        .AddGoogle(options =>
        {
            options.ClientId = Configuration["Google:ClientId"] ?? string.Empty;
            options.ClientSecret = Configuration["Google:ClientSecret"] ?? string.Empty;
        })
        .AddGitHub(options =>
        {
            options.ClientId = Configuration["GitHub:ClientId"] ?? string.Empty;
            options.ClientSecret = Configuration["GitHub:ClientSecret"] ?? string.Empty;
            options.EnterpriseDomain = Configuration["GitHub:EnterpriseDomain"] ?? string.Empty;
            options.Scope.Add("user:email");
        })
        .AddKeycloak(options =>
        {
            options.ClientId = "test-client";
            options.ClientSecret = "6LHT0sjBsgrDzQGhii4Vnbxa5XSZzOn7";
            options.AuthorizationEndpoint = "http://localhost:8080/realms/invinsense/protocol/openid-connect/auth";
            options.UserInformationEndpoint = "http://localhost:8080/realms/invinsense/protocol/openid-connect/userinfo";
        });

        services.AddMvc();
    }

    public void Configure(IApplicationBuilder app)
    {
        if (HostingEnvironment.IsDevelopment())
        {
            IdentityModelEventSource.ShowPII = true;
        }

        // Required to serve files with no extension in the .well-known folder
        var options = new StaticFileOptions()
        {
            ServeUnknownFileTypes = true,
        };

        app.UseStaticFiles(options);

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
        });
    }
}
