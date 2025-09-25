using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiGateway.Entities.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? Address { get; set; }
        public string? Note { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdateDate { get; set; }
        public string? CreateBy { get; set; } = "None";
        public string? UpdateBy { get; set; }
    }
}
