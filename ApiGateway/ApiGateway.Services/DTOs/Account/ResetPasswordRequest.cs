using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiGateway.Services.DTOs.Account
{
    public class ResetPasswordRequest
    {
        [Required]
        public string? UserId { get; set; }

        [Required]
        public string? Token { get; set; }

        [Required]
        public string? NewPassword { get; set; }
    }
}