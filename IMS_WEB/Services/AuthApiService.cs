using DTOs.Auth;
using IMS_WEB.Services.Interface;

namespace IMS_WEB.Services
{
    public class AuthApiService : IAuthApiServices
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthApiService> _logger;

        public AuthApiService(HttpClient httpClient, ILogger<AuthApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        public async Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("auth/login", request, ct);
                if (response.IsSuccessStatusCode)
                {
                    var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken: ct);
                    return authResponse;
                }
                else
                {
                    _logger.LogError("Login failed with status code: {StatusCode}", response.StatusCode);
                    return null;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to log in.");
                return null;
            }
        }
    }
}
