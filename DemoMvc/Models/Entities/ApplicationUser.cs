using Microsoft.AspNetCore.Identity;

namespace DemoMVC.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string FulName {get; set;}
    }
}