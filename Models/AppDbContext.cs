using Microsoft.EntityFrameworkCore;
using QuanLyHoatDongNgoaiKhoa.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Bảng chính
    public DbSet<HoatDong> HoatDongs { get; set; }
    public DbSet<DangKyThamGia> DangKyThamGias { get; set; }
    
    // Bảng tham chiếu
    public DbSet<TrangThaiHoatDong> TrangThaiHoatDongs { get; set; }
    public DbSet<TrangThaiDangKy> TrangThaiDangKys { get; set; }
    public DbSet<DanhMucHoatDong> DanhMucHoatDongs { get; set; }
    public DbSet<LoaiHoatDong> LoaiHoatDongs { get; set; }
    public DbSet<DiaDiem> DiaDiems { get; set; }
    public DbSet<PhongBan> PhongBans { get; set; }
    public DbSet<LopHoc> LopHocs { get; set; }
    public DbSet<Khoa> Khoas { get; set; }
    public DbSet<ThongBao> ThongBaos { get; set; }
    public DbSet<NhatKyThaoTac> NhatKyThaoTacs { get; set; }
    public DbSet<FileDinhKem> FileDinhKems { get; set; }
    public DbSet<BanDoDanhMucHoatDong> BanDoDanhMucHoatDongs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Cấu hình foreign key cho DangKyThamGia
        modelBuilder.Entity<DangKyThamGia>()
            .HasOne(d => d.HoatDong)
            .WithMany(h => h.DanhSachDangKy)
            .HasForeignKey(d => d.MaHoatDong)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DangKyThamGia>()
            .HasOne(d => d.TrangThaiDangKy)
            .WithMany(t => t.DanhSachDangKy)
            .HasForeignKey(d => d.MaTrangThaiDangKy)
            .OnDelete(DeleteBehavior.Restrict);

        // Cấu hình foreign key cho DangKyThamGia với AspNetUsers
        modelBuilder.Entity<DangKyThamGia>()
            .HasOne(d => d.User)
            .WithMany()
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Cấu hình foreign key cho HoatDong
        modelBuilder.Entity<HoatDong>()
            .HasOne(h => h.TrangThaiHoatDong)
            .WithMany(t => t.DanhSachHoatDong)
            .HasForeignKey(h => h.MaTrangThai)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<HoatDong>()
            .HasOne(h => h.DanhMucHoatDong)
            .WithMany(d => d.DanhSachHoatDong)
            .HasForeignKey(h => h.MaDanhMuc)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<HoatDong>()
            .HasOne(h => h.LoaiHoatDong)
            .WithMany(l => l.DanhSachHoatDong)
            .HasForeignKey(h => h.MaLoaiHoatDong)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<HoatDong>()
            .HasOne(h => h.DiaDiem)
            .WithMany(d => d.DanhSachHoatDong)
            .HasForeignKey(h => h.MaDiaDiem)
            .OnDelete(DeleteBehavior.Restrict);

        // Cấu hình foreign key cho LopHoc
        modelBuilder.Entity<LopHoc>()
            .HasOne(l => l.Khoa)
            .WithMany(k => k.DanhSachLopHoc)
            .HasForeignKey(l => l.MaKhoa)
            .OnDelete(DeleteBehavior.Restrict);

        // Cấu hình foreign key cho FileDinhKem
        modelBuilder.Entity<FileDinhKem>()
            .HasOne(f => f.HoatDong)
            .WithMany(h => h.DanhSachFile)
            .HasForeignKey(f => f.MaHoatDong)
            .OnDelete(DeleteBehavior.Cascade);

        // Cấu hình foreign key cho ThongBao
        modelBuilder.Entity<ThongBao>()
            .HasOne(t => t.HoatDong)
            .WithMany(h => h.DanhSachThongBao)
            .HasForeignKey(t => t.MaHoatDong)
            .OnDelete(DeleteBehavior.Cascade);

        // Cấu hình foreign key cho NhatKyThaoTac
        modelBuilder.Entity<NhatKyThaoTac>()
            .HasOne(n => n.HoatDong)
            .WithMany(h => h.DanhSachNhatKy)
            .HasForeignKey(n => n.MaHoatDong)
            .OnDelete(DeleteBehavior.Cascade);

        // Cấu hình many-to-many cho BanDoDanhMucHoatDong
        modelBuilder.Entity<BanDoDanhMucHoatDong>()
            .HasOne(b => b.HoatDong)
            .WithMany(h => h.DanhSachBanDo)
            .HasForeignKey(b => b.MaHoatDong)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BanDoDanhMucHoatDong>()
            .HasOne(b => b.DanhMucHoatDong)
            .WithMany(d => d.DanhSachBanDo)
            .HasForeignKey(b => b.MaDanhMuc)
            .OnDelete(DeleteBehavior.Cascade);

        // Cấu hình foreign key cho HoatDong với AspNetUsers (NguoiTao)
        modelBuilder.Entity<HoatDong>()
            .HasOne(h => h.NguoiTao)
            .WithMany()
            .HasForeignKey(h => h.NguoiTaoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
} 