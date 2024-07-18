using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;

namespace HxStudioAuthService.Models
{
    [ExcludeFromCodeCoverage]
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
