using Microsoft.AspNetCore.Identity;

namespace NguyenManhDuc.WebApp.Models
{
    public class AppUserModel : IdentityUser
    {
        public InformationModel? Information { get; set; }
    }
}
