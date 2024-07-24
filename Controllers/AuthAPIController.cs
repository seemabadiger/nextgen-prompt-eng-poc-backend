using HxStudioAuthService.IService;
using HxStudioAuthService.Models.Dto;
using HxStudioAuthService.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace HxStudioAuthService.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        protected ResponseDto _responseDto;
        private readonly ILogger<AuthAPIController> _logger;
        public AuthAPIController(IAuthService authService, ILogger<AuthAPIController> logger)
        {
            _authService = authService;
            _responseDto = new ResponseDto();
            _logger = logger;
        
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {
            try
            {
                // Call the authentication service to register the user
                var errorMessage = await _authService.Register(model);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    return BadRequest(new ResponseDto { IsSuccess = false, Message = errorMessage });
                }
                // If registration is successful, return an OK response with the response DTO
                return Ok(_responseDto);
            }
            catch(Exception ex)
            {
                // Log the exception and return a 500 Internal Server Error response
                _logger.LogError(ex, "Error occurred while processing registration request");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");

            }
           
        }
        /// <summary>
        /// Logs a user into the system.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            try
            { 
                // Call the authentication service to authenticate the user
                var loginResponse = await _authService.Login(model);
                if (loginResponse.User == null)
                {
                    return BadRequest(new ResponseDto { IsSuccess = false, Message = "Username or Password is incorrect" });
                }
                _responseDto.Result = loginResponse;
                // If login is successful, set the result in the response DTO and return an OK response
                return Ok(_responseDto);
            }
            catch (Exception ex)
            {
                // Log the exception and return a 500 Internal Server Error response
                _logger.LogError(ex, "Error occurred while processing login request");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");

            }
            
        }
        /// <summary>
        /// Assigns a role to a user.
        /// </summary>
        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto model)
        {
            try
            {
                var assignRoleSuccessful = await _authService.AssignRole(model.Email, model.Role.ToUpper());
                if (!assignRoleSuccessful)
                {
                    return BadRequest(new ResponseDto { IsSuccess = false, Message = "Error encountered" });
                }
                return Ok(new ResponseDto { IsSuccess = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing role assignment request");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Changes the password for a user.
        /// </summary>
        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            try
            {
                var errorMessage = await _authService.ChangePassword(model);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    return BadRequest(new ResponseDto { IsSuccess = false, Message = errorMessage });
                }
                return Ok(new ResponseDto { IsSuccess = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing change password request");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
            }
        }
    }
}
