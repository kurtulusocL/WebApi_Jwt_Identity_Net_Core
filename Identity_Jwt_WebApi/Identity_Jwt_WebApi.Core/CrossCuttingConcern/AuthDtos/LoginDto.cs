using Identity_Jwt_WebApi.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Identity_Jwt_WebApi.Core.CrossCuttingConcern.AuthDtos
{
    public class LoginDto : IDto
    {
        [Required]
        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 10)]
        public string Password { get; set; }
    }
}
