using Azure;
using HxStudioAuthService.Data;
using HxStudioAuthService.Models;
using HxStudioAuthService.Models.Dto;
using HxStudioAuthService.IService;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;

namespace HxStudioAuthService.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        public AuthService(AppDbContext db,UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager,IJwtTokenGenerator jwtTokenGenerator)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }
        /// <summary>
        /// Registers a new user with the provided registration details.
        /// </summary>
        /// <param name="registrationRequestDto">The registration details of the user.</param>
        /// <returns>A string indicating the result of the registration process.</returns>
        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        { 
            // Log the registration details for debugging purposes.
            Log.Information("Register user details" + JsonConvert.SerializeObject(registrationRequestDto));
            ApplicationUser user = new()
            {
                Name = registrationRequestDto.Name,
                UserName = registrationRequestDto.Email,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                PhoneNumber=registrationRequestDto.PhoneNumber
            };
            try 
            {
                //create the user with the provided password.
                var result = await _userManager.CreateAsync(user,registrationRequestDto.Password);
                if (result.Succeeded)
                {
                    var userReturn = _db.ApplicationUsers.FirstOrDefault(x => x.UserName == registrationRequestDto.Email);

                    // Assign role to the user
                    var roleResult = await _userManager.AddToRoleAsync(userReturn, registrationRequestDto.Role);

                    UserDto userDto = new()
                    {
                        Email = userReturn.Email,
                        PhoneNumber = userReturn.PhoneNumber,
                        ID = userReturn.Id,
                        Name = userReturn.Name                      
                    };

                    return "";
                    Log.Information("User details registered sucessfully");

                }
                else 
                {
                    // Return a generic error message in case of an exception.
                    return result.Errors.FirstOrDefault().Description;
                }
            
            
            
            }
            catch (Exception ex)
            {
               
                Log.Error(ex.Message);

            }
            return "Error";
        }
        /// <summary>
        /// Authenticates a user with the provided login details.
        /// </summary>
        /// <param name="loginRequestDto">The login details of the user.</param>
        /// <returns>A LoginResponseDto containing the user details and a token.</returns>
        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            try
            {
                // Fetch user details based on the provided username.
                var userDetails = _db.ApplicationUsers.FirstOrDefault(x => x.UserName.ToLower() == loginRequestDto.UserName.ToLower());
                // Validate password
                bool isValid = _userManager.CheckPasswordAsync(userDetails, loginRequestDto.Password).Result;
                if (userDetails == null || isValid == false)
                {
                    return new LoginResponseDto() { User = null, Token = "" };
                }
                // Generate a token for the authenticated user.
                var token = _jwtTokenGenerator.GenerateToken(userDetails);

                var roles = await _userManager.GetRolesAsync(userDetails);
                var role = roles.FirstOrDefault();

                UserDto userDto = new UserDto()
                {
                    ID = userDetails.Id,
                    Name = userDetails.Name,
                    Email = userDetails.Email,
                    PhoneNumber = userDetails.PhoneNumber,
                    Role = role

                };

                return new LoginResponseDto()
                {
                    User = userDto,
                    Token = token

                };
                Log.Information("User login sucessfully");
            }
            catch (Exception ex)
            {
                // Return a generic error message in case of an exception.
                Log.Error(ex.Message);
                return new LoginResponseDto()
                {
                    User = null,
                    Token = ""

                };

            }
           
        }
        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            if (user != null)
            {
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    // Create role if it does not exist
                    await _roleManager.CreateAsync(new IdentityRole
                    {
                        Name = roleName
                    });
                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        public async Task<string> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(changePasswordDto.Email);
                if (user == null)
                {
                    return "User not found";
                }

                var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
                if (result.Succeeded)
                {
                    return "";
                }
                return string.Join(", ", result.Errors.Select(e => e.Description));
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return "Error occurred while changing the password";
            }
        }


    }
}
