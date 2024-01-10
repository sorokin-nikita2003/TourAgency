using Microsoft.AspNetCore.Identity;

namespace agency.Models.ViewModel
{
    public class UserChangeRole
    {
        public List<IdentityUser> users { get; set; }
        public List<IdentityUserRole<string>> userRoles { get; set; }
    }
}
