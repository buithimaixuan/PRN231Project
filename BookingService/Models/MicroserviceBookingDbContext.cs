using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Models;

public partial class MicroserviceBookingDbContext : DbContext
{
    public MicroserviceBookingDbContext()
    {
    }

    public MicroserviceBookingDbContext(DbContextOptions<MicroserviceBookingDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BookingService> BookingServices { get; set; }

    public virtual DbSet<CategoryService> CategoryServices { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<ServiceRating> ServiceRatings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=MSI\\SQLEXPRESS;Database=Microservice_BookingDB;uid=sa;pwd=khoa31102003;encrypt=true;trustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookingService>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__BookingS__5DE3A5B15159E594");

            entity.ToTable("BookingService");

            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.BookingAt).HasColumnName("booking_at");
            entity.Property(e => e.BookingBy).HasColumnName("booking_by");
            entity.Property(e => e.BookingStatus)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("booking_status");
            entity.Property(e => e.Content).HasMaxLength(500);
            entity.Property(e => e.IsDeletedExpert).HasColumnName("is_deleted_expert");
            entity.Property(e => e.IsDeletedFarmer).HasColumnName("is_deleted_farmer");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");

            entity.HasOne(d => d.Service).WithMany(p => p.BookingServices)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookingSe__servi__3C69FB99");
        });

        modelBuilder.Entity<CategoryService>(entity =>
        {
            entity.HasKey(e => e.CategoryServiceId).HasName("PK__Category__8B6132CC09119F0F");

            entity.ToTable("CategoryService");

            entity.Property(e => e.CategoryServiceId).HasColumnName("category_service_id");
            entity.Property(e => e.CategoryServiceDescription)
                .HasMaxLength(500)
                .HasColumnName("category_service_description");
            entity.Property(e => e.CategoryServiceName)
                .HasMaxLength(100)
                .HasColumnName("category_service_name");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Service__3E0DB8AF5145E402");

            entity.ToTable("Service");

            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.AverageRating)
                .HasColumnType("decimal(2, 1)")
                .HasColumnName("average_rating");
            entity.Property(e => e.CategoryServiceId).HasColumnName("category_service_id");
            entity.Property(e => e.Content)
                .HasMaxLength(1000)
                .HasColumnName("content");
            entity.Property(e => e.CreateAt).HasColumnName("create_at");
            entity.Property(e => e.CreatorId).HasColumnName("creator_id");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.IsEnable).HasColumnName("is_enable");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.RatingCount).HasColumnName("rating_count");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.CategoryService).WithMany(p => p.Services)
                .HasForeignKey(d => d.CategoryServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Service__categor__398D8EEE");
        });

        modelBuilder.Entity<ServiceRating>(entity =>
        {
            entity.HasKey(e => e.RatingId).HasName("PK__ServiceR__D35B278BEFDB12DB");

            entity.ToTable("ServiceRating");

            entity.Property(e => e.RatingId).HasColumnName("rating_id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.RatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("rated_at");
            entity.Property(e => e.Rating)
                .HasColumnType("decimal(2, 1)")
                .HasColumnName("rating");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Service).WithMany(p => p.ServiceRatings)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ServiceRa__servi__3F466844");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
