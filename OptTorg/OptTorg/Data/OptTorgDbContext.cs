using System.Data.Entity;
using OptTorg.Models;

namespace OptTorg.Data
{
    public class OptTorgDbContext : DbContext
    {
        public OptTorgDbContext() : base("OptTorgConnection")
        {
            Database.SetInitializer<OptTorgDbContext>(null);
        }

        public DbSet<Polzovateli> Polzovateli { get; set; }
        public DbSet<Avtorizaciya> Avtorizaciya { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Polzovateli>()
                .HasOptional(p => p.Avtorizaciya)
                .WithRequired(a => a.Polzovatel);


            base.OnModelCreating(modelBuilder);
        }

        public bool TestConnection()
        {
            try
            {
                return Database.Exists();
            }
            catch
            {
                return false;
            }
        }
    }
}