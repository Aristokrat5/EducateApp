using EducateApp.Models.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EducateApp.Models
{
    public class AppCtx : IdentityDbContext<User>
    {
        public AppCtx(DbContextOptions<AppCtx> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<FormOfStudy> FormsOfStudy { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<TypesOfTotals> TypesOfTotals { get; set; }
        public DbSet<Disciplines> Disciplines { get; set; }
    }
}
