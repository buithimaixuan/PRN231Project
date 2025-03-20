using Microsoft.EntityFrameworkCore;
using PostService.Models;

namespace PostService.DAOs;

public partial class PostDbContext : DbContext
{
    public PostDbContext()
    {
    }

    public PostDbContext(DbContextOptions<PostDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CategoryNews> CategoryNews { get; set; }

    public virtual DbSet<CategoryPost> CategoryPosts { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<LikePost> LikePosts { get; set; }

    public virtual DbSet<News> News { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PostImage> PostImages { get; set; }

    public virtual DbSet<SharePost> SharePosts { get; set; }

    public virtual DbSet<View> Views { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        // chạy docker
        //=> optionsBuilder.UseSqlServer("Server=host.docker.internal,1433;Initial Catalog=Microservice_PostDB;Persist Security Info=True;User ID=sa;Password=1234;Encrypt=True;Trust Server Certificate=True");
        // cái cũ nè
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-G595621\\SQLEXPRESS;Initial Catalog=Microservice_PostDB;Persist Security Info=True;User ID=sa;Password=123123;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CategoryNews>(entity =>
        {
            entity.HasKey(e => e.CategoryNewsId).HasName("PK__Category__9D9BEED8D8CDA9C5");

            entity.Property(e => e.CategoryNewsId).HasColumnName("category_news_id");
            entity.Property(e => e.CategoryNewsDescription)
                .HasMaxLength(500)
                .HasColumnName("category_news_description");
            entity.Property(e => e.CategoryNewsName)
                .HasMaxLength(100)
                .HasColumnName("category_news_name");
        });

        modelBuilder.Entity<CategoryPost>(entity =>
        {
            entity.HasKey(e => e.CategoryPostId).HasName("PK__Category__02AEB4E3D40D96F7");

            entity.ToTable("CategoryPost");

            entity.Property(e => e.CategoryPostId).HasColumnName("category_post_id");
            entity.Property(e => e.CategoryPostDescription)
                .HasMaxLength(500)
                .HasColumnName("category_post_description");
            entity.Property(e => e.CategoryPostName)
                .HasMaxLength(200)
                .HasColumnName("category_post_name");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comment__E7957687B222C7AA");

            entity.ToTable("Comment");

            entity.Property(e => e.CommentId).HasColumnName("comment_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.Content)
                .HasMaxLength(200)
                .HasColumnName("content");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.Rate).HasColumnName("rate");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.Post).WithMany(p => p.Comments)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK__Comment__post_id__3F466844");
        });

        modelBuilder.Entity<LikePost>(entity =>
        {
            entity.HasKey(e => e.LikePostId).HasName("PK__LikePost__8F1D2FE806CCD133");

            entity.ToTable("LikePost");

            entity.Property(e => e.LikePostId).HasColumnName("like_post_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.UnLike).HasColumnName("un_like");

            entity.HasOne(d => d.Post).WithMany(p => p.LikePosts)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LikePost__post_i__4222D4EF");
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.HasKey(e => e.NewsId).HasName("PK__News__4C27CCD8109004D1");

            entity.Property(e => e.NewsId).HasColumnName("news_id");
            entity.Property(e => e.CategoryNewsId).HasColumnName("category_news_id");
            entity.Property(e => e.Content)
                .HasMaxLength(100)
                .HasColumnName("content");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .HasColumnName("image_url");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.CategoryNews).WithMany(p => p.News)
                .HasForeignKey(d => d.CategoryNewsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__News__category_n__4CA06362");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__Post__3ED787668C5DD69D");

            entity.ToTable("Post");

            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.CategoryPostId).HasColumnName("category_post_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.PostContent)
                .HasMaxLength(1000)
                .HasColumnName("post_content");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("update_at");

            entity.HasOne(d => d.CategoryPost).WithMany(p => p.Posts)
                .HasForeignKey(d => d.CategoryPostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Post__category_p__398D8EEE");
        });

        modelBuilder.Entity<PostImage>(entity =>
        {
            entity.HasKey(e => e.PostImageId).HasName("PK__PostImag__CD0DD560D5AA74FA");

            entity.ToTable("PostImage");

            entity.Property(e => e.PostImageId).HasColumnName("post_image_id");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("image_url");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.PublicId).HasMaxLength(255);
        });

        modelBuilder.Entity<SharePost>(entity =>
        {
            entity.HasKey(e => e.SharePostId).HasName("PK__SharePos__3B880F324061A306");

            entity.ToTable("SharePost");

            entity.Property(e => e.SharePostId).HasColumnName("share_post_id");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.ShareAt).HasColumnName("share_at");
            entity.Property(e => e.SharerId).HasColumnName("sharer_id");

            entity.HasOne(d => d.Post).WithMany(p => p.SharePosts)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SharePost__post___44FF419A");
        });

        modelBuilder.Entity<View>(entity =>
        {
            entity.HasKey(e => e.CountViewId).HasName("PK__Views__C5F7EC927324785E");

            entity.Property(e => e.CountViewId).HasColumnName("count_view_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.NewsId).HasColumnName("news_id");
            entity.Property(e => e.PostId).HasColumnName("post_id");

            entity.HasOne(d => d.Post).WithMany(p => p.Views)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK__Views__post_id__47DBAE45");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
