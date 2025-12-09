using Microsoft.AspNetCore.Mvc;
using Product.BLL.DTO;
using Product.BLL.Services;


namespace Product.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
            await _authService.SendOtpAsync(otpRequest.Email);
            return Ok();
        }
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, [FromQuery] string otp)
        {
            var user = await _authService.RegisterAsync(request, otp);
            return Ok(new { user.Id, user.Name, user.Email });

        }
    }
}
