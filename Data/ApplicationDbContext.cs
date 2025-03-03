//using DocumentationAndReports.Models;
using Microsoft.EntityFrameworkCore;

namespace DocumentationAndReports.Data
{
    public class ApplicationDbContext : DbContext 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
                
        }

        //public DbSet<Schedule> Schedules { get; set; }
        //public DbSet<Soldiers> Soldiers { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //}
    }
}
