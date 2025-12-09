using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;


namespace Product.BLL.DTO
{
    public class SendOtpRequest
    {
        [Required(ErrorMessage = "Email is required"), EmailAddress] public string Email { get; set; }
    }
}
