using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Identity_Jwt_WebApi.Business.Abstract
{
    public interface IMailService
    {
        Task SendEmailAsync(string toEmail, string subject, string content);
    }
}
