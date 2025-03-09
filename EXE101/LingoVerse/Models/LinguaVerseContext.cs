using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LingoVerse.Models;

public partial class LinguaVerseContext : DbContext
{
    public LinguaVerseContext()
    {
    }

    public LinguaVerseContext(DbContextOptions<LinguaVerseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<Level> Levels { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<Premium> Premia { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Set> Sets { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserPremium> UserPremia { get; set; }

    public virtual DbSet<Word> Words { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
 //       => optionsBuilder.UseSqlServer("Data Source=MSI\\SQLEXPRESS;Initial Catalog=LinguaVerse; Trusted_Connection=SSPI;Encrypt=false;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasKey(e => e.LanguageId).HasName("PK__Language__B93855AB71AD30AF");

            entity.HasIndex(e => e.LanguageName, "UQ__Language__E89C4A6AAA9117B5").IsUnique();

            entity.Property(e => e.LanguageName).HasMaxLength(50);
        });

        modelBuilder.Entity<Level>(entity =>
        {
            entity.HasKey(e => e.LevelId).HasName("PK__Levels__09F03C268550F35A");

            entity.HasIndex(e => e.LevelName, "UQ__Levels__9EF3BE7B732D766E").IsUnique();

            entity.Property(e => e.LevelName).HasMaxLength(50);
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__Posts__AA12601825451C3F");

            entity.Property(e => e.Content).HasColumnType("text");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Image).IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasDefaultValue("Draft");
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Author).WithMany(p => p.Posts)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK_Posts_Users");

            entity.HasOne(d => d.Language).WithMany(p => p.Posts)
                .HasForeignKey(d => d.LanguageId)
                .HasConstraintName("FK_Posts_Languages");

            entity.HasOne(d => d.Level).WithMany(p => p.Posts)
                .HasForeignKey(d => d.LevelId)
                .HasConstraintName("FK_Posts_Levels");
        });

        modelBuilder.Entity<Premium>(entity =>
        {
            entity.HasKey(e => e.PremiumId).HasName("PK__Premium__86B646851E238509");

            entity.ToTable("Premium");

            entity.HasIndex(e => e.PremiumName, "UQ__Premium__B4B5EA3397035A77").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.PremiumName).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.QuestionId).HasName("PK__Question__0DC06FAC95DB1B8F");

            entity.Property(e => e.Answer).HasColumnType("text");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.QuestionText).HasColumnType("text");

            entity.HasOne(d => d.Post).WithMany(p => p.Questions)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK_Questions_Posts");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1A6825D758");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160A2C31BEE").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Set>(entity =>
        {
            entity.HasKey(e => e.SetId).HasName("PK__Sets__7E08471D16093450");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.SetName).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Sets)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Sets_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CBE30ECFE");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E40B18F3BA").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        modelBuilder.Entity<UserPremium>(entity =>
        {
            entity.HasKey(e => e.UserPremiumId).HasName("PK__UserPrem__215D48B011044F0A");

            entity.ToTable("UserPremium");

            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.StartDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Premium).WithMany(p => p.UserPremia)
                .HasForeignKey(d => d.PremiumId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserPremium_Premium");

            entity.HasOne(d => d.User).WithMany(p => p.UserPremia)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserPremium_Users");
        });

        modelBuilder.Entity<Word>(entity =>
        {
            entity.HasKey(e => e.WordId).HasName("PK__Words__2C20F066A3FFEE10");

            entity.HasIndex(e => new { e.SetId, e.Word1 }, "UQ_Words_SetId_Word").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Definition).HasColumnType("text");
            entity.Property(e => e.Example).HasColumnType("text");
            entity.Property(e => e.Word1)
                .HasMaxLength(100)
                .HasColumnName("Word");

            entity.HasOne(d => d.Set).WithMany(p => p.Words)
                .HasForeignKey(d => d.SetId)
                .HasConstraintName("FK_Words_Sets");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
