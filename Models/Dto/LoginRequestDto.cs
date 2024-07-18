using System.ComponentModel.DataAnnotations;

namespace HxStudioAuthService.Models.Dto
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
