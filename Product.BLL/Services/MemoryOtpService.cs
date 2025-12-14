using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using SendGrid; 
using SendGrid.Helpers.Mail;

namespace Product.BLL.Services
{
    public class MemoryOtpService : IOtpService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<MemoryOtpService> _logger;
        private readonly IConfiguration _config;
        private readonly SendGridClient _sendGridClient;

        public MemoryOtpService(IMemoryCache cache, ILogger<MemoryOtpService> logger, IConfiguration config, SendGridClient
            sendGridClient)
        {
            _cache = cache;
            _logger = logger;
            _config = config;
            _sendGridClient = sendGridClient;
        }
        public string GenerateAndSaveOtp(string email)
        {
            // Use a cryptographically secure generator to avoid predictable OTPs
            var buffer = new byte[4];
            RandomNumberGenerator.Fill(buffer);
            var numeric = BitConverter.ToUInt32(buffer, 0) % 900000 + 100000; // 6 digits 100000-999999
            var otp = numeric.ToString();

            _cache.Set(email, otp, TimeSpan.FromMinutes(5));
            _logger.LogInformation($"Generated OTP {otp} for email {email}");
            return otp;
        }
        public bool ValidateOtp(string email, string otp)
        {
            if (_cache.TryGetValue(email, out string? savedOtp))
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
        public async Task SendEmailOtpAsync(string email, string otp) 
        {
            try
            {
                var fromEmail = Environment.GetEnvironmentVariable("SendGrid__FromEmail")
                    ?? _config["SendGrid:FromEmail"];
                var fromName = Environment.GetEnvironmentVariable("SendGrid__FromName")
                    ?? _config["SendGrid:FromName"];
                
                if (string.IsNullOrWhiteSpace(fromEmail))
                {
                    throw new InvalidOperationException("SendGrid:FromEmail is not configured. Please set SendGrid__FromEmail environment variable or configure SendGrid:FromEmail in appsettings.json");
                }

                var from = new EmailAddress(
                    fromEmail,
                    fromName);

                var to = new EmailAddress(email);
                var subject = "Mã OTP đăng ký tài khoản";
                var plainTextContent = $"Mã OTP của bạn là: {otp}\nHiệu lực trong 5 phút.";
                var htmlContent = $@"
            <div style='font-family: Arial, sans-serif; text-align: center; padding: 30px;'>
                <h2 style='color: #007bff;'>Mã OTP của bạn</h2>
                <h1 style='font-size: 48px; color: #28a745; letter-spacing: 5px;'>{otp}</h1>
                <p>Hiệu lực trong <strong>5 phút</strong></p>
                <p>Nếu bạn không yêu cầu mã này, vui lòng bỏ qua email.</p>
            </div>";

                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

                var response = await _sendGridClient.SendEmailAsync(msg); 

                if (response.IsSuccessStatusCode) 
                {
                    _logger.LogInformation("Gửi OTP thành công tới {Email}", email);
                }
                else
                {
                    var errorBody = await response.Body.ReadAsStringAsync();
                    _logger.LogError("Gửi OTP thất bại tới {Email}. Status: {StatusCode}. Error: {Error}",
                        email, response.StatusCode, errorBody);
                    throw new InvalidOperationException($"Failed to send OTP email. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi OTP tới {Email}", email);
                throw;
            }
        }
    }
}
