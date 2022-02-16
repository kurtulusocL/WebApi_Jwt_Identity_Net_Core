using Identity_Jwt_WebApi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity_Jwt_WebApi.Core.CrossCuttingConcern.AuthDtos
{
    public class UserManagerResponseDto : IDto
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public DateTime? ExpireDate { get; set; }
    }
}
