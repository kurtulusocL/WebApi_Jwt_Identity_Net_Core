using Identity_Jwt_WebApi.Business.Abstract;
using Identity_Jwt_WebApi.Core.CrossCuttingConcern.AuthDtos;
using Identity_Jwt_WebApi.DataAccess.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Identity_Jwt_WebApi.Business.Concrete
{
    public class UserManager : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private IConfiguration _configuration;
        private readonly IMailService _mailService;
        private readonly IUserDal _userDal;
        public UserManager(UserManager<IdentityUser> userManager, IConfiguration configuration, IMailService mailService, IUserDal userDal)
        {
            _userManager = userManager;
            _configuration = configuration;
            _mailService = mailService;
            _userDal = userDal;
        }

        public async Task<UserManagerResponseDto> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new UserManagerResponseDto
                {
                    IsSuccess = false,
                    Message = "User not found"
                };
            var decodedToken = WebEncoders.Base64UrlDecode(token);
            var normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManager.ConfirmEmailAsync(user, normalToken);
            if (result.Succeeded)
            {
                return new UserManagerResponseDto
                {
                    Message = "User email confirmed",
                    IsSuccess = true
                };
            }
            return new UserManagerResponseDto
            {
                IsSuccess = false,
                Message = "User email not confirm",
                Errors = result.Errors.Select(i => i.Description)
            };
        }

        public async Task<UserManagerResponseDto> UserLoginAsync(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new UserManagerResponseDto
                {
                    Message = "There is no user with that email address",
                    IsSuccess = false,
                };
            }
            var result = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!result)
                return new UserManagerResponseDto
                {
                    Message = "Invalid password",
                    IsSuccess = false,
                };
            var claims = new[]
            {
                new Claim("Email", model.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["AuthSettings:Issuer"],
                audience: _configuration["AuthSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );
            string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);
            return new UserManagerResponseDto
            {
                Message = tokenAsString,
                IsSuccess = true,
                ExpireDate = token.ValidTo
            };

        }

        public async Task<UserManagerResponseDto> UserRegisterAsync(RegisterDto model)
        {
            if (model == null)
                throw new NullReferenceException("Register model is null");

            if (model.Password != model.ConfirmPassword)
                return new UserManagerResponseDto
                {
                    Message = "Passwords are not same",
                    IsSuccess = false,
                };
            var identityUser = new IdentityUser
            {
                Email = model.Email,
                UserName = model.Email,
            };
            var result = await _userManager.CreateAsync(identityUser, model.Password);
            if (result.Succeeded)
            {
                var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
                var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                string url = $"{_configuration["AppUrl"]}/api/accounts/confirmemail?userId={identityUser.Id}&token={validEmailToken}";
                await _mailService.SendEmailAsync(identityUser.Email, "Confirm your email", "<html>Welcome to your account</html>" + $"<p>Please confirm to your email by <a href='{url}'</a>Click Here</p>");

                return new UserManagerResponseDto
                {
                    Message = "User created successfully",
                    IsSuccess = true,
                };
            }
            return new UserManagerResponseDto
            {
                Message = "User not created",
                IsSuccess = false,
                Errors = result.Errors.Select(i => i.Description)
            };
        }

        public async Task<UserManagerResponseDto> ForgetPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new UserManagerResponseDto
                {
                    IsSuccess = false,
                    Message = "No user with this email address"
                };
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Encoding.UTF8.GetBytes(token);
            var validToken = WebEncoders.Base64UrlEncode(encodedToken);

            string url = $"{_configuration["AppUrl"]}/ResetPassword?email={email}&token={validToken}";
            await _mailService.SendEmailAsync(email, "Reset Password", "<html>Fallow to this link for reset to your password</html>" + $"<p>You can click here <a href='{url}'</a>Click Here</p>");
            return new UserManagerResponseDto
            {
                Message = "Url for reset your password has been sent to your email address",
                IsSuccess = true,
            };
        }

        public async Task<UserManagerResponseDto> ResetPasswordAsync(ResertPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new UserManagerResponseDto
                {
                    IsSuccess = false,
                    Message = "No user with this email address"
                };
            }
            if (model.NewPassword != model.ConfirmPassword)
            {
                return new UserManagerResponseDto
                {
                    IsSuccess = false,
                    Message = "Passwords are not same."
                };
            }

            var decodedToken = WebEncoders.Base64UrlDecode(model.Token);
            var normalToken = Encoding.UTF8.GetString(decodedToken);
            var result = await _userManager.ResetPasswordAsync(user, normalToken, model.NewPassword);
            if (result.Succeeded)
            {
                return new UserManagerResponseDto
                {
                    IsSuccess = true,
                    Message = "Your password has been changed"
                };
            }
            return new UserManagerResponseDto
            {
                IsSuccess = false,
                Message = "Something went wrong",
                Errors = result.Errors.Select(i => i.Description)
            };
        }

        public List<UserManagerResponseDto> GetAllUsers()
        {
            return _userDal.GetAllUser();
        }
    }
}
