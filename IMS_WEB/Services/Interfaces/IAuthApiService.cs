using DTOs.Auth;

namespace IMS_WEB.Services.Interface
{
    public interface IAuthApiServices
    {
        Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default);
    }
}
