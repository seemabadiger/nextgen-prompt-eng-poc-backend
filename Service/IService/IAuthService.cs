using HxStudioAuthService.Models.Dto;

namespace HxStudioAuthService.IService
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationRequestDto registrationRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        Task<bool> AssignRole(string email, string roleName);
        Task<string> ChangePassword(ChangePasswordDto changePasswordDto);

    }
}
