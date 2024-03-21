using System;
using System.Collections.Generic;
using ImagiArtDomain.Model;
using Microsoft.EntityFrameworkCore;

namespace ImagiArtInfrastructure;

public partial class CloneContext : DbContext
{
    public CloneContext()
    {
    }

    public CloneContext(DbContextOptions<CloneContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Like> Likes { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserFollower> UserFollowers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-6SAKF27\\SQLEXPRESS; Database=Clone; Trusted_Connection=True; TrustServerCertificate=True; ");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Comments__E7957687FE981422");

            entity.Property(e => e.Caption)
                .HasMaxLength(64)
                .HasColumnName("caption");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Post).WithMany(p => p.Comments)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("comments_post_id_foreign");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("comments_user_id_foreign");
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Likes__992C79308223B6DD");

            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Post).WithMany(p => p.Likes)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("likes_post_id_foreign");

            entity.HasOne(d => d.User).WithMany(p => p.Likes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("likes_user_id_foreign");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Posts__3ED78766917B0BC9");

            entity.Property(e => e.Caption)
                .HasMaxLength(32)
                .HasColumnName("caption");
            entity.Property(e => e.Description)
                .HasMaxLength(64)
                .HasColumnName("description");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("image_url");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Posts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("posts_user_id_foreign");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__B9BE370F05DDB2CA");

            entity.Property(e => e.Password)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Username)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        modelBuilder.Entity<UserFollower>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserFoll__15A691442B4B1096");

            entity.Property(e => e.FollowerId).HasColumnName("follower_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Follower).WithMany(p => p.UserFollowerFollowers)
                .HasForeignKey(d => d.FollowerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserFollowers_Users");

            entity.HasOne(d => d.User).WithMany(p => p.UserFollowerUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("userfollowers_user_id_foreign");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
