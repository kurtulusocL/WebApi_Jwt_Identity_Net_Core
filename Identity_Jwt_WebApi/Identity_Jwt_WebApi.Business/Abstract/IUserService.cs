using Identity_Jwt_WebApi.Core.CrossCuttingConcern.AuthDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Identity_Jwt_WebApi.Business.Abstract
{
    public interface IUserService
    {
        Task<UserManagerResponseDto> UserRegisterAsync(RegisterDto model);
        Task<UserManagerResponseDto> UserLoginAsync(LoginDto model);
        Task<UserManagerResponseDto> ConfirmEmailAsync(string userId, string token);
        Task<UserManagerResponseDto> ForgetPasswordAsync(string email);
        Task<UserManagerResponseDto> ResetPasswordAsync(ResertPasswordDto model);
        List<UserManagerResponseDto> GetAllUsers();
    }
}
