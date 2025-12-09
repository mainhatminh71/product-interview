using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Product.BLL.DTO
{
        public class RegisterRequest
        {
            [Required] public string Name { get; set; }
            [Required, EmailAddress] public string Email { get; set; }
            [Required, MinLength(5)] public string Password { get; set; }
            [Required, Compare("Password")] public string ConfirmPassword { get; set; }
        }
}
