using CommunicateService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommunicateService.DAOs
{
    public partial class MicroserviceCommunicateDbContext : DbContext
    {
        public MicroserviceCommunicateDbContext()
        {
        }

        public MicroserviceCommunicateDbContext(DbContextOptions<MicroserviceCommunicateDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AccountConversation> AccountConversations { get; set; }

        public virtual DbSet<Conversation> Conversations { get; set; }

        public virtual DbSet<Message> Messages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
            => optionsBuilder.UseSqlServer("Server=KhoaKiro\\SQLEXPRESS;Database=Microservice_BookingDB;uid=sa;pwd=khoa31102003;encrypt=true;trustServerCertificate=true;");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountConversation>(entity =>
            {
                entity.HasKey(e => new { e.AccountId, e.ConversationId }).HasName("PK__AccountC__F5B3C524EEA2458F");

                entity.ToTable("AccountConversation");

                entity.Property(e => e.AccountId).HasColumnName("account_id");
                entity.Property(e => e.ConversationId).HasColumnName("conversation_id");
                entity.Property(e => e.IsOut).HasColumnName("is_out");
                entity.Property(e => e.OutAt).HasColumnName("out_at");

                entity.HasOne(d => d.Conversation).WithMany(p => p.AccountConversations)
                    .HasForeignKey(d => d.ConversationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__AccountCo__conve__398D8EEE");
            });

            modelBuilder.Entity<Conversation>(entity =>
            {
                entity.HasKey(e => e.ConversationId).HasName("PK__Conversa__311E7E9A21D3CDA9");

                entity.ToTable("Conversation");

                entity.Property(e => e.ConversationId).HasColumnName("conversation_id");
                entity.Property(e => e.ConversationName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("conversation_name");
                entity.Property(e => e.CreateAt).HasColumnName("create_at");
                entity.Property(e => e.CreatorId).HasColumnName("creator_id");
                entity.Property(e => e.DeleteAt).HasColumnName("delete_at");
                entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
                entity.Property(e => e.IsGroup).HasColumnName("is_group");
                entity.Property(e => e.MemberCount).HasColumnName("member_count");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e => e.MessageId).HasName("PK__Message__0BBF6EE6F0A302EB");

                entity.ToTable("Message");

                entity.Property(e => e.MessageId).HasColumnName("message_id");
                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnName("content");
                entity.Property(e => e.ConversationId).HasColumnName("conversation_id");
                entity.Property(e => e.CreateAt).HasColumnName("create_at");
                entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
                entity.Property(e => e.SenderId).HasColumnName("sender_id");

                entity.HasOne(d => d.Conversation).WithMany(p => p.Messages)
                    .HasForeignKey(d => d.ConversationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Message__convers__3C69FB99");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}