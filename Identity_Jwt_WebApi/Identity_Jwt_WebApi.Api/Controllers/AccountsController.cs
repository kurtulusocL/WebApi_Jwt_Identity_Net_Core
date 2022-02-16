using Identity_Jwt_WebApi.Business.Abstract;
using Identity_Jwt_WebApi.Core.CrossCuttingConcern.AuthDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity_Jwt_WebApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AccountsController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMailService _mailService;
        private readonly IConfiguration _configuration;
        public AccountsController(IUserService userService, IMailService mailService, IConfiguration configuration)
        {
            _userService = userService;
            _mailService = mailService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.UserRegisterAsync(model);
                if (result.IsSuccess)
                {
                    await _mailService.SendEmailAsync(model.Email, "Welcome", "Your new account is ready!");
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Is was not valid");
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginDto model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.UserLoginAsync(model);
                if (result.IsSuccess)
                {
                    await _mailService.SendEmailAsync(model.Email, "Notice", "A New Login to Your Account" + DateTime.Now.ToLocalTime());
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Is was not valid");
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            {
                return NotFound();
            }
            var result = await _userService.ConfirmEmailAsync(userId, token);
            if (result.IsSuccess)
            {
                return Redirect($"{_configuration["AppUrl"]}/ConfirmEmail.html");
            }
            return BadRequest(result);
        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return NotFound();
            }
            var result = await _userService.ForgetPasswordAsync(email);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResertPasswordDto model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.ResetPasswordAsync(model);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Someting went wrong");
        }

        [HttpGet("getalluser")]
        public IActionResult GetAllUser()
        {
            var result = _userService.GetAllUsers();
            if (result != null)
            {
                return Ok(result);
            }
            return NotFound(result);
        }
    }
}
