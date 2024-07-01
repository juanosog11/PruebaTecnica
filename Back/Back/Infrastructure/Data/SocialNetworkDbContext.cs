using Back.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Back.Infrastructure.Data
{
    public partial class SocialNetworkDbContext : DbContext
    {
        public SocialNetworkDbContext()
        {
        }

        public SocialNetworkDbContext(DbContextOptions<SocialNetworkDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Follow> Follows { get; set; }
        public virtual DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Configuración opcional aquí
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureUserEntity(modelBuilder);
            ConfigureFollowEntity(modelBuilder);
            ConfigurePostEntity(modelBuilder);

            OnModelCreatingPartial(modelBuilder);
        }

        private void ConfigureUserEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UsuarioId)
                      .HasName("PK__Users__2B3DE7B839539EFE");

                entity.HasIndex(e => e.Numero)
                      .IsUnique();

                entity.HasIndex(e => e.Email)
                      .IsUnique();

                entity.HasIndex(e => e.Usuario)
                      .IsUnique();

                entity.Property(e => e.Clave)
                      .HasMaxLength(100)
                      .IsUnicode(false);

                entity.Property(e => e.Email)
                      .HasMaxLength(50)
                      .IsUnicode(false);

                entity.Property(e => e.Numero)
                      .HasMaxLength(50)
                      .IsUnicode(false);

                entity.Property(e => e.Usuario)
                      .HasMaxLength(50)
                      .IsUnicode(false);

                entity.ToTable("Users");

                // Relación con Follows (Many-to-Many)
                entity.HasMany(u => u.Followees)
                      .WithMany(u => u.Followers)
                      .UsingEntity<Follow>(
                          j => j
                              .HasOne(f => f.Followee)
                              .WithMany()
                              .HasForeignKey(f => f.FolloweeId)
                              .OnDelete(DeleteBehavior.ClientSetNull)
                              .HasConstraintName("FK_Follows_FolloweeId_Users_UsuarioId"),
                          j => j
                              .HasOne(f => f.Follower)
                              .WithMany()
                              .HasForeignKey(f => f.FollowerId)
                              .OnDelete(DeleteBehavior.ClientSetNull)
                              .HasConstraintName("FK_Follows_FollowerId_Users_UsuarioId")
                      );
            });
        }

        private void ConfigureFollowEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Follow>(entity =>
            {
                entity.HasKey(e => new { e.FollowerId, e.FolloweeId })
                      .HasName("PK__Follows__67A1FC7C82A23FCE");

                entity.ToTable("Follows");
            });
        }

        private void ConfigurePostEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasKey(e => e.PostId)
                      .HasName("PK__Posts__AA126018436802A7");

                entity.Property(e => e.Content)
                      .HasMaxLength(280);

                entity.Property(e => e.Timestamp)
                      .IsUnicode(false);

                entity.HasIndex(e => e.UsuarioId);

                // Configuración de la relación con Usuario
                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(d => d.UsuarioId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_Post_Usuario");
            });
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
