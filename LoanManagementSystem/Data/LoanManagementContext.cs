using LoanManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LoanManagementSystem.Data;

public class LoanManagementContext(DbContextOptions<LoanManagementContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<LoanApplication> LoanApplications => Set<LoanApplication>();
    public DbSet<BackgroundVerification> BackgroundVerifications => Set<BackgroundVerification>();
    public DbSet<LoanVerification> LoanVerifications => Set<LoanVerification>();
    public DbSet<HelpReport> HelpReports => Set<HelpReport>();
    public DbSet<FeedbackQuestion> FeedbackQuestions => Set<FeedbackQuestion>();
    public DbSet<FeedbackResponse> FeedbackResponses => Set<FeedbackResponse>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>()
            .Property(u => u.FullName)
            .HasMaxLength(150);

        modelBuilder.Entity<LoanApplication>()
            .Property(l => l.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<LoanApplication>()
            .HasOne(l => l.Customer)
            .WithMany(u => u.LoanApplications)
            .HasForeignKey(l => l.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LoanApplication>()
            .HasOne(l => l.AssignedOfficer)
            .WithMany(u => u.AssignedLoanApplications)
            .HasForeignKey(l => l.AssignedOfficerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LoanApplication>()
            .HasOne(l => l.BackgroundVerification)
            .WithOne(b => b.LoanApplication)
            .HasForeignKey<BackgroundVerification>(b => b.LoanApplicationId);

        modelBuilder.Entity<LoanApplication>()
            .HasOne(l => l.LoanVerification)
            .WithOne(v => v.LoanApplication)
            .HasForeignKey<LoanVerification>(v => v.LoanApplicationId);

        modelBuilder.Entity<BackgroundVerification>()
            .HasOne(b => b.Officer)
            .WithMany()
            .HasForeignKey(b => b.OfficerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LoanVerification>()
            .HasOne(v => v.Officer)
            .WithMany()
            .HasForeignKey(v => v.OfficerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FeedbackResponse>()
            .HasOne(r => r.Customer)
            .WithMany(u => u.FeedbackResponses)
            .HasForeignKey(r => r.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FeedbackResponse>()
            .HasOne(r => r.Question)
            .WithMany(q => q.Responses)
            .HasForeignKey(r => r.QuestionId);

        modelBuilder.Entity<HelpReport>()
            .HasOne(h => h.CreatedBy)
            .WithMany(u => u.HelpReports)
            .HasForeignKey(h => h.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<HelpReport>()
            .HasOne(h => h.UpdatedBy)
            .WithMany()
            .HasForeignKey(h => h.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

