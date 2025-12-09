using Product.BLL.DTO;
using Product.DAL.Entities;
using Product.DAL.Repo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using BCrypt.Net;

namespace Product.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAccountRepo _accountRepo;
        private readonly IOtpService _otpService;
        public AuthService(IAccountRepo accountRepo, IOtpService otpService)
        {
            _accountRepo = accountRepo;
            _otpService = otpService;
        }   
        public async Task SendOtpAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));
            email = email.Trim().ToLowerInvariant();
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException("Invalid email format", nameof(email));
            var existingUser = await _accountRepo.GetByEmailAsync(email);
            if (existingUser != null)
                throw new InvalidOperationException("Email is already registered");
            string otpCode = _otpService.GenerateAndSaveOtp(email);
            await _otpService.SendEmailOtpAsync(email, otpCode);
        }
        public async Task<User> RegisterAsync(RegisterRequest dto, string otp)
        {
           if (dto == null)
                throw new ArgumentNullException(nameof(dto));
            if (dto.Password != dto.ConfirmPassword)
                throw new ArgumentException("Mật khẩu xác nhận không khớp");

            if (dto.Password.Length < 6)
                throw new ArgumentException("Mật khẩu phải ít nhất 6 ký tự");
            var email = dto.Email.Trim().ToLowerInvariant();
            if (!_otpService.ValidateOtp(email, otp))
                throw new ArgumentException("Mã OTP không hợp lệ hoặc đã hết hạn");
            var existingUser = await _accountRepo.GetByEmailAsync(email);
            if (existingUser != null)
                throw new InvalidOperationException("Email đã được đăng ký");
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var newUser = new User
            {
                Name = dto.Name.Trim(),
                Email = email,
                Password = hashedPassword,
                GoogleId = null
            };
            await _accountRepo.CreateUserAsync(newUser);
            _otpService.ClearOtp(email);
            return newUser;

        }
    }
}
