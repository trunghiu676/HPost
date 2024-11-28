using App.Models.UserManagement;
using App.Models.Provinces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using App.Models.Orders;
using App.Models.Locations;

namespace App.Models
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            // Phương thức khởi tạo này chứa options để kết nối đến MS SQL Server
            // Thực hiện điều này khi Inject trong dịch vụ hệ thống
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //Xoa ten AspNet ra khoi cac bang identity
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }

            //Các bảng tỉnh thành và khu vực
            // Tạo chỉ mục
            // modelBuilder.Entity<Province>()
            //     .HasIndex(p => p.AdministrativeRegionId)
            //     .HasDatabaseName("idx_provinces_region");

            // modelBuilder.Entity<District>()
            //     .HasIndex(d => d.ProvinceCode)
            //     .HasDatabaseName("idx_districts_province");

            // modelBuilder.Entity<Ward>()
            //     .HasIndex(w => w.DistrictCode)
            //     .HasDatabaseName("idx_wards_district");
            //End các bảng tỉnh thành và khu vực
        }
        // // public DbSet<Order> Orders { get; set; }
        public DbSet<AdministrativeRegion> AdministrativeRegions { get; set; }
        public DbSet<AdministrativeUnit> AdministrativeUnits { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Ward> Wards { get; set; }
    }

}