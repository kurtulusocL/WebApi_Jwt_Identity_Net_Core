using Identity_Jwt_WebApi.Core.CrossCuttingConcern.AuthDtos;
using Identity_Jwt_WebApi.Core.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Identity_Jwt_WebApi.DataAccess.Abstract
{
    public interface IUserDal : IEntityRepository<UserManagerResponseDto>
    {
        List<UserManagerResponseDto> GetAllUser();
    }
}
