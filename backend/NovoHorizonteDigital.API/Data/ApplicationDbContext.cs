using Microsoft.EntityFrameworkCore;
using NovoHorizonteDigital.API.Models;

namespace NovoHorizonteDigital.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Lot> Lots { get; set; }
        public DbSet<Plot> Plots { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ContractPlot> ContractPlots { get; set; }
        public DbSet<AlternativeContact> AlternativeContacts { get; set; }
        public DbSet<MonthlyPayment> MonthlyPayments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Area-Lot relationship
            modelBuilder.Entity<Lot>()
                .HasOne(l => l.Area)
                .WithMany(a => a.Lots)
                .HasForeignKey(l => l.AreaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Lot-Plot relationship
            modelBuilder.Entity<Plot>()
                .HasOne(p => p.Lot)
                .WithMany(l => l.Plots)
                .HasForeignKey(p => p.LotId)
                .OnDelete(DeleteBehavior.Restrict);

            // Area-Plot relationship
            modelBuilder.Entity<Plot>()
                .HasOne(p => p.Area)
                .WithMany(a => a.Plots)
                .HasForeignKey(p => p.AreaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Plot unique constraint
            modelBuilder.Entity<Plot>()
                .HasIndex(p => new { p.PlotNumber, p.LotId, p.AreaId })
                .IsUnique();

            // Contract-Client relationship
            modelBuilder.Entity<Contract>()
                .HasOne(c => c.Client)
                .WithMany()
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Contract-Operator relationship
            modelBuilder.Entity<Contract>()
                .HasOne(c => c.ApprovedByOperator)
                .WithMany()
                .HasForeignKey(c => c.ApprovedByOperatorId)
                .OnDelete(DeleteBehavior.SetNull);

            // ContractPlot
            modelBuilder.Entity<ContractPlot>()
                .HasOne(cp => cp.Contract)
                .WithMany(c => c.ContractPlots)
                .HasForeignKey(cp => cp.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ContractPlot>()
                .HasOne(cp => cp.Plot)
                .WithMany()
                .HasForeignKey(cp => cp.PlotId)
                .OnDelete(DeleteBehavior.Restrict);

            // MonthlyPayment
            modelBuilder.Entity<MonthlyPayment>()
                .HasOne(mp => mp.Contract)
                .WithMany(c => c.MonthlyPayments)
                .HasForeignKey(mp => mp.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MonthlyPayment>()
                .HasOne(mp => mp.ValidatedByOperator)
                .WithMany()
                .HasForeignKey(mp => mp.ValidatedByOperatorId)
                .OnDelete(DeleteBehavior.SetNull);

            // AlternativeContact
            modelBuilder.Entity<AlternativeContact>()
                .HasOne(ac => ac.Contract)
                .WithOne(c => c.AlternativeContact)
                .HasForeignKey<AlternativeContact>(ac => ac.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

            // Contract unique code
            modelBuilder.Entity<Contract>()
                .HasIndex(c => c.ContractCode)
                .IsUnique();

            // Decimal precision for currency values (Meticais)
            modelBuilder.Entity<Area>()
                .Property(a => a.AdhesionValue)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Area>()
                .Property(a => a.MonthlyInstallment)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Contract>()
                .Property(c => c.TotalAdhesionValue)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Contract>()
                .Property(c => c.TotalInstallmentValue)
                .HasPrecision(18, 2);

            modelBuilder.Entity<MonthlyPayment>()
                .Property(mp => mp.ExpectedValue)
                .HasPrecision(18, 2);

            modelBuilder.Entity<MonthlyPayment>()
                .Property(mp => mp.PaidValue)
                .HasPrecision(18, 2);
        }
    }
}
