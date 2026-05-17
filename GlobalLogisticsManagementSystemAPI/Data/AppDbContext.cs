using GlobalLogisticsManagementSystemAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GlobalLogisticsManagementSystemAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Client> Clients => Set<Client>();
        public DbSet<Contract> Contracts => Set<Contract>();
        public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contract>()
                .HasOne(c => c.Client)
                .WithMany(cl => cl.Contracts)
                .HasForeignKey(c => c.ClientId);

            modelBuilder.Entity<ServiceRequest>()
                .HasOne(sr => sr.Contract)
                .WithMany(c => c.ServiceRequests)
                .HasForeignKey(sr => sr.ContractId);

            modelBuilder.Entity<ServiceRequest>()
                .Property(sr => sr.CostUsd)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ServiceRequest>()
                .Property(sr => sr.CostZar)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ServiceRequest>()
                .Property(sr => sr.ExchangeRate)
                .HasColumnType("decimal(18,6)");
        }
    }
}
