using Microsoft.AspNetCore.Mvc;
using Product.BLL.DTO;
using Product.BLL.Services;
using System;


namespace Product.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class SignUpController : ControllerBase
    {
        private readonly IAuthService _authService;
        public SignUpController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOTP([FromBody] SendOtpRequest otpRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _authService.SendOtpAsync(otpRequest.Email);
                return Ok(new { message = "OTP sent" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, [FromQuery] string otp)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (string.IsNullOrWhiteSpace(otp))
                return BadRequest(new { error = "OTP is required" });

            try
            {
                var user = await _authService.RegisterAsync(request, otp);
                return Ok(new { user.Id, user.Name, user.Email });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }

        }
    }
}
