using HondaSensorChecker.Models;
using Microsoft.EntityFrameworkCore;

namespace HondaSensorChecker.Data.Context
{
    public class DataContext : DbContext
    {
        public DbSet<Operator> Operators { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<SapWorkOrder> SapWorkOrders { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<SupplierBox> SupplierBoxes { get; set; }
        public DbSet<ZfBox> ZfBoxes { get; set; }
        public DbSet<Log> Logs { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =====================================================
            // Required fields (mínimo para consistência)
            // =====================================================

            modelBuilder.Entity<Product>()
                .Property(p => p.Prefix)
                .IsRequired();

            modelBuilder.Entity<Product>()
                .Property(p => p.StartPartNumber)
                .IsRequired();

            modelBuilder.Entity<Product>()
                .Property(p => p.EndPartNumber)
                .IsRequired();

            modelBuilder.Entity<Operator>()
                .Property(o => o.Re)
                .IsRequired();

            modelBuilder.Entity<Operator>()
                .Property(o => o.ZfId)
                .IsRequired();

            modelBuilder.Entity<Operator>()
                .Property(o => o.Name)
                .IsRequired();

            modelBuilder.Entity<Sensor>()
                .Property(s => s.SerialNumber)
                .IsRequired();

            modelBuilder.Entity<SupplierBox>()
                .Property(sb => sb.UniqueNumber)
                .IsRequired();

            modelBuilder.Entity<ZfBox>()
                .Property(z => z.UniqueNumber)
                .IsRequired(false);

            modelBuilder.Entity<Log>()
                .Property(l => l.Description)
                .IsRequired();

            // =====================================================
            // Product relations
            // =====================================================

            modelBuilder.Entity<Sensor>()
                .HasOne(s => s.Product)
                .WithMany(p => p.Sensors)
                .HasForeignKey(s => s.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SupplierBox>()
                .HasOne(sb => sb.Product)
                .WithMany(p => p.SupplierBoxes)
                .HasForeignKey(sb => sb.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SapWorkOrder>()
                .HasOne(w => w.Product)
                .WithMany(p => p.SapWorkOrders)
                .HasForeignKey(w => w.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ZfBox>()
                .HasOne(z => z.Product)
                .WithMany(p => p.ZfBoxes)
                .HasForeignKey(z => z.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // =====================================================
            // SapWorkOrder relations
            // =====================================================

            modelBuilder.Entity<ZfBox>()
                .HasOne(z => z.SapWorkOrder)
                .WithMany(w => w.ZfBoxes)
                .HasForeignKey(z => z.SapWorkOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Sensor>()
                .HasOne(s => s.SapWorkOrder)
                .WithMany(w => w.Sensors)
                .HasForeignKey(s => s.SapWorkOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            // =====================================================
            // Operator relations
            // =====================================================

            modelBuilder.Entity<Sensor>()
                .HasOne(s => s.Operator)
                .WithMany(o => o.Sensors)
                .HasForeignKey(s => s.OperatorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ZfBox>()
                .HasOne(z => z.Operator)
                .WithMany(o => o.ZfBoxes)
                .HasForeignKey(z => z.OperatorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Log>()
                .HasOne(l => l.Operator)
                .WithMany(o => o.Logs)
                .HasForeignKey(l => l.OperatorId)
                .OnDelete(DeleteBehavior.Restrict);

            // =====================================================
            // SupplierBox relations
            // =====================================================

            modelBuilder.Entity<Sensor>()
                .HasOne(s => s.SupplierBox)
                .WithMany(sb => sb.Sensors)
                .HasForeignKey(s => s.SupplierBoxId)
                .OnDelete(DeleteBehavior.Restrict);

            // =====================================================
            // ZfBox relations
            // =====================================================

            modelBuilder.Entity<Sensor>()
                .HasOne(s => s.ZfBox)
                .WithMany(z => z.Sensors)
                .HasForeignKey(s => s.ZfBoxId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
