using Identity_Jwt_WebApi.Core.CrossCuttingConcern.AuthDtos;
using Identity_Jwt_WebApi.Core.DataAccess.EntityFramework;
using Identity_Jwt_WebApi.DataAccess.Abstract;
using Identity_Jwt_WebApi.DataAccess.Concrete.EntityFramework.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity_Jwt_WebApi.DataAccess.Concrete.EntityFramework
{
    public class UserDal : EntityRepositoryBase<UserManagerResponseDto, ApplicationDbContext>, IUserDal
    {
        public List<UserManagerResponseDto> GetAllUser()
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                return context.Set<UserManagerResponseDto>().ToList();
            }
        }
    }
}
