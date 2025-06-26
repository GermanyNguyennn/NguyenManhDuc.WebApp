using Microsoft.AspNetCore.Identity;

namespace NguyenManhDuc.WebApp.Models
{
    public class AppUserModel : IdentityUser
    {
        public string? FullName { get; set; }
        public virtual InformationModel? Information { get; set; }
    }
}
