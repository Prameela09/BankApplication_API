using System;
using BankManagement.Database.AccountData.Entities;
using BankManagement.Database.BranchData.Entities;
using BankManagement.Database.CommonEntities;
using BankManagement.Database.NotificationData.Entities;
using BankManagement.Database.TransactionData.Entities;
using BankManagement.Database.UserData.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankManagement.Database.DbContexts;

public class BankDataContext : DbContext
{

    public BankDataContext(DbContextOptions<BankDataContext> options) : base(options)
    {

    }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<SourceType> SourceTypes { get; set; }
    public DbSet<TransactionType> TransactionTypes { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<StatusType> StatusTypes { get; set; }
    public DbSet<Branch> Branches { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<AccountType> AccountTypes { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountType>()
            .Property(a => a.AccountTypeName)
            .HasConversion<string>();

        modelBuilder.Entity<Branch>()
            .Property(b => b.Location)
            .HasConversion<string>();

        modelBuilder.Entity<StatusType>()
            .Property(s => s.Status)
            .HasConversion<string>();

        modelBuilder.Entity<SourceType>()
            .Property(s => s.SourceName)
            .HasConversion<string>();

        modelBuilder.Entity<TransactionType>()
            .Property(t => t.Name)
            .HasConversion<string>();

        modelBuilder.Entity<Role>()
            .Property(r => r.Name)
            .HasConversion<string>();

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.SourceName)
            .WithMany()
            .HasForeignKey(t => t.SourceTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.StatusName)
            .WithMany()
            .HasForeignKey(t => t.StatusTypeId)
            .OnDelete(DeleteBehavior.Restrict); 

        base.OnModelCreating(modelBuilder);
    }
}
