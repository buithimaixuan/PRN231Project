using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.DAOs;

public partial class MicroserviceUserDbContext : DbContext
{
    public MicroserviceUserDbContext()
    {
    }

    public MicroserviceUserDbContext(DbContextOptions<MicroserviceUserDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<FriendRequest> FriendRequests { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        // chạy docker
        => optionsBuilder.UseSqlServer("Data Source=host.docker.internal,1433;Initial Catalog=Microservice_UserDB;Persist Security Info=True;User ID=sa;Password=1234;Encrypt=True;Trust Server Certificate=True");
        // cái cũ nè
        //=> optionsBuilder.UseSqlServer("Data Source=DESKTOP-G595621\\SQLEXPRESS;Initial Catalog=Microservice_UserDB;Persist Security Info=True;User ID=sa;Password=123123;Encrypt=True;Trust Server Certificate=True");


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Account__46A222CD833C521A");

            entity.ToTable("Account");

            entity.HasIndex(e => e.Username, "UQ__Account__F3DBC572D2311EA2").IsUnique();

            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.Address)
                .HasMaxLength(500)
                .HasColumnName("address");
            entity.Property(e => e.Avatar)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("avatar");
            entity.Property(e => e.DateOfBirth).HasColumnName("dateOfBirth");
            entity.Property(e => e.DegreeUrl)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("degree_url");
            entity.Property(e => e.EducationUrl)
                .HasMaxLength(500)
                .HasColumnName("education_url");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.EmailConfirmed).HasColumnName("email_confirmed");
            entity.Property(e => e.FacebookId)
                .HasMaxLength(255)
                .HasColumnName("facebook_id");
            entity.Property(e => e.FullName)
                .HasMaxLength(50)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("gender");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Major)
                .HasMaxLength(100)
                .HasColumnName("major");
            entity.Property(e => e.Otp).HasColumnName("otp");
            entity.Property(e => e.Password)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.PhoneConfirmed).HasColumnName("phone_confirmed");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.ShortBio)
                .HasMaxLength(500)
                .HasColumnName("short_bio");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("username");
            entity.Property(e => e.YearOfExperience).HasColumnName("year_of_experience");

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Account__role_id__3A81B327");
        });

        modelBuilder.Entity<FriendRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__FriendRe__18D3B90FC3A2E7BF");

            entity.ToTable("FriendRequest");

            entity.Property(e => e.RequestId).HasColumnName("request_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.ReceiverId).HasColumnName("receiver_id");
            entity.Property(e => e.RequestStatus)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("Pending")
                .HasColumnName("request_status");
            entity.Property(e => e.SenderId).HasColumnName("sender_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.Receiver).WithMany(p => p.FriendRequestReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FriendReq__recei__3E52440B");

            entity.HasOne(d => d.Sender).WithMany(p => p.FriendRequestSenders)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FriendReq__sende__3D5E1FD2");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__760965CC536DCC56");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("role_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
