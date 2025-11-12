using AuthService.Shared.DTOs;

namespace authService.Domain.Interfaces
{
    public interface IAuthJwtService
    {
        Task<JwtDto> LoginAsync(LoginDto loginDto);
        Task<JwtDto> RegisterAsync(RegisterDto registerDto);
        Task ChangePasswordAsync(UpdateDto updateUserDto);
        Task DeleteAsync(DeleteDto deleteDto);

    }
}
