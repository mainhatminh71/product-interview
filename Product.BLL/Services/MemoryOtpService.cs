using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Product.BLL.Services
{
    public class MemoryOtpService : IOtpService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<MemoryOtpService> _logger;

        public MemoryOtpService(IMemoryCache cache, ILogger<MemoryOtpService> logger)
        {
            _cache = cache;
            _logger = logger;
        }
        public string GenerateAndSaveOtp(string email)
        {
            var otp = new Random().Next(100000, 999999).ToString();
            _cache.Set(email, otp, TimeSpan.FromMinutes(5));
            _logger.LogInformation($"Generated OTP {otp} for email {email}");
            return otp;
        }
        public bool ValidateOtp(string email, string otp)
        {
            if (_cache.TryGetValue(email, out string savedOtp))
            {
                bool isValid = savedOtp == otp;
                _logger.LogInformation($"OTP validation for email {email}: {isValid}");
                return isValid;
            }
            _logger.LogWarning($"No OTP found for email {email}");
            return false;
        }
        public void ClearOtp(string email)
        {
            _cache.Remove(email);
            _logger.LogInformation($"Cleared OTP for email {email}");
        }
        public Task SendEmailOtpAsync(string email, string otp)
        {
            // Simulate sending email
            _logger.LogInformation($"Sending OTP {otp} to email {email}");
            return Task.CompletedTask;
        }
    }
}
