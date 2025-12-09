using Product.BLL.DTO;
using Product.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Product.BLL.Services
{
    public interface IAuthService
    {
        Task SendOtpAsync(string email);
        Task<User> RegisterAsync(RegisterRequest dto, string otp);
        
    }
}
