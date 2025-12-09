using System;
using System.Collections.Generic;
using System.Text;

namespace Product.BLL.Services
{
    public interface IOtpService
    {
        string GenerateAndSaveOtp(string email);
        bool ValidateOtp(string email, string otp);
        void ClearOtp(string email);
        Task SendEmailOtpAsync(string email, string otp);
    }
}
