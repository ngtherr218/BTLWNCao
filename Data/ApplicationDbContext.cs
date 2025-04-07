using Microsoft.EntityFrameworkCore;

namespace BTLWNCao.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<CongTy> CongTys { get; set; }
        public DbSet<UserCongTy> UserCongTys { get; set; }
        public DbSet<NhomChat> NhomChats { get; set; }
        public DbSet<UserNhomChat> UserNhomChats { get; set; }
        public DbSet<TinNhan> TinNhans { get; set; }
        public DbSet<DuAn> DuAns { get; set; }
        public DbSet<PhanCongCongViec> PhanCongCongViecs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình bảng Users
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.MaUser);
                entity.Property(u => u.TenUser).IsRequired().HasMaxLength(100);
                entity.Property(u => u.TenDangNhap).IsRequired().HasMaxLength(50);
                entity.Property(u => u.MatKhau).IsRequired().HasMaxLength(255);
                entity.Property(u => u.SoDienThoai).IsRequired().HasMaxLength(15);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
            });

            // Cấu hình bảng CongTy
            modelBuilder.Entity<CongTy>(entity =>
            {
                entity.HasKey(c => c.MaCongTy);
                entity.Property(c => c.TenCongTy).IsRequired().HasMaxLength(255);
                entity.Property(c => c.SoDienThoai).HasMaxLength(15);
                entity.Property(c => c.ThongTinCongTy).IsRequired();
            });

            // Cấu hình bảng UserCongTy
            modelBuilder.Entity<UserCongTy>(entity =>
            {
                entity.HasKey(uc => uc.MaUserCongTy);
                entity.HasOne(uc => uc.CongTy)
                      .WithMany(c => c.UserCongTys)
                      .HasForeignKey(uc => uc.MaCongTy)
                      .OnDelete(DeleteBehavior.Restrict);  // Hoặc NO ACTION
                entity.HasOne(uc => uc.User)
                      .WithMany(u => u.UserCongTys)
                      .HasForeignKey(uc => uc.MaUser)
                      .OnDelete(DeleteBehavior.Restrict);  // Hoặc NO ACTION
            });

            // Cấu hình bảng NhomChat
            modelBuilder.Entity<NhomChat>(entity =>
            {
                entity.HasKey(nc => nc.MaNhomChat);
                entity.Property(nc => nc.TenNhomChat).IsRequired().HasMaxLength(100);
                entity.HasOne(nc => nc.UserCongTy)
                      .WithMany(uc => uc.NhomChats)
                      .HasForeignKey(nc => nc.MaUserCongTy)
                      .OnDelete(DeleteBehavior.Restrict);  // Hoặc NO ACTION
            });

            // Cấu hình bảng UserNhomChat
            modelBuilder.Entity<UserNhomChat>(entity =>
            {
                entity.HasKey(unc => unc.MaUserNhomChat);
                entity.HasOne(unc => unc.NhomChat)
                      .WithMany(nc => nc.UserNhomChats)
                      .HasForeignKey(unc => unc.MaNhomChat)
                      .OnDelete(DeleteBehavior.Restrict);  // Hoặc NO ACTION
                entity.HasOne(unc => unc.UserCongTy)
                      .WithMany(uc => uc.UserNhomChats)
                      .HasForeignKey(unc => unc.MaUserCongTy)
                      .OnDelete(DeleteBehavior.Restrict);  // Hoặc NO ACTION
            });

            // Cấu hình bảng TinNhan
            modelBuilder.Entity<TinNhan>(entity =>
            {
                entity.HasKey(tn => tn.MaChat);
                entity.Property(tn => tn.NoiDung).HasColumnType("NVARCHAR(MAX)");
                entity.Property(tn => tn.Anh).HasMaxLength(255);
                entity.Property(tn => tn.FileTaiLieu).HasMaxLength(255);
                entity.HasOne(tn => tn.UserNhomChat)
                      .WithMany(unc => unc.TinNhans)
                      .HasForeignKey(tn => tn.MaUserNhomChat)
                      .OnDelete(DeleteBehavior.Restrict);  // Hoặc NO ACTION
                entity.HasOne(tn => tn.NhomChat)
                      .WithMany(nc => nc.TinNhans)
                      .HasForeignKey(tn => tn.MaNhomChat)
                      .OnDelete(DeleteBehavior.Restrict);  // Hoặc NO ACTION
            });

            // Cấu hình bảng DuAn
            modelBuilder.Entity<DuAn>(entity =>
            {
                entity.HasKey(da => da.MaDuAn);
                entity.Property(da => da.TenDuAn).IsRequired().HasMaxLength(255);
                entity.Property(da => da.NoiDungDuAn).HasColumnType("NVARCHAR(MAX)");
                entity.HasOne(da => da.UserCongTy)
                      .WithMany(uc => uc.DuAns)
                      .HasForeignKey(da => da.MaUserCongTy)
                      .OnDelete(DeleteBehavior.Restrict);  // Hoặc NO ACTION
                entity.HasOne(da => da.CongTy)
                      .WithMany(c => c.DuAns)
                      .HasForeignKey(da => da.MaCongTy)
                      .OnDelete(DeleteBehavior.Restrict);  // Hoặc NO ACTION
            });

            // Cấu hình bảng PhanCongCongViec
            modelBuilder.Entity<PhanCongCongViec>(entity =>
            {
                entity.HasKey(pc => pc.MaCongViec);
                entity.Property(pc => pc.TenCongViec).IsRequired().HasMaxLength(255);
                entity.Property(pc => pc.NoiDung).HasColumnType("NVARCHAR(MAX)");
                entity.Property(pc => pc.Anh).HasMaxLength(255);
                entity.Property(pc => pc.FileTaiLieu).HasMaxLength(255);
                entity.Property(pc => pc.Deadline).IsRequired();
                entity.HasOne(pc => pc.UserCongTy)
                      .WithMany(uc => uc.PhanCongCongViecs)
                      .HasForeignKey(pc => pc.MaUserCongTy)
                      .OnDelete(DeleteBehavior.Restrict);  // Hoặc NO ACTION
                entity.HasOne(pc => pc.DuAn)
                      .WithMany(da => da.PhanCongCongViecs)
                      .HasForeignKey(pc => pc.MaDuAn)
                      .OnDelete(DeleteBehavior.Restrict);  // Hoặc NO ACTION
            });
        }
    }
}
