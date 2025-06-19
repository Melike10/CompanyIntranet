using Microsoft.AspNetCore.Identity;

namespace CompanyIntranet.Models
{
    public class AppUser:IdentityUser
    {
        public string FullName { get; set; }
    }
}
