using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAl.Sql
{
    public class CommonContext : DbContext
    {
        public CommonContext(DbContextOptions<CommonContext> options) : base(options){}

        public DbSet<Account> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<Entity>(e =>
            //{
            //    e.HasKey(x => x.Id);
            //});

            modelBuilder.Entity<Account>(a =>
            {
                a.HasKey(x => x.Id);
                a.Property(x => x.Login).IsRequired();
                a.Property(x => x.Password).IsRequired();
                a.Property(x => x.WalletAddress);
            });
        }
    }
}
