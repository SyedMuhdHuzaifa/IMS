using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;

namespace IMS_WEB.Handlers
{
    public class AuthenticationHeaderHandler: DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationHeaderHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                // Read the JWT token securely stored inside the user's cookie
                var token = await httpContext.GetTokenAsync("access_token");
                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
