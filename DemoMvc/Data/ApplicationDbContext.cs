using Microsoft.EntityFrameworkCore;
using DemoMvc.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DemoMVC.Models;
using DemoMVC.Models.Entities;

namespace DemoMvc.Data
{
    using Microsoft.EntityFrameworkCore;
    using DemoMvc.Models;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.AspNetCore.Identity;
    using DemoMVC.Models.Entities;

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }
        public DbSet<Person> Person { get; set; }
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<MemberUnit> MemberUnit { get; set; } = default!;
    }
}