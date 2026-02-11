using Microsoft.EntityFrameworkCore;
using eVoter.Core.Models;

namespace eVoter.Data.Context;

public class eVoterDbContext : DbContext
{
    public eVoterDbContext(DbContextOptions<eVoterDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Voter> Voters { get; set; }
    public DbSet<BiometricData> BiometricData { get; set; }
    public DbSet<Election> Elections { get; set; }
    public DbSet<Candidate> Candidates { get; set; }
    public DbSet<Vote> Votes { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
            entity.Property(e => e.PasswordHash).IsRequired();
        });

        // Voter configuration
        modelBuilder.Entity<Voter>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.NationalId).IsUnique();
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.NationalId).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);

            entity.HasOne(e => e.User)
                .WithOne(u => u.Voter)
                .HasForeignKey<Voter>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // BiometricData configuration
        modelBuilder.Entity<BiometricData>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.RFIDCardNumber).IsUnique();
            
            entity.HasOne(e => e.Voter)
                .WithOne(v => v.BiometricData)
                .HasForeignKey<BiometricData>(e => e.VoterId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Election configuration
        modelBuilder.Entity<Election>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
        });

        // Candidate configuration
        modelBuilder.Entity<Candidate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Party).HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);

            entity.HasOne(e => e.Election)
                .WithMany(el => el.Candidates)
                .HasForeignKey(e => e.ElectionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Vote configuration
        modelBuilder.Entity<Vote>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.ElectionId, e.VoterId }).IsUnique();

            entity.HasOne(e => e.Election)
                .WithMany(el => el.Votes)
                .HasForeignKey(e => e.ElectionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Voter)
                .WithMany(v => v.Votes)
                .HasForeignKey(e => e.VoterId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Candidate)
                .WithMany(c => c.Votes)
                .HasForeignKey(e => e.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // AuditLog configuration
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Action).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Entity).HasMaxLength(100).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
