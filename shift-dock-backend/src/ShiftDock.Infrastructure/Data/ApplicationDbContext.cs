using Microsoft.EntityFrameworkCore;
using ShiftDock.Domain.Entities;
using ShiftDock.Domain.Enums;

namespace ShiftDock.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<OrganizationMembership> OrganizationMemberships { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Shift> Shifts { get; set; }
    public DbSet<WorkerAssignment> WorkerAssignments { get; set; }
    public DbSet<JoinRequest> JoinRequests { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User Configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Phone).IsUnique();
            entity.HasIndex(e => e.Email);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        // Organization Configuration
        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.JoinCode).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.JoinCode).IsRequired().HasMaxLength(20);
            entity.Property(e => e.OwnerId).IsRequired();
            entity.Property(e => e.DefaultHourlyRate).HasPrecision(18, 2);
            entity.Property(e => e.DefaultContainerRate).HasPrecision(18, 2);
            entity.Property(e => e.DefaultBoxRate).HasPrecision(18, 2);
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        // OrganizationMembership Configuration
        modelBuilder.Entity<OrganizationMembership>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.OrganizationId, e.UserId }).IsUnique();
            entity.Property(e => e.Role).IsRequired().HasConversion<string>();
            entity.Property(e => e.Status).IsRequired().HasConversion<string>();
            entity.Property(e => e.JoinedAt).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany(u => u.Memberships)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Organization)
                .WithMany(o => o.Memberships)
                .HasForeignKey(e => e.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Project Configuration
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.OrganizationId);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Location).HasMaxLength(500);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasOne(e => e.Organization)
                .WithMany(o => o.Projects)
                .HasForeignKey(e => e.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Shift Configuration
        modelBuilder.Entity<Shift>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ProjectId);
            entity.HasIndex(e => e.ShiftDate);
            entity.Property(e => e.ShiftDate).IsRequired().HasMaxLength(10);
            entity.Property(e => e.StartTime).IsRequired().HasMaxLength(10);
            entity.Property(e => e.EndTime).IsRequired().HasMaxLength(10);
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasOne(e => e.Project)
                .WithMany(p => p.Shifts)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // WorkerAssignment Configuration
        modelBuilder.Entity<WorkerAssignment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ShiftId);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => new { e.ShiftId, e.UserId }).IsUnique();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasOne(e => e.Shift)
                .WithMany(s => s.Assignments)
                .HasForeignKey(e => e.ShiftId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany(u => u.Assignments)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // JoinRequest Configuration
        modelBuilder.Entity<JoinRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.OrganizationId);
            entity.HasIndex(e => e.UserId);
            entity.Property(e => e.Status).IsRequired().HasConversion<string>();
            entity.Property(e => e.RequestedAt).IsRequired();

            entity.HasOne(e => e.Organization)
                .WithMany(o => o.JoinRequests)
                .HasForeignKey(e => e.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Notification Configuration
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.IsRead);
            entity.HasIndex(e => e.CreatedAt);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Message).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.IsRead).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
