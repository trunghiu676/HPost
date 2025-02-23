using App.Models.UserManagement;
using App.Models.Provinces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using App.Models.Orders;
using App.Models.Locations;
using App.Models.Blog;
using App.Models.Services;

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

            //Các bảng trong Blog
            // Đánh chỉ mục INDEX cột Slug bảng Category trong db => tìm kiếm nhanh hơn
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(p => p.Slug)
                      .IsUnique();
            });
            //tao khoa chinh cho postcategory tu post id va categoryid
            modelBuilder.Entity<PostCategory>(entity =>
            {
                entity.HasKey(c => new { c.PostID, c.CategoryID });
            });
            //thêm khóa ngoại trường categoryid cho post
            modelBuilder.Entity<Post>()
                        .HasOne(p => p.Category)
                        .WithMany(c => c.Posts)
                        .HasForeignKey(p => p.CategoryId)
                        .OnDelete(DeleteBehavior.NoAction); // Thay đổi hành vi xóa, các mục con không bị xóa khi xóa cha

            // Đánh chỉ mục INDEX cột Slug bảng Post trong db => tìm kiếm nhanh hơn
            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasIndex(p => p.Slug)
                      .IsUnique(); //thiet lap chi muc nay la duy nhat, khong duoc phep co 2 bai post co slug giong nhau
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasIndex(s => s.Slug)
                      .IsUnique();
            });

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

        //Cac bang trong blog - tin tuc
        public DbSet<Category> Categories { set; get; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostCategory> PostCategories { get; set; }
        //Các bảng dịch vụ
        public DbSet<ServiceType> ServiceTypes { get; set; }
        public DbSet<Service> Services { get; set; }
    }

}