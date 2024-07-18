namespace HxStudioAuthService.Models.Dto
{
    public class LoginResponseDto
    {
        public UserDto User { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }

    }
}
