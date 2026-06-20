using CollabFlow.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CollabFlow.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Workspace> Workspaces => Set<Workspace>();
    public DbSet<WorkspaceMember> WorkspaceMembers => Set<WorkspaceMember>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<FileAttachment> FileAttachments => Set<FileAttachment>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(x => x.FullName).IsRequired().HasMaxLength(150);
            entity.Property(x => x.Email).IsRequired().HasMaxLength(255);
            entity.Property(x => x.PasswordHash).IsRequired();
        });

        modelBuilder.Entity<Workspace>(entity =>
        {
            entity.Property(x => x.Name).IsRequired().HasMaxLength(150);
            entity.HasOne(x => x.Owner)
                .WithMany()
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<WorkspaceMember>(entity =>
        {
            entity.HasIndex(x => new { x.WorkspaceId, x.UserId }).IsUnique();
            entity.Property(x => x.Role).HasConversion<string>().HasMaxLength(30);

            entity.HasOne(x => x.Workspace)
                .WithMany(x => x.Members)
                .HasForeignKey(x => x.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.User)
                .WithMany(x => x.WorkspaceMemberships)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.Property(x => x.Name).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);

            entity.HasOne(x => x.Workspace)
                .WithMany(x => x.Projects)
                .HasForeignKey(x => x.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.Property(x => x.Title).IsRequired().HasMaxLength(250);
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
            entity.Property(x => x.Priority).HasConversion<string>().HasMaxLength(30);

            entity.HasOne(x => x.Project)
                .WithMany(x => x.Tasks)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.AssignedTo)
                .WithMany()
                .HasForeignKey(x => x.AssignedToId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.Property(x => x.Content).IsRequired();

            entity.HasOne(x => x.TaskItem)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<FileAttachment>(entity =>
        {
            entity.Property(x => x.FileName).IsRequired().HasMaxLength(255);
            entity.Property(x => x.FileUrl).IsRequired();
            entity.Property(x => x.FileType).IsRequired().HasMaxLength(100);

            entity.HasOne(x => x.TaskItem)
                .WithMany(x => x.Attachments)
                .HasForeignKey(x => x.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.UploadedBy)
                .WithMany()
                .HasForeignKey(x => x.UploadedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.Property(x => x.Type).HasConversion<string>().HasMaxLength(50);
            entity.Property(x => x.Title).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Message).IsRequired();

            entity.HasOne(x => x.User)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ActivityLog>(entity =>
        {
            entity.Property(x => x.Action).IsRequired().HasMaxLength(150);
            entity.Property(x => x.EntityType).HasConversion<string>().HasMaxLength(50);
            entity.Property(x => x.MetadataJson).HasColumnType("jsonb");

            entity.HasOne(x => x.Workspace)
                .WithMany(x => x.ActivityLogs)
                .HasForeignKey(x => x.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
