using Microsoft.AspNetCore.Authentication;
using ShoppeeEcommerce.MVC.Customer.Utils;
using ShoppeeEcommerce.SharedViewModels.Models.Authentication.Refresh;
using System.Net;
using System.Net.Http.Headers;

namespace ShoppeeEcommerce.MVC.Customer.API
{
    public class ApiAuthHandler(
        IHttpContextAccessor contextAccessor,
        IAuthApi authApi) : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var httpContext = contextAccessor.HttpContext!;
            var user = httpContext.User;

            // If not logged in, send request as anonymous user
            if (user.Identity?.IsAuthenticated != true)
            {
                return await base.SendAsync(request, cancellationToken);
            }

            // If authenticated, try add access token to header
            var accessToken = user.FindFirst(AuthCookieHelper.AccessTokenClaimName)?.Value;
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await base.SendAsync(request, cancellationToken);
            if (response.StatusCode != HttpStatusCode.Unauthorized)
                return response;

            // If response is 401, try rotate tokens with refresh token
            var refreshToken = user.FindFirst(AuthCookieHelper.RefreshTokenClaimName)?.Value;
            if (refreshToken == null)
            {
                // If for some reason, cannot fetch the refresh token in cookie
                // Force user to re-login
                await ForceLogout(httpContext);
                return response;
            }
            var refreshResponse = await authApi.Refresh(new RefreshRequest(refreshToken));

            // If refresh endpoint returns 401/400, refresh token dead, force user to re-login
            if (refreshResponse == null)
            {
                
                await ForceLogout(httpContext);
                return response;
            }
            // If rotate request succeeded, set auth tokens, add new access token to original request header
            // and send request
            await AuthCookieHelper.SetAuthCookie(
                httpContext,
                refreshResponse.AccessToken,
                refreshResponse.RefreshToken,
                rememberMe: true);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshResponse.AccessToken);
            return await base.SendAsync(request, cancellationToken);
        }

        private async Task ForceLogout(HttpContext context)
        {
            // Delete cookie
            await context.SignOutAsync();

            // Mark request so MVC knows to redirect
            context.Items["ForceLogin"] = true;
        }
    }
}
