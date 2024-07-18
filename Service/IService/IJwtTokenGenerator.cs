using HxStudioAuthService.Models;

namespace HxStudioAuthService.IService
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ApplicationUser applicationUser);
    }
}
