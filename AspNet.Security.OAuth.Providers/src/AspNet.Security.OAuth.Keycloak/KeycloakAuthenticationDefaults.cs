/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers
 * for more information concerning the license and the contributors participating to this project.
 */

namespace AspNet.Security.OAuth.Keycloak;

/// <summary>
/// Default values used by the Keycloak authentication provider.
/// </summary>
public static class KeycloakAuthenticationDefaults
{
    /// <summary>
    /// Default value for <see cref="AuthenticationScheme.Name"/>.
    /// </summary>
    public const string AuthenticationScheme = "Keycloak";

    /// <summary>
    /// Default value for <see cref="AuthenticationScheme.DisplayName"/>.
    /// </summary>
    public static readonly string DisplayName = "Keycloak";

    /// <summary>
    /// Default value for <see cref="AuthenticationSchemeOptions.ClaimsIssuer"/>.
    /// </summary>
    public static readonly string Issuer = "Keycloak";

    /// <summary>
    /// Default value for <see cref="RemoteAuthenticationOptions.CallbackPath"/>.
    /// </summary>
    public static readonly string CallbackPath = "/signin-keycloak";

    /// <summary>
    /// Default value for <see cref="OAuthOptions.AuthorizationEndpoint"/>.
    /// </summary>
    public static readonly string AuthorizationEndpoint = "http://localhost:8080/realms/invinsense/protocol/openid-connect/auth";

    /// <summary>
    /// Default value for <see cref="OAuthOptions.TokenEndpoint"/>.
    /// </summary>
    public static readonly string TokenEndpoint = "http://localhost:8080/realms/invinsense/protocol/openid-connect/token";

    /// <summary>
    /// Default value for <see cref="OAuthOptions.UserInformationEndpoint"/>.
    /// </summary>
    public static readonly string UserInformationEndpoint = "http://localhost:8080/realms/invinsense/protocol/openid-connect/userinfo";

    public static readonly string EndSessionEndpoint = "http://localhost:8080/realms/invinsense/protocol/openid-connect/logout?post_logout_redirect_uri='https://localhost:44318/*'&id_token='eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJ2RzluZFU1VFg3YXNVRmg2NEw1Q2EtTGltWFB1WmdsdDJRSElodDhKemcwIn0.eyJleHAiOjE2NzUxNDkwNDMsImlhdCI6MTY3NTE0ODc0MywiYXV0aF90aW1lIjowLCJqdGkiOiJlMDBmYmJjYy05MmI5LTQ3MWYtOGZmOC00OGY0ZjQ4MzJiNTEiLCJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjgwODAvcmVhbG1zL2ludmluc2Vuc2UiLCJhdWQiOiJ0ZXN0LWNsaWVudCIsInN1YiI6IjIzNzAyOWFjLTE5MmYtNDY3OC1hZjViLWM1NTJmM2ZiY2VhOSIsInR5cCI6IklEIiwiYXpwIjoidGVzdC1jbGllbnQiLCJzZXNzaW9uX3N0YXRlIjoiNzkxZDgwZTItMTA3My00Y2ZkLTk5ZDEtYTYyOGY3ODNhM2IyIiwiYXRfaGFzaCI6InBRazdPYzZ4c2hGS3AyczM2NUI4M2ciLCJhY3IiOiIxIiwic2lkIjoiNzkxZDgwZTItMTA3My00Y2ZkLTk5ZDEtYTYyOGY3ODNhM2IyIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJuYW1lIjoiWWFzaCBCaGF0dCIsInByZWZlcnJlZF91c2VybmFtZSI6Inlhc2giLCJnaXZlbl9uYW1lIjoiWWFzaCIsImZhbWlseV9uYW1lIjoiQmhhdHQiLCJlbWFpbCI6InRlc3QxMjNAdGVzdC5jb20ifQ.GztnJUw_bKaFL1zbxQouy4V-gJ5QG4RwHQ8u1rOdvKH3laDffAmXgFuHRoyq00WazXYQ1nOSTrkcvSkIktdeCMXIHIRdpfj4jR6yx4ykKsIat7zbH6DPZSQ5J4xfZyCvRroTbD4o4Mau3Qn045_lNOskP9dVsSZbtbANeclYFq9N_LMApSnUajNpzGaE87BvKE-l4ugkmcxnSgjZfvt0_yyttvEX_FXGeCiyTgL_DUNWc8TptcplbT5YmcDJm3nSHNY1UhYGGwV909DYJs1dvCNAgUXDCY06OfxFdLRhbNRv3yQW428DOm-9QFxJRMTP3Ascq5j64nMzR3w-s1_Mng'";
}
